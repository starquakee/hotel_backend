using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;

namespace Models.Tables
{
    public enum IdentityCardType
    {
        IdentityCard,
        Passport,
        Other

    }
    [Index(nameof(EmployeeName), nameof(IdentityCardType), nameof(IdentityCardId), IsUnique = true)]
    public class EmployeeInstance
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public uint ID { get; set; }

        [Required, MaxLength(500)]
        public string EmployeeName { get; set; } = string.Empty;

        [Required]
        public IdentityCardType IdentityCardType { get; set; }

        [Required]
        public string IdentityCardId{ get; set; } = string.Empty;

        [Required, Column(TypeName = "json")]
        public int[] Character { get; set; } = Array.Empty<int>();

        [Required, MaxLength(500)]
        public string PhoneNumber { get; set; } = string.Empty;

        public IList<HotelInstance> HotelInstances { get; set; } = new List<HotelInstance>();



    }
}
