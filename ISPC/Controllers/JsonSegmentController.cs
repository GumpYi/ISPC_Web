using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ISPC.Services.FactoryRelated;
using ISPC.EntityFramework;
namespace ISPC.Web.Controllers
{
    public class JsonSegmentController : Controller
    {
        //
        // GET: /JsonSegment/

        public ActionResult Index()
        {
            return View();
        }      
        public ActionResult GetSegments(int Building_Id)
        {
            SegmentConfig segmentconfig = new SegmentConfig();
            return Content(segmentconfig.GetSegments(Building_Id));
        }
        public ActionResult AddSegment()
        {
            string newData = Request.Form[0];
            SegmentConfig segmentconfig = new SegmentConfig();
            return Content(segmentconfig.AddSegment(newData));
        }
        public ActionResult UpdateSegment()
        {
            string updateData = Request.Form[0];
            SegmentConfig segmentconfig = new SegmentConfig();
            return Content(segmentconfig.UpdateSegment(updateData));
        }
        public ActionResult DeleteSegment(int Segment_Id)
        {
            SegmentConfig segmentconfig = new SegmentConfig();
            return Content(segmentconfig.DeleteSegment(Segment_Id));
        }
    }
}
