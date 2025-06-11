using ChainedPuzzles;
using Il2CppInterop.Runtime.InteropTypes.Fields;
using MTFO.API;
using MTFO.Ext.Scans.Dependencies;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Text.Json;

namespace MTFO.Ext.Scans.CustomPuzzleData
{
    public static class ScanDataManager
    {
        private readonly static Dictionary<uint, ScanData> _scanData = new();
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
            {
                _scanData.TryAdd(data.PersistentID, data);
            }
        }

        public static bool TryGetScanData(UnityEngine.GameObject go, [MaybeNullWhen(false)] out ScanData data)
        {
            data = null;
            var mtfoData = MTFOWrapper.GetCorePuzzleData.Invoke(go, null);
            if (mtfoData == null) return false;

            var id = (Il2CppValueField<uint>)MTFOWrapper.GetCorePuzzleDataID.GetValue(mtfoData)!;
            return TryGetScanData(id, out data);
        }

        public static bool TryGetScanData(uint id, [MaybeNullWhen(false)] out ScanData data) => _scanData.TryGetValue(id, out data);
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
