using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Newtonsoft.Json;

namespace ISPC.Services.HARelated
{
    public abstract class HADefectView
    {

        #region property define
        [JsonIgnore]
        public int StationId { get; set; }
        [JsonIgnore]
        public DateTime StartTime { get; set; }
        [JsonIgnore]
        public DateTime EndTime { get; set; }
        [JsonIgnore]
        public int ModelId { get; set; }
        #endregion

        public HADefectView(int stationId, DateTime startTime, DateTime endTime, int modelId)
        {
            this.StationId = stationId;
            this.StartTime = startTime;
            this.EndTime = endTime;
            this.ModelId = modelId;
        }

        public abstract string GetDefectViewDataCollection();

        protected abstract void GetBasicDefectViewData();
    }
}