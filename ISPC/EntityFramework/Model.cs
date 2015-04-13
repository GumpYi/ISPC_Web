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
    public partial class Model
    {
        public Model()
        {
            this.AOI_Panel = new HashSet<AOI_Panel>();
            this.SPI_Panel = new HashSet<SPI_Panel>();
        }
    
        public int Model_Id { get; set; }
        public string Model_Name { get; set; }
        public string Type { get; set; }
        public Nullable<int> Project_Id { get; set; }
        public string Components { get; set; }
        public int Creator_Id { get; set; }
        public System.DateTime Creation_Time { get; set; }
    
        public virtual ICollection<AOI_Panel> AOI_Panel { get; set; }
        public virtual ICollection<SPI_Panel> SPI_Panel { get; set; }
        public virtual Project Project { get; set; }
        public virtual UserProfile UserProfile { get; set; }
    }
    
}