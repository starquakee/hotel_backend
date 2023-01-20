using HotelManagement.DTO;
using HotelManagement.ErrorHandling;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Models;
using Models.Tables;
using SecurityUtils;
using System.Collections;
using System.Security.Principal;
namespace HotelManagement.Controllers
{
    [ApiController, Route("api/[controller]")]
    public class NewOrderController : ControllerBase
    {
        private readonly MyDbContext myDbContext;
        static string GetRandomStr(int length)
        {
            var chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghizklmnopqrstuvwxyz0123456789";
            var random = new Random();

            var ans = new string(Enumerable.Repeat(chars, length)
              .Select(s => s[random.Next(s.Length)]).ToArray());
            Console.WriteLine(ans);
            return ans;
        }

        public NewOrderController(MyDbContext myDbContext)
        {
            this.myDbContext = myDbContext;
        }




        [HttpGet("[action]")]
        public async Task<IActionResult> GetDiffOrdersByUserAccount([FromQuery] int pageSize, int currentPage, string key, string account = "")
        {
            var position = pageSize * (currentPage - 1);
            if (key == "100")
            {
                var orders = await myDbContext.Orders
                    .AsNoTracking()
                    .Include(o => o.User)
                    .Include(o => o.GuestRoom)
                    .ThenInclude(r => r.HotelInstance)
                    .Where(o => o.User.Account == account)
                    .OrderBy(o => o.RoomAmount)
                    .Skip(position)
                    .Take(pageSize)
                    .ToListAsync();


                var resp = orders.Select(item =>
                {
                    var state = "";
                    if (item.ReserveCheckOutTime > DateTimeOffset.Now)
                    {
                        state = "未完成";
                    }
                    else if (item.Grade == 0 && item.Evaluate == "")
                    {
                        state = "未评价";
                    }
                    else
                    {
                        state = "已评价";
                    }
                    return new OrderDto
                    {
                        UUID = item.UUID,
                        HotelName = item.GuestRoom.HotelInstance.HotelName,
                        HotelAddress = item.GuestRoom.HotelInstance.HotelAddress,
                        ProduceTime = item.ProduceTime,
                        ReserveCheckInTime = item.ReserveCheckInTime,
                        ReserveCheckOutTime = item.ReserveCheckOutTime,
                        RoomType = item.GuestRoom.RoomType,
                        RoomAmount = item.RoomAmount,
                        Price = item.Price,
                        GuestRoomID = item.GuestRoom.ID,
                        State = state
                    };
                });

                return Ok(resp);
            }
            else if (key == "200")
            {
                var orders = await myDbContext.Orders
         .AsNoTracking()
         .Include(o => o.User)
         .Include(o => o.GuestRoom)
         .ThenInclude(r => r.HotelInstance)
         .Where(o => o.User.Account == account && o.ReserveCheckOutTime > DateTimeOffset.Now)
         .OrderBy(o => o.RoomAmount)
         .Skip(position)
         .Take(pageSize)
         .ToListAsync();



                var resp = orders.Select(item =>
                {
                    var state = "未完成";

                    return new OrderDto
                    {
                        UUID = item.UUID,
                        HotelName = item.GuestRoom.HotelInstance.HotelName,
                        HotelAddress = item.GuestRoom.HotelInstance.HotelAddress,
                        ProduceTime = item.ProduceTime,
                        ReserveCheckInTime = item.ReserveCheckInTime,
                        ReserveCheckOutTime = item.ReserveCheckOutTime,
                        RoomType = item.GuestRoom.RoomType,
                        RoomAmount = item.RoomAmount,
                        Price = item.Price,
                        GuestRoomID = item.GuestRoom.ID,
                        State = state
                    };
                });

                return Ok(resp);
            }
            else if (key == "300")
            {
                var orders = await myDbContext.Orders
                    .AsNoTracking()
                    .Include(o => o.User)
                    .Include(o => o.GuestRoom)
                    .ThenInclude(r => r.HotelInstance)
                    .Where(o => o.User.Account == account && o.ReserveCheckOutTime < DateTimeOffset.Now && o.Grade == 0 && o.Evaluate == "")
                    .OrderBy(o => o.RoomAmount)
                    .Skip(position)
                    .Take(pageSize)
                    .ToListAsync();


                var resp = orders.Select(item =>
                {
                    var state = "未评价";

                    return new OrderDto
                    {
                        UUID = item.UUID,
                        HotelName = item.GuestRoom.HotelInstance.HotelName,
                        HotelAddress = item.GuestRoom.HotelInstance.HotelAddress,
                        ProduceTime = item.ProduceTime,
                        ReserveCheckInTime = item.ReserveCheckInTime,
                        ReserveCheckOutTime = item.ReserveCheckOutTime,
                        RoomType = item.GuestRoom.RoomType,
                        RoomAmount = item.RoomAmount,
                        Price = item.Price,
                        GuestRoomID = item.GuestRoom.ID,
                        State = state
                    };
                });

                return Ok(resp);
            }
            else if (key == "400")
            {
                var orders = await myDbContext.Orders
                    .AsNoTracking()
                    .Include(o => o.User)
                    .Include(o => o.GuestRoom)
                    .ThenInclude(r => r.HotelInstance)
                    .Where(o => o.User.Account == account && o.ReserveCheckOutTime < DateTimeOffset.Now && (o.Grade != 0 || o.Evaluate != ""))
                    .OrderBy(o => o.RoomAmount)
                    .Skip(position)
                    .Take(pageSize)
                    .ToListAsync();
                var num = orders.Count;

                var resp = orders.Select(item =>
                {
                    var state = "已评价";

                    return new OrderDto
                    {
                        UUID = item.UUID,
                        HotelName = item.GuestRoom.HotelInstance.HotelName,
                        HotelAddress = item.GuestRoom.HotelInstance.HotelAddress,
                        ProduceTime = item.ProduceTime,
                        ReserveCheckInTime = item.ReserveCheckInTime,
                        ReserveCheckOutTime = item.ReserveCheckOutTime,
                        RoomType = item.GuestRoom.RoomType,
                        RoomAmount = item.RoomAmount,
                        Price = item.Price,
                        GuestRoomID = item.GuestRoom.ID,
                        State = state
                    };
                });

                return Ok(resp);
            }
            else
            {
                throw new BackendException
                {
                    Code = 9000,
                    ErrorMessage = "Key错误"
                };
            }


        }


        [HttpGet("[action]")]
        public async Task<IActionResult> GetDiffOrdersNumByUserAccount([FromQuery] string key, string account = "")
        {

            if (key == "100")
            {
                var num = myDbContext.Orders
                        .AsNoTracking()
                        .Include(o => o.User)
                        .Include(o => o.GuestRoom)
                        .ThenInclude(r => r.HotelInstance)
                        .Where(o => o.User.Account == account)
                        .OrderBy(o => o.RoomAmount)
                        .Count();

                return Ok(num);
            }
            else if (key == "200")
            {
                var orders = await myDbContext.Orders
                        .AsNoTracking()
                        .Include(o => o.User)
                        .Include(o => o.GuestRoom)
                        .ThenInclude(r => r.HotelInstance)
                        .Where(o => o.User.Account == account && o.ReserveCheckOutTime > DateTimeOffset.Now)
                        .OrderBy(o => o.RoomAmount)
                        .ToListAsync();
                var num = orders.Count;
                return Ok(num);
            }
            else if (key == "300")
            {
                var orders = await myDbContext.Orders
          .AsNoTracking()
          .Include(o => o.User)
          .Include(o => o.GuestRoom)
          .ThenInclude(r => r.HotelInstance)
          .Where(o => o.User.Account == account && o.ReserveCheckOutTime < DateTimeOffset.Now && o.Grade == 0 && o.Evaluate == "")
          .OrderBy(o => o.RoomAmount)
          .ToListAsync();
                var num = orders.Count;
                return Ok(num);
            }
            else if (key == "400")
            {
                var orders = await myDbContext.Orders
          .AsNoTracking()
          .Include(o => o.User)
          .Include(o => o.GuestRoom)
          .ThenInclude(r => r.HotelInstance)
          .Where(o => o.User.Account == account && o.ReserveCheckOutTime < DateTimeOffset.Now && (o.Grade != 0 || o.Evaluate != ""))
          .OrderBy(o => o.RoomAmount)
          .ToListAsync();
                var num = orders.Count;
                return Ok(num);
            }
            else
            {
                throw new BackendException
                {
                    Code = 9000,
                    ErrorMessage = "Key错误"
                };
            }


        }








        [HttpGet("[action]")]
        public async Task<IActionResult> GetOrdersByCondition([FromQuery] int pageSize, int currentPage, string account, string uuid, string key, string hotelNameAddress = "")
        {

            var position = pageSize * (currentPage - 1);
            if (uuid.Equals("%"))
            {
                if (key == "100"){
                    var orders = await myDbContext.Orders
                        .AsNoTracking()
                        .Include(o => o.User)
                        .Include(o => o.GuestRoom)
                        .ThenInclude(r => r.HotelInstance)
                        .Where(o => o.User.Account == account && (o.GuestRoom.HotelInstance.HotelName.Contains(hotelNameAddress) || o.GuestRoom.HotelInstance.HotelAddress.Contains(hotelNameAddress)))
                        .OrderBy(o => o.RoomAmount)
                        .Skip(position)
                        .Take(pageSize)
                        .ToListAsync();
                    var resp = orders.Select(item =>
                    {
                        var state = "";
                        if (item.ReserveCheckOutTime > DateTimeOffset.Now)
                        {
                            state = "未完成";
                        }
                        else if (item.Grade == 0 && item.Evaluate == "")
                        {
                            state = "未评价";
                        }
                        else
                        {
                            state = "已评价";
                        }
                        return new OrderDto
                        {
                            UUID = item.UUID,
                            HotelName = item.GuestRoom.HotelInstance.HotelName,
                            HotelAddress = item.GuestRoom.HotelInstance.HotelAddress,
                            ProduceTime = item.ProduceTime,
                            ReserveCheckInTime = item.ReserveCheckInTime,
                            ReserveCheckOutTime = item.ReserveCheckOutTime,
                            RoomType = item.GuestRoom.RoomType,
                            RoomAmount = item.RoomAmount,
                            Price = item.Price,
                            GuestRoomID = item.GuestRoom.ID,
                            State = state
                        };
                    });

                    return Ok(resp);
                }
                else if (key == "200")
                {
                    var orders = await myDbContext.Orders
                        .AsNoTracking()
                        .Include(o => o.User)
                        .Include(o => o.GuestRoom)
                        .ThenInclude(r => r.HotelInstance)
                        .Where(o => o.User.Account == account && (o.GuestRoom.HotelInstance.HotelName.Contains(hotelNameAddress) || o.GuestRoom.HotelInstance.HotelAddress.Contains(hotelNameAddress)) && o.ReserveCheckOutTime > DateTimeOffset.Now)
                        .OrderBy(o => o.RoomAmount)
                        .Skip(position)
                        .Take(pageSize)
                        .ToListAsync();
                    var resp = orders.Select(item =>
                    {
                        var state = "未完成";
                        
                        return new OrderDto
                        {
                            UUID = item.UUID,
                            HotelName = item.GuestRoom.HotelInstance.HotelName,
                            HotelAddress = item.GuestRoom.HotelInstance.HotelAddress,
                            ProduceTime = item.ProduceTime,
                            ReserveCheckInTime = item.ReserveCheckInTime,
                            ReserveCheckOutTime = item.ReserveCheckOutTime,
                            RoomType = item.GuestRoom.RoomType,
                            RoomAmount = item.RoomAmount,
                            Price = item.Price,
                            GuestRoomID = item.GuestRoom.ID,
                            State = state
                        };
                    });

                    return Ok(resp);
                }
                else if (key == "300")
                {
                    var orders = await myDbContext.Orders
    .AsNoTracking()
    .Include(o => o.User)
    .Include(o => o.GuestRoom)
    .ThenInclude(r => r.HotelInstance)
    .Where(o => o.User.Account == account && (o.GuestRoom.HotelInstance.HotelName.Contains(hotelNameAddress) || o.GuestRoom.HotelInstance.HotelAddress.Contains(hotelNameAddress)) && o.ReserveCheckOutTime < DateTimeOffset.Now && o.Grade == 0 && o.Evaluate == "")
    .OrderBy(o => o.RoomAmount)
    .Skip(position)
    .Take(pageSize)
    .ToListAsync();
                    var resp = orders.Select(item =>
                    {
                        var state = "未评价";

                        return new OrderDto
                        {
                            UUID = item.UUID,
                            HotelName = item.GuestRoom.HotelInstance.HotelName,
                            HotelAddress = item.GuestRoom.HotelInstance.HotelAddress,
                            ProduceTime = item.ProduceTime,
                            ReserveCheckInTime = item.ReserveCheckInTime,
                            ReserveCheckOutTime = item.ReserveCheckOutTime,
                            RoomType = item.GuestRoom.RoomType,
                            RoomAmount = item.RoomAmount,
                            Price = item.Price,
                            GuestRoomID = item.GuestRoom.ID,
                            State = state
                        };
                    });

                    return Ok(resp);
                }
                else if (key == "400")
                {
                    var orders = await myDbContext.Orders
    .AsNoTracking()
    .Include(o => o.User)
    .Include(o => o.GuestRoom)
    .ThenInclude(r => r.HotelInstance)
    .Where(o => o.User.Account == account && (o.GuestRoom.HotelInstance.HotelName.Contains(hotelNameAddress) || o.GuestRoom.HotelInstance.HotelAddress.Contains(hotelNameAddress)) && o.ReserveCheckOutTime < DateTimeOffset.Now && (o.Grade != 0 || o.Evaluate != ""))
    .OrderBy(o => o.RoomAmount)
    .Skip(position)
    .Take(pageSize)
    .ToListAsync();
                    var resp = orders.Select(item =>
                    {
                        var state = "已评价";

                        return new OrderDto
                        {
                            UUID = item.UUID,
                            HotelName = item.GuestRoom.HotelInstance.HotelName,
                            HotelAddress = item.GuestRoom.HotelInstance.HotelAddress,
                            ProduceTime = item.ProduceTime,
                            ReserveCheckInTime = item.ReserveCheckInTime,
                            ReserveCheckOutTime = item.ReserveCheckOutTime,
                            RoomType = item.GuestRoom.RoomType,
                            RoomAmount = item.RoomAmount,
                            Price = item.Price,
                            GuestRoomID = item.GuestRoom.ID,
                            State = state
                        };
                    });

                    return Ok(resp);
                }
                else
                {

                    {
                        throw new BackendException
                        {
                            Code = 9000,
                            ErrorMessage = "Key错误"
                        };
                    }
                }

            }
            else if (hotelNameAddress.Equals("%"))
            {
                if (key == "100")
                {
                    var orders = await myDbContext.Orders
                        .AsNoTracking()
                        .Include(o => o.User)
                        .Include(o => o.GuestRoom)
                        .ThenInclude(r => r.HotelInstance)
                        .Where(o => o.UUID == uuid && o.User.Account == account)
                        .OrderBy(o => o.RoomAmount)
                        .Skip(position)
                        .Take(pageSize)
                        .ToListAsync();
                    if (orders.Count == 0)
                    {
                        throw new BackendException
                        {
                            Code = 9000,
                            ErrorMessage = "账单号错误"
                        };
                    }
                    var resp = orders.Select(item =>
                    {
                        var state = "";
                        if (item.ReserveCheckOutTime > DateTimeOffset.Now)
                        {
                            state = "未完成";
                        }
                        else if (item.Grade == 0 && item.Evaluate == "")
                        {
                            state = "未评价";
                        }
                        else
                        {
                            state = "已评价";
                        }
                        return new OrderDto
                        {
                            UUID = item.UUID,
                            HotelName = item.GuestRoom.HotelInstance.HotelName,
                            HotelAddress = item.GuestRoom.HotelInstance.HotelAddress,
                            ProduceTime = item.ProduceTime,
                            ReserveCheckInTime = item.ReserveCheckInTime,
                            ReserveCheckOutTime = item.ReserveCheckOutTime,
                            RoomType = item.GuestRoom.RoomType,
                            RoomAmount = item.RoomAmount,
                            Price = item.Price,
                            GuestRoomID = item.GuestRoom.ID,
                            State = state
                        };
                    });

                    return Ok(resp);
                }
                else if (key == "200")
                {
                    var orders = await myDbContext.Orders
                        .AsNoTracking()
                        .Include(o => o.User)
                        .Include(o => o.GuestRoom)
                        .ThenInclude(r => r.HotelInstance)
                        .Where(o => o.UUID == uuid && o.User.Account == account && o.ReserveCheckOutTime > DateTimeOffset.Now)
                        .OrderBy(o => o.RoomAmount)
                        .Skip(position)
                        .Take(pageSize)
                        .ToListAsync();
                    if (orders.Count == 0)
                    {
                        throw new BackendException
                        {
                            Code = 9000,
                            ErrorMessage = "账单号错误"
                        };
                    }
                    var resp = orders.Select(item =>
                    {
                        var state = "未完成";
                        return new OrderDto
                        {
                            UUID = item.UUID,
                            HotelName = item.GuestRoom.HotelInstance.HotelName,
                            HotelAddress = item.GuestRoom.HotelInstance.HotelAddress,
                            ProduceTime = item.ProduceTime,
                            ReserveCheckInTime = item.ReserveCheckInTime,
                            ReserveCheckOutTime = item.ReserveCheckOutTime,
                            RoomType = item.GuestRoom.RoomType,
                            RoomAmount = item.RoomAmount,
                            Price = item.Price,
                            GuestRoomID = item.GuestRoom.ID,
                            State = state
                        };
                    });

                    return Ok(resp);
                }
                else if (key == "300")
                {
                    var orders = await myDbContext.Orders
                        .AsNoTracking()
                        .Include(o => o.User)
                        .Include(o => o.GuestRoom)
                        .ThenInclude(r => r.HotelInstance)
                        .Where(o => o.UUID == uuid && o.User.Account == account && o.ReserveCheckOutTime < DateTimeOffset.Now && o.Grade == 0 && o.Evaluate == "")
                        .OrderBy(o => o.RoomAmount)
                        .Skip(position)
                        .Take(pageSize)
                        .ToListAsync();
                    if (orders.Count == 0)
                    {
                        throw new BackendException
                        {
                            Code = 9000,
                            ErrorMessage = "账单号错误"
                        };
                    }
                    var resp = orders.Select(item =>
                    {
                        var state = "未评价";
                        return new OrderDto
                        {
                            UUID = item.UUID,
                            HotelName = item.GuestRoom.HotelInstance.HotelName,
                            HotelAddress = item.GuestRoom.HotelInstance.HotelAddress,
                            ProduceTime = item.ProduceTime,
                            ReserveCheckInTime = item.ReserveCheckInTime,
                            ReserveCheckOutTime = item.ReserveCheckOutTime,
                            RoomType = item.GuestRoom.RoomType,
                            RoomAmount = item.RoomAmount,
                            Price = item.Price,
                            GuestRoomID = item.GuestRoom.ID,
                            State = state
                        };
                    });

                    return Ok(resp);
                }
                else if (key == "400")
                {
                    var orders = await myDbContext.Orders
                        .AsNoTracking()
                        .Include(o => o.User)
                        .Include(o => o.GuestRoom)
                        .ThenInclude(r => r.HotelInstance)
                        .Where(o => o.UUID == uuid && o.User.Account == account && o.ReserveCheckOutTime < DateTimeOffset.Now && (o.Grade != 0 || o.Evaluate != ""))
                        .OrderBy(o => o.RoomAmount)
                        .Skip(position)
                        .Take(pageSize)
                        .ToListAsync();
                    if (orders.Count == 0)
                    {
                        throw new BackendException
                        {
                            Code = 9000,
                            ErrorMessage = "账单号错误"
                        };
                    }
                    var resp = orders.Select(item =>
                    {
                        var state = "已评价";
                        return new OrderDto
                        {
                            UUID = item.UUID,
                            HotelName = item.GuestRoom.HotelInstance.HotelName,
                            HotelAddress = item.GuestRoom.HotelInstance.HotelAddress,
                            ProduceTime = item.ProduceTime,
                            ReserveCheckInTime = item.ReserveCheckInTime,
                            ReserveCheckOutTime = item.ReserveCheckOutTime,
                            RoomType = item.GuestRoom.RoomType,
                            RoomAmount = item.RoomAmount,
                            Price = item.Price,
                            GuestRoomID = item.GuestRoom.ID,
                            State = state
                        };
                    });

                    return Ok(resp);
                }
                else
                {

                    {
                        throw new BackendException
                        {
                            Code = 9000,
                            ErrorMessage = "Key错误"
                        };
                    }
                }
            }
            else
            {
                if (key == "100")
                {
                    var orders = await myDbContext.Orders
                        .AsNoTracking()
                        .Include(o => o.User)
                        .Include(o => o.GuestRoom)
                        .ThenInclude(r => r.HotelInstance)
                       .Where(o => (o.GuestRoom.HotelInstance.HotelName.Contains(hotelNameAddress) || o.GuestRoom.HotelInstance.HotelAddress.Contains(hotelNameAddress)) && o.UUID == uuid && o.User.Account == account)
                        .OrderBy(o => o.RoomAmount)
                        .Skip(position)
                        .Take(pageSize)
                        .ToListAsync();
                    if (orders.Count == 0)
                    {
                        throw new BackendException
                        {
                            Code = 9000,
                            ErrorMessage = "账单号错误"
                        };
                    }
                    var resp = orders.Select(item =>
                    {
                        var state = "";
                        if (item.ReserveCheckOutTime > DateTimeOffset.Now)
                        {
                            state = "未完成";
                        }
                        else if (item.Grade == 0 && item.Evaluate == "")
                        {
                            state = "未评价";
                        }
                        else
                        {
                            state = "已评价";
                        }
                        return new OrderDto
                        {
                            UUID = item.UUID,
                            HotelName = item.GuestRoom.HotelInstance.HotelName,
                            HotelAddress = item.GuestRoom.HotelInstance.HotelAddress,
                            ProduceTime = item.ProduceTime,
                            ReserveCheckInTime = item.ReserveCheckInTime,
                            ReserveCheckOutTime = item.ReserveCheckOutTime,
                            RoomType = item.GuestRoom.RoomType,
                            RoomAmount = item.RoomAmount,
                            Price = item.Price,
                            GuestRoomID = item.GuestRoom.ID,
                            State = state
                        };
                    });

                    return Ok(resp);
                }
                else if (key == "200")
                {
                    var orders = await myDbContext.Orders
                        .AsNoTracking()
                        .Include(o => o.User)
                        .Include(o => o.GuestRoom)
                        .ThenInclude(r => r.HotelInstance)
                       .Where(o => (o.GuestRoom.HotelInstance.HotelName.Contains(hotelNameAddress) || o.GuestRoom.HotelInstance.HotelAddress.Contains(hotelNameAddress)) && o.UUID == uuid && o.User.Account == account && o.ReserveCheckOutTime > DateTimeOffset.Now)
                        .OrderBy(o => o.RoomAmount)
                        .Skip(position)
                        .Take(pageSize)
                        .ToListAsync();
                    if (orders.Count == 0)
                    {
                        throw new BackendException
                        {
                            Code = 9000,
                            ErrorMessage = "账单号错误"
                        };
                    }
                    var resp = orders.Select(item =>
                    {
                        var state = "未完成";
                        return new OrderDto
                        {
                            UUID = item.UUID,
                            HotelName = item.GuestRoom.HotelInstance.HotelName,
                            HotelAddress = item.GuestRoom.HotelInstance.HotelAddress,
                            ProduceTime = item.ProduceTime,
                            ReserveCheckInTime = item.ReserveCheckInTime,
                            ReserveCheckOutTime = item.ReserveCheckOutTime,
                            RoomType = item.GuestRoom.RoomType,
                            RoomAmount = item.RoomAmount,
                            Price = item.Price,
                            GuestRoomID = item.GuestRoom.ID,
                            State = state
                        };
                    });

                    return Ok(resp);
                }
                else if (key == "300")
                {
                    var orders = await myDbContext.Orders
                        .AsNoTracking()
                        .Include(o => o.User)
                        .Include(o => o.GuestRoom)
                        .ThenInclude(r => r.HotelInstance)
                       .Where(o => (o.GuestRoom.HotelInstance.HotelName.Contains(hotelNameAddress) || o.GuestRoom.HotelInstance.HotelAddress.Contains(hotelNameAddress)) && o.UUID == uuid && o.User.Account == account && o.ReserveCheckOutTime < DateTimeOffset.Now && o.Grade == 0 && o.Evaluate == "")
                        .OrderBy(o => o.RoomAmount)
                        .Skip(position)
                        .Take(pageSize)
                        .ToListAsync();
                    if (orders.Count == 0)
                    {
                        throw new BackendException
                        {
                            Code = 9000,
                            ErrorMessage = "账单号错误"
                        };
                    }
                    var resp = orders.Select(item =>
                    {
                        var state = "未评价";
                        return new OrderDto
                        {
                            UUID = item.UUID,
                            HotelName = item.GuestRoom.HotelInstance.HotelName,
                            HotelAddress = item.GuestRoom.HotelInstance.HotelAddress,
                            ProduceTime = item.ProduceTime,
                            ReserveCheckInTime = item.ReserveCheckInTime,
                            ReserveCheckOutTime = item.ReserveCheckOutTime,
                            RoomType = item.GuestRoom.RoomType,
                            RoomAmount = item.RoomAmount,
                            Price = item.Price,
                            GuestRoomID = item.GuestRoom.ID,
                            State = state
                        };
                    });

                    return Ok(resp);
                }
                else if (key == "400")
                {
                    var orders = await myDbContext.Orders
                        .AsNoTracking()
                        .Include(o => o.User)
                        .Include(o => o.GuestRoom)
                        .ThenInclude(r => r.HotelInstance)
                       .Where(o => (o.GuestRoom.HotelInstance.HotelName.Contains(hotelNameAddress) || o.GuestRoom.HotelInstance.HotelAddress.Contains(hotelNameAddress)) && o.UUID == uuid && o.User.Account == account && o.ReserveCheckOutTime < DateTimeOffset.Now && (o.Grade != 0 || o.Evaluate != ""))
                        .OrderBy(o => o.RoomAmount)
                        .Skip(position)
                        .Take(pageSize)
                        .ToListAsync();
                    if (orders.Count == 0)
                    {
                        throw new BackendException
                        {
                            Code = 9000,
                            ErrorMessage = "账单号错误"
                        };
                    }
                    var resp = orders.Select(item =>
                    {
                        var state = "已评价";
                        return new OrderDto
                        {
                            UUID = item.UUID,
                            HotelName = item.GuestRoom.HotelInstance.HotelName,
                            HotelAddress = item.GuestRoom.HotelInstance.HotelAddress,
                            ProduceTime = item.ProduceTime,
                            ReserveCheckInTime = item.ReserveCheckInTime,
                            ReserveCheckOutTime = item.ReserveCheckOutTime,
                            RoomType = item.GuestRoom.RoomType,
                            RoomAmount = item.RoomAmount,
                            Price = item.Price,
                            GuestRoomID = item.GuestRoom.ID,
                            State = state
                        };
                    });

                    return Ok(resp);
                }
                else
                {

                    {
                        throw new BackendException
                        {
                            Code = 9000,
                            ErrorMessage = "Key错误"
                        };
                    }
                }
            }

        }



        public async Task<IActionResult> GetOrdersNumByCondition([FromQuery] string account, string uuid, string key, string hotelNameAddress = "")
        {


            if (uuid.Equals("%"))
            {
                if (key == "100")
                {
                    var orders = await myDbContext.Orders
                        .AsNoTracking()
                        .Include(o => o.User)
                        .Include(o => o.GuestRoom)
                        .ThenInclude(r => r.HotelInstance)
                        .Where(o => o.User.Account == account && (o.GuestRoom.HotelInstance.HotelName.Contains(hotelNameAddress) || o.GuestRoom.HotelInstance.HotelAddress.Contains(hotelNameAddress)))
                        .OrderBy(o => o.RoomAmount)
                        .ToListAsync();


                    return Ok(orders.Count);
                }
                else if (key == "200")
                {
                    var orders = await myDbContext.Orders
                        .AsNoTracking()
                        .Include(o => o.User)
                        .Include(o => o.GuestRoom)
                        .ThenInclude(r => r.HotelInstance)
                        .Where(o => o.User.Account == account && (o.GuestRoom.HotelInstance.HotelName.Contains(hotelNameAddress) || o.GuestRoom.HotelInstance.HotelAddress.Contains(hotelNameAddress)) && o.ReserveCheckOutTime > DateTimeOffset.Now)
                        .OrderBy(o => o.RoomAmount)
                        .ToListAsync();


                    return Ok(orders.Count);
                }
                else if (key == "300")
                {
                    var orders = await myDbContext.Orders
                        .AsNoTracking()
                        .Include(o => o.User)
                        .Include(o => o.GuestRoom)
                        .ThenInclude(r => r.HotelInstance)
                        .Where(o => o.User.Account == account && (o.GuestRoom.HotelInstance.HotelName.Contains(hotelNameAddress) || o.GuestRoom.HotelInstance.HotelAddress.Contains(hotelNameAddress)) && o.ReserveCheckOutTime < DateTimeOffset.Now && o.Grade == 0 && o.Evaluate == "")
                        .OrderBy(o => o.RoomAmount)
                        .ToListAsync();


                    return Ok(orders.Count);
                }
                else if (key == "400")
                {
                    var orders = await myDbContext.Orders
                        .AsNoTracking()
                        .Include(o => o.User)
                        .Include(o => o.GuestRoom)
                        .ThenInclude(r => r.HotelInstance)
                        .Where(o => o.User.Account == account && (o.GuestRoom.HotelInstance.HotelName.Contains(hotelNameAddress) || o.GuestRoom.HotelInstance.HotelAddress.Contains(hotelNameAddress)) && o.ReserveCheckOutTime < DateTimeOffset.Now && (o.Grade != 0 || o.Evaluate != ""))
                        .OrderBy(o => o.RoomAmount)
                        .ToListAsync();


                    return Ok(orders.Count);
                }
                else
                {

                    {
                        throw new BackendException
                        {
                            Code = 9000,
                            ErrorMessage = "Key错误"
                        };
                    }
                }
            }
            else if (hotelNameAddress.Equals("%"))
            {
                if (key == "100")
                {
                    var orders = await myDbContext.Orders
                        .AsNoTracking()
                        .Include(o => o.User)
                        .Include(o => o.GuestRoom)
                        .ThenInclude(r => r.HotelInstance)
                        .Where(o => o.User.Account == account && o.UUID == uuid)
                        .OrderBy(o => o.RoomAmount)
                        .ToListAsync();


                    return Ok(orders.Count);
                }
                else if (key == "200")
                {
                    var orders = await myDbContext.Orders
                        .AsNoTracking()
                        .Include(o => o.User)
                        .Include(o => o.GuestRoom)
                        .ThenInclude(r => r.HotelInstance)
                        .Where(o => o.User.Account == account && o.UUID == uuid && o.ReserveCheckOutTime > DateTimeOffset.Now)
                        .OrderBy(o => o.RoomAmount)
                        .ToListAsync();


                    return Ok(orders.Count);
                }
                else if (key == "300")
                {
                    var orders = await myDbContext.Orders
                        .AsNoTracking()
                        .Include(o => o.User)
                        .Include(o => o.GuestRoom)
                        .ThenInclude(r => r.HotelInstance)
                        .Where(o => o.User.Account == account && o.UUID == uuid  && o.ReserveCheckOutTime < DateTimeOffset.Now && o.Grade == 0 && o.Evaluate == "")
                        .OrderBy(o => o.RoomAmount)
                        .ToListAsync();


                    return Ok(orders.Count);
                }
                else if (key == "400")
                {
                    var orders = await myDbContext.Orders
                        .AsNoTracking()
                        .Include(o => o.User)
                        .Include(o => o.GuestRoom)
                        .ThenInclude(r => r.HotelInstance)
                        .Where(o => o.User.Account == account && o.UUID == uuid && o.ReserveCheckOutTime < DateTimeOffset.Now && (o.Grade != 0 || o.Evaluate != ""))
                        .OrderBy(o => o.RoomAmount)
                        .ToListAsync();


                    return Ok(orders.Count);
                }
                else
                {

                    {
                        throw new BackendException
                        {
                            Code = 9000,
                            ErrorMessage = "Key错误"
                        };
                    }
                }
            }
            else
            {
                if (key == "100")
                {
                    var orders = await myDbContext.Orders
                        .AsNoTracking()
                        .Include(o => o.User)
                        .Include(o => o.GuestRoom)
                        .ThenInclude(r => r.HotelInstance)
                        .Where(o => o.User.Account == account && o.UUID == uuid && (o.GuestRoom.HotelInstance.HotelName.Contains(hotelNameAddress) || o.GuestRoom.HotelInstance.HotelAddress.Contains(hotelNameAddress)))
                        .OrderBy(o => o.RoomAmount)
                        .ToListAsync();


                    return Ok(orders.Count);
                }
                else if (key == "200")
                {
                    var orders = await myDbContext.Orders
                        .AsNoTracking()
                        .Include(o => o.User)
                        .Include(o => o.GuestRoom)
                        .ThenInclude(r => r.HotelInstance)
                        .Where(o => o.User.Account == account && o.UUID == uuid && (o.GuestRoom.HotelInstance.HotelName.Contains(hotelNameAddress) || o.GuestRoom.HotelInstance.HotelAddress.Contains(hotelNameAddress)) && o.ReserveCheckOutTime > DateTimeOffset.Now)
                        .OrderBy(o => o.RoomAmount)
                        .ToListAsync();


                    return Ok(orders.Count);
                }
                else if (key == "300")
                {
                    var orders = await myDbContext.Orders
                        .AsNoTracking()
                        .Include(o => o.User)
                        .Include(o => o.GuestRoom)
                        .ThenInclude(r => r.HotelInstance)
                        .Where(o => o.User.Account == account && o.UUID == uuid && (o.GuestRoom.HotelInstance.HotelName.Contains(hotelNameAddress) || o.GuestRoom.HotelInstance.HotelAddress.Contains(hotelNameAddress)) && o.ReserveCheckOutTime < DateTimeOffset.Now && o.Grade == 0 && o.Evaluate == "")
                        .OrderBy(o => o.RoomAmount)
                        .ToListAsync();


                    return Ok(orders.Count);
                }
                else if (key == "400")
                {
                    var orders = await myDbContext.Orders
                        .AsNoTracking()
                        .Include(o => o.User)
                        .Include(o => o.GuestRoom)
                        .ThenInclude(r => r.HotelInstance)
                        .Where(o => o.User.Account == account && o.UUID == uuid && (o.GuestRoom.HotelInstance.HotelName.Contains(hotelNameAddress) || o.GuestRoom.HotelInstance.HotelAddress.Contains(hotelNameAddress)) && o.ReserveCheckOutTime < DateTimeOffset.Now && (o.Grade != 0 || o.Evaluate != ""))
                        .OrderBy(o => o.RoomAmount)
                        .ToListAsync();


                    return Ok(orders.Count);
                }
                else
                {

                    {
                        throw new BackendException
                        {
                            Code = 9000,
                            ErrorMessage = "Key错误"
                        };
                    }
                }
            }

        }



    }
}
