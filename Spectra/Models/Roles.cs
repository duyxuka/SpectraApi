using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace Spectra.Models
{
    [Table("Spectra_Roles")]
    public class Roles
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [MaxLength(100)]
        public string Name { get; set; } // Admin, Viewer, User
        public string RoleType { get; set; }
        public ICollection<UserRoleAdmin> UserRoleAdmins { get; set; }
        public ICollection<UserRoleCustomer> UserRoleCustomers { get; set; }
        public ICollection<Permissions> Permissions { get; set; }
    }
    // DTO cho request
    public class RoleCreateModel
    {
        public string Name { get; set; }
        public string RoleType { get; set; }
        public List<PermissionModel> Permissions { get; set; }
    }
    public class RoleUpdateModel
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string RoleType { get; set; }
        public List<PermissionModel> Permissions { get; set; }
    }
    public class PermissionModel
    {
        public int ModuleId { get; set; }
        public int PermissionValue { get; set; }
    }
}
