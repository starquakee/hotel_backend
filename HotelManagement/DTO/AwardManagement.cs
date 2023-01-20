using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using Models.Tables;
using System.Diagnostics.CodeAnalysis;

namespace HotelManagement.DTO
{
    public class AwardDto
    {
        public string Account { get; set; } = string.Empty;

        public string Consignee { get; set; } = string.Empty;

        public string Goods { get; set; } = string.Empty;

        public uint GoodsPoints { get; set; }

        public string DeliveryAddress { get; set; } = string.Empty;

        public string ContactNumber { get; set; } = string.Empty;

    }
}