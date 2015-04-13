using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using ISPC.EntityFramework;

namespace ISPC.Models
{
    public class BUInfo
    {
        public int BU_Id { get; set; }
        public string BU_Name { get; set; }
        public int Segment_Id { get; set; }
        public int Building_Id { get; set; }
        public bool Is_Active { get; set; }
        public string Creator_Name { get; set; }
        public DateTime Creation_Time { get; set; }
        public DateTime Effect_Time { get; set; }
        public static BUInfo FromBU(BusinessUnit bu)
        {
            return new BUInfo
            {
                BU_Id = bu.BU_Id,
                BU_Name = bu.BU_Name,
                Segment_Id = bu.Segment_Id,
                Building_Id = bu.Segment.Building_Id,
                Is_Active = bu.Is_Active,
                Creator_Name = bu.UserProfile.UserName,
                Creation_Time = bu.Creation_Time,
                Effect_Time = bu.Effect_Time
            };
        }
    }
}