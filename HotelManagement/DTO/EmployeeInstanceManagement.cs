using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Models.Tables;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace HotelManagement.DTO
{
    public class EmployeeInstanceDto
    {
        public uint ID { get; set; }

        public string EmployeeName { get; set; } = string.Empty;

        public IdentityCardType IdentityCardType { get; set; }

        public string IdentityCardId { get; set; } = string.Empty;

        public int[] Character { get; set; } = Array.Empty<int>();

        public string PhoneNumber { get; set; } = string.Empty;
    }
    public class AddEmployeeRequestDto
    {
        

        public string EmployeeName { get; set; } = string.Empty;

        public IdentityCardType IdentityCardType { get; set; }

        public string IdentityCardId { get; set; } = string.Empty;

        public int[] Character { get; set; } = Array.Empty<int>();

        public string PhoneNumber { get; set; } = string.Empty;
    }
}
