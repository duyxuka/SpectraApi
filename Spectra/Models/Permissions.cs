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
        public int Id { get; set; }

        [Required]
        [MaxLength(100)]
        public string Name { get; set; } // ViewUser, EditTask, ManageOrder...
        public string Code { get; set; }
        public ICollection<RolePermissions> RolePermissions { get; set; }
    }
}
