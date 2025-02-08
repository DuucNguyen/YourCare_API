using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System.Numerics;
using YourCare_BOs;

namespace YourCare_DAOs
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {

        }

        public ApplicationDbContext()
        {

        }

        public DbSet<ApplicationUser> ApplicationUser { get; set; }
        public DbSet<Appointment> Appointments { get; set; }
        public DbSet<DoctorProfile> Doctors { get; set; }
        public DbSet<Specialty> Specialties { get; set; }
        public DbSet<PatientProfile> PatientProfiles { get; set; }
        public DbSet<Timetable> Timetables { get; set; }
        public DbSet<TimeSlot> TimeSlots { get; set; }
        public DbSet<DoctorSpecialties> DoctorSpecialties { get; set; }


        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                optionsBuilder.UseSqlServer(new ConfigurationBuilder()
                    .AddJsonFile("appsettings.json")
                    .Build()
                    .GetConnectionString("DefaultConStr"));
            }
        }
        protected override void OnModelCreating(ModelBuilder builder)
        {
            builder.Entity<DoctorProfile>(e =>
            {
                e.ToTable("DoctorProfile");
                e.HasKey(e => e.DoctorID);
                e.HasOne(x => x.ApplicationUser)
                .WithOne(x => x.DoctorProfile)
                .HasConstraintName("FK_DoctorProfile_ApplicationUser");
            });

            builder.Entity<Specialty>(e =>
            {
                e.ToTable("Specialty");
                e.HasKey(x => x.SpecialtyID);
            });


            builder.Entity<DoctorSpecialties>(e =>
            {
                e.ToTable("DoctorSpecialties");
                e.HasKey(e => new { e.DoctorID, e.SpecialtyID });

                e.HasOne<DoctorProfile>(x => x.Doctor)
                .WithMany(x => x.DoctorSpecialties)
                .HasForeignKey(x => x.DoctorID)
                .HasConstraintName("FK_DoctorSpecialty_Doctor");


                e.HasOne<Specialty>(x => x.Specialty)
                .WithMany(x => x.DoctorSpecialties)
                .HasForeignKey(x => x.SpecialtyID)
                .HasConstraintName("FK_DoctorSpecialties_Specialty");
            });

            builder.Entity<Timetable>(e =>
            {
                e.ToTable("Timetable");
                e.HasKey(x => x.Id);
                e.HasOne(x => x.Doctor)
                .WithMany(x => x.DoctorTimetables)
                .HasForeignKey(x => x.DoctorID)
                .OnDelete(DeleteBehavior.NoAction)
                .HasConstraintName("FK_Timetable_Doctor");

                e.HasIndex(x => new { x.DoctorID, x.Date, x.TimeSlotID })
                .IsUnique();

                e.Property(x => x.Date)
                .HasColumnType("date");

                e.HasOne<TimeSlot>(x => x.TimeSlot)
                .WithMany(x=>x.Timetables)
                .HasForeignKey(x=>x.TimeSlotID)
                .HasConstraintName("FK_TimeTable_TimeSlot");
            });


            builder.Entity<Appointment>(e =>
            {
                e.ToTable("Appointment");
                e.HasKey(x => x.Id);

                e.HasOne(x => x.Doctor)
                .WithMany(x => x.DoctorAppointments)
                .HasForeignKey(x => x.DoctorID)
                .OnDelete(DeleteBehavior.NoAction)
                .HasConstraintName("FK_Appointment_Doctor");

                e.HasOne(x => x.PatientProfile).WithMany(x => x.Appointments)
                .HasForeignKey(x => x.PatientProfileID)
                .OnDelete(DeleteBehavior.NoAction)
                .HasConstraintName("FK_Appointment_Patient");

                e.HasOne(x => x.CreatedByUser)
                .WithMany(x => x.CreatedAppointments)
                .HasForeignKey(x => x.CreatedBy)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("FK_Appointment_User");

                e.HasOne(x => x.TimeTable)
                .WithMany(x => x.Appointments)
                .HasForeignKey(x => x.TimetableID)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("FK_Appointment_Timetable");

                e.Property(x => x.UpdatedOn)
                .HasColumnType("date");

            });

            builder.Entity<PatientProfile>(e =>
            {

                e.ToTable("PatientProfile");
                e.HasKey(x => x.Id);

                e.HasMany(x => x.Appointments)
                .WithOne(x => x.PatientProfile)
                .OnDelete(DeleteBehavior.NoAction)
                .HasConstraintName("FK_Patient_Appointment");

                e.HasOne(x => x.ApplicationUser)
                .WithMany(x => x.PatientProfiles)
                .OnDelete(DeleteBehavior.NoAction)
                .HasConstraintName("FK_Patient_User");
            });

            builder.Entity<TimeSlot>(e =>
            {
                e.ToTable("TimeSlot");
                e.HasKey(x => x.Id);

                e.Property(x => x.StartTime)
                .HasColumnType("time");

                e.Property(x => x.EndTime)
                .HasColumnType("time");
            });

            base.OnModelCreating(builder);
        }
    }
}
