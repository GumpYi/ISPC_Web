using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ISPC.Models.HASPI
{
    /// <summary>
    ///  store the SPI Defect type count
    /// </summary>
    public class HASPIComponent
    {
        public string ComponentName { get; set; }
        public int SuccessCounts { get; set; }
        public int ExcessiveCounts { get; set; }
        public int InsufficientCounts { get; set; }
        public int ShapeCounts { get; set; }
        public int PositionCounts { get; set; }
        public int BridgingCounts { get; set; }
        public int UpperHeightCounts { get; set; }
        public int LowerHeightCounts { get; set; }
        public int HighAreaCounts { get; set; }
        public int LowAreaCounts { get; set; }
        public int SmearCounts { get; set; }
    }
}