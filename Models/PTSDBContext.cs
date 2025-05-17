
using Microsoft.EntityFrameworkCore;


namespace PatientTrackingSite.Models

{
    public class PTSDBContext : DbContext

    {

        public PTSDBContext(DbContextOptions<PTSDBContext> options) : base(options)
        {

        }

        public DbSet<User> Users { get; set; }
        public DbSet<Appointment> Appointments { get; set; }
        public DbSet<Disease> Diseases { get; set; }
        public DbSet<Medication> Medications { get; set; }
        public DbSet<MedicalImage> MedicalImages { get; set; }


        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // User-Appointments ilişki (Hasta)
            modelBuilder.Entity<Appointment>()
                .HasOne(a => a.Patient)
                .WithMany(u => u.AppointmentsAsPatient)
                .HasForeignKey(a => a.PatientId)
                .OnDelete(DeleteBehavior.Restrict);

            // User-Appointments ilişki (Doktor)
            modelBuilder.Entity<Appointment>()
                .HasOne(a => a.Doctor)
                .WithMany(u => u.AppointmentsAsDoctor)
                .HasForeignKey(a => a.DoctorId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<User>()
                .HasIndex(u => u.TCNo)
                .IsUnique(); // Make it unique tc no should be unique

                 base.OnModelCreating(modelBuilder);
        }

    }
}
