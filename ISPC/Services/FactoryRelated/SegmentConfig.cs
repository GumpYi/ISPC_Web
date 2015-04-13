using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using ISPC.EntityFramework;
using ISPC.Models;
using System.Text;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json;
using System.Linq.Dynamic;
using WebMatrix.WebData;
namespace ISPC.Services.FactoryRelated
{
    public class SegmentConfig
    {
        public string GetSegments(int Building_Id)
        {
            using (ISPCEntities entity = new ISPCEntities())
            {
                List<SegmentInfo> segmentList = new List<SegmentInfo>();
                StringBuilder selectCondition = new StringBuilder("1 == 1");
                if (Building_Id != 0) selectCondition.Append("And Building_Id ==" + Building_Id);
                var segments = entity.Segments.Where(selectCondition.ToString()).OrderBy(segment => segment.Segment_Id);
                foreach (var segment in segments)
                {
                    segmentList.Add(SegmentInfo.FromSegment(segment));
                }
                IsoDateTimeConverter dtConverter = new IsoDateTimeConverter { DateTimeFormat = "yyyy-MM-dd HH:mm" };
                return(JsonConvert.SerializeObject(segmentList, dtConverter));
            }  
        }

        public string AddSegment(string newData)
        {
            using (ISPCEntities entity = new ISPCEntities())
            {
                try
                {
                    SegmentInfo segmentInfo = JsonConvert.DeserializeObject<SegmentInfo>(newData);
                    var segment = new Segment
                    {
                        Segment_Name = segmentInfo.Segment_Name,
                        Segment_Code = segmentInfo.Segment_Code,
                        Building_Id = segmentInfo.Building_Id,
                        Is_Active = segmentInfo.Is_Active,
                        Creator_Id = WebSecurity.CurrentUserId,
                        Creation_Time = DateTime.Now,
                        Effect_Time = segmentInfo.Effect_Time
                    };
                    entity.Segments.Add(segment);
                    entity.SaveChanges();
                    return("Execute successful");
                }
                catch (Exception ex)
                {
                    return(ex.Message);
                }
            }
        }

        public string UpdateSegment(string updateData)
        {
            using (ISPCEntities entity = new ISPCEntities())
            {
                try
                {
                    SegmentInfo segmentInfo = JsonConvert.DeserializeObject<SegmentInfo>(updateData);
                    var segment = entity.Segments.FirstOrDefault(s => s.Segment_Id == segmentInfo.Segment_Id);
                    segment.Segment_Name = segmentInfo.Segment_Name;
                    segment.Segment_Code = segmentInfo.Segment_Code;
                    segment.Effect_Time = segmentInfo.Effect_Time;
                    segment.Is_Active = segmentInfo.Is_Active;
                    segment.Building_Id = segmentInfo.Building_Id;
                    entity.SaveChanges();
                    return("Execute successful");
                }
                catch (Exception ex)
                {
                    return(ex.Message);
                }
            }
        }

        public string DeleteSegment(int Segment_Id)
        {
            using (ISPCEntities entity = new ISPCEntities())
            {
                try
                {
                    var segment = entity.Segments.Where(s => s.Segment_Id == Segment_Id).Take(1).FirstOrDefault();
                    entity.Segments.Remove(segment);
                    entity.SaveChanges();
                    return("Delete Successful!");
                }
                catch (Exception ex)
                {
                    return(ex.Message);
                }
            }
        }
    }
}