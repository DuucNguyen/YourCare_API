using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace YourCare_BOs.Enums
{

    [NotMapped]
    public class AppointmentStatus
    {
        public const string Absent = "Vắng";
        public const string Complete = "Đã khám xong";
        public const string Waiting = "Đang đợi";
        public const string Processing = "Đang xử lí";
    }
}
