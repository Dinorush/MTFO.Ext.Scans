using ChainedPuzzles;
using MTFO.API;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Text.Json;

namespace MTFO.Ext.TeamScanMulti.CustomPuzzleData
{
    public static class ScanDataManager
    {
        private readonly static Dictionary<uint, float> _fullTeamMultis = new();
        private readonly static Dictionary<IntPtr, (float[] multis, CP_PlayerScanner scanner)> _originalMultis = new();

        internal static void Init()
        {
            var filepath = Path.Combine(MTFOPathAPI.CustomPath, "PuzzleTypes.json");
            if (!File.Exists(filepath))
            {
                DinoLogger.Error("Datablocks exist, but PuzzleTypes.json does not exist!");
                return;
            }

            JsonSerializerOptions options = new()
            {
                ReadCommentHandling = JsonCommentHandling.Skip,
                IncludeFields = true,
                PropertyNameCaseInsensitive = true
            };
            List<ScanData>? list = JsonSerializer.Deserialize<ScanDataHolder>(File.ReadAllText(filepath), options)?.Scans;
            if (list == null)
            {
                DinoLogger.Error("Unable to read data from PuzzleTypes.json!");
                return;
            }

            foreach (var data in list)
                if (data.FullTeamScanMulti != 0)
                    _fullTeamMultis.TryAdd(data.PersistentID, data.FullTeamScanMulti);
        }

        public static bool TryGetFullTeamScanMulti(uint id, out float multi) => _fullTeamMultis.TryGetValue(id, out multi);
        public static bool TryCacheScanData(CP_Bioscan_Core scan, [MaybeNullWhen(false)] out (float[] multis, CP_PlayerScanner scanner) cachedData)
        {
            var ptr = scan.Pointer;
            if (_originalMultis.TryGetValue(ptr, out cachedData)) return true;

            if (scan.PlayerScanner == null) return false;

            var scanner = scan.PlayerScanner.Cast<CP_PlayerScanner>();
            var multis = new float[scanner.m_scanSpeeds.Length];
            for(int i = 0; i < multis.Length; i++)
                multis[i] = scanner.m_scanSpeeds[i];
            cachedData = (multis, scanner);

            _originalMultis.Add(ptr, cachedData);
            return true;
        }

        public static void RemoveCacheData(CP_Bioscan_Core scan) => _originalMultis.Remove(scan.Pointer);

        public static void OnCleanup() => _originalMultis.Clear();
    }
}
