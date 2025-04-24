using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace Spectra.Models
{
    [Table("Spectra_UserRoleAdmin")]
    public class UserRoleAdmin
    {
        [Key]
        public int Id { get; set; }
        public int AccountAdminId { get; set; }
        public AccountAdmin AccountAdmin { get; set; }

        public int RolesId { get; set; }
        public Roles Roles { get; set; }
    }
}
