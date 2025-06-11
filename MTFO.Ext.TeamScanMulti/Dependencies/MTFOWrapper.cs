using BepInEx.Unity.IL2CPP;
using MTFO.API;
using System;
using System.Linq;
using System.Reflection;

namespace MTFO.Ext.TeamScanMulti.Dependencies
{
    internal static class MTFOWrapper
    {
        public const string PLUGIN_GUID = "com.dak.MTFO";

        public readonly static MethodInfo GetCorePuzzleData = null!;
        public readonly static FieldInfo GetCorePuzzleDataID = null!;

        static MTFOWrapper()
        {
            var info = IL2CPPChainloader.Instance.Plugins[PLUGIN_GUID];
            try
            {
                var asm = info?.Instance?.GetType()?.Assembly;
                if (asm is null) throw new Exception("Assembly is missing!");

                var types = asm.GetTypes();
                var dataType = types.First(t => t.Name == "CorePuzzleData");
                if (dataType is null) throw new Exception("Unable to find CorePuzzleData class!");

                GetCorePuzzleDataID = dataType.GetField("PersistentID", BindingFlags.Public | BindingFlags.Instance)!;
                if (GetCorePuzzleDataID is null) throw new Exception($"Unable to find PersistentID field in CorePuzzleData!");

                MethodInfo? generic = typeof(UnityEngine.GameObject).GetMethod(nameof(UnityEngine.GameObject.GetComponent), BindingFlags.Public | BindingFlags.Instance, Array.Empty<Type>());
                if (generic is null) throw new Exception("Unable to find GetComponent function!");

                GetCorePuzzleData = generic.MakeGenericMethod(dataType);
            }
            catch (Exception e)
            {
                DinoLogger.Log($"Exception thrown while trying to find method to get CorePuzzleData:\n{e}");
            }
        }

        public static string GameDataPath => MTFOPathAPI.RundownPath;
        public static string CustomPath => MTFOPathAPI.CustomPath;
        public static bool HasCustomContent => MTFOPathAPI.HasRundownPath;
    }
}
