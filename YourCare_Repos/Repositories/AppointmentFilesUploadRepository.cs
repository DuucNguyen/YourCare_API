using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using YourCare_BOs;
using YourCare_DAOs.DAOs;
using YourCare_Repos.Interfaces;

namespace YourCare_Repos.Repositories
{
    public class AppointmentFilesUploadRepository : IAppointmentFilesUploadRepository
    {
        private readonly AppointmentFilesUploadDAO _appointmentFilesUploadDAO;

        public AppointmentFilesUploadRepository(AppointmentFilesUploadDAO appointmentFilesUploadDAO)
        {
            _appointmentFilesUploadDAO = appointmentFilesUploadDAO;
        }

        public async Task AddRange(List<AppointmentFilesUpload> request)
        { 
            await _appointmentFilesUploadDAO.AddRange(request);
        }
    }
}
