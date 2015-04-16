using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ISPC.Services.HARelated;

namespace ISPC.Controllers
{
    public class HAAOIYieldViewController : Controller
    {
        //
        // GET: /HAAOIYieldView/

        public ActionResult Index()
        {
            return View();
        }

        public ActionResult GetYieldViewData(int StationId, int ModelId, string StartTime, string EndTime, string TimeBy, string CateBy)
        {
            DateTime startTime = DateTime.Parse(StartTime);
            DateTime endTime = DateTime.Parse(EndTime);
            TimeBy timeBy;
            CategoryBy categoryBy;
            Enum.TryParse(TimeBy, out timeBy);
            Enum.TryParse(CateBy, out categoryBy);
            if (categoryBy == CategoryBy.Time) ModelId = 0;
            HAYieldView hASPIYieldView = new HAAOIYieldView(StationId, ModelId, startTime, endTime, timeBy, categoryBy);
            return Content(hASPIYieldView.GetYieldViewData());   
        }
    }
}
