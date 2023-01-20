using Models.Tables;
using System.Collections;
using System.ComponentModel.DataAnnotations;

namespace HotelManagement.DTO
{
    public class OrderDto
    {
        public ulong ID { get; set; }
        
        public string UUID { get; set; } = string.Empty;
       
        public Platform Platform { get; set; }
       
        public string PlatOrderNumber { get; set; } = string.Empty;
       
        public string Payload { get; set; } = string.Empty;
        
        public uint Price { get; set; }
        
        public uint RefoundValue { get; set; }

        public uint UserID { get; set; }

        public DateTimeOffset ProduceTime { get; set; }
        
        public DateTimeOffset ReserveCheckInTime { get; set; }
        
        public DateTimeOffset ReserveCheckOutTime { get; set; }
        
        public uint GuestRoomID { get; set; }

        public RoomType RoomType { get; set; }

        public string? HotelName { get; set; }

        public string? HotelAddress { get; set; }

        public uint RoomAmount { get; set; }

        public IList<ReserveOrder> ReserverOrders { get; set; } = new List<ReserveOrder>();

        public string? State { get; set; }

        public uint Grade { get; set; }

        public string Evaluate { get; set; } = string.Empty;

    }
    public class AddOrderRequestDto
    {
        public DateTimeOffset ProduceTime { get; set; }

        public DateTimeOffset ReserveCheckInTime { get; set; }

        public DateTimeOffset ReserveCheckOutTime { get; set; }

        public uint GuestRoomID { get; set; }

        public uint Price { get; set; }

        public uint UserID { get; set; }

        public string Account { get; set; } = string.Empty;

        public Platform Platform { get; set; }

        public string PlatOrderNumber { get; set; } = string.Empty;

        public uint RoomAmount { get; set; }


    }
    public class CountDto
    {
        public ArrayList Date { get; set; }
        public ArrayList Count { get; set; }
    }
    public class RoomCountDto
    {
        public ArrayList Date { get; set; }
        public Dictionary<string, ArrayList> Count { get; set; }
    }
}
