using System;

namespace MTFO.Ext.Scans.CustomPuzzleData
{
    public class ScanData
    {
        public float FullTeamScanMulti { get; set; } = 0f;
        public float[][] PerTeamSizeScanMultis { get; set; } = Array.Empty<float[]>();
        public uint PersistentID { get; set; } = 0;
    }
}
