using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using ISPC.EntityFramework;

namespace ISPC.Models
{
    public class LineInfo
    {
        public int Line_Id { get; set; }
        public string Line_Name { get; set; }
        public int Segment_Id { get; set; }
        public bool Is_Active { get; set; }
        public bool Is_NPI { get; set; }
        public string Line_Location { get; set; }
        public string Creator_Name { get; set; }
        public DateTime Creation_Time { get; set; }
        public DateTime Effect_Time { get; set; }
        public List<StationInfo> StationList {get;set; }
        public static LineInfo FromLine(Line line)
        {
            List<StationInfo> flag = new List<StationInfo>();
            foreach (Station station in line.Stations.OrderBy(s => s.Position).ToList())
            {
                flag.Add(StationInfo.FromStation(station));   
            }            
            return new LineInfo
            {
                Line_Id = line.Line_Id,
                Line_Name = line.Line_Name,
                Segment_Id = line.Segment_Id,
                Is_Active = line.Is_Active,
                Is_NPI = line.Is_NPI,
                Line_Location = line.Line_Location,
                Creator_Name = line.UserProfile.UserName,
                Creation_Time = line.Creation_Time,
                Effect_Time = line.Effect_Time,
                StationList = flag
            };
        }
    }
}