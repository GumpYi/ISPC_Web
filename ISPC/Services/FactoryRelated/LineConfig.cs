using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Text;
using System.Linq.Dynamic;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json;
using ISPC.EntityFramework;
using ISPC.Models;
namespace ISPC.Services.FactoryRelated
{
    public class LineConfig
    {
        public string GetLineListJson(int Building_Id, int Segment_Id, int pageSize, int pageIndex)
        {
            using (ISPCEntities entity = new ISPCEntities())
            {
                List<LineInfo> lineList = new List<LineInfo>();
                StringBuilder selectCondition = new StringBuilder("1==1");
                if (Building_Id != 0) selectCondition.Append("And Segment.Building_Id ==" + Building_Id);
                if (Segment_Id != 0) selectCondition.Append("And Segment_Id ==" + Segment_Id);
                var lines = entity.Lines
                    .Where(selectCondition.ToString())
                    .OrderBy(l => l.Line_Id)
                    .Skip(pageSize * (pageIndex - 1))
                    .Take(pageSize);
                foreach (var line in lines)
                {
                    lineList.Add(LineInfo.FromLine(line));
                }
                IsoDateTimeConverter dtConverter = new IsoDateTimeConverter { DateTimeFormat = "yyyy-MM-dd HH:mm" };
                return(JsonConvert.SerializeObject(lineList, dtConverter));
            }
        }

        public string GetLine(int Line_Id)
        {
            using (ISPCEntities entity = new ISPCEntities())
            {
                LineInfo line = new LineInfo();
                IsoDateTimeConverter dtConverter = new IsoDateTimeConverter { DateTimeFormat = "yyyy-MM-dd HH:mm" };
                return (JsonConvert.SerializeObject(line, dtConverter));
            } 
        }
    }
}