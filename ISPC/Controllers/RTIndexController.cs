using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ISPC.Models.RealTimeCache;
using Newtonsoft.Json;
using ISPC.Services.RealTimeRelated;
using ISPC.Services.SessionRelated;

namespace ISPC.Controllers
{
    public class RTIndexController : Controller
    {      
        //
        // GET: /RTIndex/

        public ActionResult Index()
        {
            return View();
        }
        public ActionResult RefreshIndexData()
        {            
            List<int> Line_IdList = (List<int>)Session["ShownLineList"];
            if (Line_IdList != null && Line_IdList.Count != 0)
            {
                RTIndexService rtIndexService = new RTIndexService();
               
                return Content(rtIndexService.GetRTLinesJson(Line_IdList, DateTime.Now));
            }
            else 
            {
                return Content("");
            }
            
        }
        public ActionResult GetULCLSetting()
        {
            if(Session["ULCLSetting"] == null)
            {
                Session["ULCLSetting"] = ULCLConfig.FetchULCLFromSetting();              
            }
            return Content((string)Session["ULCLSetting"]);
        }
        public ActionResult UpdateULCLSetting()
        {
            string updateData = Request.Form[0];
            Session["ULCLSetting"] = updateData;
            return Content("Update successful");
        }
    }
}
