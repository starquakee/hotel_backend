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
    public class ReserveOrder
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public ulong ID { get; set; }

        [Required, MaybeNull]
        public Order Order { get; set; }

        [Required, MaybeNull]
        public Room Room { get; set; }

        [Required, MaybeNull]
        public User User { get; set; }

        [Required]
        public DateTimeOffset LockStartTime { get; set; }

        [Required]
        public DateTimeOffset LockEndTime { get; set; }

        [Required, MaxLength(500)]
        public string Remark { get; set; } = string.Empty;
    }
}
