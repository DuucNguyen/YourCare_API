using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using YourCare_BOs;

namespace YourCare_Repos.Interfaces
{
    public interface IAppointmentFilesUploadRepository
    {
        public Task AddRange(List<AppointmentFilesUpload> request);
    }
}
