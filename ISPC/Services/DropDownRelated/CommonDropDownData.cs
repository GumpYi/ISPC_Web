using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using ISPC.EntityFramework;
using Newtonsoft.Json;
using System.Text;
using System.Linq.Dynamic;
using Newtonsoft.Json.Converters;
using ISPC.Properties;
namespace ISPC.Services.DropDownRelated
{
    public class CommonDropDownData
    {
        public string getBuildings()
        {
            using (ISPCEntities entity = new ISPCEntities())
            {
                var buildingList = from building in entity.Buildings orderby (building.Building_Id) select new { building.Building_Id, building.Building_Name };
                return(JsonConvert.SerializeObject(buildingList)); 
            }
        }

        public string getSegments(int Building_Id)
        {
            using (ISPCEntities entity = new ISPCEntities())
            {
                var segmentList = from segment in entity.Segments
                                  where (segment.Building_Id == Building_Id)
                                  orderby (segment.Segment_Id)

                                  select new { segment.Segment_Id, segment.Segment_Name };
                return(JsonConvert.SerializeObject(segmentList));
            }
        }

        public string getBUs(int Segment_Id)
        {
            using (ISPCEntities entity = new ISPCEntities())
            {
                var buList = from bu in entity.BusinessUnits                            
                             where (bu.Segment_Id == Segment_Id)
                             orderby (bu.Segment_Id)
                             select new { bu.BU_Id, bu.BU_Name };
                return(JsonConvert.SerializeObject(buList));
            }
        }
        public string getLines(int Segment_Id)
        {
            using (ISPCEntities entity = new ISPCEntities())
            {
                var lineList = from line in entity.Lines                               
                               where (line.Segment_Id == Segment_Id)
                               orderby (line.Line_Id)
                               select new { line.Line_Id, line.Line_Name };
                return(JsonConvert.SerializeObject(lineList));
            }
        }

        public string getLines(int Building_Id, int Segment_Id)
        {
            using (ISPCEntities entity = new ISPCEntities())
            {
                StringBuilder selectCondition = new StringBuilder("1==1");
                if (Building_Id != 0) selectCondition.Append("And Segment.Building_Id ==" + Building_Id);
                if (Segment_Id != 0) selectCondition.Append("And Segment_Id ==" + Segment_Id);
                var lines = entity.Lines
                    .Where(selectCondition.ToString())
                    .OrderBy(l => l.Line_Id)
                    .Select(line => new { line.Line_Id, line.Line_Name, line.Line_Location});
                IsoDateTimeConverter dtConverter = new IsoDateTimeConverter { DateTimeFormat = "yyyy-MM-dd HH:mm" };
                return (JsonConvert.SerializeObject(lines, dtConverter));
            }           
        }

        public string getSPIStations(int Line_Id)
        {
            using (ISPCEntities entity = new ISPCEntities())
            {
                var StationList = from station in entity.Stations
                                  where (station.Line_Id == Line_Id && station.Machine.Machine_Model.Machine_Type_Id == Settings.Default.SPI)
                                  orderby (station.Position)
                                  select new { station.Station_Id, station.Station_Name };
                return (JsonConvert.SerializeObject(StationList));
            }
        }
        public string getProjects(int Segment_Id) 
        {
            using (ISPCEntities entity = new ISPCEntities())
            {
                var projectList = from project in entity.Projects                                 
                                  where (project.Segment_Id == Segment_Id)
                                  orderby (project.Project_Id)
                                  select new { project.Project_Id, project.Project_Name };
                return (JsonConvert.SerializeObject(projectList));
            }
        }
        public string getSPIModels(int Project_Id)
        {
            using (ISPCEntities entity = new ISPCEntities())
            {
                var modelList = from model in entity.Models                               
                                where (model.Project_Id == Project_Id && model.Type == "SPI")
                                orderby (model.Creation_Time) descending
                                select new { model.Model_Id, model.Model_Name };
                return (JsonConvert.SerializeObject(modelList));
            }
        }

        public string GetAOIStations(int LineId)
        {
            using (ISPCEntities entity = new ISPCEntities())
            {
                var StationList = from station in entity.Stations
                                  where (station.Line_Id == LineId && station.Machine.Machine_Model.Machine_Type_Id == Settings.Default.AOI)
                                  orderby (station.Position)
                                  select new { station.Station_Id, station.Station_Name };
                return (JsonConvert.SerializeObject(StationList));
            } 
        }
        public string GetAOIModels(int ProjectId)
        {
            using (ISPCEntities entity = new ISPCEntities())
            {
                var modelList = from model in entity.Models
                                where (model.Project_Id == ProjectId && model.Type == "AOI")
                                orderby (model.Creation_Time) descending
                                select new { model.Model_Id, model.Model_Name };
                return (JsonConvert.SerializeObject(modelList));
            }
        }
    }
}