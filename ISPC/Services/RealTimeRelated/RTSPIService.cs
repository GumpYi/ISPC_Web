using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using ISPC.Models.RealTimeCache;
using ISPC.EntityFramework;
using ISPC.Properties;
using Newtonsoft.Json;

namespace ISPC.Services.RealTimeRelated
{
    public struct CompListByModel
    {
        public int ModelId;
        public string ModelName;
        public string[] CompList;
        public string[] CpkCompList;
        public string[] XBarCompList;
    }

    public struct SelectedPadList
    {
        public int PanelId;
        public double Area;
        public double Height;
        public double Volume;
        public double XOffset;
        public double YOffset;
    }

    public struct SPIRTPanelInfo
    {
        public int PanelId;
        public DateTime StartTime;
        public string Squeegee;
        public string ModelName;
        public string PanelBarcode;
        public string Unit;
        public string[] CpkComps;
        public string[] XBarComps;
        public List<SPIRTPadInfo> RTPadInfoList;
    }

    public struct SPIRTPadInfo
    {
        public string CompsName;
        public double Area;
        public double Height;
        public double Volume;
        public double OffsetX;
        public double OffsetY;
    }

    public class RTSPIService
    {
        private List<SPIRTPanelInfo> SPIRTPanelInfoList { get; set; }

        public string GetSPIRTDataJson(int Station_Id, SPIRangeData data)
        {
            this.GetLastTenPanelList(Station_Id);
           
            List<float> lasttenpanelsvcpk = new List<float>();
            List<float> lasttenpanelshcpk = new List<float>();
            List<float> lasttenpanelsacpk = new List<float>();
            List<float> lasttenpanelsxocpk = new List<float>();
            List<float> lasttenpanelsyocpk = new List<float>();
                             
            //the first element in "lastTenPanelIdList is the data of last the first panel"
            foreach (var Panel in this.SPIRTPanelInfoList)
            {
                var selectedCpkPadList = (from pad in Panel.RTPadInfoList where Panel.CpkComps.Contains(pad.CompsName) select pad).ToList();
                double HeightAvg = selectedCpkPadList.Average(o => o.Height);
                double AreaAvg = selectedCpkPadList.Average(o => o.Area);
                double VolumeAvg = selectedCpkPadList.Average(o => o.Volume);
                double OffsetXAvg = selectedCpkPadList.Average(o => o.OffsetX);
                double OffsetYAvg = selectedCpkPadList.Average(o => o.OffsetY);
                double flagVolumeSum = 0;
                double flagAreaSum = 0;
                double flagHeightSum = 0;
                double flagOffsetXSum = 0;
                double flagOffsetYSum = 0;
                foreach (var selectedpad in selectedCpkPadList)
                {
                    flagVolumeSum += Math.Pow(selectedpad.Volume - VolumeAvg, 2);
                    flagAreaSum += Math.Pow(selectedpad.Area - AreaAvg, 2);
                    flagHeightSum += Math.Pow(selectedpad.Height - HeightAvg, 2);
                    flagOffsetXSum += Math.Pow(selectedpad.OffsetX - OffsetXAvg, 2);
                    flagOffsetYSum += Math.Pow(selectedpad.OffsetY - OffsetYAvg, 2);
                }
                // get this Panel SD

                lasttenpanelshcpk.Add(this.CpkFormula(HeightAvg, Math.Pow(flagHeightSum / selectedCpkPadList.Count(), 0.5), data.HUSL, data.HLSL));
                lasttenpanelsvcpk.Add(this.CpkFormula(VolumeAvg, Math.Pow(flagVolumeSum / selectedCpkPadList.Count(), 0.5), data.VUSL, data.VLSL));
                lasttenpanelsacpk.Add(this.CpkFormula(AreaAvg, Math.Pow(flagAreaSum / selectedCpkPadList.Count(), 0.5), data.AUSL, data.ALSL));
                lasttenpanelsxocpk.Add(this.CpkFormula(OffsetXAvg, Math.Pow(flagOffsetXSum / selectedCpkPadList.Count(), 0.5), data.XOUSL, data.YOUCL));
                lasttenpanelsyocpk.Add(this.CpkFormula(OffsetYAvg, Math.Pow(flagOffsetYSum / selectedCpkPadList.Count(), 0.5), data.YOUSL, data.YOLSL));
                   
            }
            Dictionary<string, List<double>> xBarACompDataList = new Dictionary<string, List<double>>();
            Dictionary<string, List<double>> xBarHCompDataList = new Dictionary<string, List<double>>();
            Dictionary<string, List<double>> xBarVCompDataList = new Dictionary<string, List<double>>();
            Dictionary<string, List<double>> xBarOXCompDataList = new Dictionary<string, List<double>>();
            Dictionary<string, List<double>> xBarOYCompDataList = new Dictionary<string, List<double>>();
            RTSPIStation rtspistation = new RTSPIStation();
            rtspistation.LastTenPanelsVCpk = lasttenpanelsvcpk;
            rtspistation.LastTenPanelsACpk = lasttenpanelsacpk;
            rtspistation.LastTenPanelsHCpk = lasttenpanelshcpk;
            rtspistation.LastTenPanelsXOCpk = lasttenpanelsxocpk;
            rtspistation.LastTenPanelsYOCpk = lasttenpanelsyocpk;
            rtspistation.LastTenPanelsScatterList = this.GetSPIRTPanelList();
            this.GetSPIRTCompXBarList(
                out xBarACompDataList,
                out xBarHCompDataList,
                out xBarVCompDataList,
                out xBarOXCompDataList,
                out xBarOYCompDataList
                );
            rtspistation.XBarACompDataList = xBarACompDataList;
            rtspistation.XBarHCompDataList = xBarHCompDataList;
            rtspistation.XBarVCompDataList = xBarVCompDataList;
            rtspistation.XBarOXCompDataList = xBarOXCompDataList;
            rtspistation.XBarOYCompDataList = xBarOYCompDataList;
            rtspistation.Station_Id = Station_Id;
            return (JsonConvert.SerializeObject(rtspistation, new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore }));      
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="Station_Id"></param>
        /// <returns></returns>
        private List<SPIRTPanel> GetSPIRTPanelList()
        {         
                List<SPIRTPanel> spirtpanelList = new List<SPIRTPanel>();
               
                #region
                foreach (var Panel in SPIRTPanelInfoList)
                {
                    SPIRTPanel spirtpanel = new SPIRTPanel();
                    List<float[]> PadVDList = new List<float[]>();
                    List<float[]> PadHDList = new List<float[]>();
                    List<float[]> PadADList = new List<float[]>();
                    List<float[]> PadOSList = new List<float[]>();
                    RangeValue rangeValue;

                    var tempListByPanelId = Panel.RTPadInfoList;
                    rangeValue.AreaAvg = (float)Math.Round(tempListByPanelId.Average(pad => pad.Area), 2);
                    rangeValue.AreaMax = (float)Math.Round(tempListByPanelId.Max(pad => pad.Area), 2);
                    rangeValue.AreaMin = (float)Math.Round(tempListByPanelId.Min(pad => pad.Area), 2);

                    rangeValue.HeightAvg = (float)Math.Round(tempListByPanelId.Average(pad => pad.Height), 2);
                    rangeValue.HeightMax = (float)Math.Round(tempListByPanelId.Max(pad => pad.Height), 2);
                    rangeValue.HeightMin = (float)Math.Round(tempListByPanelId.Min(pad => pad.Height), 2);

                    rangeValue.VolumeAvg = (float)Math.Round(tempListByPanelId.Average(pad => pad.Volume), 2);
                    rangeValue.VolumeMax = (float)Math.Round(tempListByPanelId.Max(pad => pad.Volume), 2);
                    rangeValue.VolumeMin = (float)Math.Round(tempListByPanelId.Min(pad => pad.Volume), 2);

                    rangeValue.XOffsetAvg = (float)Math.Round(tempListByPanelId.Average(pad => pad.OffsetX), 2);
                    rangeValue.XOffsetMax = (float)Math.Round(tempListByPanelId.Max(pad => pad.OffsetX), 2);
                    rangeValue.XOffsetMin = (float)Math.Round(tempListByPanelId.Min(pad => pad.OffsetX), 2);

                    rangeValue.YOffsetAvg = (float)Math.Round(tempListByPanelId.Average(pad => pad.OffsetY), 2);
                    rangeValue.YOffsetMax = (float)Math.Round(tempListByPanelId.Max(pad => pad.OffsetY), 2);
                    rangeValue.YOffsetMin = (float)Math.Round(tempListByPanelId.Min(pad => pad.OffsetY), 2);


                    var volumedistributeList = (from pad in tempListByPanelId.AsParallel()
                                                group pad by Math.Round((double)pad.Volume, 0)
                                                    into g
                                                    select new
                                                    {
                                                        g.Key,
                                                        Count = g.Count()
                                                    }).OrderBy(V => V.Key).ToList();
                    var heightdistributeList = (from pad in tempListByPanelId.AsParallel()
                                                group pad by Math.Round((double)pad.Height, 1)
                                                    into g
                                                    select new
                                                    {
                                                        g.Key,
                                                        Count = g.Count()
                                                    }).OrderBy(V => V.Key).ToList();
                    var areadistributeList = (from pad in tempListByPanelId.AsParallel()
                                              group pad by Math.Round((double)pad.Area, 0)
                                                  into g
                                                  select new
                                                  {
                                                      g.Key,
                                                      Count = g.Count()
                                                  }).OrderBy(V => V.Key).ToList();

                    var offsetsplashList = (from pad in tempListByPanelId.AsParallel()
                                            select new { X = (double)Math.Floor((double)pad.OffsetX * 10) / 10, Y = (double)Math.Floor((double)pad.OffsetY * 10) / 10 }).Distinct().ToList();
                    foreach (var volumedistribute in volumedistributeList)
                    {
                        float[] flag = { (float)volumedistribute.Key, volumedistribute.Count };
                        PadVDList.Add(flag);
                    }
                    foreach (var heightdistribute in heightdistributeList)
                    {
                        float[] flag = { (float)heightdistribute.Key, heightdistribute.Count };
                        PadHDList.Add(flag);
                    }
                    foreach (var areadistribute in areadistributeList)
                    {
                        float[] flag = { (float)areadistribute.Key, areadistribute.Count };
                        PadADList.Add(flag);
                    }
                    foreach (var offsetsplash in offsetsplashList)
                    {
                        float[] flag = { (float)offsetsplash.X, (float)offsetsplash.Y };
                        PadOSList.Add(flag);
                    }
               
                   
                    spirtpanel.TestTime = Panel.StartTime.ToString("MM-dd HH:mm:ss");
                    spirtpanel.Squeegee = Panel.Squeegee;
                    spirtpanel.Panel_Id = Panel.PanelId;
                    spirtpanel.Unit = Panel.Unit;
                    spirtpanel.ModelName = Panel.ModelName;
                    spirtpanel.PanelBarcode = Panel.PanelBarcode;
                    spirtpanel.Range = rangeValue;
                    spirtpanel.PadVolumeDistributeList = PadVDList;
                    spirtpanel.PadHeightDistributeList = PadHDList;
                    spirtpanel.PadAreaDistributeList = PadADList;
                    spirtpanel.PadOffsetSplashList = PadOSList;
                    spirtpanelList.Add(spirtpanel);
                }
                #endregion
                return spirtpanelList;
         
        }

        public void GetLastTenPanelList(int StationId)
        {
            using (ISPCEntities entity = new ISPCEntities())
            {
                #region select last ten panels based on selected station and the last one hour
                List<SPIRTPanelInfo> panelInfoList = new List<SPIRTPanelInfo>();
                var tempPanelInfoList = entity.SPI_Panel                                            
                                            .Where(panel => panel.Station_Id == StationId)
                                            .OrderByDescending(panel => panel.Start_Time)
                                            .Select(panel => new { 
                                                PanelId = panel.Panel_Id, 
                                                StartTime = panel.Start_Time,
                                                Squeegee = panel.Squeegee,
                                                ModelName = panel.Model.Model_Name,
                                                PanelBarcode = panel.Panel_Barcode,
                                                Unit = panel.Unit,
                                                CpkComps = panel.Model.CpkComponents,
                                                XBarComps = panel.Model.XBarComponents
                                            }).Take(10).ToList();
                int[] lastTenPanelIdArray = tempPanelInfoList.Select(m=>m.PanelId).ToArray();

                var tempTotalPadList = (from pad in entity.SPI_Pad
                                        where lastTenPanelIdArray.Contains(pad.SPI_Board.Panel_Id)
                                select new {
                                    pad.SPI_Board.Panel_Id, pad.Area, pad.Height, pad.Volume, pad.Offset_X, pad.Offset_Y, pad.Component_Name }).ToList();
                foreach (var tempPanelInfo in tempPanelInfoList)
                {
                    SPIRTPanelInfo data;
                    data.PanelId = tempPanelInfo.PanelId;
                    data.StartTime = tempPanelInfo.StartTime;
                    data.Squeegee = tempPanelInfo.Squeegee;
                    data.ModelName = tempPanelInfo.ModelName;
                    data.PanelBarcode = tempPanelInfo.PanelBarcode;
                    data.Unit = tempPanelInfo.Unit;
                    data.CpkComps = tempPanelInfo.CpkComps.Split(',');
                    data.XBarComps = tempPanelInfo.XBarComps.Split(',');
                    var indexTotalPadList = tempTotalPadList.Where(p => p.Panel_Id == tempPanelInfo.PanelId);
                    List<SPIRTPadInfo> padInfoList = new List<SPIRTPadInfo>();
                    foreach (var indexPad in indexTotalPadList)
                    {
                        SPIRTPadInfo pad;
                        pad.CompsName = indexPad.Component_Name;
                        pad.Area = (double)indexPad.Area;
                        pad.Height = (double)indexPad.Height;
                        pad.Volume = (double)indexPad.Volume;
                        pad.OffsetX = (double)indexPad.Offset_X;
                        pad.OffsetY = (double)indexPad.Offset_Y;
                        padInfoList.Add(pad);
                    }
                    data.RTPadInfoList = padInfoList;
                    panelInfoList.Add(data);
                }
                #endregion
                this.SPIRTPanelInfoList = panelInfoList;                
            }
        }

        private void GetSPIRTCompXBarList(
            out Dictionary<string, List<double>> xBarACompDataList, 
            out Dictionary<string, List<double>> xBarHCompDataList,
            out Dictionary<string, List<double>> xBarVCompDataList,
            out Dictionary<string, List<double>> xBarOXCompDataList,
            out Dictionary<string, List<double>> xBarOYCompDataList
            )
        {
            
                Dictionary<int, List<SPIRTComp>> xBarPanelDataList = new Dictionary<int, List<SPIRTComp>>();
                Dictionary<string, List<double>> tempXBarACompDataList = new Dictionary<string, List<double>>();
                Dictionary<string, List<double>> tempXBarHCompDataList = new Dictionary<string, List<double>>();
                Dictionary<string, List<double>> tempXBarVCompDataList = new Dictionary<string, List<double>>();
                Dictionary<string, List<double>> tempXBarOXCompDataList= new Dictionary<string, List<double>>();
                Dictionary<string, List<double>> tempXBarOYCompDataList= new Dictionary<string, List<double>>();
                #region               
                foreach (var Panel in this.SPIRTPanelInfoList)
                {
                    var groupByCompList = from pad in Panel.RTPadInfoList
                                          where Panel.XBarComps.Contains(pad.CompsName)
                                          group pad by pad.CompsName into g
                                          select new
                                          {
                                              CompName = g.Key,
                                              AvgVolume = g.Average(pad => pad.Volume),
                                              AvgHeight = g.Average(pad => pad.Height),
                                              AvgArea = g.Average(pad => pad.Area),
                                              AvgXOffset = g.Average(pad => pad.OffsetX),
                                              AvgYOffset = g.Average(pad => pad.OffsetY)
                                          };
                    List<SPIRTComp> spirtcompList = new List<SPIRTComp>();                   
                    foreach (var data in groupByCompList)
                    { 
                        SPIRTComp spiRTComp;
                        spiRTComp.CompName = data.CompName;
                        spiRTComp.XBarArea =data.AvgArea;
                        spiRTComp.XBarHeight = data.AvgHeight;
                        spiRTComp.XBarVolume = data.AvgVolume;
                        spiRTComp.XBarXOffset = data.AvgXOffset;
                        spiRTComp.XBarYOffset = data.AvgYOffset;
                        spirtcompList.Add(spiRTComp);
                    }
                    xBarPanelDataList.Add(Panel.PanelId, spirtcompList);
                }
                #endregion
                #region
                foreach (var xBarPanelData in xBarPanelDataList)
                {
                    string[] xBarCompArray = this.SPIRTPanelInfoList.FirstOrDefault().XBarComps;
                    foreach (string comp in xBarCompArray)
                    {
                        double VXBar = new double();
                        double HXBar = new double();
                        double AXBar = new double();
                        double OXBar = new double();
                        double OYBar = new double();
                        if (xBarPanelData.Value.Where(data => data.CompName == comp).Count() != 0)
                        {
                            var tempData = xBarPanelData.Value.Where(data => data.CompName == comp).FirstOrDefault();
                            VXBar = tempData.XBarVolume;
                            HXBar = tempData.XBarHeight;
                            AXBar = tempData.XBarArea;
                            OXBar = tempData.XBarXOffset;
                            OYBar = tempData.XBarYOffset;
                        }
                        this.OptimizedXbarCompDataList(comp, AXBar, ref tempXBarACompDataList);
                        this.OptimizedXbarCompDataList(comp, HXBar, ref tempXBarHCompDataList);
                        this.OptimizedXbarCompDataList(comp, VXBar, ref tempXBarVCompDataList);
                        this.OptimizedXbarCompDataList(comp, OXBar, ref tempXBarOXCompDataList);
                        this.OptimizedXbarCompDataList(comp, OYBar, ref tempXBarOYCompDataList);
                       
                    }
                }
                #endregion
                xBarACompDataList = tempXBarACompDataList;
                xBarHCompDataList = tempXBarHCompDataList;
                xBarVCompDataList = tempXBarVCompDataList;
                xBarOXCompDataList = tempXBarOXCompDataList;
                xBarOYCompDataList = tempXBarOYCompDataList;          
        }
        private void OptimizedXbarCompDataList(string comp, double xBarXData, ref Dictionary<string, List<double>> xBarXCompDataList)
        {
            if (xBarXCompDataList.ContainsKey(comp))
            {
                List<double> tempXBarACompData = xBarXCompDataList.Where(data => data.Key == comp).Select(data => data.Value).FirstOrDefault();
                tempXBarACompData.Add(xBarXData);
                xBarXCompDataList[comp] = tempXBarACompData;
            }
            else
            {
                List<double> initialData = new List<double>();
                initialData.Add(xBarXData);
                xBarXCompDataList.Add(comp, initialData);
            }
        }
        private float CpkFormula(double average, double sd, double usl, double lsl)
        {
            float cpu = (float)Math.Round((usl - average) / (3 * sd), 4);
            float cpl = (float)Math.Round((average - lsl) / (3 * sd), 4);
            return cpu > cpl ? cpu : cpl;
        }

        public string GetComponentList(int Station_Id)
        {
            using (ISPCEntities entity = new ISPCEntities())
            {
                var compList = (from panel in entity.SPI_Panel
                                where panel.Station_Id == Station_Id
                                orderby panel.Panel_Id descending
                                select new { 
                                    ModelId = panel.Model_Id,
                                    ModelName = panel.Model.Model_Name, 
                                    Comps = panel.Model.Components, 
                                    CpkComps=panel.Model.CpkComponents, 
                                    XBarComps=panel.Model.XBarComponents }).Take(1).FirstOrDefault();
                CompListByModel compListByModel;
                compListByModel.ModelId = compList.ModelId;
                compListByModel.ModelName = compList.ModelName;
                compListByModel.CompList = compList.Comps.ToString().Split(',');
                string[] flagStringArray = {""};
                compListByModel.CpkCompList = compList.CpkComps== null? flagStringArray:compList.CpkComps.ToString().Split(',');
                compListByModel.XBarCompList = compList.XBarComps == null ? flagStringArray : compList.XBarComps.ToString().Split(',');
                return (JsonConvert.SerializeObject(compListByModel));
            }
        }

        public void UpdateCpkXBarComps(int modelId, string selectedCpkComp, string selectedXBarComp)
        {
            using (ISPCEntities entity = new ISPCEntities())
            {
                var model = entity.Models.Where(m => m.Model_Id == modelId).Take(1).FirstOrDefault();
                model.CpkComponents = selectedCpkComp;
                model.XBarComponents = selectedXBarComp;
                entity.SaveChanges();
            }
        }
    }
}