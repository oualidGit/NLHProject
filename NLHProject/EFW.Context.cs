﻿//------------------------------------------------------------------------------
// <auto-generated>
//     Ce code a été généré à partir d'un modèle.
//
//     Des modifications manuelles apportées à ce fichier peuvent conduire à un comportement inattendu de votre application.
//     Les modifications manuelles apportées à ce fichier sont remplacées si le code est régénéré.
// </auto-generated>
//------------------------------------------------------------------------------

namespace NLHProject
{
    using System;
    using System.Data.Entity;
    using System.Data.Entity.Infrastructure;
    
    public partial class NLHEntities : DbContext
    {
        public NLHEntities()
            : base("name=NLHEntities")
        {
        }
    
        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            throw new UnintentionalCodeFirstException();
        }
    
        public virtual DbSet<sysdiagram> sysdiagrams { get; set; }
        public virtual DbSet<tblAdmin> tblAdmins { get; set; }
        public virtual DbSet<tblCompagny> tblCompagnies { get; set; }
        public virtual DbSet<tblDepartement> tblDepartements { get; set; }
        public virtual DbSet<tblDossierAdmission> tblDossierAdmissions { get; set; }
        public virtual DbSet<tblLit> tblLits { get; set; }
        public virtual DbSet<tblMedecin> tblMedecins { get; set; }
        public virtual DbSet<tblParent> tblParents { get; set; }
        public virtual DbSet<tblPatient> tblPatients { get; set; }
        public virtual DbSet<tblTypesLit> tblTypesLits { get; set; }
    }
}
