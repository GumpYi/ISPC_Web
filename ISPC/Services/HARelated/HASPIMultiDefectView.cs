using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Newtonsoft.Json;
using ISPC.EntityFramework;
using System.Text;
using System.Linq.Dynamic;
using ISPC.Properties;
namespace ISPC.Services.HARelated
{
    public class HASPIMultiDefectView
    {

        #region property define

        public int BuildingId { get; set; }
        public int SegmentId { get; set; }
        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }
        public Dictionary<string, int> TopTenLineDefectCountList { get; set; }

        #endregion

        public HASPIMultiDefectView(int buildingId, int segmentId, DateTime startTime, DateTime endTime)
        {
            this.BuildingId = buildingId;
            this.SegmentId = segmentId;
            this.StartTime = startTime;
            this.EndTime = endTime;
            this.TopTenLineDefectCountList = new Dictionary<string, int>();
        }

        public string GetTopTenLineDefectCountListByJSON()
        {
            using (ISPCEntities entity = new ISPCEntities())
            {
                StringBuilder selectCondition = new StringBuilder("1==1");
                if (this.BuildingId != 0) selectCondition.Append("And Segment.Building_Id ==" + this.BuildingId);
                if (this.SegmentId != 0) selectCondition.Append("And Segment_Id ==" + this.SegmentId);
                int[] lines = entity.Lines
                    .Where(selectCondition.ToString())
                    .OrderBy(l => l.Line_Id)
                    .Select(line => line.Line_Id).ToArray();

                var defectCountListByLineId = (from panel in entity.SPI_Panel
                                               where lines.Contains(panel.Station.Line_Id)
                                               && panel.Start_Time >= this.StartTime
                                               && panel.End_Time <= this.EndTime
                                               && panel.Result.Result_Name == Settings.Default.SPI_Panel_Result_NG
                                               group panel by panel.Station.Line_Id
                                                   into p
                                                   select new
                                                   {
                                                       p.Key,
                                                       Count = p.Sum(panel => panel.Error_Pads_Num)
                                                   }).OrderByDescending(data => data.Count).Take(10).ToList();
                foreach (var data in defectCountListByLineId)
                {
                    this.TopTenLineDefectCountList.Add(entity.Lines
                        .Where(line => line.Line_Id == data.Key)
                        .Select(line => line.Line_Name).FirstOrDefault(),
                        data.Count);
                }
                return (JsonConvert.SerializeObject(this.TopTenLineDefectCountList));
            }
        }
    }
}