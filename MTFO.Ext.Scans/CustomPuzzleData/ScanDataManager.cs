using Il2CppInterop.Runtime.InteropTypes.Fields;
using MTFO.API;
using MTFO.Ext.Scans.Dependencies;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Text.Json;

namespace MTFO.Ext.Scans.CustomPuzzleData
{
    public static class ScanDataManager
    {
        private readonly static Dictionary<uint, ScanData> _scanData = new();

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
    }
}
