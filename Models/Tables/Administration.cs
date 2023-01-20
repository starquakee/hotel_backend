using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics.CodeAnalysis;

namespace Models.Tables
{
    [Index(nameof(Account), IsUnique = true)]
    public class Administration
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public uint ID { get; set; }

        [Required, MaxLength(500)]
        public string Name { get; set; } = string.Empty;

        [Required, MaxLength(500)]
        public string Account { get; set; } = string.Empty;

        [Required, MaxLength(500)]
        public string Password { get; set; } = string.Empty;

    }
}
