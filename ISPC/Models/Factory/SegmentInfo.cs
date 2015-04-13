using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using ISPC.EntityFramework;
namespace ISPC.Models
{
    public class SegmentInfo
    {
        public int Segment_Id { get; set; }
        public string Segment_Name { get; set; }
        public string Segment_Code { get; set; }
        public int Building_Id { get; set; }
        public bool Is_Active { get; set; }
        public string Creator_Name { get; set; }
        public DateTime Creation_Time { get; set; }
        public DateTime Effect_Time { get; set; }
        public static SegmentInfo FromSegment(Segment segment)
        {
            return new SegmentInfo
            {
                Segment_Id = segment.Segment_Id,
                Segment_Name = segment.Segment_Name,
                Segment_Code = segment.Segment_Code,
                Building_Id = segment.Building_Id,
                Is_Active = segment.Is_Active,
                Creation_Time = segment.Creation_Time,
                Creator_Name = segment.UserProfile.UserName,
                Effect_Time = segment.Effect_Time
            };
        }
    }

}