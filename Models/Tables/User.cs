using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics.CodeAnalysis;

namespace Models.Tables
{
    [Index(nameof(IDNumber), IsUnique = true)]
    [Index(nameof(Account), IsUnique = true)]
    public class User
    {

        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public uint ID { get; set; }

        [MaybeNull, MaxLength(500)]
        public string Name { get; set; } = string.Empty;

        [Required, MaxLength(500)]
        public string NickName { get; set; } = string.Empty;

        [Required, MaxLength(500)]
        public string Account{ get; set; } = string.Empty;

        [Required, MaxLength(500)]
        public string Password { get; set; } = string.Empty;

        [MaybeNull, MaxLength(500)]
        public string IDNumber { get; set; } = string.Empty;


        [Required, MaxLength(500)]
        public string PhoneNumber { get; set; } = string.Empty;

        [Required, Column(TypeName = "json")]
        public uint[] Coupon { get; set; } = Array.Empty<uint>();
        [Required, Column(TypeName = "json")]
        public string[] Info { get; set; } = Array.Empty<string>();

        [Required, Column(TypeName = "json")]
        public uint[] Subscription { get; set; } = Array.Empty<uint>();

        [Required]
        public uint Points { get; set; }

    }
}
