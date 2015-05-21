﻿//------------------------------------------------------------------------------
// <auto-generated>
//    This code was generated from a template.
//
//    Manual changes to this file may cause unexpected behavior in your application.
//    Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

using System;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;

namespace ISPC.EntityFramework
{
    public partial class ISPCEntities : DbContext
    {
        public ISPCEntities()
            : base("name=ISPCEntities")
        {
        }
    
        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            throw new UnintentionalCodeFirstException();
        }
    
        public DbSet<AOI_Board> AOI_Board { get; set; }
        public DbSet<AOI_Component> AOI_Component { get; set; }
        public DbSet<AOI_Panel> AOI_Panel { get; set; }
        public DbSet<Building> Buildings { get; set; }
        public DbSet<BusinessUnit> BusinessUnits { get; set; }
        public DbSet<Defect_Type> Defect_Type { get; set; }
        public DbSet<Feeder> Feeders { get; set; }
        public DbSet<Line> Lines { get; set; }
        public DbSet<Machine> Machines { get; set; }
        public DbSet<Machine_Brand> Machine_Brand { get; set; }
        public DbSet<Machine_Model> Machine_Model { get; set; }
        public DbSet<Machine_Status> Machine_Status { get; set; }
        public DbSet<Machine_Type> Machine_Type { get; set; }
        public DbSet<Model> Models { get; set; }
        public DbSet<Project> Projects { get; set; }
        public DbSet<Result> Results { get; set; }
        public DbSet<Segment> Segments { get; set; }
        public DbSet<SPI_Board> SPI_Board { get; set; }
        public DbSet<SPI_Pad> SPI_Pad { get; set; }
        public DbSet<SPI_Panel> SPI_Panel { get; set; }
        public DbSet<Station> Stations { get; set; }
        public DbSet<UserProfile> UserProfiles { get; set; }
        public DbSet<webpages_Membership> webpages_Membership { get; set; }
        public DbSet<webpages_OAuthMembership> webpages_OAuthMembership { get; set; }
        public DbSet<webpages_Roles> webpages_Roles { get; set; }
    }
}
