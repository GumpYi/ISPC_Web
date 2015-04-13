using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ISPC.Models.RealTimeCache
{
    public class SessionShownLine
    {
        public int Line_Id { get; set; }
        public string Line_Name { get; set; }
        public string Line_Location { get; set; }
        public bool Status { get; set; }
    }
}