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
    public partial class Machine_Brand
    {
        public Machine_Brand()
        {
            this.Machine_Model = new HashSet<Machine_Model>();
        }
    
        public int Machine_Brand_Id { get; set; }
        public string Machine_Brand_Name { get; set; }
        public int Creator_Id { get; set; }
        public System.DateTime Creation_Time { get; set; }
    
        public virtual ICollection<Machine_Model> Machine_Model { get; set; }
        public virtual UserProfile UserProfile { get; set; }
    }
    
}