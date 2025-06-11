using BepInEx;
using BepInEx.Unity.IL2CPP;
using HarmonyLib;
using MTFO.Ext.TeamScanMulti.CustomPuzzleData;
using MTFO.Ext.TeamScanMulti.Dependencies;

namespace MTFO.Ext.TeamScanMulti
{
    [BepInPlugin("MTFO.Extension.TeamScanMulti", MODNAME, "1.0.0")]
    [BepInDependency("dev.gtfomodding.gtfo-api", BepInDependency.DependencyFlags.HardDependency)]
    [BepInDependency(MTFOWrapper.PLUGIN_GUID, BepInDependency.DependencyFlags.HardDependency)]
    internal sealed class EntryPoint : BasePlugin
    {
        public const string MODNAME = "MTFO.Ext.TeamScanMulti";

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
            GTFO.API.LevelAPI.OnLevelCleanup += ScanDataManager.OnCleanup;

            Log.LogMessage("Loaded " + MODNAME);
        }
    }
}