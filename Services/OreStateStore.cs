using System.Collections.Generic;
using NoMoreMountains.Data;
using UnityEngine;

namespace NoMoreMountains.Services
{
    internal sealed class OreStateStore
    {
        private readonly Dictionary<int, OreRuntimeState> _runtimeById = new Dictionary<int, OreRuntimeState>();
        private readonly Dictionary<int, OreBaselineState> _baselineById = new Dictionary<int, OreBaselineState>();

        public OreRuntimeState GetOrCreateRuntime(int instanceId, Vector3 position, float now, out bool wasCreated)
        {
            if (_runtimeById.TryGetValue(instanceId, out OreRuntimeState? runtimeState))
            {
                wasCreated = false;
                return runtimeState;
            }

            runtimeState = new OreRuntimeState(position, now);
            _runtimeById[instanceId] = runtimeState;
            wasCreated = true;
            return runtimeState;
        }

        public void RemoveRuntime(int instanceId)
        {
            _runtimeById.Remove(instanceId);
        }

        public void EnsureBaselineCaptured(OrePiece ore)
        {
            int instanceId = ore.GetInstanceID();
            if (_baselineById.ContainsKey(instanceId))
            {
                return;
            }

            List<ColliderBaseline> colliderStates = new List<ColliderBaseline>();
            Collider[] colliders = ore.GetComponentsInChildren<Collider>(includeInactive: true);

            for (int index = 0; index < colliders.Length; index++)
            {
                Collider collider = colliders[index];
                if (collider == null)
                {
                    continue;
                }

                colliderStates.Add(new ColliderBaseline(collider, collider.enabled, collider.isTrigger));
            }

            bool? detectCollisions = ore.Rb != null ? ore.Rb.detectCollisions : (bool?)null;
            _baselineById[instanceId] = new OreBaselineState(ore.BaseSellValue, detectCollisions, colliderStates);
        }

        public void RestoreBaseline(OrePiece ore)
        {
            int instanceId = ore.GetInstanceID();
            if (!_baselineById.TryGetValue(instanceId, out OreBaselineState? baselineState))
            {
                return;
            }

            ore.BaseSellValue = baselineState.BaseSellValue;

            if (ore.Rb != null && baselineState.DetectCollisions.HasValue)
            {
                ore.Rb.detectCollisions = baselineState.DetectCollisions.Value;
            }

            for (int index = 0; index < baselineState.Colliders.Count; index++)
            {
                ColliderBaseline colliderState = baselineState.Colliders[index];
                if (colliderState.Collider == null)
                {
                    continue;
                }

                colliderState.Collider.enabled = colliderState.Enabled;
                colliderState.Collider.isTrigger = colliderState.IsTrigger;
            }
        }

        public void PruneRuntimeState(HashSet<int> seenIds)
        {
            if (_runtimeById.Count == 0)
            {
                return;
            }

            List<int>? idsToRemove = null;

            foreach (KeyValuePair<int, OreRuntimeState> pair in _runtimeById)
            {
                if (seenIds.Contains(pair.Key))
                {
                    continue;
                }

                idsToRemove ??= new List<int>();
                idsToRemove.Add(pair.Key);
            }

            if (idsToRemove == null)
            {
                return;
            }

            for (int index = 0; index < idsToRemove.Count; index++)
            {
                _runtimeById.Remove(idsToRemove[index]);
            }
        }
    }
}
