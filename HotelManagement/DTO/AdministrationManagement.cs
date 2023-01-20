using System.ComponentModel.DataAnnotations;

namespace HotelManagement.DTO
{
    public class AdministrationDto
    {
        public uint ID { get; set; }
        
        public string Name { get; set; } = string.Empty;
        
        public string Account { get; set; } = string.Empty;
        
        public string Password { get; set; } = string.Empty;

        public string Token { get; set; } = string.Empty;
    }
}
