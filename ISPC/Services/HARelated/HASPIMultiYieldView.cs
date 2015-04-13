using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Newtonsoft.Json;
using ISPC.EntityFramework;
using ISPC.Properties;

namespace ISPC.Services.HARelated
{
    public class HASPIMultiYieldView
    {
        [JsonIgnore]
        public DateTime StartTime { get; set; }
        [JsonIgnore]
        public DateTime EndTime { get; set; }
        [JsonIgnore]
        public int[] LineIdList { get; set; }
        [JsonIgnore]
        public TimeBy TimeByObj { get; set; }

        public Dictionary<string, Dictionary<string, Yield>> DataList { get; set; }

        public HASPIMultiYieldView(DateTime StartTime, DateTime EndTime, int[] LineIdList, TimeBy TimeByObj)
        {
            this.StartTime = StartTime;
            this.EndTime = EndTime;
            this.LineIdList = LineIdList;
            this.TimeByObj = TimeByObj;
            this.DataList = new Dictionary<string, Dictionary<string, Yield>>();
        }

        public string GetMultiYieldViewData()
        { 
            using (ISPCEntities entity = new ISPCEntities())
            {

                var subStationList = (from station in entity.Stations
                                        where LineIdList.Contains(station.Line_Id) && station.Machine.Machine_Model.Machine_Type_Id == Settings.Default.SPI
                                        select new { station.Station_Id, station.Station_Name }).ToList();                
                foreach (var subStation in subStationList)
                {
                    HASPIYieldView hASPIYieldView = new HASPIYieldView(subStation.Station_Id, 0, this.StartTime, this.EndTime, this.TimeByObj, CategoryBy.Time);
                    hASPIYieldView.GetYieldViewData();
                    Dictionary<string, Yield> yieldDataList = new Dictionary<string, Yield>();
                    foreach (var data in hASPIYieldView.FPYBySelectedSection.FirstOrDefault().Value)
                    {
                        Yield yield;
                        yield.FPY = data.Value;
                        yield.DPPM = hASPIYieldView.DPPMBySelectedSection.Where(d => d.Key == data.Key).Select(d => d.Value).FirstOrDefault();
                        yieldDataList.Add(data.Key, yield);
                    }
                    this.DataList.Add(subStation.Station_Name, yieldDataList);
                }
                return (JsonConvert.SerializeObject(this));
            }
        }

      
    }

    public struct Yield
    {
        public double DPPM;
        public double FPY;
    }
}