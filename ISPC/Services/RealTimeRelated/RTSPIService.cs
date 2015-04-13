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
        public string ModelName;
        public string[] CompList;
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


    public class RTSPIService
    {
        public string GetSPIRTDataJson(int Station_Id, string[] selectedCpkComp, string[] selectedXBarComp, SPIRangeData data)
        {
            using (ISPCEntities entity = new ISPCEntities())
            {
                List<float> lasttenpanelsvcpk = new List<float>();
                List<float> lasttenpanelshcpk = new List<float>();
                List<float> lasttenpanelsacpk = new List<float>();
                List<float> lasttenpanelsxocpk = new List<float>();
                List<float> lasttenpanelsyocpk = new List<float>();
              
                int[] lastTenPanelIdList = entity.SPI_Panel                                            
                                            .Where(panel => panel.Station_Id == Station_Id)
                                            .OrderByDescending(panel => panel.Start_Time)
                                            .Select(panel => panel.Panel_Id)
                                            .Take(10)
                                            .ToArray();
                var selectedTotalPadList = (from pad in entity.SPI_Pad
                                       where lastTenPanelIdList.Contains(pad.SPI_Board.Panel_Id) && selectedCpkComp.Contains(pad.Component_Name)
                                       orderby pad.Pad_Id
                                       select new { pad.SPI_Board.Panel_Id, pad.Area, pad.Height, pad.Volume, pad.Offset_X, pad.Offset_Y }).ToList();
                //the first element in "lastTenPanelIdList is the data of last the first panel"
                foreach (int PanelId in lastTenPanelIdList)
                {
                    var selectedpadList = (from pad in selectedTotalPadList where pad.Panel_Id == PanelId select pad).ToList();
                    // This panel not belong to the current model, so set the X_cpk equals 0
                    if (selectedpadList.Count == 0)
                    {
                        lasttenpanelshcpk.Add(0);
                        lasttenpanelsvcpk.Add(0);
                        lasttenpanelsacpk.Add(0);
                        lasttenpanelsxocpk.Add(0);
                        lasttenpanelsyocpk.Add(0);                                       
                    }
                    else
                    {
                        double HeightAvg = (double)selectedpadList.Average(o => o.Height);
                        double AreaAvg = (double)selectedpadList.Average(o => o.Area);
                        double VolumeAvg = (double)selectedpadList.Average(o => o.Volume);
                        double OffsetXAvg = (double)selectedpadList.Average(o => o.Offset_X);
                        double OffsetYAvg = (double)selectedpadList.Average(o => o.Offset_Y);
                        double flagVolumeSum = 0;
                        double flagAreaSum = 0;
                        double flagHeightSum = 0;
                        double flagOffsetXSum = 0;
                        double flagOffsetYSum = 0;
                        foreach (var selectedpad in selectedpadList)
                        {
                            flagVolumeSum += Math.Pow((double)selectedpad.Volume - VolumeAvg, 2);
                            flagAreaSum += Math.Pow((double)selectedpad.Area - AreaAvg, 2);
                            flagHeightSum += Math.Pow((double)selectedpad.Height - HeightAvg, 2);
                            flagOffsetXSum += Math.Pow((double)selectedpad.Offset_X - OffsetXAvg, 2);
                            flagOffsetYSum += Math.Pow((double)selectedpad.Offset_Y - OffsetYAvg, 2);
                        }
                        // get this Panel SD

                        lasttenpanelshcpk.Add(this.CpkFormula(HeightAvg, Math.Pow(flagHeightSum / selectedpadList.Count, 0.5), data.HUSL, data.HLSL));
                        lasttenpanelsvcpk.Add(this.CpkFormula(VolumeAvg, Math.Pow(flagVolumeSum / selectedpadList.Count, 0.5), data.VUSL, data.VLSL));

                        lasttenpanelsacpk.Add(this.CpkFormula(AreaAvg, Math.Pow(flagAreaSum / selectedpadList.Count, 0.5), data.AUSL, data.ALSL));

                        lasttenpanelsxocpk.Add(this.CpkFormula(OffsetXAvg, Math.Pow(flagOffsetXSum / selectedpadList.Count, 0.5), data.XOUSL, data.YOUCL));
                        lasttenpanelsyocpk.Add(this.CpkFormula(OffsetYAvg, Math.Pow(flagOffsetYSum / selectedpadList.Count, 0.5), data.YOUSL, data.YOLSL));
                    }
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
                rtspistation.LastTenPanelsScatterList = this.GetSPIRTPanelList(Station_Id, lastTenPanelIdList);
                this.GetSPIRTCompXBarList(lastTenPanelIdList, selectedXBarComp,
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
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="Station_Id"></param>
        /// <returns></returns>
        private List<SPIRTPanel> GetSPIRTPanelList(int Station_Id, int[] lastTenPanelIdList)
        {
            using (ISPCEntities entity = new ISPCEntities())
            {
                List<SPIRTPanel> spirtpanelList = new List<SPIRTPanel>();
                var tempList = (from pad in entity.SPI_Pad
                                where lastTenPanelIdList.Contains(pad.SPI_Board.Panel_Id)
                                select new {pad.SPI_Board.Panel_Id, pad.Area, pad.Height, pad.Volume, pad.Offset_X, pad.Offset_Y }).ToList();
                var otherParameterList = (from panel in entity.SPI_Panel
                                          where lastTenPanelIdList.Contains(panel.Panel_Id)
                                          select new
                                          {
                                              panel.Panel_Id,
                                              panel.Start_Time,
                                              panel.Squeegee,
                                              panel.Model.Model_Name,
                                              panel.Panel_Barcode,
                                              panel.Unit
                                          }).ToList();
                foreach (int PanelId in lastTenPanelIdList)
                {
                    SPIRTPanel spirtpanel = new SPIRTPanel();
                    List<float[]> PadVDList = new List<float[]>();
                    List<float[]> PadHDList = new List<float[]>();
                    List<float[]> PadADList = new List<float[]>();
                    List<float[]> PadOSList = new List<float[]>();
                    RangeValue rangeValue;

                    var tempListByPanelId = tempList.Where(pad => pad.Panel_Id == PanelId);
                    rangeValue.AreaAvg = (float)Math.Round((double)tempListByPanelId.Average(pad => pad.Area), 2);
                    rangeValue.AreaMax = (float)Math.Round((double)tempListByPanelId.Max(pad => pad.Area), 2);
                    rangeValue.AreaMin = (float)Math.Round((double)tempListByPanelId.Min(pad => pad.Area), 2);

                    rangeValue.HeightAvg = (float)Math.Round((double)tempListByPanelId.Average(pad => pad.Height), 2);
                    rangeValue.HeightMax = (float)Math.Round((double)tempListByPanelId.Max(pad => pad.Height), 2);
                    rangeValue.HeightMin = (float)Math.Round((double)tempListByPanelId.Min(pad => pad.Height), 2);

                    rangeValue.VolumeAvg = (float)Math.Round((double)tempListByPanelId.Average(pad => pad.Volume), 2);
                    rangeValue.VolumeMax = (float)Math.Round((double)tempListByPanelId.Max(pad => pad.Volume), 2);
                    rangeValue.VolumeMin = (float)Math.Round((double)tempListByPanelId.Min(pad => pad.Volume), 2);

                    rangeValue.XOffsetAvg = (float)Math.Round((double)tempListByPanelId.Average(pad => pad.Offset_X), 2);
                    rangeValue.XOffsetMax = (float)Math.Round((double)tempListByPanelId.Max(pad => pad.Offset_X), 2);
                    rangeValue.XOffsetMin = (float)Math.Round((double)tempListByPanelId.Min(pad => pad.Offset_X), 2);

                    rangeValue.YOffsetAvg = (float)Math.Round((double)tempListByPanelId.Average(pad => pad.Offset_Y), 2);
                    rangeValue.YOffsetMax = (float)Math.Round((double)tempListByPanelId.Max(pad => pad.Offset_Y), 2);
                    rangeValue.YOffsetMin = (float)Math.Round((double)tempListByPanelId.Min(pad => pad.Offset_Y), 2);


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
                                            select new { X = (double)Math.Floor((double)pad.Offset_X * 10) / 10, Y = (double)Math.Floor((double)pad.Offset_Y * 10) / 10 }).Distinct().ToList();
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
                    var otherParameter = otherParameterList.Where(data => data.Panel_Id == PanelId).Take(1).FirstOrDefault();
                    spirtpanel.TestTime = otherParameter.Start_Time.ToString("MM-dd HH:mm:ss");
                    spirtpanel.Squeegee = otherParameter.Squeegee;
                    spirtpanel.Panel_Id = PanelId;
                    spirtpanel.Unit = otherParameter.Unit;
                    spirtpanel.ModelName = otherParameter.Model_Name;
                    spirtpanel.PanelBarcode = otherParameter.Panel_Barcode;
                    spirtpanel.Range = rangeValue;
                    spirtpanel.PadVolumeDistributeList = PadVDList;
                    spirtpanel.PadHeightDistributeList = PadHDList;
                    spirtpanel.PadAreaDistributeList = PadADList;
                    spirtpanel.PadOffsetSplashList = PadOSList;
                    spirtpanelList.Add(spirtpanel);
                }
                return spirtpanelList;
            }
        }

        private void GetSPIRTCompXBarList(int[] lastTenPanelIdList, string[] selectedComp,
            out Dictionary<string, List<double>> xBarACompDataList, 
            out Dictionary<string, List<double>> xBarHCompDataList,
            out Dictionary<string, List<double>> xBarVCompDataList,
            out Dictionary<string, List<double>> xBarOXCompDataList,
            out Dictionary<string, List<double>> xBarOYCompDataList
            )
        {
            using (ISPCEntities entity = new ISPCEntities())
            {
                Dictionary<int, List<SPIRTComp>> xBarPanelDataList = new Dictionary<int, List<SPIRTComp>>();
                Dictionary<string, List<double>> tempXBarACompDataList = new Dictionary<string, List<double>>();
                Dictionary<string, List<double>> tempXBarHCompDataList = new Dictionary<string, List<double>>();
                Dictionary<string, List<double>> tempXBarVCompDataList = new Dictionary<string, List<double>>();
                Dictionary<string, List<double>> tempXBarOXCompDataList= new Dictionary<string, List<double>>();
                Dictionary<string, List<double>> tempXBarOYCompDataList= new Dictionary<string, List<double>>();
                #region
                var groupByCompAndPanelList = (from pad in entity.SPI_Pad
                                       where lastTenPanelIdList.Contains(pad.SPI_Board.Panel_Id) && selectedComp.Contains(pad.Component_Name)
                                       group pad by new { pad.Component_Name, pad.SPI_Board.Panel_Id } into g
                                       select new
                                       {
                                           g.Key.Panel_Id,
                                           g.Key.Component_Name,
                                           AvgVolume = g.Average(pad => pad.Volume),
                                           AvgHeight = g.Average(pad => pad.Height),
                                           AvgArea = g.Average(pad => pad.Area),
                                           AvgXOffset = g.Average(pad => pad.Offset_X),
                                           AvgYOffset = g.Average(pad => pad.Offset_Y)
                                       }).ToList();
                foreach (int PanelId in lastTenPanelIdList)
                {
                    List<SPIRTComp> spirtcompList = new List<SPIRTComp>();
                    var groupByCompList = groupByCompAndPanelList.Where(data => data.Panel_Id == PanelId);
                    foreach (var data in groupByCompList)
                    { 
                        SPIRTComp spiRTComp;
                        spiRTComp.CompName = data.Component_Name;
                        spiRTComp.XBarArea = (double)data.AvgArea;
                        spiRTComp.XBarHeight = (double)data.AvgHeight;
                        spiRTComp.XBarVolume = (double)data.AvgVolume;
                        spiRTComp.XBarXOffset = (double)data.AvgXOffset;
                        spiRTComp.XBarYOffset = (double)data.AvgYOffset;
                        spirtcompList.Add(spiRTComp);
                    }
                    xBarPanelDataList.Add(PanelId, spirtcompList);
                }
                #endregion
                #region
                foreach (var xBarPanelData in xBarPanelDataList)
                {
                    foreach (string comp in selectedComp)
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
                                select new { ModelName = panel.Model.Model_Name, Comps = panel.Model.Components }).Take(1).FirstOrDefault();
                CompListByModel compListByModel;
                compListByModel.ModelName = compList.ModelName;
                compListByModel.CompList = compList.Comps.ToString().Split(',');
                return (JsonConvert.SerializeObject(compListByModel));
            }
        }
    }
}