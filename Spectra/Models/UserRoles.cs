using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace Spectra.Models
{
    [Table("Spectra_UserRoles")]
    public class UserRoles
    {
        [Key]
        public int Id { get; set; }

        // ID của tài khoản (admin hoặc user)
        public int AccountAdminId { get; set; }
        public int AccountUserId { get; set; }

        [ForeignKey("AccountAdminId")]
        public AccountAdmin AccountAdmin { get; set; }

        [ForeignKey("AccountUserId")]
        public AccountUser AccountUser { get; set; }

        // Loại người dùng: Admin = 1, User = 2
        public int UserTypeId { get; set; }
        [ForeignKey("UserTypeId")]
        public UserTypes UserTypes { get; set; }
        public int RoleId { get; set; }

        [ForeignKey("RoleId")]
        public Roles Roles { get; set; }
    }
}
