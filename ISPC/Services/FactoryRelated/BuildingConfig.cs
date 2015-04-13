using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using ISPC.EntityFramework;
using ISPC.Models;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json;
using WebMatrix.WebData;

namespace ISPC.Services.FactoryRelated
{
    public class BuildingConfig
    {
        public string GetBuildingsJson()
        {
            using (ISPCEntities entity = new ISPCEntities())
            {
                List<BuildingInfo> buildingList = new List<BuildingInfo>();
                var buildings = from building in entity.Buildings.OrderBy(building => building.Building_Id) select building;
                foreach (var item in buildings)
                {
                    buildingList.Add(BuildingInfo.FromBuilding(item));
                }
                IsoDateTimeConverter dtConverter = new IsoDateTimeConverter { DateTimeFormat = "yyyy-MM-dd HH:mm" };
                return(JsonConvert.SerializeObject(buildingList, dtConverter));
            }
        }

        public string UpdateBuildingJson(string updateData)
        {
            BuildingInfo buildingInfo = JsonConvert.DeserializeObject<BuildingInfo>(updateData);
            using (ISPCEntities entity = new ISPCEntities())
            {
                try
                {
                    var building = entity.Buildings.Where(b => b.Building_Id == buildingInfo.Building_Id).FirstOrDefault();
                    building.Building_Name = buildingInfo.Building_Name;
                    building.Effect_Time = buildingInfo.Effect_Time;
                    building.Is_Active = buildingInfo.Is_Active;
                    // need to explain that EF would check which property had changed before executd updating opertion 
                    //and only changed the field which is inconsistent with original.
                    entity.SaveChanges();
                    return("Execute successful!");
                }
                catch (Exception ex)
                {
                    return(ex.Message);
                    throw;
                }
            }
        }

        public string AddBuildingJson(string newData)
        {
            using (ISPCEntities entity = new ISPCEntities())
            {
                try
                {
                    BuildingInfo buildingInfo = JsonConvert.DeserializeObject<BuildingInfo>(newData);
                    var building = new Building
                    {
                        Building_Name = buildingInfo.Building_Name.Trim(),
                        Is_Active = buildingInfo.Is_Active,
                        Creator_Id = WebSecurity.CurrentUserId,
                        Creation_Time = DateTime.Now,
                        Effect_Time = buildingInfo.Effect_Time
                    };
                    entity.Buildings.Add(building);
                    entity.SaveChanges();
                    return("Execute successful! ");
                }
                catch (Exception ex)
                {
                    return(ex.Message);
                }
            }
        }

        public string DeleteBuildingJson(int Building_Id)
        {
            using (ISPCEntities entity = new ISPCEntities())
            {
                try
                {
                    var building = entity.Buildings.Where(b => b.Building_Id == Building_Id).Take(1).FirstOrDefault();
                    entity.Buildings.Remove(building);
                    entity.SaveChanges();
                    return("Delete Successful!");
                }
                catch (Exception)
                {
                    return("Please confirm this building inner is null");
                }
            }
        }
    }
}