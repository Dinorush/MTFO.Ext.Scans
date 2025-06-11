using ChainedPuzzles;
using MTFO.Ext.Scans.CustomPuzzleData;
using HarmonyLib;
using Player;

namespace MTFO.Ext.Scans.Patches
{
    [HarmonyPatch]
    internal static class BioscanPatches
    {
        [HarmonyPatch(typeof(CP_Bioscan_Core), nameof(CP_Bioscan_Core.OnDestroy))]
        [HarmonyPostfix]
        private static void CleanupDeadScanner(CP_Bioscan_Core __instance)
        {
            ScanDataManager.RemoveCacheData(__instance);
        }

        [HarmonyPatch(typeof(CP_Bioscan_Core), nameof(CP_Bioscan_Core.Master_OnPlayerScanChangedCheckProgress))]
        [HarmonyPostfix]
        private static void UpdateFullTeamMultiChange(CP_Bioscan_Core __instance, Il2CppSystem.Collections.Generic.List<PlayerAgent> playersInScan, int inScanMax)
        {
            if (!ScanDataManager.TryGetScanData(__instance.gameObject, out var scanData) || scanData.FullTeamScanMulti == 0) return;
            if (!ScanDataManager.TryCacheScanData(__instance, out (float[] multis, CP_PlayerScanner scanner) cache)) return;

            var scannerMultis = cache.scanner.m_scanSpeeds;
            if (inScanMax == playersInScan.Count)
            {
                for (int i = 0; i < cache.multis.Length; i++)
                {
                    scannerMultis[i] = scanData.FullTeamScanMulti;
                }
            }
            else
            {
                for (int i = 0; i < cache.multis.Length; i++)
                {
                    scannerMultis[i] = cache.multis[i];
                }
            }
        }
    }
}
