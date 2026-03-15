using HarmonyLib;

namespace NoMoreMountains.Patches
{
    [HarmonyPatch(typeof(OrePiece), "OnEnable")]
    internal static class OrePieceOnEnablePatch
    {
        [HarmonyPostfix]
        private static void Postfix(OrePiece __instance)
        {
            NoMoreMountains.Instance?.Runtime.HandleOreEnabled(__instance);
        }
    }
}
