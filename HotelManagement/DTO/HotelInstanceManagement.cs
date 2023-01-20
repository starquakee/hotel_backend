namespace HotelManagement.DTO
{
    public class HotelInstanceDto
    {
        public uint HotelId { get; set; } = 0;
        public string HotelName { get; set; } = string.Empty;
        public string HotelAddress { get; set; } = string.Empty;
        public string City { get; set; } = string.Empty;
        public string[] ContactList { get; set; } = Array.Empty<string>();
        public string CompanyName { get; set; } = string.Empty;
    }

    public class AddHotelRequestDto
    {
        public uint CompanyId { get; set; } = 0;
        public HotelInstanceDto HotelInstance { get; set; }
    }
}
