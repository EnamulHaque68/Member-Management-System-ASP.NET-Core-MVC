using System.ComponentModel.DataAnnotations;

namespace CoreIdentityWithOWIN.Models.ViewModels
{
    public class ManageUserRoleViewModel
    {
        [Key]
        public string UserId { get; set; }
        public string Email { get; set; }
        public List<RoleSelection> Roles { get; set; } = new();
    }
}
