using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Newtonsoft.Json;
using ISPC.EntityFramework;
using System.Text;
using System.Linq.Dynamic;
using ISPC.Utils;
using ISPC.Properties;
namespace ISPC.Services.HARelated
{
    public class HAAOIYieldView: HAYieldView
    {
        #region Property declaration
        public Dictionary<string, float> DPMOBySelectedSection { get; set; }
        public Dictionary<string, float> FPPMBySelectedSection { get; set; }
        #endregion

        public HAAOIYieldView(int StationId, int ModelId, DateTime StartTime, DateTime EndTime, TimeBy timeBy, CategoryBy categoryBy)
        {
            this.StationId = StationId;
            this.ModelId = ModelId;
            this.StartTime = StartTime;
            this.EndTime = EndTime;
            this.timeBy = timeBy;
            this.categoryBy = categoryBy;
            this.FPYBySelectedSection = new Dictionary<string, Dictionary<string, double>>();
            this.DPMOBySelectedSection = new Dictionary<string, float>();
            this.FPPMBySelectedSection = new Dictionary<string, float>();
        }
        public override string GetYieldViewData()
        {
            if (this.categoryBy == CategoryBy.TimeAndModel && this.ModelId == 0)
            {
                this.GetRelatedFPYListByTimeWithMultiModel();
            }
            else
            {
                this.GetRelatedFPYListByTimeWithSimpleModelOrNoModel();
            }
            this.GetRelatedDPMOAndFPPMListByTime();
            if (this.timeBy != TimeBy.Week)
            {
                this.OptimizeDicStructure();
            }
            return (JsonConvert.SerializeObject(this));
        }

        protected override void OptimizeDicStructure()
        {
            string filter = string.Empty;
            this.GetDateTimeFilter(ref filter);
            Dictionary<string, Dictionary<string, double>> tempFpyBySelectedSection = new Dictionary<string, Dictionary<string, double>>();
            Dictionary<string, float> tempDPMOBySelectedSection = new Dictionary<string, float>();
            Dictionary<string, float> tempFPPMBySelectedSection = new Dictionary<string, float>();

            #region optimized the this.FPYBySelectedSection
            foreach (var modelDataList in this.FPYBySelectedSection)
            {
                DateTime startTimeByTimeBy = DateTime.ParseExact(this.StartTime.ToString(filter), filter, System.Globalization.CultureInfo.InvariantCulture);
                DateTime endTimeByTimeBy = DateTime.ParseExact(this.EndTime.ToString(filter), filter, System.Globalization.CultureInfo.InvariantCulture);
                while (startTimeByTimeBy <= endTimeByTimeBy)
                {
                    if (!modelDataList.Value.ContainsKey(startTimeByTimeBy.ToString(filter)))
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
            #endregion

            #region optimized the this.DPMOSelectedSection
            DateTime startTimeByTimeBy_DPMO = DateTime.ParseExact(this.StartTime.ToString(filter), filter, System.Globalization.CultureInfo.InvariantCulture);
            DateTime endTimeByTimeBy_DPMO = DateTime.ParseExact(this.EndTime.ToString(filter), filter, System.Globalization.CultureInfo.InvariantCulture);
            while (startTimeByTimeBy_DPMO <= endTimeByTimeBy_DPMO)
            {
                if (!this.DPMOBySelectedSection.ContainsKey(startTimeByTimeBy_DPMO.ToString(filter)))
                {
                    this.DPMOBySelectedSection.Add(startTimeByTimeBy_DPMO.ToString(filter), 0);
                }
                switch (this.timeBy)
                {
                    case TimeBy.Hour:
                        startTimeByTimeBy_DPMO = startTimeByTimeBy_DPMO.AddHours(1);
                        break;
                    case TimeBy.Day:
                        startTimeByTimeBy_DPMO = startTimeByTimeBy_DPMO.AddDays(1);
                        break;
                    case TimeBy.Month:
                        startTimeByTimeBy_DPMO = startTimeByTimeBy_DPMO.AddMonths(1);
                        break;
                }
            }
            #endregion

            #region optimized the this.FPPMSelectedSection
                DateTime startTimeByTimeBy_FPPM = DateTime.ParseExact(this.StartTime.ToString(filter), filter, System.Globalization.CultureInfo.InvariantCulture);
                DateTime endTimeByTimeBy_FPPM = DateTime.ParseExact(this.EndTime.ToString(filter), filter, System.Globalization.CultureInfo.InvariantCulture);
                while (startTimeByTimeBy_FPPM <= endTimeByTimeBy_FPPM)
                {
                    if (!this.FPPMBySelectedSection.ContainsKey(startTimeByTimeBy_FPPM.ToString(filter)))
                    {
                        this.FPPMBySelectedSection.Add(startTimeByTimeBy_FPPM.ToString(filter), 0);
                    }
                    switch (this.timeBy)
                    {
                        case TimeBy.Hour:
                            startTimeByTimeBy_FPPM = startTimeByTimeBy_FPPM.AddHours(1);
                            break;
                        case TimeBy.Day:
                            startTimeByTimeBy_FPPM = startTimeByTimeBy_FPPM.AddDays(1);
                            break;
                        case TimeBy.Month:
                            startTimeByTimeBy_FPPM = startTimeByTimeBy_FPPM.AddMonths(1);
                            break;
                    }
                }
            #endregion
            
            tempDPMOBySelectedSection = this.DPMOBySelectedSection.OrderBy(m => DateTime.ParseExact(m.Key, filter, System.Globalization.CultureInfo.InvariantCulture)).ToDictionary(m => m.Key, m => m.Value);
            tempFPPMBySelectedSection = this.FPPMBySelectedSection.OrderBy(m => DateTime.ParseExact(m.Key, filter, System.Globalization.CultureInfo.InvariantCulture)).ToDictionary(m => m.Key, m => m.Value);
            this.DPMOBySelectedSection = tempDPMOBySelectedSection;
            this.FPPMBySelectedSection = tempFPPMBySelectedSection;
            this.FPYBySelectedSection = tempFpyBySelectedSection;       
        }

        protected override void GetRelatedFPYListByTimeWithMultiModel()
        {
            using (ISPCEntities entity = new ISPCEntities())
            {
                var panelCountListByResult = (from panel in entity.AOI_Panel
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
                                                  panel.Total_Comps_Num,
                                                  panel.Indict_Comps_Num
                                              }).ToList();
                var modelIdList = from rowData in panelCountListByResult
                                  group rowData by rowData.Model_Id
                                      into r
                                      select r.Key;
                foreach (var ModelId in modelIdList)
                {
                    string ModelName = entity.Models.Where(m => m.Model_Id == ModelId)
                        .Select(m => m.Model_Name).Take(1).FirstOrDefault();
                    var selectedModelResultList = from panel in panelCountListByResult where panel.Model_Id == ModelId select panel;
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

                            int badPanels = panelCountByWeek.Where(r => r.Result == Settings.Default.AOIPanelResult_Failed)
                            .Select(r => r.Count).Take(1).FirstOrDefault();
                            int PanelsSum = panelCountByWeek.Sum(r => r.Count);
                            temp.Add(startTimeTemp.ToString("MM-dd") + " To " + endTimeTemp.ToString("MM-dd"),
                                PanelsSum != 0 ? Math.Round((double)(PanelsSum - badPanels) / PanelsSum, 4) * 100 : 0);
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
                                            BadPanelsNum = r.Count(panel => panel.Result_Name == Settings.Default.AOIPanelResult_Failed),
                                            TotalPanelsNum = r.Count()
                                        }).ToList();
                        foreach (var data in flag)
                        {
                            temp.Add(data.Time, data.TotalPanelsNum != 0 ? Math.Round((double)(data.TotalPanelsNum - data.BadPanelsNum) / data.TotalPanelsNum, 4) * 100 : 0);
                        }
                    }
                    this.FPYBySelectedSection.Add(ModelName, temp);
                }
            }
        }

        protected override void GetRelatedFPYListByTimeWithSimpleModelOrNoModel()
        {
            using (ISPCEntities entity = new ISPCEntities())
            {
                string ModelName = "Only By Time";
                StringBuilder selectCondition = new StringBuilder("1 == 1");
                selectCondition.Append("And Station_Id =="+this.StationId);
                if (this.ModelId != 0)
                {
                    ModelName = entity.Models
                        .Where(m => m.Model_Id == this.ModelId)
                        .Select(m => m.Model_Name)
                        .Take(1)
                        .FirstOrDefault();
                    selectCondition.Append(" And Model_Id =="+this.ModelId);
                }
                var panelCountListByResult = entity.AOI_Panel.Where(selectCondition.ToString())
                    .Where(panel => (panel.Start_Time >= this.StartTime && panel.End_Time <= this.EndTime))
                    .Select(panel => new
                    {
                        panel.Panel_Id,
                        panel.Result.Result_Name,
                        panel.Start_Time,
                        panel.End_Time,
                        panel.Total_Comps_Num,
                        panel.Indict_Comps_Num
                    }).ToList();
                Dictionary<string, double> temp = new Dictionary<string, double>();
                if (this.timeBy == TimeBy.Week)
                {
                    List<WeekOfTimeSection> weekOfTimeSectionList = CalculateWeeks.GetWeekOfTimeSection(this.StartTime, this.EndTime);
                    // Get the FPY by week based on selected time
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
                        int badPanels = panelCountByWeek.Where(r => r.Result == Settings.Default.AOIPanelResult_Failed)
                            .Select(r => r.Count).Take(1).FirstOrDefault();
                        int PanelsSum = panelCountByWeek.Sum(r => r.Count);
                        temp.Add(startTimeTemp.ToString("MM-dd") + " To " + endTimeTemp.ToString("MM-dd"),
                            PanelsSum != 0 ? Math.Round((double)(PanelsSum - badPanels) / PanelsSum, 4) * 100 : 0);
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
                                        BadPanelsNum = r.Count(panel => panel.Result_Name == Settings.Default.AOIPanelResult_Failed),
                                        TotalPanelsNum = r.Count()
                                    }).ToList();
                    foreach (var data in flag)
                    {
                        temp.Add(data.Time, data.TotalPanelsNum != 0 ? Math.Round((double)(data.TotalPanelsNum - data.BadPanelsNum) / data.TotalPanelsNum, 4) * 100 : 0);
                    }
                }
                this.FPYBySelectedSection.Add(ModelName, temp);
            }
        }

        private void GetRelatedDPMOAndFPPMListByTime()
        {
            using (ISPCEntities entity = new ISPCEntities())
            {
                var tempData = (from panel in entity.AOI_Panel
                                where panel.Start_Time >= this.StartTime && panel.End_Time <= this.EndTime && panel.Station_Id == this.StationId
                                select new
                                {
                                    panel.Panel_Id,
                                    panel.Start_Time,
                                    panel.End_Time,
                                    panel.Result.Result_Name,
                                    panel.Total_Comps_Num,
                                    panel.Indict_Comps_Num,
                                    panel.FalseCall_Comps_Num
                                }).ToList();
                if (this.timeBy == TimeBy.Week)
                {
                    List<WeekOfTimeSection> weekOfTimeSelectionList = CalculateWeeks.GetWeekOfTimeSection(StartTime, EndTime);
                    foreach (var weekOfTimeSelection in weekOfTimeSelectionList)
                    {
                        DateTime startTimeTemp = weekOfTimeSelection.StartTime;
                        DateTime endTimeTemp = weekOfTimeSelection.EndTime;
                        var flag = tempData.Where(rowdata => rowdata.Start_Time >= startTimeTemp && rowdata.End_Time <= endTimeTemp);
                        int total = (int)flag.Sum(data => data.Total_Comps_Num);
                        int error = (int)flag.Sum(data => data.Indict_Comps_Num);
                        int falseCall = (int)flag.Sum(data => data.FalseCall_Comps_Num);
                        this.DPMOBySelectedSection.Add(startTimeTemp.ToString("MM-dd") + " To " + endTimeTemp.ToString("MM-dd"),
                            total != 0 ? ((float)error / total) * 100 * 100 * 100 : 0);
                        this.FPPMBySelectedSection.Add(startTimeTemp.ToString("MM-dd") + " To " + endTimeTemp.ToString("MM-dd"),
                            total != 0 ? ((float)falseCall / total) * 100 * 100 * 100 : 0);
                    }
                }
                else
                {
                    string filter = string.Empty;
                    this.GetDateTimeFilter(ref filter);
                    var flag = (from panel in tempData
                                group panel by panel.Start_Time.ToString(filter)
                                    into g
                                    select new
                                    {
                                        g.Key,
                                        DPMO = (g.Sum(panel => panel.Indict_Comps_Num) / (float)g.Sum(panel => panel.Total_Comps_Num)) * 100 * 100 * 100,
                                        FPPM = (g.Sum(panel => panel.FalseCall_Comps_Num) / (float)g.Sum(panel => panel.Total_Comps_Num)) * 100 * 100 * 100
                                    }).ToList();
                    foreach (var data in flag)
                    {
                        this.DPMOBySelectedSection.Add(data.Key, (float)data.DPMO);
                        this.FPPMBySelectedSection.Add(data.Key, (float)data.FPPM);
                    }
                }
            }
        }      
    }
}