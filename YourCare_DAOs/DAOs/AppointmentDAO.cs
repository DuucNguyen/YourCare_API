using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using YourCare_BOs;

namespace YourCare_DAOs.DAOs
{
    public class AppointmentDAO
    {
        private readonly ApplicationDbContext _context;

        public AppointmentDAO(
            ApplicationDbContext context
            )
        {
            _context = context;
        }

        public async Task Create(Appointment appointment)
        {
            await _context.Appointments.AddAsync(appointment);
            await _context.SaveChangesAsync();
        }

        public async Task Update(Appointment appointment)
        {
            _context.Appointments.Update(appointment);
            await _context.SaveChangesAsync();
        }
        public async Task Delete(Appointment appointment)
        {
            _context.Appointments.Remove(appointment);
            await _context.SaveChangesAsync();
        }

        public async Task<Appointment> GetByID(int id)
        {
            return await _context.Appointments
                .Include(x => x.PatientProfile)
                .Include(x => x.Doctor)
                .Include(x => x.TimeTable)
                .Include(x => x.Files)
                .FirstOrDefaultAsync(x => x.Id == id);
        }

        public async Task<List<Appointment>> GetAll()
        {
            return await _context.Appointments
              .Include(x => x.Doctor)
              .Include(x => x.TimeTable)
              .Include(x => x.PatientProfile)
              .Include(x => x.CreatedByUser)
              .ToListAsync();
        }

        public async Task<List<Appointment>> GetAllByUserID(string userID)
        {
            return await _context.Appointments
                .Include(x => x.Doctor)
                .Include(x => x.TimeTable)
                .Include(x => x.PatientProfile)
                .Include(x => x.CreatedByUser)
                .Where(x => x.CreatedBy == userID).ToListAsync();
        }

        public async Task<List<Appointment>> GetAllByDoctorID(Guid doctorID)
        {
            return await _context.Appointments
                .Include(x => x.Doctor)
                .Include(x => x.TimeTable)
                .Include(x => x.PatientProfile)
                .Include(x => x.CreatedByUser)
                .Where(x => x.DoctorID == doctorID).ToListAsync();
        }

        public async Task<List<Appointment>> GetDoctorAppointmentByDate(Guid doctorID, DateTime date)
        {
            return await _context.Appointments
                .Include(x => x.Doctor)
                .Include(x => x.TimeTable)
                .Include(x => x.PatientProfile)
                .Include(x => x.CreatedByUser)
                .Where(x => x.DoctorID == doctorID && x.TimeTable.Date.Date == date.Date).ToListAsync();
        }

        public async Task<Appointment> GetAllByID(int id)
        {
            return await _context.Appointments
                .Include(x => x.Doctor)
                .Include(x => x.TimeTable)
                .Include(x => x.PatientProfile)
                .Include(x => x.CreatedByUser)
                .FirstOrDefaultAsync(x => x.Id == id);
        }

        public async Task<int> CountAppointmentByDate(Guid doctorID, DateTime date)
        {
            var result = await _context.Appointments
                .Include(x => x.TimeTable)
                .Where(x => x.DoctorID == doctorID && x.TimeTable.Date.Date == date.Date).ToListAsync();

            return result.Count;
        }
    }
}
