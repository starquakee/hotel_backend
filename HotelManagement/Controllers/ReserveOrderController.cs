using HotelManagement.DTO;
using HotelManagement.ErrorHandling;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Models;
using Models.Tables;
using System.Security.Principal;
namespace HotelManagement.Controllers
{
    [ApiController, Route("api/[controller]")]
    public class ReserveOrderController : ControllerBase
    {
        private readonly MyDbContext myDbContext;

        public ReserveOrderController(MyDbContext myDbContext)
        {
            this.myDbContext = myDbContext;
        }

        [HttpGet("[action]")]
        public async Task<IActionResult> GetReserveOrder([FromQuery] Order order)
        {
            var reserveOrder = await myDbContext.ReserveOrders
                .AsNoTracking()
                .Where(reserveOrder => reserveOrder.Order == order)
                .ToListAsync();
            if (reserveOrder.Count < 1)
            {
                throw new BackendException
                {
                    Code = 9000,
                    ErrorMessage = "Order错误"
                };
            }
            var resp = reserveOrder.Select(item =>
            {
                return new ReserveOrderDto
                {
                    ID = item.ID,
                    OrderID = item.Order.ID,
                    GuestRoomID = item.Room.ID,
                    UserID = item.User.ID,
                    LockStartTime = item.LockStartTime,
                    LockEndTime = item.LockEndTime,
                    Remark = item.Remark

                };
            });

            return Ok(resp);
        }


        [HttpPost("[action]")]
        public async Task<IActionResult> AddReserveOrder([FromBody] AddReserveOrderRequestDto addReserveOrderRequest)
        {
            var order = await myDbContext.Orders
                .SingleOrDefaultAsync(it => it.ID==addReserveOrderRequest.OrderID);
            var room = await myDbContext.GuestRooms
    .SingleOrDefaultAsync(it => it.ID == addReserveOrderRequest.GuestRoomID);
            var user = await myDbContext.Users
    .SingleOrDefaultAsync(it => it.ID == addReserveOrderRequest.UserID);


            var newReserveOrder = new ReserveOrder
            {


                Order = order,
                Room = room,
                User = user,
                LockStartTime = addReserveOrderRequest.LockStartTime,
                LockEndTime = addReserveOrderRequest.LockEndTime,
                Remark = addReserveOrderRequest.Remark
            };

            var entry = await myDbContext.ReserveOrders.AddAsync(newReserveOrder);
            newReserveOrder = entry.Entity;
            await myDbContext.SaveChangesAsync();

            var resp = new ReserveOrderDto
            {

                OrderID = newReserveOrder.Order.ID,
                GuestRoomID = newReserveOrder.Room.ID,
                UserID = newReserveOrder.User.ID,
                LockStartTime = newReserveOrder.LockStartTime,
                LockEndTime = newReserveOrder.LockEndTime,
                Remark = newReserveOrder.Remark,
                ID = newReserveOrder.ID
            };

            return Ok(resp);
        }


        [HttpPut("[action]")]
        public async Task<IActionResult> ModifyReserveOrder([FromBody] ReserveOrderDto reserveOrderDto)
        {
            var reserveOrder = await myDbContext.ReserveOrders
                .SingleOrDefaultAsync(it => it.Order.ID== reserveOrderDto.OrderID);
            var room = await myDbContext.GuestRooms
    .SingleOrDefaultAsync(it => it.ID == reserveOrderDto.GuestRoomID);
            if (reserveOrder== null)
            {
                throw new BackendException
                {
                    Code = 6000,
                    ErrorMessage = "Order不存在"
                };
            }
            //ReserveOrder可以修改Room,LockStartTime,LockEndTime,remark
            if (reserveOrderDto.LockStartTime != DateTimeOffset.MinValue)
            {
                reserveOrder.LockStartTime = reserveOrderDto.LockStartTime;
            }
            if (reserveOrderDto.LockEndTime != DateTimeOffset.MinValue)
            {
                reserveOrder.LockEndTime = reserveOrderDto.LockEndTime;
            }
            if (room != null)
            {
                reserveOrder.Room = room;
            }
            if(reserveOrderDto.Remark != null)
            {
                reserveOrder.Remark = reserveOrderDto.Remark;
            }
            await myDbContext.SaveChangesAsync();
            var resp = new ReserveOrderDto
            {
                OrderID = reserveOrderDto.OrderID,
                GuestRoomID = reserveOrderDto.GuestRoomID,
                UserID = reserveOrderDto.UserID,
                LockStartTime = reserveOrderDto.LockStartTime,
                LockEndTime = reserveOrderDto.LockEndTime,
                Remark = reserveOrderDto.Remark,
                ID = reserveOrderDto.ID
            };
            return Ok(resp);
        }

        [HttpDelete("[action]")]
        public async Task<IActionResult> DeleteReserveOrder([FromQuery] Order order)
        {
            var reserveOrder = await myDbContext.ReserveOrders
                .SingleOrDefaultAsync(it => it.Order == order);

            if (reserveOrder == null)
            {
                throw new BackendException
                {
                    Code = 6000,
                    ErrorMessage = "Order不存在"
                };
            }
            myDbContext.ReserveOrders.Remove(reserveOrder);
            await myDbContext.SaveChangesAsync();

            return Ok();
        }

    }
}
