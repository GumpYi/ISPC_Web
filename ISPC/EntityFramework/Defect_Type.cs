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
    public partial class Defect_Type
    {
        public Defect_Type()
        {
            this.AOI_Component = new HashSet<AOI_Component>();
            this.SPI_Pad = new HashSet<SPI_Pad>();
        }
    
        public int Defect_Type_Id { get; set; }
        public string Defect_Type_Name { get; set; }
        public string Description { get; set; }
        public string MachineBrandName { get; set; }
        public int Machine_Type_Id { get; set; }
        public int CreatorId { get; set; }
        public System.DateTime Creation_Time { get; set; }
    
        public virtual ICollection<AOI_Component> AOI_Component { get; set; }
        public virtual ICollection<SPI_Pad> SPI_Pad { get; set; }
        public virtual Machine_Type Machine_Type { get; set; }
    }
    
}