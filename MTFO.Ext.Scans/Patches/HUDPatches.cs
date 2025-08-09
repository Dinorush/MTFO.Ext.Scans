using ChainedPuzzles;
using HarmonyLib;

namespace MTFO.Ext.Scans.Patches
{
    [HarmonyPatch]
    internal static class HUDPatches
    {
        [HarmonyPatch(typeof(CP_Bioscan_Hud), nameof(CP_Bioscan_Hud.Awake))]
        [HarmonyPrefix]
        private static void NullBioscanHudTMPFix(CP_Bioscan_Hud __instance)
        {
            if (__instance.m_bioscanWorldText == null)
                __instance.m_bioscanWorldText = new TMPro.TextMeshPro(); // fix null TMP on some bioscan huds
        }
    }
}
