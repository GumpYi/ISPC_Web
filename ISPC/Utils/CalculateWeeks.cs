using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ISPC.Utils
{ 
    public struct WeekOfTimeSection
    {
        public DateTime StartTime;
        public DateTime EndTime;
    }
    public class CalculateWeeks
    {
        public static List<WeekOfTimeSection> GetWeekOfTimeSection(DateTime StartTime, DateTime EndTime)
        {
            List<WeekOfTimeSection> weekOfTimeSectionList = new List<WeekOfTimeSection>();
            DateTime startTimeTemp = StartTime;
            DateTime endTimeTemp = StartTime;
            while (endTimeTemp < EndTime)
            {
                int startDayOfWeek = (int)startTimeTemp.DayOfWeek;
                WeekOfTimeSection weekOfTimeSection;
                weekOfTimeSection.StartTime = startTimeTemp;
                if (startTimeTemp.AddDays(6 - startDayOfWeek) > EndTime)
                {
                    endTimeTemp = EndTime;
                }
                else
                {
                    endTimeTemp = DateTime.Parse(startTimeTemp.AddDays(6 - startDayOfWeek).ToString("yyyy-MM-dd"));
                    startTimeTemp = DateTime.Parse(startTimeTemp.AddDays(6 - startDayOfWeek + 1).ToString("yyyy-MM-dd"));
                }
                weekOfTimeSection.EndTime = endTimeTemp;
                weekOfTimeSectionList.Add(weekOfTimeSection);
            }
            return weekOfTimeSectionList;
        }
    }
}