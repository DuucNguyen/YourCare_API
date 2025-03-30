using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using YourCare_BOs;

namespace YourCare_Repos.Interfaces
{
    public interface IAppointmentReposiory
    {
        public Task<bool> Add(Appointment request);
        public Task<bool> Update(Appointment request);
        public Task<bool> Delete(Appointment request);
        public Task<bool> CancelAppointment(Appointment request);
        public Task<Appointment> GetById(int id);
        public Task<List<Appointment>> GetAllByDoctorId(Guid doctorId);
        public Task<List<Appointment>> GetDoctorAppointmentByDate(Guid doctorId, DateTime date);
        public Task<List<Appointment>> GetAllAppointmentByDate(DateTime date);
        public Task<List<Appointment>> GetAll();
        public Task<List<Appointment>> GetAllByYear(int year);
        public Task<List<Appointment>> GetAllByUserId(string userId);
        public Task<bool> CompleteAppointment(int id, string dianosis, string note);
        public Task<int> CountDoctorAppointmentByDate(Guid doctorID, DateTime date);



    }
}
