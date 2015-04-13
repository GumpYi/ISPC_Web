using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ISPC.Services.HARelated;

namespace ISPC.Controllers
{
    public class HASPIDefectViewController : Controller
    {
        //
        // GET: /HASPIDefectView/

        public ActionResult Index()
        {
            return View();
        }
      
        public ActionResult GetDefectViewDataCollection(int Station_Id, string StartTime, string EndTime, int Model_Id)
        {
            DateTime startTime = DateTime.Parse(StartTime);
            DateTime endTime = DateTime.Parse(EndTime);
            HASPIDefectView haSPIDefectView = new HASPIDefectView(Station_Id, startTime, endTime, Model_Id);            
            return Content(haSPIDefectView.GetDefectViewDataCollection());
        }

        public ActionResult GetMultiDefectViewDataCollection(int BuildingId, int SegmentId, string StartTime, string EndTime)
        {
            DateTime startTime = DateTime.Parse(StartTime);
            DateTime endTime = DateTime.Parse(EndTime);
            HASPIMultiDefectView haSPIMultiDefectView = new HASPIMultiDefectView(BuildingId, SegmentId, startTime, endTime);
            return Content(haSPIMultiDefectView.GetTopTenLineDefectCountListByJSON());
        }
    }
}
