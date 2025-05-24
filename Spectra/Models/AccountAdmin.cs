using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace Spectra.Models
{
    [Table("Spectra_AccountAdmin")]
    public class AccountAdmin
    {
        [Key]
        public int Id { get; set; }
        [Required(ErrorMessage = "This field can't blank")]
        public string Code { get; set; }
        [MaxLength(250, ErrorMessage = "Max of length is 30 characters")]
        [MinLength(2, ErrorMessage = "This field can't least 2 characters")]
        public string Name { get; set; }
        public string Email { get; set; }
        public string Password { get; set; } = "null";
        public string PasswordHash { get; set; } // Thêm trường này
        public string PasswordSalt { get; set; } // Thêm trường này
        public bool Status { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime ModifiedDate { get; set; }
        public ICollection<UserRoleAdmin> UserRoleAdmins { get; set; }
        public ICollection<AccountPermissions> AccountPermissions { get; set; }
    }
    public class AdminDTO : AccountAdmin
    {
        public List<string> RoleNames { get; set; }
        public Dictionary<string, int> AccountPermissions { get; set; }
    }
}
