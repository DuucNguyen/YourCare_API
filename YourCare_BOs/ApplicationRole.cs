using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace YourCare_BOs
{
    public class ApplicationRole: IdentityRole<string>
    {
        public bool IsActive { get; set; }
    }
}
