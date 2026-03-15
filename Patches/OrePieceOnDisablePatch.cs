using HarmonyLib;

namespace NoMoreMountains.Patches
{
    [HarmonyPatch(typeof(OrePiece), "OnDisable")]
    internal static class OrePieceOnDisablePatch
    {
        [HarmonyPostfix]
        private static void Postfix(OrePiece __instance)
        {
            NoMoreMountains.Instance?.Runtime.HandleOreDisabled(__instance);
        }
    }
}
