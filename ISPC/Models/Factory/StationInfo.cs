using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using ISPC.EntityFramework;

namespace ISPC.Models
{
    public class StationInfo
    {
        public int Station_Id { get; set; }
        public string Station_Name { get; set; }
        public bool Is_Active { get; set; }
        public int Line_Id { get; set; }
        public int Machine_Id { get; set; }
        public int Position { get; set; }
        public string Creator_Name { get; set; }
        public DateTime Creation_Time { get; set; }
        public DateTime Effect_Time { get; set; }
        public string Machine_Picture_Url { get; set; }
        public static StationInfo FromStation(Station station)
        {
            return new StationInfo
            {
                Station_Id = station.Station_Id,
                Station_Name = station.Station_Name,
                Is_Active = station.Is_Active,
                Line_Id = station.Line_Id,
                Machine_Id = station.Machine_Id == null ? 0 : (int)station.Machine_Id,
                Position = station.Position,
                Creator_Name = station.UserProfile.UserName,
                Creation_Time = station.Creation_Time,
                Effect_Time = station.Effect_Time,
                Machine_Picture_Url = station.Machine.Machine_Model.Machine_Picture_Url
            };
        }
    }
}