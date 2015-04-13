using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using ISPC.EntityFramework;
using ISPC.Properties;
using Newtonsoft.Json;
using ISPC.Models.HASPI;

namespace ISPC.Services.HARelated
{
    public struct DefectAndComp
    {
        public string ComponentName;
        public string DefectType;
    }
    public class HASPIDefectView
    {
        public HASPIDefectView(int StationId, DateTime StartTime, DateTime EndTime, int ModelId)
        {
            this.StationId = StationId;
            this.StartTime = StartTime;
            this.EndTime = EndTime;
            this.ModelId = ModelId;
            this.ComponentDetailList = new List<HASPIComponent>();
            this.TopFiveExcessiveComp = new Dictionary<string, int>();
            this.TopFiveBridgingComp = new Dictionary<string, int>();
            this.TopFiveHighAreaComp = new Dictionary<string, int>();
            this.TopFiveInsufficientComp = new Dictionary<string, int>();
            this.TopFiveLowAreaComp = new Dictionary<string, int>();
            this.TopFiveLowerHeightComp = new Dictionary<string, int>();
            this.TopFivePositionComp = new Dictionary<string, int>();
            this.TopFiveShapeComp = new Dictionary<string, int>();
            this.TopFiveSmearComp = new Dictionary<string, int>();
            this.TopFiveUpperHeightComp = new Dictionary<string, int>();
        }
        #region property define
        public int StationId { get; set; }
        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }
        public int ModelId { get; set; }
        public Dictionary<string, float> DefectTypeCountPercent { get; set; }
        public int TotalPanelCounts { get; set; }
        public int GoodPanelCounts { get; set; }
        public int NGPanelCounts { get; set; }
        public int PassPanelCounts { get; set; }
        public int WarningPanelCounts { get; set; }
        public int TotalDefectPadCounts { get; set; }
        public List<HASPIComponent> ComponentDetailList { get; set; }
        public Dictionary<string, int> TopFiveExcessiveComp { get; set; }
        public Dictionary<string, int> TopFiveInsufficientComp { get; set; }
        public Dictionary<string, int> TopFiveHighAreaComp { get; set; }
        public Dictionary<string, int> TopFiveLowAreaComp { get; set; }
        public Dictionary<string, int> TopFiveUpperHeightComp { get; set; }
        public Dictionary<string, int> TopFiveLowerHeightComp { get; set; }
        public Dictionary<string, int> TopFivePositionComp { get; set; }
        public Dictionary<string, int> TopFiveBridgingComp { get; set; }
        public Dictionary<string, int> TopFiveSmearComp { get; set; }
        public Dictionary<string, int> TopFiveShapeComp { get; set; }
        private List<DefectAndComp> TempSelectedSPIPadDataList { get; set; }
        #endregion

        public string GetDefectViewDataCollection()
        {
            this.TempSelectedSPIPadDataList = new List<DefectAndComp>();
            this.GetCountByTopTenLocationWithDetail();
            this.GetDefectTypesPercentData();
            this.GetBasicDefectViewData();
            this.GetTopFiveCompByDefectType();
            return (JsonConvert.SerializeObject(this));
        }

        private void GetBasicDefectViewData()
        {
            using (ISPCEntities entity = new ISPCEntities())
            {
                var PanelTypeCountList = (from panel in entity.SPI_Panel
                                          where panel.Station_Id == this.StationId
                                          && panel.Start_Time >= this.StartTime
                                          && panel.End_Time <= this.EndTime
                                          && panel.Model_Id == this.ModelId
                                          group panel by panel.Result.Result_Name
                                              into p
                                              select new
                                              {
                                                  Result = p.Key,
                                                  Count = p.Count()
                                              }).ToList();
                foreach (var panelType in PanelTypeCountList)
                {
                    this.TotalPanelCounts += panelType.Count;
                    if (panelType.Result == Settings.Default.SPI_Panel_Result_Good)
                    {
                        this.GoodPanelCounts = panelType.Count;
                    }
                    else if (panelType.Result == Settings.Default.SPI_Panel_Result_NG)
                    {
                        this.NGPanelCounts = panelType.Count;
                    }
                    else if (panelType.Result == Settings.Default.SPI_Panel_Result_Pass)
                    {
                        this.PassPanelCounts = panelType.Count;
                    }
                }
            }
        }

        private void GetCountByTopTenLocationWithDetail()
        {
            using (ISPCEntities entity = new ISPCEntities())
            {
                int[] selectedPanelIdArray = (from panel in entity.SPI_Panel
                                              where panel.Station_Id == this.StationId                                             
                                              && panel.Model_Id == this.ModelId
                                               && panel.Start_Time >= this.StartTime
                                              && panel.End_Time <= this.EndTime
                                              && panel.Result.Result_Name == Settings.Default.SPI_Panel_Result_NG
                                              select panel.Panel_Id).ToArray();

                var tempSelectedSPIPadDataList = (from pad in entity.SPI_Pad
                                                  where selectedPanelIdArray.Contains(pad.SPI_Board.Panel_Id)
                                                  && pad.Defect_Type.Defect_Type_Name != Settings.Default.SPISuccess
                                                  select
                                                  new
                                                  {
                                                      ComponentName = pad.Component_Name,
                                                      DefectType = pad.Defect_Type.Defect_Type_Name
                                                  }).ToList();
                

                foreach (var data in tempSelectedSPIPadDataList)
                {
                    DefectAndComp defectAndComp;
                    defectAndComp.ComponentName = data.ComponentName;
                    defectAndComp.DefectType = data.DefectType;
                    this.TempSelectedSPIPadDataList.Add(defectAndComp);
                }
                this.TotalDefectPadCounts = this.TempSelectedSPIPadDataList.Count();
                var TopTenComponentCountList = (from pad in this.TempSelectedSPIPadDataList
                                                group pad by pad.ComponentName
                                                    into p
                                                    select new
                                                    {
                                                        ComponentName = p.Key,
                                                        Count = p.Count()
                                                    }).OrderByDescending(P => P.Count).Take(10);

                List<string> ComponentTemp = new List<string>();
                foreach (var CompCount in TopTenComponentCountList)
                {
                    ComponentTemp.Add(CompCount.ComponentName);
                }

                var TopTenComponentWithDetail = (from pad in this.TempSelectedSPIPadDataList
                                                 where ComponentTemp.Contains(pad.ComponentName)
                                                 group pad by new { ComponentName = pad.ComponentName, DefectType = pad.DefectType}
                                                     into p
                                                     select new
                                                     {
                                                         ComponentName = p.Key.ComponentName,
                                                         DefectType = p.Key.DefectType,
                                                         Count = p.Count()
                                                     }).ToList();
                foreach (string ComponentName in ComponentTemp)
                {
                    HASPIComponent Component = new HASPIComponent();
                    var selectComponent = from comp in TopTenComponentWithDetail
                                          where comp.ComponentName == ComponentName
                                          select comp;
                    Component.ComponentName = ComponentName;
                    foreach (var rowData in selectComponent)
                    {
                        switch (rowData.DefectType)
                        {
                            case "Excessive":
                                Component.ExcessiveCounts = rowData.Count;
                                break;
                            case "Insufficient":
                                Component.InsufficientCounts = rowData.Count;
                                break;
                            case "Shape":
                                Component.ShapeCounts = rowData.Count;
                                break;
                            case "Position":
                                Component.PositionCounts = rowData.Count;
                                break;
                            case "Bridging":
                                Component.BridgingCounts = rowData.Count;
                                break;
                            case "UpperHeight":
                                Component.UpperHeightCounts = rowData.Count;
                                break;
                            case "LowerHeight":
                                Component.LowerHeightCounts = rowData.Count;
                                break;
                            case "HighArea":
                                Component.HighAreaCounts = rowData.Count;
                                break;
                            case "LowArea":
                                Component.LowAreaCounts = rowData.Count;
                                break;
                            case "Smear":
                                Component.SmearCounts = rowData.Count;
                                break;
                        }//close the switch
                    }//close the one component defectType
                    this.ComponentDetailList.Add(Component);
                }//close the top ten component
            }
        }

        private void GetDefectTypesPercentData()
        {

            var DTCountList = (from pad in this.TempSelectedSPIPadDataList
                               group pad by pad.DefectType
                                   into p
                                   select new
                                   {
                                       DefectType = p.Key,
                                       Count = p.Count()
                                   }).ToList();
            Dictionary<string, float> DefectTypeCount = new Dictionary<string, float>();
            int defectTypeCountSum = 0;
            foreach (var DTCount in DTCountList)
            {
                defectTypeCountSum += DTCount.Count;
            }

            foreach (var DTCount in DTCountList)
            {
                DefectTypeCount.Add(DTCount.DefectType, ((float)DTCount.Count / defectTypeCountSum) * 100);
            }
            this.DefectTypeCountPercent = DefectTypeCount;
        }

        private void GetTopFiveCompByDefectType()
        {

            var compCountsByDefectTypeList = (from pad in this.TempSelectedSPIPadDataList
                                              group pad by new { DefectType = pad.DefectType, ComponentName = pad.ComponentName }
                                                  into p
                                                  select new
                                                  {
                                                      DefectType = p.Key.DefectType,
                                                      ComponentName = p.Key.ComponentName,
                                                      Count = p.Count()
                                                  }).ToList();
            // Get Top 5 Excessive Component's pad num
            var compCountsByExcessive = (from rowdata in compCountsByDefectTypeList
                                         where rowdata.DefectType == Settings.Default.SPIExcessive
                                         orderby rowdata.Count descending
                                         select rowdata)
                                        .Take(5);
            foreach (var rowdata in compCountsByExcessive)
            {
                this.TopFiveExcessiveComp.Add(rowdata.ComponentName, rowdata.Count);
            }

            // Get Top 5 Insufficient Component's pad num
            var compCountsByInsufficient = (from rowdata in compCountsByDefectTypeList
                                            where rowdata.DefectType == Settings.Default.SPIInsufficient
                                            orderby rowdata.Count descending
                                            select rowdata)
                                        .Take(5);
            foreach (var rowdata in compCountsByInsufficient)
            {
                this.TopFiveInsufficientComp.Add(rowdata.ComponentName, rowdata.Count);
            }

            // Get Top 5 HighArea Component's pad num
            var compCountsByHighArea = (from rowdata in compCountsByDefectTypeList
                                        where rowdata.DefectType == Settings.Default.SPIHighArea
                                        orderby rowdata.Count descending
                                        select rowdata)
                                        .Take(5);
            foreach (var rowdata in compCountsByHighArea)
            {
                this.TopFiveHighAreaComp.Add(rowdata.ComponentName, rowdata.Count);
            }

            // Get Top 5 LowArea Component's pad num
            var compCountsByLowArea = (from rowdata in compCountsByDefectTypeList
                                       where rowdata.DefectType == Settings.Default.SPILowArea
                                       orderby rowdata.Count descending
                                       select rowdata)
                                        .Take(5);
            foreach (var rowdata in compCountsByLowArea)
            {
                this.TopFiveLowAreaComp.Add(rowdata.ComponentName, rowdata.Count);
            }

            // Get Top 5 UpperHeight Component's pad num
            var compCountsByUpperHeight = (from rowdata in compCountsByDefectTypeList
                                           where rowdata.DefectType == Settings.Default.SPIUpperHeight
                                           orderby rowdata.Count descending
                                           select rowdata)
                                        .Take(5);
            foreach (var rowdata in compCountsByUpperHeight)
            {
                this.TopFiveUpperHeightComp.Add(rowdata.ComponentName, rowdata.Count);
            }

            // Get Top 5 LowerHeight Component's pad num
            var compCountsByLowerHeight = (from rowdata in compCountsByDefectTypeList
                                           where rowdata.DefectType == Settings.Default.SPILowerHeight
                                           orderby rowdata.Count descending
                                           select rowdata)
                                        .Take(5);
            foreach (var rowdata in compCountsByLowerHeight)
            {
                this.TopFiveLowerHeightComp.Add(rowdata.ComponentName, rowdata.Count);
            }

            // Get Top 5 Position Component's pad num
            var compCountsByPosition = (from rowdata in compCountsByDefectTypeList
                                        where rowdata.DefectType == Settings.Default.SPIPosition
                                        orderby rowdata.Count descending
                                        select rowdata)
                                        .Take(5);
            foreach (var rowdata in compCountsByPosition)
            {
                this.TopFivePositionComp.Add(rowdata.ComponentName, rowdata.Count);
            }

            // Get Top 5 Bridging Component's pad num
            var compCountsByBridging = (from rowdata in compCountsByDefectTypeList
                                        where rowdata.DefectType == Settings.Default.SPIBridging
                                        orderby rowdata.Count descending
                                        select rowdata)
                                        .Take(5);
            foreach (var rowdata in compCountsByBridging)
            {
                this.TopFiveBridgingComp.Add(rowdata.ComponentName, rowdata.Count);
            }

            // Get Top 5 Smear Component's pad num
            var compCountsBySmear = (from rowdata in compCountsByDefectTypeList
                                     where rowdata.DefectType == Settings.Default.SPISmear
                                     orderby rowdata.Count descending
                                     select rowdata)
                                        .Take(5);
            foreach (var rowdata in compCountsBySmear)
            {
                this.TopFiveSmearComp.Add(rowdata.ComponentName, rowdata.Count);
            }

            // Get Top 5 Shape Component's pad num
            var compCountsByShape = (from rowdata in compCountsByDefectTypeList
                                     where rowdata.DefectType == Settings.Default.SPIShape
                                     orderby rowdata.Count descending
                                     select rowdata)
                                        .Take(5);
            foreach (var rowdata in compCountsByShape)
            {
                this.TopFiveShapeComp.Add(rowdata.ComponentName, rowdata.Count);
            }
        }

    }
}