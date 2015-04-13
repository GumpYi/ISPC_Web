using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace ISPC.Controllers
{
    public class JsonMachineController : Controller
    {
        //
        // GET: /JsonMachine/

        public ActionResult Index()
        {
            return View();
        }
        public ActionResult GetSupplierList()
        {
            return Content("");
        }
    }
}
