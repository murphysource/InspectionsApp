﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace InspectionsApp.Models
{
    using System;
    using System.Data.Entity;
    using System.Data.Entity.Infrastructure;
    
    public partial class db_a9ffb8_murphysourceEntities : DbContext
    {
        public db_a9ffb8_murphysourceEntities()
            : base("name=db_a9ffb8_murphysourceEntities")
        {
        }
    
        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            throw new UnintentionalCodeFirstException();
        }
    
        public virtual DbSet<AffectedPart> AffectedParts { get; set; }
        public virtual DbSet<Area> Areas { get; set; }
        public virtual DbSet<AssemblyLine> AssemblyLines { get; set; }
        public virtual DbSet<Defect> Defects { get; set; }
        public virtual DbSet<Inspection> Inspections { get; set; }
        public virtual DbSet<Shift> Shifts { get; set; }
        public virtual DbSet<Users> Users { get; set; }
        public virtual DbSet<InspectionSearch> InspectionSearches { get; set; }
    }
}
