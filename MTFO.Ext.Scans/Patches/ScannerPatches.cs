﻿using ChainedPuzzles;
using MTFO.Ext.Scans.CustomPuzzleData;
using HarmonyLib;
using Player;
using System.Diagnostics.CodeAnalysis;
using System;
using SNetwork;

namespace MTFO.Ext.Scans.Patches
{
    [HarmonyPatch]
    internal static class ScannerPatches
    {
        [HarmonyPatch(typeof(CP_PlayerScanner), nameof(CP_PlayerScanner.StartScan))]
        [HarmonyPriority(Priority.High)] // Run before LobbyExpansion
        [HarmonyPrefix]
        private static void ApplyScanSpeed(CP_PlayerScanner __instance)
        {
            if (!SNet.IsMaster || __instance.m_scanActive) return;

            if (!ScanDataManager.TryGetScanData(__instance.gameObject, out var scanData)) return;

            int playerCount = PlayerManager.PlayerAgentsInLevel.Count;
            if (scanData.PerTeamSizeScanMultis != null && TryGetScanMultis(scanData.PerTeamSizeScanMultis, playerCount, out var list))
            {
                if (list.Length > 4)
                    __instance.m_scanSpeeds = new float[list.Length];
                for (int i = 0; i < __instance.m_scanSpeeds.Length; i++)
                    __instance.m_scanSpeeds[i] = list[Math.Min(i, list.Length - 1)];
            }

            if (scanData.FullTeamScanMulti != 0)
            {
                var oldSpeeds = __instance.m_scanSpeeds;
                if (oldSpeeds.Length < playerCount)
                {
                    __instance.m_scanSpeeds = new float[playerCount];
                    for (int i = 0; i < __instance.m_scanSpeeds.Length; i++)
                        __instance.m_scanSpeeds[i] = oldSpeeds[Math.Min(i, oldSpeeds.Length - 1)];
                }

                __instance.m_scanSpeeds[playerCount - 1] = scanData.FullTeamScanMulti;
            }
        }

        private static bool TryGetScanMultis(float[][] lists, int playerCount, [MaybeNullWhen(false)] out float[] scanMultis)
        {
            for (int i = playerCount - 1; i < lists.Length; i++)
            {
                if (lists[i].Length != 0)
                {
                    scanMultis = lists[i];
                    return true;
                }
            }
            scanMultis = null;
            return false;
        }
    }
}
