using Models.Tables;

namespace HotelManagement.DTO
{
    public class RoomDto
    {
        public uint ID { get; set; }

        public int Floor { get; set; } = 0;
        
        public string Address { get; set; } = string.Empty;
        
        public RoomStatus RoomStatus { get; set; }//房间的当前状态：空闲；已预定待住；已入住；已离店待清洁

        public RoomType RoomType { get; set; }

        public uint HotelInstanceID { get; set; }

        public uint Price { get; set; }

        public string Title { get; set; } = string.Empty;

        public string Ichnography { get; set; } = string.Empty;

        public ushort Area { get; set; }
    }

    public class GuestRoomDto : RoomDto
    {
        public uint BedCount { get; set; }

        public uint WindowCount { get; set; }

        public uint MineralWaterCount { get; set; }

        public uint CondomCount { get; set; }

        public GuestRoomType GuestRoomType { get; set; }

    }

    public class AddRoomRequestDto
    {
        public int Floor { get; set; }

        public string Address { get; set; } = string.Empty;

        public RoomStatus RoomStatus { get; set; }//房间的当前状态：空闲；已预定待住；已入住；已离店待清洁

        public RoomType RoomType { get; set; }

        public uint HotelInstanceID { get; set; }

        public uint Price { get; set; }

        public string Title { get; set; } = string.Empty;

        public string Ichnography { get; set; } = string.Empty;

        public ushort Area { get; set; }
    }

    public class AddGuestRoomRequestDto : AddRoomRequestDto
    {
        public uint BedCount { get; set; }

        public uint WindowCount { get; set; }

        public uint MineralWaterCount { get; set; }

        public uint CondomCount { get; set; }

        public GuestRoomType GuestRoomType { get; set; }
    }

}
