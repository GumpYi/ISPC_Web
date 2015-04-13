using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using ISPC.EntityFramework;
namespace ISPC.Models
{
    class BuildingInfo
    {
        public int Building_Id { get; set; }
        public string Building_Name { get; set; }
        public bool Is_Active { get; set; }
        public string Creator_Name { get; set; }
        public DateTime Creation_Time { get; set; }
        public DateTime Effect_Time { get; set; }
        public static BuildingInfo FromBuilding(Building building)
        {
            return new BuildingInfo
            {
                Building_Id = building.Building_Id,
                Building_Name = building.Building_Name,
                Is_Active = building.Is_Active,
                Creator_Name = building.UserProfile.UserName,
                Creation_Time = building.Creation_Time,
                Effect_Time = building.Effect_Time
            };
        }
    }
}