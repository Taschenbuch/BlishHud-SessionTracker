using Blish_HUD.Controls;
using System.Collections.Generic;

namespace SessionTracker.SelectStats
{
    public class SelectStatsControls
    {
        public Container CategoryContainer { get; set; }
        public FlowPanel CategoryFlowPanel { get; set; }
        public List<SelectStatContainer> StatContainers { get; } = new List<SelectStatContainer>();
    }
}
