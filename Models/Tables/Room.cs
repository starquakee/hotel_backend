using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics.CodeAnalysis;
using Microsoft.EntityFrameworkCore;

namespace Models.Tables
{
    public enum RoomStatus
    {
        Free=0,
        Reserved=1,
        CheckIn=2,
        LeftNeedClean=3,
        NotOpen=4,
        OnCleaning=5,
        WaitChecking=6
    }

    public enum RoomType
    {
        GuestRoom = 0,
        LaundryRoom= 1,
        GymRoom = 2,
        StaffRoom = 3,
        MeetingRoom = 4
    }

    public enum GuestRoomType
    {
        BarrierFree = 0,
        Single = 1,
        Double = 2,
        Triple = 3,
        Quadruple = 4,
        Deluxe = 5

    }

    [Index(nameof(Address), nameof(Title), IsUnique = true)]
    public abstract class Room
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public uint ID { get; set; }
        public int Floor { get; set; }
        [Required, MaxLength(300)]
        public string Address { get; set; } = string.Empty;
        [Required]
        public RoomStatus RoomStatus { get; set; }//房间的当前状态：空闲；已预定待住；已入住；已离店待清洁

        [Required]
        public RoomType RoomType{ get; set; }

        [Required, MaybeNull]
        public HotelInstance HotelInstance { get; set; }


        [Required]
        public uint Price{ get; set; }

        [Required, MaxLength(300)]
        public string Title { get; set; } = string.Empty;

        [Required, MaxLength(500)]
        public string Ichnography { get; set; } = string.Empty;
        [Required]
        public ushort Area{ get; set; }
    }

  
    public class GuestRoom : Room
    {
        [Required]
        public GuestRoomType GuestRoomType { get; set; }
        [Required]
        public uint BedCount { get; set; }

        [Required]
        public uint WindowCount { get; set; }

        [Required]
        public uint MineralWaterCount { get; set; }

        [Required]
        public uint CondomCount { get; set; }

    }

    public class LaundryRoom : Room
    {
        [Required]
        public uint WasherCount { get; set; }
    }

  
    public class GymRoom : Room
    {
        [Required]
        public string EquipmentType { get; set; } = string.Empty;

        [Required]
        public uint EquipmentCount{ get; set; }
    }

 
    public class StaffRoom : Room
    {
        [Required]
        public uint BedCount { get; set; }
    }


    public class MeetingRoom : Room
    {
        [Required]
        public uint SeatCount { get; set; }

        [Required]
        public uint SocketCount { get; set; }

    }

}
