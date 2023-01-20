using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;
using Microsoft.EntityFrameworkCore;

namespace Models.Tables
{
    [Index(nameof(HotelName), nameof(CompanyGroup), IsUnique = true)]
    public class HotelInstance
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public uint ID { get; set; }

        [Required, MaybeNull]
        public CompanyGroup CompanyGroup { get; set; }

        [Required, MaxLength(500)]
        public string HotelName { get; set; } = string.Empty;

        [Required]
        public string HotelAddress { get; set; } = string.Empty;

        [Required]
        public string City { get; set; } = string.Empty;


        [Required, Column(TypeName = "json")]
        public string[] ContactList { get; set; } = Array.Empty<string>();

        public IList<EmployeeInstance> EmployeeInstances { get; set; } = new List<EmployeeInstance>();

    }
}
