using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace ISPC.Controllers
{
    public class FactoryController : Controller
    {
        //
        // GET: /Factory/

        public ActionResult Index()
        {
            return View();
        }
        public ActionResult BuildingConfig()
        {
            return View();
        }
        public ActionResult LineList()
        {
            return View(); 
        }
        public ActionResult LineConfig(int Line_Id)
        {
            ViewBag.Line_Id = Line_Id;
            return View();
        }
        public ActionResult Machine()
        {
            return View();
        }
        public ActionResult MachineRelated()
        {
            return View();
        }
    }
}
