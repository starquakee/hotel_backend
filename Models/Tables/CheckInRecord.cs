using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics.CodeAnalysis;

namespace Models.Tables
{
    public class CheckInRecord
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public ulong ID { get; set; }

        [Required, MaybeNull]
        public ReserveOrder ReserveOrder { get; set; }

        [Required, MaybeNull]
        public Room Room { get; set; }

        [Required, MaxLength(500)]
        public string ResidentInformation { get; set; } = string.Empty;

        [Required]
        public DateTimeOffset CheckInTime { get; set; }

        [Required]
        public DateTimeOffset CheckOutTime { get; set; }

        [Required]
        public uint Deposit { get; set; }

        [Required]
        public uint ExtraExpense { get; set; }

    }
}
