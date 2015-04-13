using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ISPC.EntityFramework;
using Newtonsoft.Json;
using System.Text;
using System.Linq.Dynamic;
using Newtonsoft.Json.Converters;
using ISPC.Models.RealTimeCache;
using ISPC.Services.SessionRelated;
using ISPC.Services.DropDownRelated;
namespace ISPC.Controllers
{
    public class CommonController : Controller
    {
        public ActionResult GetBuildings()
        {
            CommonDropDownData dropdownData = new CommonDropDownData();
            return Content(dropdownData.getBuildings());
        }
        public ActionResult GetSegments(int Building_Id)
        {
            CommonDropDownData dropdownData = new CommonDropDownData();
            return Content(dropdownData.getSegments(Building_Id));
        }
        public ActionResult GetBUs(int Segment_Id)
        {
            CommonDropDownData dropdownData = new CommonDropDownData();
            return Content(dropdownData.getBUs(Segment_Id));
        }
        public ActionResult GetLines(int Segment_Id)
        {
            CommonDropDownData dropdownData = new CommonDropDownData();
            return Content(dropdownData.getLines(Segment_Id));
        }
        public ActionResult GetHAMultiLineList(int Building_Id, int Segment_Id)
        {
            CommonDropDownData dropdownData = new CommonDropDownData();
            return Content(dropdownData.getLines(Building_Id, Segment_Id));
        }
        public ActionResult GetSPIStations(int Line_Id)
        {
            CommonDropDownData dropdownData = new CommonDropDownData();
            return Content(dropdownData.getSPIStations(Line_Id));
        }
        public ActionResult GetShownLineList(int Building_Id, int Segment_Id)
        {
            List<int> line_IdList = new List<int>();
            if(Session["ShownLineList"] != null) line_IdList = (List<int>)Session["ShownLineList"];
            ShownLineConfig shownline = new ShownLineConfig();
            return Content(shownline.GetShownLineListJson(Building_Id, Segment_Id, line_IdList));
        }
        public ActionResult UpdateSessionLine()
        {
            string updateData = Request.Form[0];
            ShownLineConfig shownline = new ShownLineConfig();
            Session["ShownLineList"] = shownline.UpdateShownLineList(updateData) ;
            return Content("Update successed");
        }
        public ActionResult GetProjects(int Segment_Id)
        {
            CommonDropDownData dropdownData = new CommonDropDownData();
            return Content(dropdownData.getProjects(Segment_Id));
        }
        public ActionResult GetSPIModels(int Project_Id) 
        {
            CommonDropDownData dropdownData = new CommonDropDownData();
            return Content(dropdownData.getSPIModels(Project_Id));
        }
    }
}
