using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using ISPC.EntityFramework;
using ISPC.Utils;
using System.Linq.Dynamic;
using ISPC.Properties;
using System.Text;
using Newtonsoft.Json;

namespace ISPC.Services.HARelated
{
    public class HASPIYieldView
    {
        #region Property declaration
        [JsonIgnore]
        public int StationId { get; set; }
        [JsonIgnore]
        public DateTime StartTime { get; set; }
        [JsonIgnore]
        public DateTime EndTime { get; set; }
        [JsonIgnore]
        public int ModelId { get; set; }
        [JsonIgnore]
        public TimeBy timeBy { get; set; }
        [JsonIgnore]
        public CategoryBy categoryBy { get; set; }
        public Dictionary<string, Dictionary<string, double>> FPYBySelectedSection { get; set; }
        public Dictionary<string, float> DPPMBySelectedSection { get; set; }
        #endregion

        public HASPIYieldView(int StationId, int ModelId, DateTime StartTime, DateTime EndTime, TimeBy timeBy, CategoryBy categoryBy)
        {
            this.StationId = StationId;
            this.ModelId = ModelId;
            this.StartTime = StartTime;
            this.EndTime = EndTime;
            this.timeBy = timeBy;
            this.categoryBy = categoryBy;
            this.FPYBySelectedSection = new Dictionary<string, Dictionary<string, double>>();
            this.DPPMBySelectedSection = new Dictionary<string, float>();
        }

        public string GetYieldViewData()
        {
            if (this.categoryBy == CategoryBy.TimeAndModel && this.ModelId == 0)
            {
                this.GetRelatedFPYListByTimeWithMultiModel();
            }
            else
            {
                this.GetRelatedFPYListByTimeWithSimpleModelOrNoModel();
            }
            this.GetRelatedDPPMListByTime();
            if (this.timeBy != TimeBy.Week)
            {
                this.OptimizeDicStructure();    
            }   
            return (JsonConvert.SerializeObject(this));
        }

        private void OptimizeDicStructure()
        {
            string filter = string.Empty;
            this.GetDateTimeFilter(ref filter);
            Dictionary<string, Dictionary<string, double>> tempFpyBySelectedSection  = new Dictionary<string, Dictionary<string, double>>();
            Dictionary<string, float> tempDPPMBySelectedSection = new Dictionary<string, float>();
            foreach (var modelDataList in this.FPYBySelectedSection)
            {
                DateTime startTimeByTimeBy = DateTime.ParseExact(this.StartTime.ToString(filter), filter, System.Globalization.CultureInfo.InvariantCulture);
                DateTime endTimeByTimeBy = DateTime.ParseExact(this.EndTime.ToString(filter), filter, System.Globalization.CultureInfo.InvariantCulture);           
                while (startTimeByTimeBy <= endTimeByTimeBy)
                {
                    if(!modelDataList.Value.ContainsKey(startTimeByTimeBy.ToString(filter)))
                    {
                        modelDataList.Value.Add(startTimeByTimeBy.ToString(filter), 0.0);                          
                    }
                    switch (this.timeBy)
                    { 
                        case TimeBy.Hour:
                            startTimeByTimeBy = startTimeByTimeBy.AddHours(1);
                            break;
                        case TimeBy.Day:
                            startTimeByTimeBy = startTimeByTimeBy.AddDays(1);
                            break;
                        case TimeBy.Month:
                            startTimeByTimeBy = startTimeByTimeBy.AddMonths(1);
                            break;
                    }
                }
                Dictionary<string, double> dicOrderInter = new Dictionary<string, double>();
                dicOrderInter = modelDataList.Value.OrderBy(m => DateTime.ParseExact(m.Key, filter, System.Globalization.CultureInfo.InvariantCulture)).ToDictionary(m => m.Key, m => m.Value);
                tempFpyBySelectedSection.Add(modelDataList.Key, dicOrderInter);
            }
            DateTime startTimeByTimeBy_DPPM = DateTime.ParseExact(this.StartTime.ToString(filter), filter, System.Globalization.CultureInfo.InvariantCulture);
            DateTime endTimeByTimeBy_DPPM = DateTime.ParseExact(this.EndTime.ToString(filter), filter, System.Globalization.CultureInfo.InvariantCulture);
            while (startTimeByTimeBy_DPPM <= endTimeByTimeBy_DPPM)
            {
                if (!this.DPPMBySelectedSection.ContainsKey(startTimeByTimeBy_DPPM.ToString(filter)))
                {
                    this.DPPMBySelectedSection.Add( startTimeByTimeBy_DPPM.ToString(filter), 0);
                }
                switch (this.timeBy)
                {
                    case TimeBy.Hour:
                        startTimeByTimeBy_DPPM = startTimeByTimeBy_DPPM.AddHours(1);
                        break;
                    case TimeBy.Day:
                        startTimeByTimeBy_DPPM = startTimeByTimeBy_DPPM.AddDays(1);
                        break;
                    case TimeBy.Month:
                        startTimeByTimeBy_DPPM = startTimeByTimeBy_DPPM.AddMonths(1);
                        break;
                }
            }

            tempDPPMBySelectedSection = this.DPPMBySelectedSection.OrderBy(m => DateTime.ParseExact(m.Key, filter, System.Globalization.CultureInfo.InvariantCulture)).ToDictionary(m => m.Key, m => m.Value);
            this.DPPMBySelectedSection = tempDPPMBySelectedSection;
            this.FPYBySelectedSection = tempFpyBySelectedSection;
        }

        private void GetRelatedFPYListByTimeWithMultiModel()
        {
            using (ISPCEntities entity = new ISPCEntities())
            {
                var panelCountListByResult = (from panel in entity.SPI_Panel
                                              where panel.Start_Time >= this.StartTime
                                              && panel.End_Time <= this.EndTime
                                              && panel.Station_Id == this.StationId
                                              select new
                                              {
                                                  panel.Panel_Id,
                                                  panel.Model_Id,
                                                  panel.Result.Result_Name,
                                                  panel.Start_Time,
                                                  panel.End_Time,
                                                  panel.Total_Pads_Num,
                                                  panel.Error_Pads_Num
                                              }).ToList();
                var modelIdList = from rowData in panelCountListByResult
                                  group rowData by rowData.Model_Id
                                      into r
                                      select r.Key;
                foreach (var data in modelIdList)
                {
                    string ModelName = entity.Models.Where(m => m.Model_Id == data).Select(m => m.Model_Name).Take(1).FirstOrDefault();
                    var selectedModelResultList = from panel in panelCountListByResult where panel.Model_Id == data select panel;
                    Dictionary<string, double> temp = new Dictionary<string, double>();
                    if (this.timeBy == TimeBy.Week)
                    {
                        List<WeekOfTimeSection> weekOfTimeSectionList = CalculateWeeks.GetWeekOfTimeSection(this.StartTime, this.EndTime);
                        // get the FPY by Week based on selected time

                        foreach (var weekOfTimeSection in weekOfTimeSectionList)
                        {
                            DateTime startTimeTemp = weekOfTimeSection.StartTime;
                            DateTime endTimeTemp = weekOfTimeSection.EndTime;
                            var panelCountByWeek = (from rowData in selectedModelResultList
                                                    where rowData.Start_Time >= startTimeTemp
                                                    && rowData.End_Time <= endTimeTemp
                                                    group rowData by rowData.Result_Name
                                                        into r
                                                        select new
                                                        {
                                                            Result = r.Key,
                                                            Count = r.Count(),
                                                        }).ToList();

                            int goodPanels = panelCountByWeek.Where(r => r.Result != Settings.Default.SPI_Panel_Result_NG).Select(r => r.Count).FirstOrDefault();
                            int PanelsSum = panelCountByWeek.Sum(r => r.Count);

                            temp.Add(startTimeTemp.ToString("MM-dd") + " To " + endTimeTemp.ToString("MM-dd"),
                            PanelsSum != 0 ? Math.Round((double)goodPanels / PanelsSum, 4) * 100 : 0);
                        }
                    }
                    else
                    {
                        string filter = string.Empty;
                        this.GetDateTimeFilter(ref filter);
                        var flag = (from rowData in selectedModelResultList
                                    group rowData by rowData.Start_Time.ToString(filter)
                                        into r
                                        select new
                                        {
                                            Time = r.Key,
                                            GoodPanelsNum = r.Count(panel => panel.Result_Name != Settings.Default.SPI_Panel_Result_NG),
                                            TotalPanelsNum = r.Count()
                                        }).ToList();
                        foreach (var rowData in flag)
                        {
                            temp.Add(rowData.Time, Math.Round((double)rowData.GoodPanelsNum / rowData.TotalPanelsNum, 4) * 100);
                        }
                    }
                    this.FPYBySelectedSection.Add(ModelName, temp);
                }
            }
        }

        private void GetRelatedFPYListByTimeWithSimpleModelOrNoModel()
        {
            using (ISPCEntities entity = new ISPCEntities())
            {
                string ModelName = "Only By Time";
                StringBuilder selectCondition = new StringBuilder("1 == 1");
                selectCondition.Append(" And Station_Id ==" + this.StationId);
                if (this.ModelId != 0)
                {
                    ModelName = entity.Models.Where(m => m.Model_Id == this.ModelId).Select(m => m.Model_Name).Take(1).FirstOrDefault();
                    selectCondition.Append(" And Model_Id ==" + this.ModelId);
                    
                } 
                var panelCountListByResult = entity.SPI_Panel.Where(selectCondition.ToString()).Where(panel => (panel.Start_Time >= this.StartTime && panel.End_Time <= this.EndTime)).Select(panel => new
                {
                    panel.Panel_Id,
                    panel.Result.Result_Name,
                    panel.Start_Time,
                    panel.End_Time,
                    panel.Total_Pads_Num,
                    panel.Error_Pads_Num
                }).ToList();

                Dictionary<string, double> temp = new Dictionary<string, double>();

                if (this.timeBy == TimeBy.Week)
                {
                    List<WeekOfTimeSection> weekOfTimeSectionList = CalculateWeeks.GetWeekOfTimeSection(StartTime, EndTime);
                    // get the FPY by Week based on selected time
                    foreach (var weekOfTimeSection in weekOfTimeSectionList)
                    {
                        DateTime startTimeTemp = weekOfTimeSection.StartTime;
                        DateTime endTimeTemp = weekOfTimeSection.EndTime;
                        var panelCountByWeek = from rowData in panelCountListByResult
                                               where rowData.Start_Time >= startTimeTemp
                                               && rowData.End_Time <= endTimeTemp
                                               group rowData by rowData.Result_Name
                                                   into r
                                                   select new
                                                   {
                                                       Result = r.Key,
                                                       Count = r.Count(),
                                                   };
                        int goodPanels = panelCountByWeek.Where(r => r.Result != Settings.Default.SPI_Panel_Result_NG).Select(r => r.Count).FirstOrDefault();
                        int PanelsSum = panelCountByWeek.Sum(r => r.Count);

                        temp.Add(startTimeTemp.ToString("MM-dd") + " To " + endTimeTemp.ToString("MM-dd"), 
                            PanelsSum != 0?Math.Round((double)goodPanels / PanelsSum, 4) * 100 : 0);
                    }
                }
                else
                {
                    string filter = string.Empty;
                    this.GetDateTimeFilter(ref filter);
                    var flag = (from rowData in panelCountListByResult
                                group rowData by rowData.Start_Time.ToString(filter)
                                    into r
                                    select new
                                    {
                                        Time = r.Key,
                                        GoodPanelsNum = r.Count(panel => panel.Result_Name != Settings.Default.SPI_Panel_Result_NG),
                                        TotalPanelsNum = r.Count()
                                    }).ToList();
                    foreach (var data in flag)
                    {
                        temp.Add(data.Time, Math.Round((double)data.GoodPanelsNum / data.TotalPanelsNum, 4) * 100);
                    }
                }
                this.FPYBySelectedSection.Add(ModelName, temp);
            }
        }
        private void GetRelatedDPPMListByTime()
        {
            using (ISPCEntities entity = new ISPCEntities())
            { 
                var tempdata = (from panel in entity.SPI_Panel
                                where panel.Start_Time >= StartTime && panel.End_Time <= EndTime && panel.Station_Id == this.StationId
                                select new { panel.Panel_Id, panel.Start_Time, panel.End_Time, panel.Total_Pads_Num, panel.Error_Pads_Num, panel.Result.Result_Name }).ToList();
                if (this.timeBy == TimeBy.Week)
                {
                    List<WeekOfTimeSection> weekOfTimeSectionList = CalculateWeeks.GetWeekOfTimeSection(StartTime, EndTime);
                    foreach (var weekOfTimeSection in weekOfTimeSectionList)
                    {
                        DateTime startTimeTemp = weekOfTimeSection.StartTime;
                        DateTime endTimeTemp = weekOfTimeSection.EndTime;
                        var flag = tempdata.Where(rowdata => rowdata.Start_Time >= startTimeTemp && rowdata.End_Time <= endTimeTemp);
                        int total = flag.Sum(data => data.Total_Pads_Num);
                        int error = flag.Where(data => data.Result_Name == Settings.Default.SPI_Panel_Result_NG).Sum(data => data.Error_Pads_Num);
                        this.DPPMBySelectedSection.Add(startTimeTemp.ToString("MM-dd") + " To " + endTimeTemp.ToString("MM-dd"), total != 0 ?((float)error/total)*100*100*100 : 0);                       
                    }
                }
                else
                {                                 
                    string filter = string.Empty;
                    this.GetDateTimeFilter(ref filter);                    
                    var flag = (from panel in tempdata                            
                                group panel by panel.Start_Time.ToString(filter)
                                    into g
                                    select new
                                    {
                                        g.Key,
                                        DPPM = (g.Where(data => data.Result_Name == Settings.Default.SPI_Panel_Result_NG).Sum(panel => panel.Error_Pads_Num) / (float)g.Sum(panel => panel.Total_Pads_Num)) * 1000000
                                    }).ToList();
                    foreach (var data in flag)
                    {
                        this.DPPMBySelectedSection.Add(data.Key, data.DPPM);
                    }
                }
            }   

            }
    
        private void GetDateTimeFilter(ref string filter)
        {
           
            switch (this.timeBy)
            {
                case TimeBy.Hour:
                    filter = "yyyy-MM-dd HH";
                    break;
                case TimeBy.Day:
                    filter = "yyyy-MM-dd";
                    break;
                case TimeBy.Month:
                    filter = "yyyy-MM";
                    break;
            }            
        }
    }

    public enum TimeBy
    {
        Hour,
        Day,
        Week,
        Month
    }

    public enum CategoryBy
    {
        Time,
        TimeAndModel
    }
}