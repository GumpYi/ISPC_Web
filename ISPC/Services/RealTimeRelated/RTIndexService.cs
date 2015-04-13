using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using ISPC.EntityFramework;
using ISPC.Models.RealTimeCache;
using ISPC.Properties;
using Newtonsoft.Json;

namespace ISPC.Services.RealTimeRelated
{
    public class RTIndexService
    {
        public string GetRTLinesJson(List<int> line_IdList, DateTime dateTime)
        {
            using (ISPCEntities entity = new ISPCEntities())
            {
                List<RTLine> selectedLine = new List<RTLine>();
                foreach (var line in entity.Lines)
                {
                    if (line_IdList.Contains(line.Line_Id))
                    {
                        List<RTSPIStation> TempRealTimeData_SPIList = new List<RTSPIStation>();
                        List<RTAOIStation> TempRealTimeData_AOIList = new List<RTAOIStation>();
                        foreach (var station in line.Stations)
                        {
                            int Machine_Type_Id = station.Machine.Machine_Model.Machine_Type_Id;
                            // if station type is SPI
                            DateTime date = DateTime.Parse(dateTime.AddHours(-11).ToString("yyyy-MM-dd HH:00:00"));
                            int nowHour = int.Parse(dateTime.ToString("HH"));
                            if (Machine_Type_Id == Settings.Default.SPI)
                            {
                                //calculate the FPY and DPPM based per hour
                                var spiDataList = (from panel in station.SPI_Panel
                                                   where panel.Start_Time >= date
                                                   && panel.End_Time <= dateTime
                                                   group panel by panel.Start_Time.ToString("yyyy-MM-dd HH")
                                                       into g
                                                       orderby g.Key descending
                                                       select new
                                                       {
                                                           g.Key,
                                                           FPY = (g.Count(panel => panel.Result.Result_Name != Settings.Default.SPI_Panel_Result_NG) / (double)g.Count()) * 100,
                                                           DPPM = (g.Where(panel => panel.Result.Result_Name == Settings.Default.SPI_Panel_Result_NG)
                                                           .Sum(panel => panel.Error_Pads_Num) / (double)g.Sum(panel => panel.Total_Pads_Num)) * 1000000
                                                       }).ToList();

                                double[] FPYList_flag = new double[12];
                                double[] DPPMList_flag = new double[12];
                                foreach (var s in spiDataList)
                                {
                                    int key = int.Parse(s.Key.Substring(s.Key.IndexOf(' ') + 1, 2));                                    
                                    int aa = key > nowHour ? 24 - key + nowHour : nowHour - key;
                                    FPYList_flag[aa] = s.FPY;
                                    DPPMList_flag[aa] = s.DPPM;
                                }
                                TempRealTimeData_SPIList.Add(new RTSPIStation
                                {
                                    Station_Id = station.Station_Id,
                                    Station_Name = station.Station_Name,
                                    DPPM = DPPMList_flag,
                                    FPY = FPYList_flag,
                                });
                            }
                            //if station type is AOI
                            else if (Machine_Type_Id == Settings.Default.AOI)
                            {
                                //calculate the FPY, FPPM and DPPM based on AOI                               
                                var aoiDataList = (from panel in station.AOI_Panel
                                                   where panel.Start_Time >= date
                                                   && panel.End_Time <= dateTime
                                                   group panel by panel.Start_Time.ToString("yyyy-MM-dd HH")
                                                       into g
                                                       orderby g.Key descending
                                                       select new
                                                       {
                                                           g.Key,
                                                           FPY = (g.Count(panel => panel.Result.Result_Name != Settings.Default.AOIPanelResult_Failed) / (double)g.Count()) * 100,
                                                           DPPM = (g.Where(panel => panel.Result.Result_Name == Settings.Default.AOIPanelResult_Failed)
                                                           .Sum(panel => panel.Active_Comps_Num) / (double)g.Sum(panel => panel.Total_Comps_Num)) * 1000000,
                                                           FPPM = (g.Sum(panel => panel.FalseCall_Comps_Num) / (double)g.Sum(panel => panel.Total_Comps_Num)) * 1000000
                                                       }).ToList();
                                double[] FPYList_flag = new double[12];
                                double[] DPPMList_flag = new double[12];
                                double[] FPPMList_flag = new double[12];
                                foreach (var s in aoiDataList)
                                {
                                    int key = int.Parse(s.Key.Substring(s.Key.IndexOf(' ') + 1, 2));                                   
                                    int aa = key > nowHour ? 24 - key + nowHour : nowHour - key;
                                    FPYList_flag[aa] =s.FPY;
                                    DPPMList_flag[aa] = (double)s.DPPM;
                                    FPPMList_flag[aa] = (double)s.FPPM;
                                }
                                TempRealTimeData_AOIList.Add(new RTAOIStation
                                {
                                    Station_Id = station.Station_Id,
                                    Station_Name = station.Station_Name,
                                    DPPM = DPPMList_flag,
                                    FPPM = FPPMList_flag,
                                    FPY = FPYList_flag
                                });
                            }
                        }
                        selectedLine.Add(new RTLine
                        {
                            Line_Id = line.Line_Id,
                            Line_Name = line.Line_Name,
                            Building_Id = line.Segment.Building_Id,
                            Segment_Id = line.Segment_Id,
                            RealTimeData_SPIList = TempRealTimeData_SPIList,
                            RealTimeData_AOIList = TempRealTimeData_AOIList
                        });
                    }
                }
                return (JsonConvert.SerializeObject(selectedLine, new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore }));
            }
        }
    }
}