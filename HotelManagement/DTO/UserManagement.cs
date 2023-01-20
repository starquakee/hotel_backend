using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace HotelManagement.DTO
{
    public class UserDto
    {
        public uint ID { get; set; }

        public string Name { get; set; } = string.Empty;
        
        public string NickName { get; set; } = string.Empty;
        
        public string Account { get; set; } = string.Empty;
        
        public string Password { get; set; } = string.Empty;
        
        public string IDNumber { get; set; } = string.Empty;
        
        public string PhoneNumber { get; set; } = string.Empty;
        
        public uint[] Coupon { get; set; } = Array.Empty<uint>();

        public string Token { get; set; } = string.Empty;

        public uint[] Subscription { get; set; } = Array.Empty<uint>();

        public uint Points { get; set; }

    }
    public class AddUserRequestDto
    {
        public string NickName { get; set; } = string.Empty;
        
        public string Account { get; set; } = string.Empty;
        
        public string Password { get; set; } = string.Empty;
        
        public string PhoneNumber { get; set; } = string.Empty;

    }
    public class EmailDto
    {
        public string Email { get; set; } = string.Empty;
        public string Code { get; set; } = string.Empty;
    }
    public class InfoDto
    {
        public string[] Info  { get; set; } 
    }
}
