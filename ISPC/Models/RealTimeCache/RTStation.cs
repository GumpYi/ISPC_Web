using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ISPC.Models.RealTimeCache
{
    public class RTStation
    {
        public int Station_Id { get; set; }
        public string Station_Name { get; set; }
        public double[] FPY { get; set; }
    }
}