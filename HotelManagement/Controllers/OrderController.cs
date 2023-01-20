
using HotelManagement.DTO;
using HotelManagement.ErrorHandling;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Models;
using Models.Tables;
using SecurityUtils;
using System.Collections;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Security.Principal;
using System.Text;
using static System.Formats.Asn1.AsnWriter;


namespace HotelManagement.Controllers
{
    [ApiController, Route("api/[controller]")]
    public class OrderController : ControllerBase
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

        public OrderController(MyDbContext myDbContext)
        {
            this.myDbContext = myDbContext;
        }

        [HttpPost("[action]")]
        public async Task<IActionResult> AddOrder([FromBody] AddOrderRequestDto addOrderRequest)
        {
            try
            {
                var (id, payload) = HttpContext.GetUserIdFromHeader();
                var us = await myDbContext.Users
                .SingleOrDefaultAsync(it => it.ID == id);
                if (us == null || payload != "User")
                {
                    throw new BackendException
                    {
                        Code = 100001,
                        ErrorMessage = "请先登录"
                    };
                }
                var user = await myDbContext.Users
                .SingleOrDefaultAsync(u => u.Account == addOrderRequest.Account);

                var room = await myDbContext.GuestRooms
                .SingleOrDefaultAsync(r => r.ID == addOrderRequest.GuestRoomID);

                Console.WriteLine(addOrderRequest.GuestRoomID);
                Console.WriteLine(user == null);
                Console.WriteLine(room == null);
                if (user == null || room == null)
                {
                    return BadRequest();
                }
                

                await myDbContext.SaveChangesAsync();
                var uuid = GetRandomStr(15);

                var formNumber = GetRandomStr(15);

                //这里进行排序，并取出优惠卷
                //Array.Reverse(user.Coupon);
                int index = 0;
                uint x = 0;
                for (int i = 0; i < user.Coupon.Length; i++)
                {
                    if (user.Coupon[i] > x)
                    {
                        x = user.Coupon[i];
                        index = i;
                    }
                }
                uint coupon = x;
                user.Coupon[index] = 0;

                Console.WriteLine("####################################");
                foreach (uint u in user.Coupon)
                {
                    Console.WriteLine(u);
                }
                Console.WriteLine("@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@");

                var newOrder = new Order
                {
                    User = user,
                    ProduceTime = DateTimeOffset.Now,
                    Price = addOrderRequest.Price - coupon*100,
                    GuestRoom = room,
                    ReserveCheckInTime = addOrderRequest.ReserveCheckInTime,
                    ReserveCheckOutTime = addOrderRequest.ReserveCheckOutTime,
                    UUID = uuid,
                    Platform = addOrderRequest.Platform,
                    PlatOrderNumber = formNumber,
                    RoomAmount = addOrderRequest.RoomAmount
                };
                user.Points += (addOrderRequest.Price - coupon*100);
                for (int i = 0; i < user.Info.Length; i++)
                {
                    if (user.Info[i] == null)
                    {
                        user.Info[i] = user.NickName + "，您已预定时间为" + addOrderRequest.ReserveCheckInTime.ToString() + "的房间，请及时入住。$$" + DateTimeOffset.Now.ToString();
                        break;
                    }
                }
                var entry = await myDbContext.Orders.AddAsync(newOrder);
                newOrder = entry.Entity;
                myDbContext.Users.Update(user);
                await myDbContext.SaveChangesAsync();

                var resp = new OrderDto
                {
                    UUID = uuid,

                    ProduceTime = newOrder.ProduceTime,
                    ReserveCheckInTime = newOrder.ReserveCheckInTime,
                    ReserveCheckOutTime = newOrder.ReserveCheckOutTime,
                    GuestRoomID = newOrder.GuestRoom.ID,
                    Price = newOrder.Price,
                    ID = newOrder.ID
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
                    Code = 100001,
                    ErrorMessage = "请先登录",
                };
            }
        }

        [HttpGet("[action]")]
        public async Task<IActionResult> GetOrderByUUID([FromQuery] string uuid)
        {
            try
            {
                var (id, payload) = HttpContext.GetUserIdFromHeader();
                var u = await myDbContext.Users
                .SingleOrDefaultAsync(it => it.ID == id);
                if (u == null || payload != "User")
                {
                    throw new BackendException
                    {
                        Code = 100001,
                        ErrorMessage = "请先登录"
                    };
                }
                var order = await myDbContext.Orders
                .AsNoTracking()
                .Where(order => order.UUID == uuid)
                .ToListAsync();
                if (order.Count < 1)
                {
                    throw new BackendException
                    {
                        Code = 9000,
                        ErrorMessage = "UUID错误"
                    };
                }
                var resp = order.Select(item =>
                {
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
                        ReserverOrders = item.ReserverOrders

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
                    Code = 100001,
                    ErrorMessage = "请先登录",
                };
            }
        }

        [HttpGet("[action]")]
        public async Task<IActionResult> GetDiffOrdersByUserAccount([FromQuery] int pageSize, int currentPage, string account = "")
        {
            try
            {
                var (id, payload) = HttpContext.GetUserIdFromHeader();
                var u = await myDbContext.Users
                .SingleOrDefaultAsync(it => it.ID == id);
                if (u == null || payload != "User")
                {
                    throw new BackendException
                    {
                        Code = 100001,
                        ErrorMessage = "请先登录"
                    };
                }

                var position = pageSize * (currentPage - 1);

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
                var num = orders.Count;

                Console.WriteLine(num);

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
            catch (Exception ex)
            {
                if (ex is BackendException)
                {
                    throw;
                }
                throw new BackendException
                {
                    Code = 100001,
                    ErrorMessage = "请先登录",
                };
            }
        }

        [HttpGet("[action]")]
        public async Task<IActionResult> GetOrdersByUserAccount([FromQuery] int pageSize, int currentPage, string account = "")
        {
            try
            {
                var (id, payload) = HttpContext.GetUserIdFromHeader();
                var u = await myDbContext.Users
                .SingleOrDefaultAsync(it => it.ID == id);
                if (u == null || payload != "User")
                {
                    throw new BackendException
                    {
                        Code = 100001,
                        ErrorMessage = "请先登录"
                    };
                }
                var position = pageSize * (currentPage - 1);

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
            catch (Exception ex)
            {
                if (ex is BackendException)
                {
                    throw;
                }
                throw new BackendException
                {
                    Code = 100001,
                    ErrorMessage = "请先登录",
                };
            }
        }

        [HttpGet("[action]")]
        public async Task<IActionResult> GetOrdersNumByUserAccount([FromQuery] string account = "")
        {
            try
            {
                var (id, payload) = HttpContext.GetUserIdFromHeader();
                var u = await myDbContext.Users
                .SingleOrDefaultAsync(it => it.ID == id);
                if (u == null || payload != "User")
                {
                    throw new BackendException
                    {
                        Code = 99999,
                        ErrorMessage = "请先登录"
                    };
                }
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
            catch (Exception ex)
            {
                if (ex is BackendException)
                {
                    throw;
                }
                throw new BackendException
                {
                    Code = 99999,
                    ErrorMessage = "请先登录",
                };
            }
        }

        [HttpGet("[action]")]
        public async Task<IActionResult> GetUnfinishedOrdersByUserAccount([FromQuery] int pageSize, int currentPage, string account = "")
        {
            try
            {
                var (id, payload) = HttpContext.GetUserIdFromHeader();
                var u = await myDbContext.Users
                .SingleOrDefaultAsync(it => it.ID == id);
                if (u == null || payload != "User")
                {
                    throw new BackendException
                    {
                        Code = 100001,
                        ErrorMessage = "请先登录"
                    };
                }
                var position = pageSize * (currentPage - 1);

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
            catch (Exception ex)
            {
                if (ex is BackendException)
                {
                    throw;
                }
                throw new BackendException
                {
                    Code = 100001,
                    ErrorMessage = "请先登录",
                };
            }
        }

        [HttpGet("[action]")]
        public async Task<IActionResult> GetUnfinishedOrdersNumByUserAccount([FromQuery] string account = "")
        {
            try
            {
                var (id, payload) = HttpContext.GetUserIdFromHeader();
                var u = await myDbContext.Users
                .SingleOrDefaultAsync(it => it.ID == id);
                if (u == null || payload != "User")
                {
                    throw new BackendException
                    {
                        Code = 99999,
                        ErrorMessage = "请先登录"
                    };
                }
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
            catch (Exception ex)
            {
                if (ex is BackendException)
                {
                    throw;
                }
                throw new BackendException
                {
                    Code = 99999,
                    ErrorMessage = "请先登录",
                };
            }
        }

        [HttpGet("[action]")]
        public async Task<IActionResult> GetUnevaluatedOrdersByUserAccount([FromQuery] int pageSize, int currentPage, string account = "")
        {
            try
            {
                var (id, payload) = HttpContext.GetUserIdFromHeader();
                var u = await myDbContext.Users
                .SingleOrDefaultAsync(it => it.ID == id);
                if (u == null || payload != "User")
                {
                    throw new BackendException
                    {
                        Code = 100001,
                        ErrorMessage = "请先登录"
                    };
                }
                var position = pageSize * (currentPage - 1);

                var orders = await myDbContext.Orders
                        .AsNoTracking()
                        .Include(o => o.User)
                        .Include(o => o.GuestRoom)
                        .ThenInclude(r => r.HotelInstance)
                        .Where(o => o.User.Account == account && o.ReserveCheckOutTime < DateTimeOffset.Now && o.Grade == 0)
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
            catch (Exception ex)
            {
                if (ex is BackendException)
                {
                    throw;
                }
                throw new BackendException
                {
                    Code = 100001,
                    ErrorMessage = "请先登录",
                };
            }
        }

        [HttpGet("[action]")]
        public async Task<IActionResult> GetUnevaluatedOrdersNumByUserAccount([FromQuery] string account = "")
        {
            try
            {
                var (id, payload) = HttpContext.GetUserIdFromHeader();
                var u = await myDbContext.Users
                .SingleOrDefaultAsync(it => it.ID == id);
                if (u == null || payload != "User")
                {
                    throw new BackendException
                    {
                        Code = 99999,
                        ErrorMessage = "请先登录"
                    };
                }
                var orders = await myDbContext.Orders
                    .AsNoTracking()
                    .Include(o => o.User)
                    .Include(o => o.GuestRoom)
                    .ThenInclude(r => r.HotelInstance)
                    .Where(o => o.User.Account == account && o.ReserveCheckOutTime < DateTimeOffset.Now && o.Grade == 0)
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
                    ErrorMessage = "请先登录",
                };
            }
        }

        [HttpGet("[action]")]
        public async Task<IActionResult> GetEvaluatedOrdersByUserAccount([FromQuery] int pageSize, int currentPage, string account = "")
        {
            try
            {
                var (id, payload) = HttpContext.GetUserIdFromHeader();
                var u = await myDbContext.Users
                .SingleOrDefaultAsync(it => it.ID == id);
                if (u == null || payload != "User")
                {
                    throw new BackendException
                    {
                        Code = 100001,
                        ErrorMessage = "请先登录"
                    };
                }
                var position = pageSize * (currentPage - 1);

                var orders = await myDbContext.Orders
                        .AsNoTracking()
                        .Include(o => o.User)
                        .Include(o => o.GuestRoom)
                        .ThenInclude(r => r.HotelInstance)
                        .Where(o => o.User.Account == account && o.ReserveCheckOutTime < DateTimeOffset.Now && (o.Grade != 0))
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
            catch (Exception ex)
            {
                if (ex is BackendException)
                {
                    throw;
                }
                throw new BackendException
                {
                    Code = 100001,
                    ErrorMessage = "请先登录",
                };
            }
        }

        [HttpGet("[action]")]
        public async Task<IActionResult> GetEvaluatedOrdersNumByUserAccount([FromQuery] string account = "")
        {
            try
            {
                var (id, payload) = HttpContext.GetUserIdFromHeader();
                var u = await myDbContext.Users
                .SingleOrDefaultAsync(it => it.ID == id);
                if (u == null || payload != "User")
                {
                    throw new BackendException
                    {
                        Code = 100001 - 2,
                        ErrorMessage = "请先登录"
                    };
                }
                var orders = await myDbContext.Orders
                    .AsNoTracking()
                    .Include(o => o.User)
                    .Include(o => o.GuestRoom)
                    .ThenInclude(r => r.HotelInstance)
                    .Where(o => o.User.Account == account && o.ReserveCheckOutTime < DateTimeOffset.Now && (o.Grade != 0))
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
                    Code = 100001 - 2,
                    ErrorMessage = "请先登录",
                };
            }
        }

        [HttpGet("[action]")]
        public async Task<IActionResult> GetOrdersByConditionWithAccount([FromQuery] int pageSize, int currentPage, string account, string uuid, string key, string hotelNameAddress = "")
        {
            try
            {
                var (id, payload) = HttpContext.GetUserIdFromHeader();
                var u = await myDbContext.Users
                .SingleOrDefaultAsync(it => it.ID == id);
                if (u == null || payload != "User")
                {
                    throw new BackendException
                    {
                        Code = 100001,
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
        .Where(o => o.User.Account == account && (o.GuestRoom.HotelInstance.HotelName.Contains(hotelNameAddress) || o.GuestRoom.HotelInstance.HotelAddress.Contains(hotelNameAddress)) && o.ReserveCheckOutTime < DateTimeOffset.Now && o.Grade == 0)
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
        .Where(o => o.User.Account == account && (o.GuestRoom.HotelInstance.HotelName.Contains(hotelNameAddress) || o.GuestRoom.HotelInstance.HotelAddress.Contains(hotelNameAddress)) && o.ReserveCheckOutTime < DateTimeOffset.Now && (o.Grade != 0))
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
                            .Where(o => o.UUID == uuid && o.User.Account == account)
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
                            .Where(o => o.UUID == uuid && o.User.Account == account && o.ReserveCheckOutTime > DateTimeOffset.Now)
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
                            .Where(o => o.UUID == uuid && o.User.Account == account && o.ReserveCheckOutTime < DateTimeOffset.Now && o.Grade == 0)
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
                            .Where(o => o.UUID == uuid && o.User.Account == account && o.ReserveCheckOutTime < DateTimeOffset.Now && (o.Grade != 0))
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
                           .Where(o => (o.GuestRoom.HotelInstance.HotelName.Contains(hotelNameAddress) || o.GuestRoom.HotelInstance.HotelAddress.Contains(hotelNameAddress)) && o.UUID == uuid && o.User.Account == account)
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
                           .Where(o => (o.GuestRoom.HotelInstance.HotelName.Contains(hotelNameAddress) || o.GuestRoom.HotelInstance.HotelAddress.Contains(hotelNameAddress)) && o.UUID == uuid && o.User.Account == account && o.ReserveCheckOutTime > DateTimeOffset.Now)
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
                           .Where(o => (o.GuestRoom.HotelInstance.HotelName.Contains(hotelNameAddress) || o.GuestRoom.HotelInstance.HotelAddress.Contains(hotelNameAddress)) && o.UUID == uuid && o.User.Account == account && o.ReserveCheckOutTime < DateTimeOffset.Now && o.Grade == 0)
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
                           .Where(o => (o.GuestRoom.HotelInstance.HotelName.Contains(hotelNameAddress) || o.GuestRoom.HotelInstance.HotelAddress.Contains(hotelNameAddress)) && o.UUID == uuid && o.User.Account == account && o.ReserveCheckOutTime < DateTimeOffset.Now && (o.Grade != 0))
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
                    Code = 100001,
                    ErrorMessage = "请先登录",
                };
            }
        }

        [HttpGet("[action]")]
        public async Task<IActionResult> GetOrdersNumByConditionWithAccount([FromQuery] string account, string uuid, string key, string hotelNameAddress = "")
        {
            try
            {
                var (id, payload) = HttpContext.GetUserIdFromHeader();
                var u = await myDbContext.Users
                .SingleOrDefaultAsync(it => it.ID == id);
                if (u == null || payload != "User")
                {
                    throw new BackendException
                    {
                        Code = 100001 - 2,
                        ErrorMessage = "请先登录"
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
                        var orders = myDbContext.Orders
                            .AsNoTracking()
                            .Include(o => o.User)
                            .Include(o => o.GuestRoom)
                            .ThenInclude(r => r.HotelInstance)
                            .Where(o => o.User.Account == account && (o.GuestRoom.HotelInstance.HotelName.Contains(hotelNameAddress) || o.GuestRoom.HotelInstance.HotelAddress.Contains(hotelNameAddress)))
                            .OrderBy(o => o.RoomAmount)
                            .Count();



                        return Ok(orders);
                    }
                    else if (key == "200")
                    {
                        var orders = myDbContext.Orders
                            .AsNoTracking()
                            .Include(o => o.User)
                            .Include(o => o.GuestRoom)
                            .ThenInclude(r => r.HotelInstance)
                            .Where(o => o.User.Account == account && (o.GuestRoom.HotelInstance.HotelName.Contains(hotelNameAddress) || o.GuestRoom.HotelInstance.HotelAddress.Contains(hotelNameAddress)) && o.ReserveCheckOutTime > DateTimeOffset.Now)
                            .OrderBy(o => o.RoomAmount)
                            .Count();


                        return Ok(orders);
                    }
                    else if (key == "300")
                    {
                        var orders = myDbContext.Orders
                            .AsNoTracking()
                            .Include(o => o.User)
                            .Include(o => o.GuestRoom)
                            .ThenInclude(r => r.HotelInstance)
                            .Where(o => o.User.Account == account && (o.GuestRoom.HotelInstance.HotelName.Contains(hotelNameAddress) || o.GuestRoom.HotelInstance.HotelAddress.Contains(hotelNameAddress)) && o.ReserveCheckOutTime < DateTimeOffset.Now && o.Grade == 0)
                            .OrderBy(o => o.RoomAmount)
                            .Count();


                        return Ok(orders);
                    }
                    else if (key == "400")
                    {
                        var orders = myDbContext.Orders
                            .AsNoTracking()
                            .Include(o => o.User)
                            .Include(o => o.GuestRoom)
                            .ThenInclude(r => r.HotelInstance)
                            .Where(o => o.User.Account == account && (o.GuestRoom.HotelInstance.HotelName.Contains(hotelNameAddress) || o.GuestRoom.HotelInstance.HotelAddress.Contains(hotelNameAddress)) && o.ReserveCheckOutTime < DateTimeOffset.Now && (o.Grade != 0))
                            .OrderBy(o => o.RoomAmount)
                            .Count();


                        return Ok(orders);
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
                        var orders = myDbContext.Orders
                            .AsNoTracking()
                            .Include(o => o.User)
                            .Include(o => o.GuestRoom)
                            .ThenInclude(r => r.HotelInstance)
                            .Where(o => o.User.Account == account && o.UUID == uuid)
                            .OrderBy(o => o.RoomAmount)
                            .Count();


                        return Ok(orders);
                    }
                    else if (key == "200")
                    {
                        var orders = myDbContext.Orders
                            .AsNoTracking()
                            .Include(o => o.User)
                            .Include(o => o.GuestRoom)
                            .ThenInclude(r => r.HotelInstance)
                            .Where(o => o.User.Account == account && o.UUID == uuid && o.ReserveCheckOutTime > DateTimeOffset.Now)
                            .OrderBy(o => o.RoomAmount)
                            .Count();


                        return Ok(orders);
                    }
                    else if (key == "300")
                    {
                        var orders = myDbContext.Orders
                            .AsNoTracking()
                            .Include(o => o.User)
                            .Include(o => o.GuestRoom)
                            .ThenInclude(r => r.HotelInstance)
                            .Where(o => o.User.Account == account && o.UUID == uuid && o.ReserveCheckOutTime < DateTimeOffset.Now && o.Grade == 0)
                            .OrderBy(o => o.RoomAmount)
                            .Count();


                        return Ok(orders);
                    }
                    else if (key == "400")
                    {
                        var orders = myDbContext.Orders
                            .AsNoTracking()
                            .Include(o => o.User)
                            .Include(o => o.GuestRoom)
                            .ThenInclude(r => r.HotelInstance)
                            .Where(o => o.User.Account == account && o.UUID == uuid && o.ReserveCheckOutTime < DateTimeOffset.Now && (o.Grade != 0))
                            .OrderBy(o => o.RoomAmount)
                            .Count();


                        return Ok(orders);
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
                        Console.WriteLine("!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!");
                        var orders = myDbContext.Orders
                            .AsNoTracking()
                            .Include(o => o.User)
                            .Include(o => o.GuestRoom)
                            .ThenInclude(r => r.HotelInstance)
                            .Where(o => o.User.Account == account && o.UUID == uuid && (o.GuestRoom.HotelInstance.HotelName.Contains(hotelNameAddress) || o.GuestRoom.HotelInstance.HotelAddress.Contains(hotelNameAddress)))
                            .OrderBy(o => o.RoomAmount)
                            .Count();


                        return Ok(orders);
                    }
                    else if (key == "200")
                    {
                        var orders = myDbContext.Orders
                            .AsNoTracking()
                            .Include(o => o.User)
                            .Include(o => o.GuestRoom)
                            .ThenInclude(r => r.HotelInstance)
                            .Where(o => o.User.Account == account && o.UUID == uuid && (o.GuestRoom.HotelInstance.HotelName.Contains(hotelNameAddress) || o.GuestRoom.HotelInstance.HotelAddress.Contains(hotelNameAddress)) && o.ReserveCheckOutTime > DateTimeOffset.Now)
                            .OrderBy(o => o.RoomAmount)
                            .Count();


                        return Ok(orders);
                    }
                    else if (key == "300")
                    {
                        var orders = myDbContext.Orders
                            .AsNoTracking()
                            .Include(o => o.User)
                            .Include(o => o.GuestRoom)
                            .ThenInclude(r => r.HotelInstance)
                            .Where(o => o.User.Account == account && o.UUID == uuid && (o.GuestRoom.HotelInstance.HotelName.Contains(hotelNameAddress) || o.GuestRoom.HotelInstance.HotelAddress.Contains(hotelNameAddress)) && o.ReserveCheckOutTime < DateTimeOffset.Now && o.Grade == 0)
                            .OrderBy(o => o.RoomAmount)
                            .Count();


                        return Ok(orders);
                    }
                    else if (key == "400")
                    {
                        var orders = myDbContext.Orders
                            .AsNoTracking()
                            .Include(o => o.User)
                            .Include(o => o.GuestRoom)
                            .ThenInclude(r => r.HotelInstance)
                            .Where(o => o.User.Account == account && o.UUID == uuid && (o.GuestRoom.HotelInstance.HotelName.Contains(hotelNameAddress) || o.GuestRoom.HotelInstance.HotelAddress.Contains(hotelNameAddress)) && o.ReserveCheckOutTime < DateTimeOffset.Now && (o.Grade != 0))
                            .OrderBy(o => o.RoomAmount)
                            .Count();


                        return Ok(orders);
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
                    Code = 100001 - 2,
                    ErrorMessage = "请先登录",
                };
            }

        }

        [HttpPut("[action]")]
        public async Task<IActionResult> ModifyOrderByUser([FromBody] OrderDto orderDto)
        {
            try
            {
                var (id, payload) = HttpContext.GetUserIdFromHeader();
                var u = await myDbContext.Users
                .SingleOrDefaultAsync(it => it.ID == id);
                if (u == null || payload != "User")
                {
                    throw new BackendException
                    {
                        Code = 100001,
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
                DateTimeOffset dt = item.ReserveCheckInTime.AddDays(1).AddHours(12);
                DateTimeOffset dd = new(item.ReserveCheckInTime.Year, item.ReserveCheckInTime.Month, item.ReserveCheckInTime.Day + 1, 12, 0, 0, item.ReserveCheckInTime.Offset);

                if (orderDto.ReserveCheckInTime > dd || orderDto.ReserveCheckInTime < item.ReserveCheckInTime || orderDto.ReserveCheckOutTime > item.ReserveCheckOutTime || orderDto.ReserveCheckInTime >= orderDto.ReserveCheckOutTime)
                {


                    throw new BackendException
                    {

                        Code = 3003,
                        ErrorMessage = "该时段无法入住，请重新选择时间"
                    };
                }

                item.ReserveCheckInTime = orderDto.ReserveCheckInTime;
                item.ReserveCheckOutTime = orderDto.ReserveCheckOutTime;

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
                    Code = 100001,
                    ErrorMessage = "请先登录",
                };
            }
        }

        [HttpDelete("[action]")]
        public async Task<IActionResult> DeleteUnfinishedOrderByUUID([FromQuery] string uuid)
        {
            try
            {
                var (id, payload) = HttpContext.GetUserIdFromHeader();
                var u = await myDbContext.Users
                .SingleOrDefaultAsync(it => it.ID == id);
                if (u == null || payload != "User")
                {
                    throw new BackendException
                    {
                        Code = 100001,
                        ErrorMessage = "请先登录"
                    };
                }
                var order = await myDbContext.Orders
                .Include(o => o.User)
                .SingleOrDefaultAsync(it => it.UUID == uuid && it.ReserveCheckOutTime > DateTimeOffset.Now);

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
                    Code = 100001,
                    ErrorMessage = "请先登录",
                };
            }
        }

        [HttpGet("[action]")]
        public async Task<IActionResult> GetAllOrdersByUser([FromQuery] int pageSize, int currentPage)
        {
            try
            {
                var (id, payload) = HttpContext.GetUserIdFromHeader();
                var position = pageSize * (currentPage - 1);
                var user = await myDbContext.Users
                .SingleOrDefaultAsync(it => it.ID == id);

                if (user == null)
                {
                    throw new BackendException
                    {
                        Code = 6006,
                        ErrorMessage = "账号不存在"
                    };
                }
                var orders = await myDbContext.Orders
                    .AsNoTracking()
                    .Include(o => o.User)
                    .Where(o => o.User.ID == id)
                    .Skip(position)
                    .Take(pageSize)
                    .ToListAsync();
                var resp = orders.Select(item =>
                {
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
                        GuestRoomID = item.GuestRoom.ID,
                        UserID = item.User.ID,
                        RoomAmount = item.RoomAmount,
                        ReserverOrders = item.ReserverOrders
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
                    Code = 100001,
                    ErrorMessage = "请先登录",
                };
            }
        }

        [HttpGet("[action]")]
        public async Task<IActionResult> GetWeekOrderSumByDay([FromQuery] DateTimeOffset day)
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
                var Count = new ArrayList();
                var Date = new ArrayList();
                var t = -7;
                while (t < 0)
                {
                    var date = day.AddDays(t).Date;
                    var cnt = myDbContext.Orders
                            .AsNoTracking()
                            .Where(o => o.ProduceTime.Date == date)
                            .Sum(o => o.Price);
                    Count.Add(cnt);
                    Date.Add(date.ToString("d"));
                    t += 1;
                }
                var resp = new CountDto
                {
                    Count = Count,
                    Date = Date
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
        public async Task<IActionResult> GetWeekOrderSumByNow()
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
                var day = DateTimeOffset.Now;
                var Count = new ArrayList();
                var Date = new ArrayList();
                var t = -7;
                while (t < 0)
                {
                    var date = day.AddDays(t).Date;
                    var cnt = myDbContext.Orders
                            .AsNoTracking()
                            .Where(o => o.ProduceTime.Date == date)
                            .Sum(o => o.Price);
                    Count.Add(cnt);
                    Date.Add(date.ToString("d"));
                    t += 1;
                }
                var resp = new CountDto
                {
                    Count = Count,
                    Date = Date
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
        public async Task<IActionResult> GetWeekOrderCountByDay([FromQuery] DateTimeOffset day)
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
                var Count = new ArrayList();
                var Date = new ArrayList();
                var t = -7;
                while (t < 0)
                {
                    var date = day.AddDays(t).Date;
                    var cnt = myDbContext.Orders
                            .AsNoTracking()
                            .Count(o => o.ProduceTime.Date == date);
                    Count.Add(cnt);
                    Date.Add(date.ToString("d"));
                    t += 1;
                }
                var resp = new CountDto
                {
                    Count = Count,
                    Date = Date
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
        public async Task<IActionResult> GetWeekOrderCountByNow()
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
                var day = DateTimeOffset.Now;
                var Count = new ArrayList();
                var Date = new ArrayList();
                var t = -7;
                while (t < 0)
                {
                    var date = day.AddDays(t).Date;
                    var cnt = myDbContext.Orders
                            .AsNoTracking()
                            .Count(o => o.ProduceTime.Date == date);
                    Count.Add(cnt);
                    Date.Add(date.ToString("d"));
                    t += 1;
                }
                var resp = new CountDto
                {
                    Count = Count,
                    Date = Date
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
        public async Task<IActionResult> GetWeekOrderCountByRoomDay([FromQuery] DateTimeOffset day)
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

                var Count = new Dictionary<String, ArrayList>();
                var Date = new ArrayList();
                foreach (GuestRoomType guestRoomType in Enum.GetValues(typeof(GuestRoomType)))
                {
                    var c = new ArrayList();
                    Count.Add(guestRoomType.ToString(), c);
                }
                var t = -7;
                while (t < 0)
                {
                    var date = day.AddDays(t).Date;
                    foreach (GuestRoomType guestRoomType in Enum.GetValues(typeof(GuestRoomType)))
                    {
                        var c = myDbContext.Orders
                                .AsNoTracking()
                                .Include(o => o.GuestRoom)
                                .Count(o => o.ProduceTime.Date == date && o.GuestRoom.GuestRoomType == guestRoomType);
                        Count.GetValueOrDefault(guestRoomType.ToString()).Add(c);
                    }
                    Date.Add(date.ToString("d"));
                    t += 1;
                }
                var resp = new RoomCountDto
                {
                    Count = Count,
                    Date = Date
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
        public async Task<IActionResult> GetWeekOrderCountByRoomNow()
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
                var day = DateTimeOffset.Now;
                var Count = new Dictionary<String, ArrayList>();
                var Date = new ArrayList();
                foreach (GuestRoomType guestRoomType in Enum.GetValues(typeof(GuestRoomType)))
                {
                    var c = new ArrayList();
                    Count.Add(guestRoomType.ToString(), c);
                }
                var t = -7;
                while (t < 0)
                {
                    var date = day.AddDays(t).Date;
                    foreach (GuestRoomType guestRoomType in Enum.GetValues(typeof(GuestRoomType)))
                    {
                        var c = myDbContext.Orders
                                .AsNoTracking()
                                .Include(o => o.GuestRoom)
                                .Count(o => o.ProduceTime.Date == date && o.GuestRoom.GuestRoomType == guestRoomType);
                        Count.GetValueOrDefault(guestRoomType.ToString()).Add(c);
                    }
                    Date.Add(date.ToString("d"));
                    t += 1;
                }
                var resp = new RoomCountDto
                {
                    Count = Count,
                    Date = Date
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
        public async Task<IActionResult> GetWeekOrderSumByRoomDay([FromQuery] DateTimeOffset day)
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
                var Count = new Dictionary<String, ArrayList>();
                var Date = new ArrayList();
                foreach (GuestRoomType guestRoomType in Enum.GetValues(typeof(GuestRoomType)))
                {
                    var c = new ArrayList();
                    Count.Add(guestRoomType.ToString(), c);
                }
                var t = -7;
                while (t < 0)
                {
                    var date = day.AddDays(t).Date;
                    foreach (GuestRoomType guestRoomType in Enum.GetValues(typeof(GuestRoomType)))
                    {
                        var c = myDbContext.Orders
                                .AsNoTracking()
                                .Include(o => o.GuestRoom)
                                .Where(o => o.ProduceTime.Date == date && o.GuestRoom.GuestRoomType == guestRoomType)
                                .Sum(o => o.Price);
                        Count.GetValueOrDefault(guestRoomType.ToString()).Add(c);
                    }
                    Date.Add(date.ToString("d"));
                    t += 1;
                }
                var resp = new RoomCountDto
                {
                    Count = Count,
                    Date = Date
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
        public async Task<IActionResult> GetWeekOrderSumByRoomNow()
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
                var day = DateTimeOffset.Now;
                var Count = new Dictionary<String, ArrayList>();
                var Date = new ArrayList();
                foreach (GuestRoomType guestRoomType in Enum.GetValues(typeof(GuestRoomType)))
                {
                    var c = new ArrayList();
                    Count.Add(guestRoomType.ToString(), c);
                }
                var t = -7;
                while (t < 0)
                {
                    var date = day.AddDays(t).Date;
                    foreach (GuestRoomType guestRoomType in Enum.GetValues(typeof(GuestRoomType)))
                    {
                        var c = myDbContext.Orders
                                .AsNoTracking()
                                .Include(o => o.GuestRoom)
                                .Where(o => o.ProduceTime.Date == date && o.GuestRoom.GuestRoomType == guestRoomType)
                                .Sum(o => o.Price);
                        Count.GetValueOrDefault(guestRoomType.ToString()).Add(c);
                    }
                    Date.Add(date.ToString("d"));
                    t += 1;
                }
                var resp = new RoomCountDto
                {
                    Count = Count,
                    Date = Date
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


        [HttpPost("[action]")]
        public async ValueTask<string> AddEvaluate([FromQuery] string uuid, uint grade, string evaluate)
        {
            try
            {
                var (id, payload) = HttpContext.GetUserIdFromHeader();
                var u = await myDbContext.Users
                .SingleOrDefaultAsync(it => it.ID == id);
                if (u == null || payload != "User")
                {
                    throw new BackendException
                    {
                        Code = 100001,
                        ErrorMessage = "请先登录"
                    };
                }
                var order = await myDbContext.Orders
    .Include(o => o.User)
    .Include(o => o.GuestRoom)
    .ThenInclude(r => r.HotelInstance)
    .SingleOrDefaultAsync(it => it.UUID == uuid);
                order.Evaluate = evaluate;
                order.Grade = grade;
                await myDbContext.SaveChangesAsync();
                return "evaluate successfully";
            }
            catch (Exception ex)
            {
                if (ex is BackendException)
                {
                    throw;
                }
                throw new BackendException
                {
                    Code = 100001,
                    ErrorMessage = "请先登录",
                };
            }
        }

        [HttpGet("[action]")]
        public async Task<IActionResult> GetGradeEvaluate([FromQuery] string uuid)
        {
            var order = await myDbContext.Orders
                .AsNoTracking()
                .Where(order => order.UUID == uuid)
                .ToListAsync();
            if (order.Count < 1)
            {
                throw new BackendException
                {
                    Code = 9000,
                    ErrorMessage = "UUID错误"
                };
            }
            var resp = order.Select(item =>
            {
                return new OrderDto
                {
                    Grade = item.Grade,
                    Evaluate = item.Evaluate
                };
            });

            return Ok(resp);
        }



        [HttpPost("[action]")]
        public async ValueTask<string> UploadPictures([FromForm(Name = "uuid")] string uuid, [FromForm(Name = "pictures")] List<IFormFile> pictures)
        {
            try
            {
                var (id, payload) = HttpContext.GetUserIdFromHeader();
                var u = await myDbContext.Users
                .SingleOrDefaultAsync(it => it.ID == id);
                if (u == null || payload != "User")
                {
                    throw new BackendException
                    {
                        Code = 100001,
                        ErrorMessage = "请先登录"
                    };
                }
                var item = await myDbContext.Orders
        .Include(o => o.User)
        .Include(o => o.GuestRoom)
        .ThenInclude(r => r.HotelInstance)
        .SingleOrDefaultAsync(it => it.UUID == uuid);
                if (item == null)
                {
                    throw new BackendException
                    {
                        Code = 6000,
                        ErrorMessage = "订单不存在"
                    };
                }

                item.Pictures = new string[pictures.Count];
                var i = 0;

                pictures.ForEach(file =>
                {
                    using var stream = file.OpenReadStream();
                    byte[] bytes = new byte[stream.Length];
                    stream.Read(bytes, 0, bytes.Length);
                    //string str = Encoding.Default.GetString(bytes);
                    string str = Convert.ToBase64String(bytes);
                    item.Pictures[i] = str;
                    i += 1;
                });

                await myDbContext.SaveChangesAsync();
                return "Upload pictures successfully";
            }
            catch (Exception ex)
            {
                if (ex is BackendException)
                {
                    throw;
                }
                throw new BackendException
                {
                    Code = 100001,
                    ErrorMessage = "请先登录",
                };
            }
        }

        [HttpPost("[action]")]
        public async ValueTask<string> UploadVideo([FromForm(Name = "uuid")] string uuid, [FromForm(Name = "video")] List<IFormFile> video)
        {
            try
            {
                var (id, payload) = HttpContext.GetUserIdFromHeader();
                var u = await myDbContext.Users
                .SingleOrDefaultAsync(it => it.ID == id);
                if (u == null || payload != "User")
                {
                    throw new BackendException
                    {
                        Code = 100001,
                        ErrorMessage = "请先登录"
                    };
                }
                var item = await myDbContext.Orders
.Include(o => o.User)
.Include(o => o.GuestRoom)
.ThenInclude(r => r.HotelInstance)
.SingleOrDefaultAsync(it => it.UUID == uuid);
                if (item == null)
                {
                    throw new BackendException
                    {
                        Code = 6000,
                        ErrorMessage = "订单不存在"
                    };
                }

                video.ForEach(file =>
                {
                    //FileStream fs = new FileStream("C:\\Users\\DELL\\Desktop\\11to11.png", FileMode.Open, FileAccess.Read);
                    //byte[] buffer = new byte[fs.Length];
                    //fs.Read(buffer, 0, buffer.Length);
                    using var stream = file.OpenReadStream();
                    byte[] bytes = new byte[stream.Length];
                    stream.Read(bytes, 0, bytes.Length);
                    item.Videos = bytes;

                });
                await myDbContext.SaveChangesAsync();
                return "Upload video successfully";
            }
            catch (Exception ex)
            {
                if (ex is BackendException)
                {
                    throw;
                }
                throw new BackendException
                {
                    Code = 100001,
                    ErrorMessage = "请先登录",
                };
            }
        }

        [HttpGet("[action]")]
        public async Task<IActionResult> GetVideo(string uuid)
        {
            var order = await myDbContext.Orders
                .AsNoTracking()
                .Where(order => order.UUID == uuid)
                .ToListAsync();
            if (order.Count < 1)
            {
                throw new BackendException
                {
                    Code = 9000,
                    ErrorMessage = "UUID错误"
                };
            }

            var resp = order.Select(item =>
            {
                var buffer = item.Videos;
                Console.WriteLine(buffer.Length);
                //FileStream fs = new FileStream("C:\\Users\\DELL\\Desktop\\demo2.mp4",FileMode.Create);
                //using FileStream fs = new FileStream(buffer);
                //fs.Write(buffer,0, buffer.Length);
                //fs.Close();
                //Bitmap bmp = new Bitmap(ms);
                //ms.Seek(0, SeekOrigin.Begin);

                //image.Save("C:\\Users\\DELL\\Desktop\\demo.png");
                return buffer;
            });
            return Ok(resp);
        }

        [HttpGet("[action]")]
        public async Task<IActionResult> GetPictures(string uuid)
        {
            var order = await myDbContext.Orders
                .AsNoTracking()
                .Where(order => order.UUID == uuid)
                .ToListAsync();
            if (order.Count < 1)
            {
                throw new BackendException
                {
                    Code = 9000,
                    ErrorMessage = "UUID错误"
                };
            }
            Console.WriteLine("!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!");
            var resp = order.Select(item =>
            {

                var buffer = item.Pictures;
                byte[][] ans = new byte[buffer.Length][];
                //System.Drawing.Image[] images = new System.Drawing.Image[buffer.Length];
                for (int i = 0; i < buffer.Length; i++)
                {
                    //byte[] data = Encoding.Default.GetBytes(buffer[i]);
                    ans[i] = Convert.FromBase64String(buffer[i]);

                    //byte[] data = Convert.FromBase64String(buffer[i]);
                    //using MemoryStream ms = new(data);

                    //System.Drawing.Image image = System.Drawing.Image.FromStream(ms);
                    //image.Save("C:\\Users\\DELL\\Desktop\\demo"+i+".png");

                    //images[i] = image;
                }

                return ans;
            });
            return Ok(resp);
        }

    }
}

