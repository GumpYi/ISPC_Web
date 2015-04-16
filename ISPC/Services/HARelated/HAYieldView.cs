using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Newtonsoft.Json;

namespace ISPC.Services.HARelated
{
    public abstract class HAYieldView
    {
        #region Property declaration
        public int StationId { get; set; }
        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }
        public int ModelId { get; set; }
        [JsonIgnore]
        public TimeBy timeBy { get; set; }
        [JsonIgnore]
        public CategoryBy categoryBy { get; set; }
        public Dictionary<string, Dictionary<string, double>> FPYBySelectedSection { get; set; }
        #endregion
        protected void GetDateTimeFilter(ref string filter)
        {

            switch (this.timeBy)
            {
                case TimeBy.Hour:
                    filter = "yyyy-MM-dd HH";
                    break;
                case TimeBy.Day:
                    filter = "yyyy-MM-dd";
                    break;
                case TimeBy.Month:
                    filter = "yyyy-MM";
                    break;
            }
        }
        public virtual string GetYieldViewData() 
        {
            return string.Empty;
        }
        protected virtual void OptimizeDicStructure() 
        {
            Console.WriteLine("virtual method");
        }

        protected virtual void GetRelatedFPYListByTimeWithMultiModel()
        {
            Console.WriteLine("virtual method");
        }

        protected virtual void GetRelatedFPYListByTimeWithSimpleModelOrNoModel()
        {
            Console.WriteLine("virtual method");
        }
    }
    public enum TimeBy
    {
        Hour,
        Day,
        Week,
        Month
    }

    public enum CategoryBy
    {
        Time,
        TimeAndModel
    }
}