using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CoreIdentityWithOWIN.Models
{
    public class MemberType
    {
        [Key]
        public int TypeId { get; set; }
        public string Title { get; set; } = null!;
        public virtual ICollection<Member> Members { get; set; } = new List<Member>();
    }

    public class Member
    {
        [Key]
        public int MemberId { get; set; }

        [Required(ErrorMessage = "Member Name is required")]
        [StringLength(100, ErrorMessage = "Member Name cannot exceed 100 characters")]
        public string MemberName { get; set; } = null!;

        [Required(ErrorMessage = "Admission Date is required")]
        [DataType(DataType.Date)]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:yyyy-MM-dd}")]
        public DateTime JointDate { get; set; }

        [Required(ErrorMessage = "Mobile Number is required")]
        [RegularExpression(@"^[0-9]{10,15}$", ErrorMessage = "Please enter a valid mobile number (10-15 digits)")]
        public string MobileNo { get; set; } = null!;

        public bool IsActive { get; set; }
        public string? ImageUrl { get; set; }

        [Range(0, 1000000, ErrorMessage = "Registration Fee must be between 0 and 1,000,000")]
        public decimal RegFee { get; set; }

        [Required(ErrorMessage = "Member Type is required")]
        [ForeignKey("MemberType")] 
        public int TypeId { get; set; }

        public virtual MemberType MemberType { get; set; } = null!;
        public virtual ICollection<Transaction> Transactions { get; set; } = new List<Transaction>();
    }

    public class Transaction
    {
        [Key]
        public int TransactionId { get; set; }

        [Required(ErrorMessage = "Book Name is required")]
        [StringLength(200, ErrorMessage = "Book Name cannot exceed 200 characters")]
        public string BookName { get; set; } = null!;

        [Required(ErrorMessage = "Duration is required")]
        [Range(1, 365, ErrorMessage = "Duration must be between 1 and 365 days")]
        public int Duration { get; set; }

        public int MemberId { get; set; }
        public virtual Member? Member { get; set; } = null!;
    }
}