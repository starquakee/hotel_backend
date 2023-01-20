using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics.CodeAnalysis;

namespace Models.Tables
{
    public enum Platform
    {
        FlyPig=0,
        MeiTuan=1,
        Ctrip=2,
        SpecialClient=3,
        TravelAgency=4,
        SelfBuiltPlatform=5
    }

    [Index(nameof(UUID), IsUnique = true)]
    [Index(nameof(Platform), nameof(PlatOrderNumber), IsUnique = true)]

    public class Order
    {

        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public ulong ID { get; set; }

        [Required, MaxLength(500)]
        public string UUID { get; set; } = string.Empty;
        
        [Required]
        public Platform Platform{ get; set; }

        [Required, MaxLength(500)]
        public string PlatOrderNumber { get; set; } = string.Empty;

        [Required, MaxLength(500)]
        public string Payload { get; set; } = string.Empty;

        [Required]
        public uint Price { get; set; } 

        [Required]
        public uint RefoundValue { get; set; }

        [Required]
        public DateTimeOffset ProduceTime { get; set; }

        [Required]
        public DateTimeOffset ReserveCheckInTime { get; set; }

        [Required]
        public DateTimeOffset ReserveCheckOutTime { get; set; }

        [Required, MaybeNull]
        public GuestRoom GuestRoom { get; set; }

        [Required, MaybeNull]
        public User User { get; set; }

        [Required]
        public uint RoomAmount { get; set; }

        [Required]
        public uint Grade { get; set; }

        [Required]
        public string Evaluate { get; set; } = string.Empty;

        public IList<ReserveOrder> ReserverOrders { get; set; } = new List<ReserveOrder>();

        [Required, Column(TypeName = "json")]
        public string[] Pictures { get; set; } = Array.Empty<string>();

        [Required, Column(TypeName = "json")]
        public byte[] Videos { get; set; } = Array.Empty<byte>();

    }
}
