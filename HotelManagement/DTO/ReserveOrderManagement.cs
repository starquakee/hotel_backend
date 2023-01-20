namespace HotelManagement.DTO
{
    public class ReserveOrderDto
    {
        public ulong ID { get; set; }

        public ulong OrderID { get; set; }

        public uint GuestRoomID { get; set; }

        public uint UserID { get; set; }

        public DateTimeOffset LockStartTime { get; set; }

        public DateTimeOffset LockEndTime { get; set; }

        public string Remark { get; set; } = string.Empty;
    }
    public class AddReserveOrderRequestDto
    {
        public ulong OrderID { get; set; }

        public uint GuestRoomID { get; set; }

        public uint UserID { get; set; }

        public string? Remark { get; set; }

        public DateTimeOffset LockStartTime { get; set; }

        public DateTimeOffset LockEndTime { get; set; }

    }
}
