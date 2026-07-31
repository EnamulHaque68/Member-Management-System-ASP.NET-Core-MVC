using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System.ComponentModel.DataAnnotations;

namespace CoreIdentityWithOWIN.Models.ViewModels
{
    public class MemberViewModel
    {
        public int MemberId { get; set; }

        [Required(ErrorMessage = "Member Name is required")]
        [Display(Name = "Member Name")]
        [StringLength(100, MinimumLength = 2, ErrorMessage = "Name must be between 2 and 100 characters")]
        public string MemberName { get; set; } = null!;

        [Required(ErrorMessage = "Join Date is required")]
        [Display(Name = "Admission Date")]
        [DataType(DataType.Date)]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:yyyy-MM-dd}")]
        public DateTime JointDate { get; set; }

        [Required(ErrorMessage = "Mobile Number is required")]
        [Display(Name = "Mobile Number")]
        [RegularExpression(@"^[0-9]{10,15}$", ErrorMessage = "Please enter a valid mobile number (10-15 digits)")]
        public string MobileNo { get; set; } = null!;

        [Display(Name = "Is Active?")]
        public bool IsActive { get; set; } = true;

        [Display(Name = "Profile Image")]
        public string? ImageUrl { get; set; }

        [Display(Name = "Registration Fee")]
        [DataType(DataType.Currency)]
        [Range(0, 100000, ErrorMessage = "Fee must be between 0 and 100,000")]
        public decimal RegFee { get; set; }

        [Required(ErrorMessage = "Please select a Member Type")]
        [Display(Name = "Member Type")]
        [Range(1, int.MaxValue, ErrorMessage = "Please select a valid Member Type")]
        public int TypeId { get; set; }

        [ValidateNever]
        public List<MemberType>? MemberTypes { get; set; }

        [Display(Name = "Profile Picture")]
        [ValidateNever]
        public IFormFile? ProfileFile { get; set; }

        [ValidateNever]
        public List<Transaction> Transactions { get; set; } = new List<Transaction>();
    }
}