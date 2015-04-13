using System;
using System.Collections.Generic;
using System.Web;
using System.Web.Mvc;
using ISPC.Services.FactoryRelated;

namespace ISPC.Controllers
{
    public class JsonBUController : Controller
    {
        //
        // GET: /BU/

        public ActionResult Index()
        {
            return View();
        }
        public ActionResult GetBUs(int Building_Id, int Segment_Id)
        {
            BUConfig buconfig = new BUConfig();
            return Content(buconfig.GetBUsJson(Building_Id, Segment_Id));
        }
        public ActionResult AddBU()
        {
            string newData = Request.Form[0];
            BUConfig buconfig = new BUConfig();
            return Content(buconfig.AddBUsJson(newData));
        }
        public ActionResult UpdateBU()
        {
            string updateData = Request.Form[0];
            BUConfig buconfig = new BUConfig();
            return Content(buconfig.UpdateBUJson(updateData));
        }
        public ActionResult DeleteBU(int BU_Id)
        {           
            BUConfig buconfig = new BUConfig();
            return Content(buconfig.DeleteBUJson(BU_Id));           
        }
    }
}
