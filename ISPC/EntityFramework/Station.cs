//------------------------------------------------------------------------------
// <auto-generated>
//    This code was generated from a template.
//
//    Manual changes to this file may cause unexpected behavior in your application.
//    Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

using System;
using System.Collections.Generic;

namespace ISPC.EntityFramework
{
    public partial class Station
    {
        public Station()
        {
            this.AOI_Panel = new HashSet<AOI_Panel>();
            this.Feeders = new HashSet<Feeder>();
            this.SPI_Panel = new HashSet<SPI_Panel>();
        }
    
        public int Station_Id { get; set; }
        public string Station_Name { get; set; }
        public string StationComputerAddress { get; set; }
        public Nullable<int> Machine_Id { get; set; }
        public bool Is_Active { get; set; }
        public int Line_Id { get; set; }
        public int Position { get; set; }
        public int Creator_Id { get; set; }
        public System.DateTime Creation_Time { get; set; }
        public System.DateTime Effect_Time { get; set; }
    
        public virtual ICollection<AOI_Panel> AOI_Panel { get; set; }
        public virtual ICollection<Feeder> Feeders { get; set; }
        public virtual Line Line { get; set; }
        public virtual Machine Machine { get; set; }
        public virtual ICollection<SPI_Panel> SPI_Panel { get; set; }
        public virtual UserProfile UserProfile { get; set; }
    }
    
}
