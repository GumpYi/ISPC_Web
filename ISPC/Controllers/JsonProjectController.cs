using System;
using System.Collections.Generic;
using System.Web;
using System.Web.Mvc;
using ISPC.Services.FactoryRelated;
namespace ISPC.Web.Controllers
{
    public class JsonProjectController : Controller
    {
        //
        // GET: /JsonProject/

        public ActionResult Index()
        {
            return View();
        }

        //[OutputCache(Duration = Int32.MaxValue, SqlDependency = "ISPCCaching", VaryByParam = "Building_Id;Segment_Id;BU_Id")]
        public ActionResult GetProjects(int Building_Id, int Segment_Id, int BU_Id)
        {
            ProjectConfig projectconfig = new ProjectConfig();
            return Content(projectconfig.GetProjectsJson(Building_Id, Segment_Id, BU_Id));
        }
        public ActionResult AddProject()
        {
            string newData = Request.Form[0];
            ProjectConfig projectconfig = new ProjectConfig();
            return Content(projectconfig.AddProjectJson(newData));            
        }
        public ActionResult UpdateProject()
        {
            string updateData = Request.Form[0];
            ProjectConfig projectconfig = new ProjectConfig();
            return Content(projectconfig.UpdateProjectJson(updateData));
        }
        public ActionResult DeleteProject(int Project_Id)
        {
            ProjectConfig projectconfig = new ProjectConfig();
            return Content(projectconfig.DeleteProjectJson(Project_Id));
        }
    }
}
