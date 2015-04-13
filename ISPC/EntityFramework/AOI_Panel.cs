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
    public partial class AOI_Panel
    {
        public AOI_Panel()
        {
            this.AOI_Board = new HashSet<AOI_Board>();
        }
    
        public int Panel_Id { get; set; }
        public int Station_Id { get; set; }
        public string Panel_Barcode { get; set; }
        public int Model_Id { get; set; }
        public System.DateTime Start_Time { get; set; }
        public System.DateTime End_Time { get; set; }
        public double Cycle_Time { get; set; }
        public int Result_Id { get; set; }
        public string Operator_Name { get; set; }
        public Nullable<int> Total_Comps_Num { get; set; }
        public Nullable<int> Total_Pins_Num { get; set; }
        public Nullable<int> Indict_Comps_Num { get; set; }
        public Nullable<int> Indict_Pins_Num { get; set; }
        public Nullable<int> Defect_Num { get; set; }
        public Nullable<int> Active_Defect_Num { get; set; }
        public Nullable<int> Active_Pins_Num { get; set; }
        public Nullable<int> Active_Comps_Num { get; set; }
        public Nullable<int> FalseCall_Defect_Num { get; set; }
        public Nullable<int> FalseCall_Pins_Num { get; set; }
        public Nullable<int> FalseCall_Comps_Num { get; set; }
        public System.DateTime Timespan { get; set; }
    
        public virtual ICollection<AOI_Board> AOI_Board { get; set; }
        public virtual Model Model { get; set; }
        public virtual Result Result { get; set; }
        public virtual Station Station { get; set; }
    }
    
}