using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using ISPC.Models.RealTimeCache;
using ISPC.Properties;
using Newtonsoft.Json;

namespace ISPC.Services.SessionRelated
{
    public class ULCLConfig
    {
        public static string FetchULCLFromSetting()
        {
            ULCLSetting ulcl;
            ulcl.SPIFPYLCL = Settings.Default.SPIFPYLCL;
            ulcl.SPIDPPMUCL = Settings.Default.SPIDPPMUCL;
            ulcl.AOIFPYLCL = Settings.Default.AOIFPYLCL;
            ulcl.AOIFPPMUCL = Settings.Default.AOIFPPMUCL;
            ulcl.AOIDPPMUCL = Settings.Default.AOIDPPMUCL;
            return (JsonConvert.SerializeObject(ulcl));
        }
       
    }
}