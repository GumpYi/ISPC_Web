using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ISPC.Services.FactoryRelated;
namespace ISPC.Web.Controllers
{
    public class JsonLineController : Controller
    {
        //
        // GET: /JsonLine/

        public ActionResult Index()
        {
            return View();
        }
        public ActionResult GetLines(int Building_Id, int Segment_Id, int pageSize, int pageIndex)
        {
            LineConfig lineconfig = new LineConfig();
            return Content(lineconfig.GetLineListJson(Building_Id, Segment_Id, pageSize, pageIndex));
        }       
    }
}
