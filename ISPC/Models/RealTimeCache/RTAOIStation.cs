using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
namespace ISPC.Models.RealTimeCache
{
    public class RTAOIStation : RTStation
    {
        public double[] DPPM { get; set; }
        public double[] FPPM { get; set; }
    }
}