using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace Spectra.Models
{
    [Table("Spectra_UserRoleCustomer")]
    public class UserRoleCustomer
    {
        [Key]
        public int Id { get; set; }
        public int AccountUserId { get; set; }
        public AccountUser AccountUser { get; set; }

        public int RolesId { get; set; }
        public Roles Roles { get; set; }
    }
}
