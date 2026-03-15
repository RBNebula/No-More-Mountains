using BepInEx.Configuration;

namespace NoMoreMountains.Config
{
    internal sealed class NoMoreMountainsConfig
    {
        public ConfigEntry<bool> Enabled { get; }
        public ConfigEntry<float> IdleSecondsBeforeDrop { get; }
        public ConfigEntry<float> ScanIntervalSeconds { get; }
        public ConfigEntry<float> MovementThreshold { get; }
        public ConfigEntry<float> VelocityThreshold { get; }
        public ConfigEntry<bool> VerboseLogging { get; }

        private NoMoreMountainsConfig(
            ConfigEntry<bool> enabled,
            ConfigEntry<float> idleSecondsBeforeDrop,
            ConfigEntry<float> scanIntervalSeconds,
            ConfigEntry<float> movementThreshold,
            ConfigEntry<float> velocityThreshold,
            ConfigEntry<bool> verboseLogging)
        {
            Enabled = enabled;
            IdleSecondsBeforeDrop = idleSecondsBeforeDrop;
            ScanIntervalSeconds = scanIntervalSeconds;
            MovementThreshold = movementThreshold;
            VelocityThreshold = velocityThreshold;
            VerboseLogging = verboseLogging;
        }

        public static NoMoreMountainsConfig Bind(ConfigFile config)
        {
            ConfigEntry<bool> enabled = config.Bind(
                "General",
                "Enabled",
                true,
                "Enable No More Mountains cleanup behavior.");

            ConfigEntry<float> idleSecondsBeforeDrop = config.Bind(
                "General",
                "IdleSecondsBeforeDrop",
                30f,
                "Seconds an eligible ore piece must remain still (outside baskets) before being dropped through the world.");

            ConfigEntry<float> scanIntervalSeconds = config.Bind(
                "General",
                "ScanIntervalSeconds",
                0.25f,
                "How often to scan active ore pieces.");

            ConfigEntry<float> movementThreshold = config.Bind(
                "General",
                "MovementThreshold",
                0.03f,
                "World-space movement distance considered motion (meters).");

            ConfigEntry<float> velocityThreshold = config.Bind(
                "General",
                "VelocityThreshold",
                0.05f,
                "Linear/angular velocity magnitude considered motion.");

            ConfigEntry<bool> verboseLogging = config.Bind(
                "Debug",
                "VerboseLogging",
                false,
                "Log each cleanup drop.");

            return new NoMoreMountainsConfig(
                enabled,
                idleSecondsBeforeDrop,
                scanIntervalSeconds,
                movementThreshold,
                velocityThreshold,
                verboseLogging);
        }
    }
}
