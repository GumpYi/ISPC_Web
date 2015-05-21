using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using ISPC.EntityFramework;
using Newtonsoft.Json;
using ISPC.Properties;

namespace ISPC.Services.HARelated
{
    public struct TempGroupComps
    {
        public string Component;
        public string Algorithm;
        public string PartNumber;
        public string DefectType;
        public string Result;
        public int Count;
    }

    public struct DrilldownData
    {
        /// <summary>
        /// Component Name/Algorithm/PartNumber/
        /// </summary>
        public string X;

        /// <summary>
        /// Related Counts 
        /// </summary>
        public int Y;
        
        /// <summary>
        /// 
        /// </summary>
        public IDictionary<string, int> SonData;
       
    }

    public sealed class HAAOIDefectView : HADefectView
    {
        #region
        [JsonIgnore]
        private IList<TempGroupComps> TempGroupCompsList { get; set; }

        public IDictionary<string, double> DefectTypeCountPercent { get; private set; }

        public int TotalPanelCounts { get; private set; }

        public int GoodPanelCounts { get; private set; }

        public int NGPanelCounts { get; private set; }

        public int FalseCallPanelCounts { get; private set; }

        public int TotalDefectCompCounts { get; private set; }

        public IList<DrilldownData> CompGroupDrilldownDataList { get; private set; }

        public IList<DrilldownData> PartNumberGroupDrilldownDataList { get; private set; }

        public IList<DrilldownData> AlgorithmDrilldownDataList { get; private set; }

        public IList<DrilldownData> FeederGroupDrilldownDataList { get; private set; }
        #endregion

        public HAAOIDefectView(int stationId, DateTime startTime, DateTime endTime, int modelId)
            : base(stationId, startTime, endTime, modelId) 
        {
            
        }

        protected override void GetBasicDefectViewData()
        {
            using (ISPCEntities entity = new ISPCEntities())
            {
                var panelGroupList = entity.AOI_Panel
                    .Where(p => p.Station_Id == this.StationId
                    && p.Start_Time >= this.StartTime
                    && p.End_Time <= this.EndTime
                    && p.Model_Id == ModelId
                    )
                    .GroupBy(p => p.Result.Result_Name)
                    .Select(g => new
                    {
                        Result = g.Key,
                        Count = g.Count()
                    }).ToList();
                this.TotalPanelCounts = panelGroupList.Sum(p => p.Count);
                this.GoodPanelCounts = panelGroupList.Where(p => p.Result == Settings.Default.AOIPanelResult_Good).Select(p => p.Count).FirstOrDefault();
                this.FalseCallPanelCounts = panelGroupList.Where(p => p.Result == Settings.Default.AOIPanelResult_DF || p.Result == Settings.Default.AOIPanelResult_Repaired).Select(p => p.Count).FirstOrDefault();
                this.NGPanelCounts = panelGroupList.Where(p => p.Result == Settings.Default.AOIPanelResult_Failed).Select(p => p.Count).FirstOrDefault();
                this.TotalDefectCompCounts = this.TempGroupCompsList.Where(c => c.Result == Settings.Default.AOIPanelResult_Failed).Sum(c => c.Count);
                var defectCountList = this.TempGroupCompsList
                    .Where(c => c.Result == Settings.Default.AOIPanelResult_Failed)
                    .GroupBy(c => c.DefectType)
                    .Select(g =>
                        new
                        {
                            g.Key,
                            Count = g.Sum(c => c.Count)
                        });
                IDictionary<string, double> defectTypeCountPercent = new Dictionary<string, double>();
                foreach (var defect in defectCountList)
                {
                    defectTypeCountPercent.Add(defect.Key, defect.Count / this.TotalDefectCompCounts*100);                                    
                }
                this.DefectTypeCountPercent = defectTypeCountPercent;
            }
        }

        private void LoadTempCompGoupList()
        {
            using (ISPCEntities entity = new ISPCEntities())
            {
                IList<TempGroupComps> tempGroupCompsList = new List<TempGroupComps>();
                var compGroupList = entity.AOI_Component
                    .Where(c => c.AOI_Board.AOI_Panel.Station_Id == this.StationId
                        && c.AOI_Board.AOI_Panel.Start_Time >= this.StartTime
                        && c.AOI_Board.AOI_Panel.End_Time <= this.EndTime
                        && c.AOI_Board.AOI_Panel.Model_Id == ModelId
                        )
                    .GroupBy(c => new { c.Component_Name, c.Defect_Type.Defect_Type_Name, c.Algorithm, c.PartNumber, c.Result.Result_Name })
                    .Select(g => new
                    {
                        Component = g.Key.Component_Name,
                        Algorithm = g.Key.Algorithm,
                        PartNumber = g.Key.PartNumber,
                        DefectType = g.Key.Defect_Type_Name,
                        Result = g.Key.Result_Name,
                        Count = g.Count()
                    }).ToList();
                foreach (var compGroup in compGroupList)
                {
                    TempGroupComps tempGroupComps;
                    tempGroupComps.Algorithm = compGroup.Algorithm;
                    tempGroupComps.Component = compGroup.Component;
                    tempGroupComps.PartNumber = compGroup.PartNumber;
                    tempGroupComps.DefectType = compGroup.DefectType;
                    tempGroupComps.Count = compGroup.Count;
                    tempGroupComps.Result = compGroup.Result;
                    tempGroupCompsList.Add(tempGroupComps);
                }
                this.TempGroupCompsList = tempGroupCompsList;
            }
        }

        public override string GetDefectViewDataCollection()
        {
            this.LoadTempCompGoupList();
            this.GetTopTenDefectCountsByComp();
            this.GetTopTenDefectCountsByAlgorithm();
            this.GetTopTenDefectCountsByPartNumer();
            this.GetBasicDefectViewData();
            return (JsonConvert.SerializeObject(this));
        }

        private void GetTopTenDefectCountsByComp()
        {
            IList<DrilldownData> compGroupDrilldownDataList = new List<DrilldownData>();
            var topTenDefectCountsByComp = (from data in this.TempGroupCompsList
                                            group data by data.Component
                                                into g
                                                select new
                                                {
                                                    CompName = g.Key,
                                                    Count = g.Sum(data => data.Count)
                                                }).OrderByDescending(g => g.Count).Take(10).ToList();
            foreach (var data in topTenDefectCountsByComp)
            {
                var topFiveCountsByDefectInSelectedComp = (from rawdata in this.TempGroupCompsList
                                                           where rawdata.Component == data.CompName
                                                           group rawdata by rawdata.DefectType
                                                               into g
                                                               select new
                                                               {
                                                                   DefectType = g.Key,
                                                                   Count = g.Sum(gData => gData.Count)
                                                               }).OrderByDescending(g => g.Count).Take(5).ToDictionary(c=>c.DefectType, c=>c.Count);
                DrilldownData drilldownData;
                drilldownData.X = data.CompName;
                drilldownData.Y = data.Count;
                drilldownData.SonData = topFiveCountsByDefectInSelectedComp;
                compGroupDrilldownDataList.Add(drilldownData);
            }
            this.CompGroupDrilldownDataList = compGroupDrilldownDataList;
        }

        private void GetTopTenDefectCountsByPartNumer()
        {
            IList<DrilldownData> partNumberGroupDrilldownDataList = new List<DrilldownData>();
            var topTenDefectCountsByType = (from data in this.TempGroupCompsList
                                            group data by data.PartNumber
                                                into g
                                                select new
                                                {
                                                    Type = g.Key,
                                                    Count = g.Sum(data => data.Count)
                                                }).OrderByDescending(g => g.Count).Take(10).ToList();
            foreach (var data in topTenDefectCountsByType)
            {
                var topFiveCountsByDefectInSelectedType = (from rawdata in this.TempGroupCompsList
                                                           where rawdata.PartNumber == data.Type
                                                           group rawdata by rawdata.DefectType
                                                               into g
                                                               select new
                                                               {
                                                                   DefectType = g.Key,
                                                                   Count = g.Sum(gData => gData.Count)
                                                               }).OrderByDescending(g => g.Count).Take(5).ToDictionary(c => c.DefectType, c => c.Count);
                DrilldownData drilldownData;
                drilldownData.X = data.Type;
                drilldownData.Y = data.Count;
                drilldownData.SonData = topFiveCountsByDefectInSelectedType;
                partNumberGroupDrilldownDataList.Add(drilldownData);
            }
            this.PartNumberGroupDrilldownDataList = partNumberGroupDrilldownDataList;
        }

        private void GetTopTenDefectCountsByAlgorithm()
        {
            IList<DrilldownData> algorithmGroupDrilldownDataList = new List<DrilldownData>();
            var topTenDefectCountsByType = (from data in this.TempGroupCompsList
                                            group data by data.Algorithm
                                                into g
                                                select new
                                                {
                                                    Type = g.Key,
                                                    Count = g.Sum(data => data.Count)
                                                }).OrderByDescending(g => g.Count).Take(10).ToList();
            foreach (var data in topTenDefectCountsByType)
            {
                var topFiveCountsByDefectInSelectedType = (from rawdata in this.TempGroupCompsList
                                                           where rawdata.Algorithm == data.Type
                                                           group rawdata by rawdata.DefectType
                                                               into g
                                                               select new
                                                               {
                                                                   DefectType = g.Key,
                                                                   Count = g.Sum(gData => gData.Count)
                                                               }).OrderByDescending(g => g.Count).Take(5).ToDictionary(c => c.DefectType, c => c.Count);
                DrilldownData drilldownData;
                drilldownData.X = data.Type;
                drilldownData.Y = data.Count;
                drilldownData.SonData = topFiveCountsByDefectInSelectedType;
                algorithmGroupDrilldownDataList.Add(drilldownData);
            }
            this.AlgorithmDrilldownDataList = algorithmGroupDrilldownDataList;
        }

    }
}