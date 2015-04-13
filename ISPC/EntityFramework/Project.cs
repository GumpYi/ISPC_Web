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
    public partial class Project
    {
        public Project()
        {
            this.Models = new HashSet<Model>();
        }
    
        public int Project_Id { get; set; }
        public string Project_Name { get; set; }
        public bool Is_Active { get; set; }
        public int Segment_Id { get; set; }
        public Nullable<int> BU_Id { get; set; }
        public int Creator_Id { get; set; }
        public System.DateTime Creation_Time { get; set; }
        public System.DateTime Effect_Time { get; set; }
    
        public virtual BusinessUnit BusinessUnit { get; set; }
        public virtual ICollection<Model> Models { get; set; }
        public virtual Segment Segment { get; set; }
        public virtual UserProfile UserProfile { get; set; }
    }
    
}