using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ISPC.Services.HARelated;

namespace ISPC.Controllers
{
    public class HAAOIDefectViewController : Controller
    {
        //
        // GET: /HAAOIDefectView/

        public ActionResult Index()
        {
            return View();
        }

        public ActionResult GetDefectViewDataCollection(int stationId, string startTime, string endTime, int modelId)
        {
            DateTime tempStartTime = DateTime.Parse(startTime);
            DateTime tempEndTime = DateTime.Parse(endTime);

            HADefectView hAAOIDefectView = new HAAOIDefectView(stationId, tempStartTime, tempEndTime, modelId);
            return Content(hAAOIDefectView.GetDefectViewDataCollection());
        }
    }
}
