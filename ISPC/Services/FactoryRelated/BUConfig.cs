using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using ISPC.EntityFramework;
using ISPC.Models;
using System.Text;
using Newtonsoft.Json.Converters;
using System.Linq.Dynamic;
using Newtonsoft.Json;
using WebMatrix.WebData;
namespace ISPC.Services.FactoryRelated
{
    public class BUConfig
    {
        public string GetBUsJson(int Building_Id, int Segment_Id)
        {
            using (ISPCEntities entity = new ISPCEntities())
            {
                List<BUInfo> buList = new List<BUInfo>();
                StringBuilder selectCondition = new StringBuilder("1 == 1");
                if (Building_Id != 0) selectCondition.Append(" And Segment.Building_Id ==" + Building_Id);
                if (Segment_Id != 0) selectCondition.Append(" And Segment_Id ==" + Segment_Id);
                var segments = entity.BusinessUnits.Where(selectCondition.ToString()).OrderBy(bu => bu.BU_Id);
                foreach (var segment in segments)
                {
                    buList.Add(BUInfo.FromBU(segment));
                }
                IsoDateTimeConverter dtConverter = new IsoDateTimeConverter { DateTimeFormat = "yyyy-MM-dd HH:mm" };
                return (JsonConvert.SerializeObject(buList, dtConverter));
            }
        }

        public string AddBUsJson(string newData)
        {
            using (ISPCEntities entity = new ISPCEntities())
            {
                try
                {
                    BUInfo buInfo = JsonConvert.DeserializeObject<BUInfo>(newData);
                    var bu = new BusinessUnit
                    {
                        BU_Name = buInfo.BU_Name,
                        Is_Active = buInfo.Is_Active,
                        Segment_Id = buInfo.Segment_Id,
                        Creator_Id = WebSecurity.CurrentUserId,
                        Creation_Time = DateTime.Now,
                        Effect_Time = buInfo.Effect_Time
                    };
                    entity.BusinessUnits.Add(bu);
                    entity.SaveChanges();
                    return("Execute successful");
                }
                catch (Exception ex)
                {
                    return(ex.Message);
                }
            }
        }

        public string UpdateBUJson(string updateData)
        {
            using (ISPCEntities entity = new ISPCEntities())
            {
                try
                {
                    BUInfo buInfo = JsonConvert.DeserializeObject<BUInfo>(updateData);
                    var bu = entity.BusinessUnits.FirstOrDefault(BU => BU.BU_Id == buInfo.BU_Id);
                    bu.BU_Name = buInfo.BU_Name;
                    bu.Is_Active = buInfo.Is_Active;
                    bu.Segment_Id = buInfo.Segment_Id;
                    bu.Effect_Time = buInfo.Effect_Time;
                    entity.SaveChanges();
                    return("Execute successful");
                }
                catch (Exception ex)
                {
                    return(ex.Message);
                }
            }
        }

        public string DeleteBUJson(int BU_Id)
        {
            using (ISPCEntities entity = new ISPCEntities())
            {
                try
                {
                    var bu = entity.BusinessUnits.Where(BU => BU.BU_Id == BU_Id).Take(1).FirstOrDefault();
                    entity.BusinessUnits.Remove(bu);
                    entity.SaveChanges();
                    return("Delete Successful");
                }
                catch (Exception ex)
                {
                    return(ex.Message);
                }
            }
        }
    }
}