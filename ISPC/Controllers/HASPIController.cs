using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace ISPC.Controllers
{
    public class HASPIController : Controller
    {
        //
        // GET: /HASPI/

        public ActionResult Index()
        {
            return View();
        }
        /// <summary>
        /// Access to the SPIDefectView
        /// </summary>
        /// <returns>The view</returns>
        public ActionResult DefectView()
        {
            return View();
        }

        public ActionResult MultiYieldView()
        {
            return View();
        }

        /// <summary>
        /// Access to the SPIMultiDefectView
        /// </summary>
        /// <returns></returns>
        public ActionResult MultiDefectView()
        {
            return View();
        }

        /// <summary>
        /// Access to the Flex customized function charts
        /// </summary>
        /// <returns></returns>
        public ActionResult Overview()
        {
            return View();
        }

        public ActionResult YieldView()
        {
            return View();
        }

    }
}
