using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using ISPC.Models.RealTimeCache;
using ISPC.Properties;

namespace ISPC.Services.SessionRelated
{
    public class SPIRangeConfig
    {
        public static SPIRangeData FetchSPIRangeFromSetting()
        {
            SPIRangeData data;
            data.ALCL = Settings.Default.SPIALCL;
            data.ALSL = Settings.Default.SPIALSL;
            data.AUCL = Settings.Default.SPIAUCL;
            data.AUSL = Settings.Default.SPIAUSL;
            
            data.HLCL = Settings.Default.SPIHLCL;
            data.HLSL = Settings.Default.SPIHLSL;
            data.HUCL = Settings.Default.SPIHUCL;
            data.HUSL = Settings.Default.SPIHUSL;

            data.VLCL = Settings.Default.SPIVLCL;
            data.VLSL = Settings.Default.SPIVLSL;
            data.VUCL = Settings.Default.SPIVUCL;
            data.VUSL = Settings.Default.SPIVUSL;

            data.XOLCL = Settings.Default.SPIXOLCL;
            data.XOLSL = Settings.Default.SPIXOLSL;
            data.XOUSL = Settings.Default.SPIXOUSL;
            data.XOUCL = Settings.Default.SPIXOUCL;

            data.YOLCL = Settings.Default.SPIYOLCL;
            data.YOLSL = Settings.Default.SPIYOLSL;
            data.YOUCL = Settings.Default.SPIYOUCL;
            data.YOUSL = Settings.Default.SPIYOUSL;
            return data;
        }
    }
}