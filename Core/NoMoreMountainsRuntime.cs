using System.Collections.Generic;
using BepInEx.Logging;
using NoMoreMountains.Config;
using NoMoreMountains.Data;
using NoMoreMountains.Services;
using UnityEngine;

namespace NoMoreMountains.Core
{
    internal sealed class NoMoreMountainsRuntime
    {
        private readonly ManualLogSource _logger;
        private readonly NoMoreMountainsConfig _config;
        private readonly OreCleanupService _cleanupService;
        private readonly OreStateStore _stateStore;

        private float _nextScanTime;

        public NoMoreMountainsRuntime(ManualLogSource logger, NoMoreMountainsConfig config)
        {
            _logger = logger;
            _config = config;
            _cleanupService = new OreCleanupService(config);
            _stateStore = new OreStateStore();
        }

        public void Tick(float now)
        {
            if (!_config.Enabled.Value)
            {
                return;
            }

            float scanInterval = Mathf.Max(0.05f, _config.ScanIntervalSeconds.Value);
            if (now < _nextScanTime)
            {
                return;
            }

            _nextScanTime = now + scanInterval;
            RunCleanupScan(now);
        }

        public void HandleOreEnabled(OrePiece ore)
        {
            if (ore == null)
            {
                return;
            }

            _stateStore.EnsureBaselineCaptured(ore);
            _stateStore.RestoreBaseline(ore);
            _stateStore.RemoveRuntime(ore.GetInstanceID());
        }

        public void HandleOreDisabled(OrePiece ore)
        {
            if (ore == null)
            {
                return;
            }

            _stateStore.RemoveRuntime(ore.GetInstanceID());
        }

        private void RunCleanupScan(float now)
        {
            IList<OrePiece>? allOrePieces = OrePiece.AllOrePieces;
            if (allOrePieces == null || allOrePieces.Count == 0)
            {
                return;
            }

            HashSet<int> seenIds = new HashSet<int>();
            float idleSeconds = Mathf.Max(0.5f, _config.IdleSecondsBeforeDrop.Value);

            for (int index = 0; index < allOrePieces.Count; index++)
            {
                OrePiece ore = allOrePieces[index];
                if (!_cleanupService.IsEligiblePieceType(ore))
                {
                    continue;
                }

                int instanceId = ore.GetInstanceID();
                seenIds.Add(instanceId);
                _stateStore.EnsureBaselineCaptured(ore);

                OreRuntimeState runtimeState = _stateStore.GetOrCreateRuntime(instanceId, ore.transform.position, now, out bool wasCreated);
                if (wasCreated)
                {
                    continue;
                }

                if (runtimeState.HasDropped)
                {
                    runtimeState.LastPosition = ore.transform.position;
                    continue;
                }

                if (_cleanupService.IsProtectedFromCleanup(ore))
                {
                    runtimeState.LastPosition = ore.transform.position;
                    runtimeState.LastMotionTime = now;
                    continue;
                }

                if (_cleanupService.HasMovedEnough(ore, runtimeState.LastPosition))
                {
                    runtimeState.LastPosition = ore.transform.position;
                    runtimeState.LastMotionTime = now;
                    continue;
                }

                if (now - runtimeState.LastMotionTime < idleSeconds)
                {
                    continue;
                }

                _cleanupService.DropOreThroughWorld(ore);
                runtimeState.HasDropped = true;
                runtimeState.LastPosition = ore.transform.position;
                runtimeState.LastMotionTime = now;

                if (_config.VerboseLogging.Value)
                {
                    _logger.LogInfo($"{ModInfo.LOG_PREFIX} Dropped idle {ore.PieceType} ({ore.ResourceType}) at {ore.transform.position}");
                }
            }

            _stateStore.PruneRuntimeState(seenIds);
        }
    }
}
