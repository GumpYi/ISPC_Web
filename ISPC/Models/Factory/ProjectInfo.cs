using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using ISPC.EntityFramework;

namespace ISPC.Models
{
    public class ProjectInfo
    {
        public int Project_Id { get; set; }
        public string Project_Name { get; set; }
        public bool Is_Active { get; set; }
        public int Segment_Id { get; set; }
        public int Building_Id { get; set; }
        public int BU_Id { get; set; }
        public string Creator_Name { get; set; }
        public DateTime Creation_Time { get; set; }
        public DateTime Effect_Time { get; set; }
        public static ProjectInfo FromProject(Project project)
        {
            return new ProjectInfo 
            { 
                Project_Id = project.Project_Id,
                Project_Name = project.Project_Name,
                Is_Active = project.Is_Active,
                Segment_Id = project.Segment_Id,
                Building_Id = project.Segment.Building_Id,
                BU_Id = (project.BU_Id != null) ? (int)project.BU_Id : 0,
                Creator_Name = project.UserProfile.UserName,
                Creation_Time = project.Creation_Time,
                Effect_Time = project.Effect_Time
            };
        }
    }
}