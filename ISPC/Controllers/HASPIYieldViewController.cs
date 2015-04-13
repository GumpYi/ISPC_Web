using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ISPC.Services.HARelated;

namespace ISPC.Controllers
{
    public class HASPIYieldViewController : Controller
    {
        //
        // GET: /HASPIYieldView/

        public ActionResult Index()
        {
            return View();
        }
        /// <summary>
        ///  Get the single YieldViewData
        /// </summary>
        /// <param name="StationId"></param>
        /// <param name="ModelId"></param>
        /// <param name="StartTime"></param>
        /// <param name="EndTime"></param>
        /// <param name="TimeBy"></param>
        /// <param name="CategroyBy"></param>
        /// <returns></returns>
        public ActionResult GetYieldViewData(int StationId, int ModelId, string StartTime, string EndTime, string TimeBy, string CategroyBy)
        {
            DateTime startTime = DateTime.Parse(StartTime);
            DateTime endTime = DateTime.Parse(EndTime);
            TimeBy timeBy;
            CategoryBy categoryBy;
            Enum.TryParse(TimeBy, out timeBy);
            Enum.TryParse(CategroyBy, out categoryBy);
            if (categoryBy == CategoryBy.Time) ModelId = 0;
            HASPIYieldView hASPIYieldView = new HASPIYieldView(StationId, ModelId, startTime, endTime, timeBy, categoryBy);
            return Content(hASPIYieldView.GetYieldViewData());
            
        }

        public ActionResult GetMultiYieldView()
        {

            DateTime startTime = DateTime.Parse(Request.Form[0]);
            DateTime endTime = DateTime.Parse(Request.Form[1]);
            string[] flag = Request.Form[2].Split(',');
            int[] LineIdList = new int[flag.Length];
            for (int i = 0; i < flag.Length; i++)
            {
                LineIdList[i] = int.Parse(flag[i]);
            }
            string TimeBy = Request.Form[3];
            TimeBy timeBy;
            Enum.TryParse(TimeBy, out timeBy);
            HASPIMultiYieldView hASPIMultiYieldView = new HASPIMultiYieldView(startTime, endTime, LineIdList, timeBy);            
            return Content(hASPIMultiYieldView.GetMultiYieldViewData());
        }
    }
}
