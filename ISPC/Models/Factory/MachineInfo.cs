using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ISPC.Models
{
    public class MachineInfo
    {
        public int Machine_Id { get; set; }
        public string Machine_Asset_Num { get; set; }
        public string Machine_Name { get; set; }
        public int Machine_Status_Id { get; set; }
        public int Machine_Model_Id { get; set; }
        public int Machine_Config_Id { get; set; }
        public int Building_Id { get; set; }
        public int Creator_Name { get; set; }
        public DateTime Creation_Time { get; set; }
    }
}