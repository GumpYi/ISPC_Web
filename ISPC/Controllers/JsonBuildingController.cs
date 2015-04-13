using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using WebMatrix.WebData;
using ISPC.Models;
using ISPC.EntityFramework;
using ISPC.Services.FactoryRelated;
namespace ISPC.Controllers
{
    public class JsonBuildingController : Controller
    {
        //
        // GET: /JsonFactory/

        public ActionResult Index()
        {
            return View();
        }
        /// <summary>
        /// get the BuildingList as json format
        /// </summary>
        /// <returns>JSON Format</returns>
        public ActionResult GetBuildings()
        {
            BuildingConfig buildingconfig = new BuildingConfig();
            return Content(buildingconfig.GetBuildingsJson());
        }
        /// <summary>
        /// Update a existing building
        /// </summary>
        /// <returns>message</returns>
        public ActionResult UpdateBuilding()
        {
            string updateData = Request.Form[0];
            BuildingConfig buildingconfig = new BuildingConfig();
            return Content(buildingconfig.UpdateBuildingJson(updateData));
        }
        /// <summary>
        /// Add a new Building
        /// </summary>
        /// <returns>message</returns>
        public ActionResult AddBuilding()
        {
            string newData = Request.Form[0];
            BuildingConfig buildingconfig = new BuildingConfig();
            return Content(buildingconfig.AddBuildingJson(newData));
        }
        /// <summary>
        /// delete a existing Building
        /// </summary>
        /// <returns></returns>
        public ActionResult DeleteBuilding(int Building_Id ) 
        {
            BuildingConfig buildingconfig = new BuildingConfig();
            return Content(buildingconfig.DeleteBuildingJson(Building_Id)); 
        }      
    }
}
