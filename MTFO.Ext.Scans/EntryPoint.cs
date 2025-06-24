using BepInEx;
using BepInEx.Unity.IL2CPP;
using HarmonyLib;
using MTFO.Ext.Scans.CustomPuzzleData;
using MTFO.Ext.Scans.Dependencies;

namespace MTFO.Ext.Scans
{
    [BepInPlugin("MTFO.Extension.Scans", MODNAME, "1.0.1")]
    [BepInDependency(MTFOWrapper.PLUGIN_GUID, BepInDependency.DependencyFlags.HardDependency)]
    internal sealed class EntryPoint : BasePlugin
    {
        public const string MODNAME = "MTFO.Ext.Scans";

        public override void Load()
        {
            if (!MTFOWrapper.HasCustomContent)
            {
                Log.LogMessage("No Custom folder, not loading...");
                return;
            }
            else if (MTFOWrapper.GetCorePuzzleData == null)
            {
                DinoLogger.Error("Unable to find method to get CorePuzzleData, not loading...");
                return;
            }

            ScanDataManager.Init();
            new Harmony(MODNAME).PatchAll();
            Log.LogMessage("Loaded " + MODNAME);
        }
    }
}