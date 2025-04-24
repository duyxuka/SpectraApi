using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace Spectra.Models
{
    [Table("Spectra_Permissions")]
    public class Permissions
    {
        [Key]
        public int Id { get; set; } // Không dùng được nếu dùng composite key -> bỏ dòng này
        public int RolesId { get; set; }
        public Roles Roles { get; set; }
        public int ModulesId { get; set; }
        public Modules Modules { get; set; }
        public int PermissionValue { get; set; } // Bitwise: View=1, Create=2, Update=4, Delete=8 
    }

}
