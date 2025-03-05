using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using YourCare_BOs;

namespace YourCare_DAOs.DAOs
{
    public class AppointmentFilesUploadDAO
    {
        private readonly ApplicationDbContext _context;

        public AppointmentFilesUploadDAO(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task AddRange(List<AppointmentFilesUpload> request)
        {
            await _context.AppointmentFilesUploads.AddRangeAsync(request);
            await _context.SaveChangesAsync();
        }
    }
}
