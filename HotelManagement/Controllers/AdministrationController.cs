using HotelManagement.DTO;
using Microsoft.AspNetCore.Mvc;
using Models;
using Microsoft.EntityFrameworkCore;
using HotelManagement.ErrorHandling;
using SecurityUtils;
using System.Security.Principal;
using Models.Tables;


namespace HotelManagement.Controllers
{
    [ApiController, Route("api/[controller]")]
    public class AdministrationController : ControllerBase
    {
        private readonly MyDbContext myDbContext;
        public AdministrationController(MyDbContext myDbContext)
        {
            this.myDbContext = myDbContext;
        }

        [HttpGet("[action]")]
        public async Task<IActionResult> GetAdministration([FromQuery] string account, string password)
        {
            var administrator = await myDbContext.Administrations
                .AsNoTracking()
                .Where(administrator => administrator.Account == account && administrator.Password == password)
                .ToListAsync();
            if (administrator.Count < 1)
            {
                throw new BackendException
                {
                    Code = 8000,
                    ErrorMessage = "用户名或密码错误"
                };
            }
            var resp = administrator.Select(Item =>
            {
                return new AdministrationDto
                {
                    Token = JwtGenerator.GetJwt(Item.ID, "Admin")
                };
            });
            //每次管理员登录的时候都会给所有用户一个代金卷
            await myDbContext.SaveChangesAsync();
            return Ok(resp);
        }

        [HttpGet("[action]")]
        public async Task<IActionResult> GetAllOrders([FromQuery] int pageSize, int currentPage)
        {
            try
            {
                var (id, payload) = HttpContext.GetUserIdFromHeader();
                var admin = await myDbContext.Administrations.SingleOrDefaultAsync(item => item.ID == id);
                if (admin == null || payload != "Admin")
                {
                    throw new BackendException
                    {
                        Code = 100000,
                        ErrorMessage = "请先登录"
                    };
                }
                var position = pageSize * (currentPage - 1);

                var orders = await myDbContext.Orders
                        .AsNoTracking()
                        .Include(o => o.User)
                        .Include(o => o.GuestRoom)
                        .ThenInclude(r => r.HotelInstance)
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
                    else if (item.Grade == 0)
                    {
                        state = "未评价";
                    }
                    else
                    {
                        state = "已评价";
                    }
                    return new OrderDto
                    {
                        ID = item.ID,
                        UUID = item.UUID,
                        Platform = item.Platform,
                        PlatOrderNumber = item.PlatOrderNumber,
                        Payload = item.Payload,
                        Price = item.Price,
                        RefoundValue = item.RefoundValue,
                        ProduceTime = item.ProduceTime,
                        ReserveCheckInTime = item.ReserveCheckInTime,
                        ReserveCheckOutTime = item.ReserveCheckOutTime,
                        RoomAmount = item.RoomAmount,
                        ReserverOrders = item.ReserverOrders,
                        HotelName = item.GuestRoom.HotelInstance.HotelName,
                        HotelAddress = item.GuestRoom.HotelInstance.HotelAddress,
                        State = state
                    };
                });
                return Ok(resp);
            }
            catch (Exception ex)
            {
                if (ex is BackendException)
                {
                    throw;
                }
                throw new BackendException
                {
                    Code = 100000,
                    ErrorMessage = "请先登录",
                };
            }
        }

        [HttpGet("[action]")]
        public async Task<IActionResult> GetAllOrdersNum()
        {
            try
            {
                var (id, payload) = HttpContext.GetUserIdFromHeader();
                var admin = await myDbContext.Administrations.SingleOrDefaultAsync(item => item.ID == id);
                if (admin == null || payload != "Admin")
                {
                    throw new BackendException
                    {
                        Code = 99999,
                        ErrorMessage = "管理员不存在"
                    };
                }
                var num = myDbContext.Orders
                    .AsNoTracking()
                    .Include(o => o.User)
                    .Include(o => o.GuestRoom)
                    .ThenInclude(r => r.HotelInstance)
                    .OrderBy(o => o.RoomAmount)
                    .Count();
                return Ok(num);
            }
            catch (Exception ex)
            {
                if (ex is BackendException)
                {
                    throw;
                }
                throw new BackendException
                {
                    Code = 99999,
                    ErrorMessage = ex.Message,
                };
            }
        }

        [HttpGet("[action]")]
        public async Task<IActionResult> GetAllUnfinishedOrders([FromQuery] int pageSize, int currentPage)
        {
            try
            {
                var (id, payload) = HttpContext.GetUserIdFromHeader();
                var admin = await myDbContext.Administrations.SingleOrDefaultAsync(item => item.ID == id);
                if (admin == null || payload != "Admin")
                {
                    throw new BackendException
                    {
                        Code = 100000,
                        ErrorMessage = "请先登录"
                    };
                }
                var position = pageSize * (currentPage - 1);

                var orders = await myDbContext.Orders
                        .AsNoTracking()
                        .Include(o => o.User)
                        .Include(o => o.GuestRoom)
                        .ThenInclude(r => r.HotelInstance)
                        .Where(o => o.ReserveCheckOutTime > DateTimeOffset.Now)
                        .OrderBy(o => o.RoomAmount)
                        .Skip(position)
                        .Take(pageSize)
                        .ToListAsync();

                var resp = orders.Select(item =>
                {
                    var state = "未完成";

                    return new OrderDto
                    {
                        ID = item.ID,
                        UUID = item.UUID,
                        Platform = item.Platform,
                        PlatOrderNumber = item.PlatOrderNumber,
                        Payload = item.Payload,
                        Price = item.Price,
                        RefoundValue = item.RefoundValue,
                        ProduceTime = item.ProduceTime,
                        ReserveCheckInTime = item.ReserveCheckInTime,
                        ReserveCheckOutTime = item.ReserveCheckOutTime,
                        RoomAmount = item.RoomAmount,
                        ReserverOrders = item.ReserverOrders,
                        HotelName = item.GuestRoom.HotelInstance.HotelName,
                        HotelAddress = item.GuestRoom.HotelInstance.HotelAddress,
                        State = state

                    };
                });

                return Ok(resp);
            }
            catch (Exception ex)
            {
                if (ex is BackendException)
                {
                    throw;
                }
                throw new BackendException
                {
                    Code = 100000,
                    ErrorMessage = "请先登录",
                };
            }
        }

        [HttpGet("[action]")]
        public async Task<IActionResult> GetAllUnfinishedOrdersNum()
        {
            try
            {
                var (id, payload) = HttpContext.GetUserIdFromHeader();
                var admin = await myDbContext.Administrations.SingleOrDefaultAsync(item => item.ID == id);
                if (admin == null || payload != "Admin")
                {
                    throw new BackendException
                    {
                        Code = 99999,
                        ErrorMessage = "管理员不存在"
                    };
                }
                var orders = await myDbContext.Orders
                    .AsNoTracking()
                    .Include(o => o.User)
                    .Include(o => o.GuestRoom)
                    .ThenInclude(r => r.HotelInstance)
                    .Where(o => o.ReserveCheckOutTime > DateTimeOffset.Now)
                    .OrderBy(o => o.RoomAmount)
                    .ToListAsync();
                var num = orders.Count;
                return Ok(num);
            }
            catch (Exception ex)
            {
                if (ex is BackendException)
                {
                    throw;
                }
                throw new BackendException
                {
                    Code = 99999,
                    ErrorMessage = ex.Message,
                };
            }
        }

        [HttpGet("[action]")]
        public async Task<IActionResult> GetAllUnevaluatedOrders([FromQuery] int pageSize, int currentPage)
        {
            try
            {
                var (id, payload) = HttpContext.GetUserIdFromHeader();
                var admin = await myDbContext.Administrations.SingleOrDefaultAsync(item => item.ID == id);
                if (admin == null || payload != "Admin")
                {
                    throw new BackendException
                    {
                        Code = 100000,
                        ErrorMessage = "请先登录"
                    };
                }
                var position = pageSize * (currentPage - 1);

                var orders = await myDbContext.Orders
                        .AsNoTracking()
                        .Include(o => o.User)
                        .Include(o => o.GuestRoom)
                        .ThenInclude(r => r.HotelInstance)
                        .Where(o => o.ReserveCheckOutTime < DateTimeOffset.Now && o.Grade == 0)
                        .OrderBy(o => o.RoomAmount)
                        .Skip(position)
                        .Take(pageSize)
                        .ToListAsync();

                var resp = orders.Select(item =>
                {
                    var state = "未评价";
                    return new OrderDto
                    {
                        ID = item.ID,
                        UUID = item.UUID,
                        Platform = item.Platform,
                        PlatOrderNumber = item.PlatOrderNumber,
                        Payload = item.Payload,
                        Price = item.Price,
                        RefoundValue = item.RefoundValue,
                        ProduceTime = item.ProduceTime,
                        ReserveCheckInTime = item.ReserveCheckInTime,
                        ReserveCheckOutTime = item.ReserveCheckOutTime,
                        RoomAmount = item.RoomAmount,
                        ReserverOrders = item.ReserverOrders,
                        HotelName = item.GuestRoom.HotelInstance.HotelName,
                        HotelAddress = item.GuestRoom.HotelInstance.HotelAddress,
                        State = state

                    };
                });

                return Ok(resp);
            }
            catch (Exception ex)
            {
                if (ex is BackendException)
                {
                    throw;
                }
                throw new BackendException
                {
                    Code = 100000,
                    ErrorMessage = "请先登录",
                };
            }
        }

        [HttpGet("[action]")]
        public async Task<IActionResult> GetAllUnevaluatedOrdersNum()
        {
            try
            {
                var (id, payload) = HttpContext.GetUserIdFromHeader();
                var admin = await myDbContext.Administrations.SingleOrDefaultAsync(item => item.ID == id);
                if (admin == null || payload != "Admin")
                {
                    throw new BackendException
                    {
                        Code = 99999,
                        ErrorMessage = "管理员不存在"
                    };
                }
                var orders = await myDbContext.Orders
                    .AsNoTracking()
                    .Include(o => o.User)
                    .Include(o => o.GuestRoom)
                    .ThenInclude(r => r.HotelInstance)
                    .Where(o => o.ReserveCheckOutTime < DateTimeOffset.Now && o.Grade == 0)
                    .OrderBy(o => o.RoomAmount)
                    .ToListAsync();
                var num = orders.Count;
                return Ok(num);
            }
            catch (Exception ex)
            {
                if (ex is BackendException)
                {
                    throw;
                }
                throw new BackendException
                {
                    Code = 99999,
                    ErrorMessage = ex.Message,
                };
            }
        }

        [HttpGet("[action]")]
        public async Task<IActionResult> GetAllEvaluatedOrders([FromQuery] int pageSize, int currentPage)
        {
            try
            {
                var (id, payload) = HttpContext.GetUserIdFromHeader();
                var admin = await myDbContext.Administrations.SingleOrDefaultAsync(item => item.ID == id);
                if (admin == null || payload != "Admin")
                {
                    throw new BackendException
                    {
                        Code = 100000,
                        ErrorMessage = "请先登录"
                    };
                }
                var position = pageSize * (currentPage - 1);

                var orders = await myDbContext.Orders
                        .AsNoTracking()
                        .Include(o => o.User)
                        .Include(o => o.GuestRoom)
                        .ThenInclude(r => r.HotelInstance)
                        .Where(o => o.ReserveCheckOutTime < DateTimeOffset.Now && (o.Grade != 0))
                        .OrderBy(o => o.RoomAmount)
                        .Skip(position)
                        .Take(pageSize)
                        .ToListAsync();

                var resp = orders.Select(item =>
                {
                    var state = "已评价";
                    return new OrderDto
                    {
                        ID = item.ID,
                        UUID = item.UUID,
                        Platform = item.Platform,
                        PlatOrderNumber = item.PlatOrderNumber,
                        Payload = item.Payload,
                        Price = item.Price,
                        RefoundValue = item.RefoundValue,
                        ProduceTime = item.ProduceTime,
                        ReserveCheckInTime = item.ReserveCheckInTime,
                        ReserveCheckOutTime = item.ReserveCheckOutTime,
                        RoomAmount = item.RoomAmount,
                        ReserverOrders = item.ReserverOrders,
                        HotelName = item.GuestRoom.HotelInstance.HotelName,
                        HotelAddress = item.GuestRoom.HotelInstance.HotelAddress,
                        State = state

                    };
                });

                return Ok(resp);
            }
            catch (Exception ex)
            {
                if (ex is BackendException)
                {
                    throw;
                }
                throw new BackendException
                {
                    Code = 100000,
                    ErrorMessage = "请先登录",
                };
            }
        }

        [HttpGet("[action]")]
        public async Task<IActionResult> GetAllEvaluatedOrdersNum()
        {
            try
            {
                var (id, payload) = HttpContext.GetUserIdFromHeader();
                var admin = await myDbContext.Administrations.SingleOrDefaultAsync(item => item.ID == id);
                if (admin == null || payload != "Admin")
                {
                    throw new BackendException
                    {
                        Code = 99999,
                        ErrorMessage = "管理员不存在"
                    };
                }
                var orders = await myDbContext.Orders
                    .AsNoTracking()
                    .Include(o => o.User)
                    .Include(o => o.GuestRoom)
                    .ThenInclude(r => r.HotelInstance)
                    .Where(o => o.ReserveCheckOutTime < DateTimeOffset.Now && (o.Grade != 0))
                    .OrderBy(o => o.RoomAmount)
                    .ToListAsync();
                var num = orders.Count;
                return Ok(num);
            }
            catch (Exception ex)
            {
                if (ex is BackendException)
                {
                    throw;
                }
                throw new BackendException
                {
                    Code = 99999,
                    ErrorMessage = ex.Message,
                };
            }
        }

        [HttpDelete("[action]")]
        public async Task<IActionResult> DeleteOrderByUUID([FromQuery] string uuid)
        {
            try
            {
                var (id, payload) = HttpContext.GetUserIdFromHeader();
                var admin = await myDbContext.Administrations.SingleOrDefaultAsync(item => item.ID == id);
                if (admin == null || payload != "Admin")
                {
                    throw new BackendException
                    {
                        Code = 100000,
                        ErrorMessage = "请先登录"
                    };
                }
                var order = await myDbContext.Orders.Include(o => o.User)
                .SingleOrDefaultAsync(it => it.UUID == uuid);

                if (order == null)
                {
                    throw new BackendException
                    {
                        Code = 6000,
                        ErrorMessage = "订单不存在"
                    };
                }
                var user = await myDbContext.Users
                .SingleOrDefaultAsync(u => u.Account == order.User.Account);
                user.Points -= order.Price;
                myDbContext.Orders.Remove(order);
                await myDbContext.SaveChangesAsync();

                return Ok();
            }
            catch (Exception ex)
            {
                if (ex is BackendException)
                {
                    throw;
                }
                throw new BackendException
                {
                    Code = 100000,
                    ErrorMessage = "请先登录",
                };
            }
        }

        [HttpPut("[action]")]
        public async Task<IActionResult> ModifyOrderByAdmin([FromBody] OrderDto orderDto)
        {
            try
            {
                var (id, payload) = HttpContext.GetUserIdFromHeader();
                var admin = await myDbContext.Administrations.SingleOrDefaultAsync(item => item.ID == id);
                if (admin == null || payload != "Admin")
                {
                    throw new BackendException
                    {
                        Code = 100000,
                        ErrorMessage = "请先登录"
                    };
                }
                var item = await myDbContext.Orders
                    .Include(o => o.User)
                    .Include(o => o.GuestRoom)
                    .ThenInclude(r => r.HotelInstance)
                    .SingleOrDefaultAsync(it => it.UUID == orderDto.UUID);
                if (item == null)
                {
                    throw new BackendException
                    {
                        Code = 6000,
                        ErrorMessage = "订单不存在"
                    };
                }
                DateTimeOffset dd = new DateTimeOffset(item.ReserveCheckInTime.Year, item.ReserveCheckInTime.Month, item.ReserveCheckInTime.Day + 1, 12, 0, 0, item.ReserveCheckInTime.Offset);
                if (orderDto.ReserveCheckInTime > dd || orderDto.ReserveCheckInTime < item.ReserveCheckInTime || orderDto.ReserveCheckOutTime > item.ReserveCheckOutTime || orderDto.ReserveCheckInTime >= orderDto.ReserveCheckOutTime)
                {


                    throw new BackendException
                    {

                        Code = 3003,
                        ErrorMessage = "该时段无法入住，请重新选择时间"
                    };
                }
                //Order可以修改预订的入住和离开时间和价格
                if (orderDto.ReserveCheckInTime != DateTimeOffset.MinValue)
                {
                    item.ReserveCheckInTime = orderDto.ReserveCheckInTime;
                }
                if (orderDto.ReserveCheckOutTime != DateTimeOffset.MinValue)
                {
                    item.ReserveCheckOutTime = orderDto.ReserveCheckOutTime;
                }
                if (orderDto.Price != 0)
                {
                    var user = await myDbContext.Users
    .SingleOrDefaultAsync(u => u.Account == item.User.Account);
                    user.Points += orderDto.Price;
                    user.Points -= item.Price;
                    item.Price = orderDto.Price;

                }

                await myDbContext.SaveChangesAsync();
                var state = "";
                if (item.ReserveCheckOutTime > DateTimeOffset.Now)
                {
                    state = "未完成";
                }
                else if (item.Grade == 0)
                {
                    state = "未评价";
                }
                else
                {
                    state = "已评价";
                }

                var resp = new OrderDto
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

                return Ok(resp);
            }
            catch (Exception ex)
            {
                if (ex is BackendException)
                {
                    throw;
                }
                throw new BackendException
                {
                    Code = 100000,
                    ErrorMessage = "请先登录",
                };
            }
        }

        [HttpGet("[action]")]
        public async Task<IActionResult> GetOrdersByConditionWithoutAccount([FromQuery] int pageSize, int currentPage, string uuid, string key, string hotelNameAddress = "")
        {
            try
            {
                var (id, payload) = HttpContext.GetUserIdFromHeader();
                var admin = await myDbContext.Administrations.SingleOrDefaultAsync(item => item.ID == id);
                if (admin == null || payload != "Admin")
                {
                    throw new BackendException
                    {
                        Code = 100000,
                        ErrorMessage = "请先登录"
                    };
                }
                var position = pageSize * (currentPage - 1);
                if (uuid.Equals("%"))
                {
                    if (key == "100")
                    {
                        var orders = await myDbContext.Orders
                            .AsNoTracking()
                            .Include(o => o.User)
                            .Include(o => o.GuestRoom)
                            .ThenInclude(r => r.HotelInstance)
                            .Where(o => (o.GuestRoom.HotelInstance.HotelName.Contains(hotelNameAddress) || o.GuestRoom.HotelInstance.HotelAddress.Contains(hotelNameAddress)))
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
                            else if (item.Grade == 0)
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
                            .Where(o => (o.GuestRoom.HotelInstance.HotelName.Contains(hotelNameAddress) || o.GuestRoom.HotelInstance.HotelAddress.Contains(hotelNameAddress)) && o.ReserveCheckOutTime > DateTimeOffset.Now)
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
        .Where(o => (o.GuestRoom.HotelInstance.HotelName.Contains(hotelNameAddress) || o.GuestRoom.HotelInstance.HotelAddress.Contains(hotelNameAddress)) && o.ReserveCheckOutTime < DateTimeOffset.Now && o.Grade == 0)
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
        .Where(o => (o.GuestRoom.HotelInstance.HotelName.Contains(hotelNameAddress) || o.GuestRoom.HotelInstance.HotelAddress.Contains(hotelNameAddress)) && o.ReserveCheckOutTime < DateTimeOffset.Now && (o.Grade != 0))
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
                                Code = 5005,
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
                            .Where(o => o.UUID == uuid)
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
                            else if (item.Grade == 0)
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
                            .Where(o => o.UUID == uuid && o.ReserveCheckOutTime > DateTimeOffset.Now)
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
                            .Where(o => o.UUID == uuid && o.ReserveCheckOutTime < DateTimeOffset.Now && o.Grade == 0)
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
                            .Where(o => o.UUID == uuid && o.ReserveCheckOutTime < DateTimeOffset.Now && (o.Grade != 0))
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
                                Code = 5005,
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
                           .Where(o => (o.GuestRoom.HotelInstance.HotelName.Contains(hotelNameAddress) || o.GuestRoom.HotelInstance.HotelAddress.Contains(hotelNameAddress)) && o.UUID == uuid)
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
                            else if (item.Grade == 0)
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
                           .Where(o => (o.GuestRoom.HotelInstance.HotelName.Contains(hotelNameAddress) || o.GuestRoom.HotelInstance.HotelAddress.Contains(hotelNameAddress)) && o.UUID == uuid && o.ReserveCheckOutTime > DateTimeOffset.Now)
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
                           .Where(o => (o.GuestRoom.HotelInstance.HotelName.Contains(hotelNameAddress) || o.GuestRoom.HotelInstance.HotelAddress.Contains(hotelNameAddress)) && o.UUID == uuid && o.ReserveCheckOutTime < DateTimeOffset.Now && o.Grade == 0)
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
                           .Where(o => (o.GuestRoom.HotelInstance.HotelName.Contains(hotelNameAddress) || o.GuestRoom.HotelInstance.HotelAddress.Contains(hotelNameAddress)) && o.UUID == uuid && o.ReserveCheckOutTime < DateTimeOffset.Now && (o.Grade != 0))
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
                                Code = 5005,
                                ErrorMessage = "Key错误"
                            };
                        }
                    }
                }
            }


            catch (Exception ex)
            {
                if (ex is BackendException)
                {
                    throw;
                }
                throw new BackendException
                {
                    Code = 100000,
                    ErrorMessage = "请先登录",
                };
            }
        }

        [HttpGet("[action]")]
        public async Task<IActionResult> GetOrdersNumByConditionWithoutAccount([FromQuery] string uuid, string key, string hotelNameAddress = "")
        {
            try
            {
                var (id, payload) = HttpContext.GetUserIdFromHeader();
                var admin = await myDbContext.Administrations.SingleOrDefaultAsync(item => item.ID == id);
                if (admin == null || payload != "Admin")
                {
                    throw new BackendException
                    {
                        Code = 99999,
                        ErrorMessage = "管理员不存在"
                    };
                }
                if (uuid != "%")
                {
                    var orders = myDbContext.Orders
                        .AsNoTracking()
                        .Include(o => o.User)
                        .Include(o => o.GuestRoom)
                        .ThenInclude(r => r.HotelInstance)
                        .Where(o => o.UUID == uuid)
                        .OrderBy(o => o.RoomAmount)
                        .Count();
                    if (orders == 0)
                    {
                        throw new BackendException
                        {
                            Code = 9000,
                            ErrorMessage = "账单号错误"
                        };
                    }
                }
                if (uuid.Equals("%"))
                {
                    if (key == "100")
                    {
                        var orders = await myDbContext.Orders
                            .AsNoTracking()
                            .Include(o => o.User)
                            .Include(o => o.GuestRoom)
                            .ThenInclude(r => r.HotelInstance)
                            .Where(o => (o.GuestRoom.HotelInstance.HotelName.Contains(hotelNameAddress) || o.GuestRoom.HotelInstance.HotelAddress.Contains(hotelNameAddress)))
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
                            .Where(o => (o.GuestRoom.HotelInstance.HotelName.Contains(hotelNameAddress) || o.GuestRoom.HotelInstance.HotelAddress.Contains(hotelNameAddress)) && o.ReserveCheckOutTime > DateTimeOffset.Now)
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
                            .Where(o => (o.GuestRoom.HotelInstance.HotelName.Contains(hotelNameAddress) || o.GuestRoom.HotelInstance.HotelAddress.Contains(hotelNameAddress)) && o.ReserveCheckOutTime < DateTimeOffset.Now && o.Grade == 0)
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
                            .Where(o => (o.GuestRoom.HotelInstance.HotelName.Contains(hotelNameAddress) || o.GuestRoom.HotelInstance.HotelAddress.Contains(hotelNameAddress)) && o.ReserveCheckOutTime < DateTimeOffset.Now && (o.Grade != 0))
                            .OrderBy(o => o.RoomAmount)
                            .ToListAsync();


                        return Ok(orders.Count);
                    }
                    else
                    {

                        {
                            throw new BackendException
                            {
                                Code = 5005,
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
                            .Where(o => o.UUID == uuid)
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
                            .Where(o => o.UUID == uuid && o.ReserveCheckOutTime > DateTimeOffset.Now)
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
                            .Where(o => o.UUID == uuid && o.ReserveCheckOutTime < DateTimeOffset.Now && o.Grade == 0)
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
                            .Where(o => o.UUID == uuid && o.ReserveCheckOutTime < DateTimeOffset.Now && (o.Grade != 0))
                            .OrderBy(o => o.RoomAmount)
                            .ToListAsync();


                        return Ok(orders.Count);
                    }
                    else
                    {

                        {
                            throw new BackendException
                            {
                                Code = 5005,
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
                            .Where(o => o.UUID == uuid && (o.GuestRoom.HotelInstance.HotelName.Contains(hotelNameAddress) || o.GuestRoom.HotelInstance.HotelAddress.Contains(hotelNameAddress)))
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
                            .Where(o => o.UUID == uuid && (o.GuestRoom.HotelInstance.HotelName.Contains(hotelNameAddress) || o.GuestRoom.HotelInstance.HotelAddress.Contains(hotelNameAddress)) && o.ReserveCheckOutTime > DateTimeOffset.Now)
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
                            .Where(o => o.UUID == uuid && (o.GuestRoom.HotelInstance.HotelName.Contains(hotelNameAddress) || o.GuestRoom.HotelInstance.HotelAddress.Contains(hotelNameAddress)) && o.ReserveCheckOutTime < DateTimeOffset.Now && o.Grade == 0)
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
                            .Where(o => o.UUID == uuid && (o.GuestRoom.HotelInstance.HotelName.Contains(hotelNameAddress) || o.GuestRoom.HotelInstance.HotelAddress.Contains(hotelNameAddress)) && o.ReserveCheckOutTime < DateTimeOffset.Now && (o.Grade != 0))
                            .OrderBy(o => o.RoomAmount)
                            .ToListAsync();


                        return Ok(orders.Count);
                    }
                    else
                    {

                        {
                            throw new BackendException
                            {
                                Code = 5005,
                                ErrorMessage = "Key错误"
                            };
                        }
                    }
                }

            }
            catch (Exception ex)
            {
                if (ex is BackendException)
                {
                    throw;
                }
                throw new BackendException
                {
                    Code = 99999,
                    ErrorMessage = ex.Message,
                };
            }

        }

    }
}
