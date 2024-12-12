using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace YourCare_BOs.Enums
{

    [NotMapped]
    public class AppointmentStatusEnum
    {
        public enum Status
        {
            Vắng = 0,
            Đã_hủy = 2,
            Đang_xử_lí = 3,
            Đã_hoàn_thành = 4,
            Đang_Chờ = 5,
        }
    }
}
