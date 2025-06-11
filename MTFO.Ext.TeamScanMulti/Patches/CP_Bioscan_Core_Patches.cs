using ChainedPuzzles;
using MTFO.Ext.TeamScanMulti.CustomPuzzleData;
using HarmonyLib;
using Il2CppInterop.Runtime.InteropTypes.Fields;
using Player;
using MTFO.Ext.TeamScanMulti.Dependencies;

namespace MTFO.Ext.TeamScanMulti.Patches
{
    [HarmonyPatch(typeof(CP_Bioscan_Core))]
    internal static class CP_Bioscan_Core_Patches
    {
        [HarmonyPatch(nameof(CP_Bioscan_Core.OnDestroy))]
        [HarmonyPostfix]
        private static void CleanupDeadScanner(CP_Bioscan_Core __instance)
        {
            ScanDataManager.RemoveCacheData(__instance);
        }

        [HarmonyPatch(nameof(CP_Bioscan_Core.Master_OnPlayerScanChangedCheckProgress))]
        [HarmonyPostfix]
        private static void UpdateFullTeamMultiChange(CP_Bioscan_Core __instance, Il2CppSystem.Collections.Generic.List<PlayerAgent> playersInScan, int inScanMax)
        {
            var mtfoData = MTFOWrapper.GetCorePuzzleData.Invoke(__instance.gameObject, null);
            if (mtfoData == null) return;

            var id = (Il2CppValueField<uint>)MTFOWrapper.GetCorePuzzleDataID.GetValue(mtfoData)!;
            if (!ScanDataManager.TryGetFullTeamScanMulti(id, out var fullTeamMulti)) return;
            if (!ScanDataManager.TryCacheScanData(__instance, out (float[] multis, CP_PlayerScanner scanner) cache)) return;

            var scannerMultis = cache.scanner.m_scanSpeeds;
            if (inScanMax == playersInScan.Count)
            {
                for (int i = 0; i < cache.multis.Length; i++)
                {
                    scannerMultis[i] = fullTeamMulti;
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
