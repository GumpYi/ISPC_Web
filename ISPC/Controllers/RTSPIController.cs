using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ISPC.Services.RealTimeRelated;
using ISPC.Models.RealTimeCache;
using ISPC.Services.SessionRelated;
using Newtonsoft.Json;
using System.Diagnostics;

namespace ISPC.Controllers
{
    public class RTSPIController : Controller
    {
        //
        // GET: /RTSPI/

        public ActionResult Index()
        {
            ViewBag.Station_Id = 0;
           
            return View();
        }
        public ActionResult GetSPIRTData()
        {            
            int Station_Id = int.Parse(Request.Form[0]);
            string[] selectedCpkComp = Request.Form[1].Split(',');
            string[] slectedXBarComp = Request.Form[2].Split(',');
            RTSPIService target = new RTSPIService();           
            SPIRangeData data = Session["SPIRangeData"] == null ? SPIRangeConfig.FetchSPIRangeFromSetting() : (SPIRangeData)Session["SPIRangeData"];
            string a = target.GetSPIRTDataJson(Station_Id, selectedCpkComp, slectedXBarComp, data);                     
            return Content(a);
        }
        public ActionResult GetSPIRange()
        {
            SPIRangeData data = Session["SPIRangeData"] == null ? SPIRangeConfig.FetchSPIRangeFromSetting() : (SPIRangeData)Session["SPIRangeData"];
            return Content(JsonConvert.SerializeObject(data));
        }
        public ActionResult UpdateSPIRange()
        {
            string updateData = Request.Form[0];
            Session["SPIRangeData"] = JsonConvert.DeserializeObject<SPIRangeData>(updateData);
            return Content("Update successful");
        }
        public ActionResult GetCompList(int Station_Id)
        {
            RTSPIService service = new RTSPIService();
            return Content(service.GetComponentList(Station_Id));
        }
    }
}
