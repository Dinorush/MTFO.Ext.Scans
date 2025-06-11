using System.Collections.Generic;

namespace MTFO.Ext.Scans.CustomPuzzleData
{
    internal class ScanDataHolder
    {
        public List<ScanData> Scans { get; set; } = null!;
        public List<EmptyClusterHolder> Clusters { get; set; } = null!;
    }

    internal class EmptyClusterHolder {}
}
