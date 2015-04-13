using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ISPC.Models.RealTimeCache
{
    public class RTBuilding
    {
        public int Builidng_Id { get; set; }
        public string Building_Name { get; set; }
        public List<RTSegment> RealTimeData_SegmentList { get; set; }
    }
}