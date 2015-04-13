using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ISPC.Models.RealTimeCache
{
    public class RTSPIStation : RTStation
    {
        public double[] DPPM { get; set; }        
        public List<float> LastTenPanelsVCpk { get; set; }
        public List<float> LastTenPanelsHCpk { get; set; }
        public List<float> LastTenPanelsACpk { get; set; }
        public List<float> LastTenPanelsXOCpk { get; set; }
        public List<float> LastTenPanelsYOCpk { get; set; }
        public List<SPIRTPanel> LastTenPanelsScatterList { get; set; }
        public Dictionary<string, List<double>> XBarACompDataList { get; set; }
        public Dictionary<string, List<double>> XBarHCompDataList { get; set; }
        public Dictionary<string, List<double>> XBarVCompDataList { get; set; }
        public Dictionary<string, List<double>> XBarOXCompDataList { get; set; }
        public Dictionary<string, List<double>> XBarOYCompDataList { get; set; }        
    }

    public class SPIRTPanel
    {
        public int Panel_Id { get; set; }
        public string PanelBarcode { get; set; }
        public RangeValue Range{ get;set; }
        public string Unit { get; set; }
        public string ModelName { get; set; }
        public string TestTime { get; set; }
        public string Squeegee { get; set; }
        public List<float[]> PadVolumeDistributeList { get; set; }
        public List<float[]> PadHeightDistributeList { get; set; }
        public List<float[]> PadAreaDistributeList { get; set; }
        public List<float[]> PadOffsetSplashList { get; set; }
       
    }
    public struct SPIRTComp
    {
        public string CompName;
        public double XBarVolume;
        public double XBarHeight;
        public double XBarArea;
        public double XBarXOffset;
        public double XBarYOffset;
    }
    public struct RangeValue
    {
        public float VolumeMax;
        public float HeightMax;
        public float AreaMax;
        public float XOffsetMax;
        public float YOffsetMax;

        public float VolumeMin;
        public float HeightMin;
        public float AreaMin;
        public float XOffsetMin;
        public float YOffsetMin;

        public float VolumeAvg;
        public float HeightAvg;
        public float AreaAvg;
        public float XOffsetAvg;
        public float YOffsetAvg;        
    }
}
