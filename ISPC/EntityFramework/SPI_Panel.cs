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
    public partial class SPI_Panel
    {
        public SPI_Panel()
        {
            this.SPI_Board = new HashSet<SPI_Board>();
        }
    
        public int Panel_Id { get; set; }
        public int Station_Id { get; set; }
        public int Model_Id { get; set; }
        public string Unit { get; set; }
        public string Operator_Name { get; set; }
        public string Panel_Barcode { get; set; }
        public int Total_Pads_Num { get; set; }
        public int Error_Pads_Num { get; set; }
        public string Squeegee { get; set; }
        public System.DateTime Start_Time { get; set; }
        public System.DateTime End_Time { get; set; }
        public double Cycle_Time { get; set; }
        public int Result_Id { get; set; }
        public double Volume_Avg { get; set; }
        public double Height_Avg { get; set; }
        public double Area_Avg { get; set; }
        public System.DateTime Timespan { get; set; }
        public Nullable<double> Cp { get; set; }
        public Nullable<double> Cpk { get; set; }
    
        public virtual Model Model { get; set; }
        public virtual Result Result { get; set; }
        public virtual ICollection<SPI_Board> SPI_Board { get; set; }
        public virtual Station Station { get; set; }
    }
    
}
