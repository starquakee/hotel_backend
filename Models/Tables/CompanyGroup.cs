using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Models.Tables
{
    [Index(nameof(GroupName), IsUnique = true)]
    public class CompanyGroup
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public uint ID { get; set; }


        [Required, MaxLength(500)]
        public string GroupName { get; set; } = string.Empty;

        public IList<HotelInstance> HotelInstances { get; set; } = new List<HotelInstance>();
    }
}
