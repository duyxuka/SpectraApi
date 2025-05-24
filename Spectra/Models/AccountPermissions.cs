using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace Spectra.Models
{
    [Table("Spectra_AccountPermissions")]
    public class AccountPermissions
    {
        [Key]
        public int Id { get; set; }
        public int AccountAdminId { get; set; }
        public AccountAdmin AccountAdmin { get; set; }
        public int ModulesId { get; set; }
        public Modules Modules { get; set; }
        public int PermissionValue { get; set; } // Bitwise: View=1, Create=2, Update=4, Delete=8
    }
}
