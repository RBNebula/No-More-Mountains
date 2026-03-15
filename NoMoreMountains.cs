using BepInEx;
using HarmonyLib;
using NoMoreMountains.Config;
using NoMoreMountains.Core;

namespace NoMoreMountains
{
    [BepInPlugin(ModInfo.PLUGIN_GUID, ModInfo.PLUGIN_NAME, ModInfo.PLUGIN_VERSION)]
    public sealed class NoMoreMountains : BaseUnityPlugin
    {
        private Harmony? _harmony;

        internal static NoMoreMountains? Instance { get; private set; }
        internal NoMoreMountainsRuntime Runtime { get; private set; } = null!;

        private void Awake()
        {
            Instance = this;

            NoMoreMountainsConfig config = NoMoreMountainsConfig.Bind(Config);
            Runtime = new NoMoreMountainsRuntime(Logger, config);

            _harmony = new Harmony(ModInfo.HARMONY_ID);
            _harmony.PatchAll();

            Logger.LogInfo($"{ModInfo.LOG_PREFIX} Initialized");
        }

        private void OnDestroy()
        {
            _harmony?.UnpatchSelf();
            _harmony = null;

            if (Instance == this)
            {
                Instance = null;
            }
        }

        private void Update()
        {
            Runtime.Tick(UnityEngine.Time.time);
        }
    }
}
