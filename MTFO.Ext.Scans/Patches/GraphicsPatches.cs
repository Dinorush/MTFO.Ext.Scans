using ChainedPuzzles;
using HarmonyLib;
using MTFO.Ext.Scans.CustomPuzzleData;

namespace MTFO.Ext.Scans.Patches
{
    [HarmonyPatch]
    internal static class GraphicsPatches
    {
        [HarmonyPatch(typeof(CP_Bioscan_Graphics), nameof(CP_Bioscan_Graphics.SetVisible))]
        [HarmonyPostfix]
        [HarmonyPriority(Priority.Low)]
        private static void CustomBulkheadBioscanFix(CP_Bioscan_Graphics __instance, bool visible)
        {
            if (!visible || !ScanDataManager.TryGetScanData(__instance.gameObject, out var scanData)) return;

            __instance.SetText(scanData.BioScanGraphics.ScanText); // override BH scan text
            if (scanData.BioScanGraphics.HideBulkheadSkullGraphic)
            {
                foreach (var info in __instance.transform.FindChildrenRecursive("Skull", false))
                {
                    info?.gameObject.SetActive(false); // disable skull graphics
                }
            }
        }
    }
}
