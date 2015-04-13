using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
namespace ISPC.Models.RealTimeCache
{
    public class RTLine
    {
        public int Line_Id { get; set; }
        public string Line_Name { get; set; }
        public int Building_Id { get; set; }
        public int Segment_Id { get; set; }
        public List<RTSPIStation> RealTimeData_SPIList { get; set; }
        public List<RTAOIStation> RealTimeData_AOIList { get; set; }
    }
}