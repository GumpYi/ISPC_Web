using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ISPC.Models.RealTimeCache
{
    public class RTSegment
    {
        public int Segment_Id { get; set; }
        public string Segment_Name { get; set; }
        public List<RTLine> RealTime_LineList { get; set; }
    }
}