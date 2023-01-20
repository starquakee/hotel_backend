using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics.CodeAnalysis;

namespace Models.Tables
{

    public class Award
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public uint ID { get; set; }

        [Required]
        public User User { get; set; } 

        [Required, MaxLength(500)]
        public string Account { get; set; } = string.Empty;

        [Required, MaxLength(500)]
        public string Consignee { get; set; } = string.Empty;

        [MaybeNull, MaxLength(500)]
        public string Goods { get; set; } = string.Empty;

        [Required]
        public uint GoodsPoints { get; set; }

        [Required, MaxLength(500)]
        public string DeliveryAddress { get; set; } = string.Empty;

        [Required, MaxLength(500)]
        public string ContactNumber { get; set; } = string.Empty;

    }
}
