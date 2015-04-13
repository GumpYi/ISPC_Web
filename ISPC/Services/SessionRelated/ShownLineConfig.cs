using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using ISPC.EntityFramework;
using System.Text;
using System.Linq.Dynamic;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json;
using ISPC.Models.RealTimeCache;
namespace ISPC.Services.SessionRelated
{
    public class ShownLineConfig
    {
        public string GetShownLineListJson(int Building_Id, int Segment_Id, List<int> line_IdList)
        {
            using (ISPCEntities entity = new ISPCEntities())
            {               
                StringBuilder selectCondition = new StringBuilder("1==1");
                if (Building_Id != 0) selectCondition.Append("And Segment.Building_Id ==" + Building_Id);
                if (Segment_Id != 0) selectCondition.Append("And Segment_Id ==" + Segment_Id);
                var lines = entity.Lines
                    .Where(selectCondition.ToString())
                    .OrderBy(l => l.Line_Id)
                    .Select(line => new { line.Line_Id, line.Line_Name, line.Line_Location, Status = line_IdList.Contains(line.Line_Id) ? true : false });
                IsoDateTimeConverter dtConverter = new IsoDateTimeConverter { DateTimeFormat = "yyyy-MM-dd HH:mm" };
                return(JsonConvert.SerializeObject(lines, dtConverter));
            }
        }
        public List<int> UpdateShownLineList(string updateData)
        {
            List<SessionShownLine> list = JsonConvert.DeserializeObject<List<SessionShownLine>>(updateData);
            List<int> ShownLine_IdList = new List<int>();
            foreach (var line in list)
            {
                if (line.Status)
                {
                    ShownLine_IdList.Add(line.Line_Id);
                }
            }
            return ShownLine_IdList;
        }
    }
}