using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using ISPC.EntityFramework;
using ISPC.Models;
using System.Text;
using Newtonsoft.Json.Converters;
using System.Linq.Dynamic;
using Newtonsoft.Json;
using WebMatrix.WebData;
namespace ISPC.Services.FactoryRelated
{
    public class ProjectConfig
    {
        public string GetProjectsJson(int Building_Id, int Segment_Id, int BU_Id)
        {
            using (ISPCEntities entity = new ISPCEntities())
            {
                List<ProjectInfo> projectList = new List<ProjectInfo>();
                StringBuilder selectCondition = new StringBuilder("1 == 1");
                if (Building_Id != 0) selectCondition.Append(" And Segment.Building_Id ==" + Building_Id);
                if (Segment_Id != 0) selectCondition.Append(" And Segment_Id ==" + Segment_Id);
                if (BU_Id != 0) selectCondition.Append(" And BU_Id ==" + BU_Id);
                var projects = entity.Projects.Where(selectCondition.ToString()).OrderBy(p => p.Project_Id);
                foreach (var project in projects)
                {
                    projectList.Add(ProjectInfo.FromProject(project));
                }
                IsoDateTimeConverter dtConverter = new IsoDateTimeConverter { DateTimeFormat = "yyyy-MM-dd HH:mm" };
                return (JsonConvert.SerializeObject(projectList, dtConverter));
            }
        }

        public string AddProjectJson(string newData)
        {
            using (ISPCEntities entity = new ISPCEntities())
            {
                try
                {
                    ProjectInfo projectInfo = JsonConvert.DeserializeObject<ProjectInfo>(newData);
                    var project = new Project
                    {
                        Project_Name = projectInfo.Project_Name,
                        Is_Active = projectInfo.Is_Active,
                        Segment_Id = projectInfo.Segment_Id,
                        BU_Id = projectInfo.BU_Id,
                        Creator_Id = WebSecurity.CurrentUserId,
                        Creation_Time = DateTime.Now,
                        Effect_Time = projectInfo.Effect_Time
                    };
                    entity.Projects.Add(project);
                    entity.SaveChanges();
                    return ("Execute successful");
                }
                catch (Exception ex)
                {
                    return (ex.Message);
                }
            }
        }

        public string UpdateProjectJson(string updateData)
        {
            using (ISPCEntities entity = new ISPCEntities())
            {
                try
                {
                    ProjectInfo projectInfo = JsonConvert.DeserializeObject<ProjectInfo>(updateData);
                    var pt = entity.Projects.FirstOrDefault(project => project.Project_Id == projectInfo.Project_Id);
                    pt.Project_Name = projectInfo.Project_Name;
                    pt.Is_Active = projectInfo.Is_Active;
                    pt.Segment_Id = projectInfo.Segment_Id;
                    pt.BU_Id = projectInfo.BU_Id;
                    pt.Effect_Time = projectInfo.Effect_Time;
                    entity.SaveChanges();
                    return("Execute successful");
                }
                catch (Exception ex)
                {
                    return(ex.Message);
                }
            }
        }

        public string DeleteProjectJson(int Project_Id)
        {
            using (ISPCEntities entity = new ISPCEntities())
            {
                try
                {
                    var pt = entity.Projects.Where(Project => Project.Project_Id == Project_Id).Take(1).FirstOrDefault();
                    entity.Projects.Remove(pt);
                    entity.SaveChanges();
                    return("Delete successful");
                }
                catch (Exception ex)
                {
                    return(ex.Message);
                }
            }
        }
    }
}