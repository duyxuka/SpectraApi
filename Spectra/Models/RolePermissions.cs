using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace Spectra.Models
{
    [Table("Spectra_RolePermissions")]
    public class RolePermissions
    {
        [Key]
        public int Id { get; set; }

        public int RoleId { get; set; }
        public int PermissionId { get; set; }

        [ForeignKey("RoleId")]
        public Roles Roles { get; set; }

        [ForeignKey("PermissionId")]
        public Permissions Permissions { get; set; }
    }
}
