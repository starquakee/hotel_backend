using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using Models.Tables;
using System.Diagnostics.CodeAnalysis;

namespace HotelManagement.DTO
{
    public class CompanyGroupDto
    {

        public uint ID { get; set; }



        public string GroupName { get; set; } = string.Empty;

        public IList<HotelInstance> HotelInstances { get; set; } = new List<HotelInstance>();
    }
    public class AddCompanyGroupRequestDto
    {

        public string GroupName { get; set; } = string.Empty;

        public IList<HotelInstance> HotelInstances { get; set; } = new List<HotelInstance>();


    }

}
