using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Project_Task_Management.Data.Entities.Identity
{
    public class UserRefreshToken
    {
        [Key]
        public int Id { get; set; }
        public string? Token { get; set; }
        public string? RefershToken { get; set; }
        public string? JwtId { get; set; }

        public bool IsUsed { get; set; }
        public bool IsRevoked { get; set; }
        public DateTime AddedTime { get; set; }
        public DateTime ExpiryDate { get; set; }
        public int UserId { get; set; }
        [ForeignKey(nameof(UserId))]
        public virtual ApplicationUser? user { get; set; }
    }
}
