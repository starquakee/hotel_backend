using HotelManagement.DTO;
using HotelManagement.ErrorHandling;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Models;
using Models.Tables;
using SecurityUtils;

namespace HotelManagement.Controllers
{
    [ApiController, Route("api/[controller]")]
    public class RoomController : ControllerBase
    {
        private readonly MyDbContext myDbContext;

        public RoomController(MyDbContext myDbContext)
        {
            this.myDbContext = myDbContext;
        }

        [HttpGet("[action]")]
        public async Task<IActionResult> GetAllRooms([FromQuery] int pageSize, int currentPage)
        {
            var position = pageSize * (currentPage - 1);
            var guestRooms = await myDbContext.GuestRooms
                .AsNoTracking()
                .Include(r => r.HotelInstance)
                .Skip(position)
                .Take(pageSize)
                .ToListAsync();
            /*
            var gymRooms = await myDbContext.GymRooms
                .AsNoTracking()
                .Include(r => r.HotelInstance)
                .Skip(position)
                .Take(pageSize)
                .ToListAsync();
            var laundryRooms = await myDbContext.LaundryRooms
                .AsNoTracking()
                .Include(r => r.HotelInstance)
                .Skip(position)
                .Take(pageSize)
                .ToListAsync();
            var meetingRooms = await myDbContext.OtherGuestRooms
                .AsNoTracking()
                .Include(r => r.HotelInstance)
                .Skip(position)
                .Take(pageSize)
                .ToListAsync();
            var staffRooms = await myDbContext.StaffRooms
                .AsNoTracking()
                .Include(r => r.HotelInstance)
                .Skip(position)
                .Take(pageSize)
                .ToListAsync();
            */
            var resp = guestRooms.Select(item =>
            {
                return new GuestRoomDto
                {
                    ID = item.ID,
                    Floor = item.Floor,
                    Address = item.Address,
                    RoomStatus = item.RoomStatus,
                    RoomType = item.RoomType,
                    HotelInstanceID = item.HotelInstance.ID,
                    Price = item.Price,
                    Title = item.Title,
                    Ichnography = item.Ichnography,
                    Area = item.Area,
                    BedCount = item.BedCount,
                    WindowCount = item.WindowCount,
                    MineralWaterCount = item.MineralWaterCount,
                    CondomCount = item.CondomCount,
                    GuestRoomType= item.GuestRoomType
                };
            });
            return Ok(resp);
        }

        [HttpGet("[action]")]
        public int GetAllRoomsCount()
        {
            var number = myDbContext.GuestRooms
                .AsNoTracking()
                .Count();
            return number;
        }

        [HttpGet("[action]")]
        public async Task<IActionResult> GetHotelRooms([FromQuery] uint companyGroupID, string hotelName, int pageSize, int currentPage)
        {
            var position = pageSize * (currentPage - 1);
            var guestRooms = await myDbContext.GuestRooms
                .AsNoTracking()
                .Include(r => r.HotelInstance)
                .ThenInclude(h => h.CompanyGroup)
                .Where(r => r.HotelInstance.CompanyGroup.ID == companyGroupID && r.HotelInstance.HotelName == hotelName)
                .Skip(position)
                .Take(pageSize)
                .ToListAsync();
            var resp = guestRooms.Select(item =>
            {
                return new GuestRoomDto
                {
                    ID = item.ID,
                    Floor = item.Floor,
                    Address = item.Address,
                    RoomStatus = item.RoomStatus,
                    RoomType = item.RoomType,
                    HotelInstanceID = item.HotelInstance.ID,
                    Price = item.Price,
                    Title = item.Title,
                    Ichnography = item.Ichnography,
                    Area = item.Area,
                    BedCount = item.BedCount,
                    WindowCount = item.WindowCount,
                    MineralWaterCount = item.MineralWaterCount,
                    CondomCount = item.CondomCount,
                    GuestRoomType= item.GuestRoomType
                };
            });
            return Ok(resp);
        }

        [HttpGet("[action]")]
        public int GetHotelRoomsCount([FromQuery] uint companyGroupID, string hotelName)
        {
            var number = myDbContext.GuestRooms
                .AsNoTracking()
                .Include(r => r.HotelInstance)
                .ThenInclude(h => h.CompanyGroup)
                .Where(r => r.HotelInstance.CompanyGroup.ID == companyGroupID && r.HotelInstance.HotelName == hotelName)
                .Count();
            return number;
        }

        [HttpGet("[action]")]
        public async Task<IActionResult> GetGuestRoomByID([FromQuery] uint ID)
        {
            var room = await myDbContext.GuestRooms
                .AsNoTracking()
                .SingleOrDefaultAsync(r => r.ID == ID);
            if (room == null)
            {
                throw new BackendException
                {
                    Code = 6001,
                    ErrorMessage = "房间不存在"
                };
            }
            var resp = new GuestRoomDto
            {
                ID = room.ID,
                Floor = room.Floor,
                Address = room.Address,
                RoomStatus = room.RoomStatus,
                RoomType = room.RoomType,
                HotelInstanceID = room.HotelInstance.ID,
                Price = room.Price,
                Title=room.Title,
                Ichnography = room.Ichnography,
                Area = room.Area,
                BedCount = room.BedCount,
                WindowCount=room.WindowCount,
                CondomCount=room.CondomCount,
                MineralWaterCount = room.MineralWaterCount,
                GuestRoomType= room.GuestRoomType
            };
            return Ok(resp);
        }

        [HttpGet("[action]")]
        public async Task<IActionResult> GetHotelGuestRoomsConditional([FromQuery] int pageSize, int currentPage ,  uint CompanyGroupID, string HotelName, uint Price, DateTimeOffset StartTime, DateTimeOffset EndTime)
        {
            
            var position = pageSize * (currentPage - 1);
            var guestRooms = await myDbContext.GuestRooms
                .AsNoTracking()
                .Include(r => r.HotelInstance)
                .ThenInclude(h => h.CompanyGroup)
                .Where(r => r.HotelInstance.CompanyGroup.ID == CompanyGroupID && r.HotelInstance.HotelName == HotelName && r.Price <= Price)
                .ToListAsync();

            for (int i = 0; i < guestRooms.Count; i++)
            {
                var r = guestRooms[i];
                var orders = myDbContext.Orders
                .AsNoTracking()
                .Include(o => o.GuestRoom)
                .Where(o => o.GuestRoom == r && ((o.ReserveCheckInTime >= StartTime && o.ReserveCheckInTime <= EndTime) ||(o.ReserveCheckOutTime >= StartTime && o.ReserveCheckOutTime <= EndTime)))
                .ToList();
                if (orders.Count > 0)
                {
                    guestRooms.Remove(r);
                }
            }

            guestRooms = guestRooms.Skip(position).Take(pageSize).ToList();

            var resp = guestRooms.Select(item =>
            {
                return new GuestRoomDto
                {
                    ID = item.ID,
                    Floor = item.Floor,
                    Address = item.Address,
                    RoomStatus = item.RoomStatus,
                    RoomType = item.RoomType,
                    HotelInstanceID = item.HotelInstance.ID,
                    Price = item.Price,
                    Title = item.Title,
                    Ichnography = item.Ichnography,
                    Area = item.Area,
                    BedCount = item.BedCount,
                    WindowCount = item.WindowCount,
                    MineralWaterCount = item.MineralWaterCount,
                    CondomCount = item.CondomCount,
                    GuestRoomType= item.GuestRoomType
                };
            });
            return Ok(resp);
        }

        [HttpGet("[action]")]
        public int GetHotelGuestRoomsConditionalCount([FromQuery] uint CompanyGroupID, string HotelName, uint Price, DateTimeOffset StartTime, DateTimeOffset EndTime)
        {
            
            var guestRooms = myDbContext.GuestRooms
               .AsNoTracking()
               .Include(r => r.HotelInstance)
               .ThenInclude(h => h.CompanyGroup)
               .Where(r => r.HotelInstance.CompanyGroup.ID == CompanyGroupID && r.HotelInstance.HotelName == HotelName && r.Price <= Price)
               .ToList();

            for (int i = 0; i < guestRooms.Count; i++)
            {
                var r = guestRooms[i];
                var orders = myDbContext.Orders
                .AsNoTracking()
                .Include(o => o.GuestRoom)
                .Where(o => o.GuestRoom == r && ((o.ReserveCheckInTime >= StartTime && o.ReserveCheckInTime <= EndTime) || (o.ReserveCheckOutTime >= StartTime && o.ReserveCheckOutTime <= EndTime)))
                .ToList();
                if (orders.Count > 0)
                {
                    guestRooms.Remove(r);
                }
            }

            return guestRooms.Count;
        }

        [HttpGet("[action]")]
        public async Task<IActionResult> GetHotelGuestRoomsConditionalAdmin([FromQuery] int pageSize, int currentPage, uint companyGroupID, string hotelName, int price, int roomStatus)
        {
            if (price < 0)
            {
                price = int.MaxValue;
            }


            var position = pageSize * (currentPage - 1);
            var guestRooms = await myDbContext.GuestRooms
                .AsNoTracking()
                .Include(r => r.HotelInstance)
                .ThenInclude(h => h.CompanyGroup)
                .Where(r => r.HotelInstance.CompanyGroup.ID == companyGroupID && r.HotelInstance.HotelName == hotelName && r.Price <= price)
                .ToListAsync();
            if (roomStatus != -1)
            {
                guestRooms = guestRooms.Where(r => r.RoomStatus == (RoomStatus)roomStatus).ToList();
            }
            guestRooms = guestRooms.Skip(position).Take(pageSize).ToList();
            var resp = guestRooms.Select(item =>
            {
                return new GuestRoomDto
                {
                    ID = item.ID,
                    Floor = item.Floor,
                    Address = item.Address,
                    RoomStatus = item.RoomStatus,
                    RoomType = item.RoomType,
                    HotelInstanceID = item.HotelInstance.ID,
                    Price = item.Price,
                    Title = item.Title,
                    Ichnography = item.Ichnography,
                    Area = item.Area,
                    BedCount = item.BedCount,
                    WindowCount = item.WindowCount,
                    MineralWaterCount = item.MineralWaterCount,
                    CondomCount = item.CondomCount,
                    GuestRoomType= item.GuestRoomType
                };
            });
            return Ok(resp);
        }

        [HttpGet("[action]")]
        public int GetHotelGuestRoomsConditionalAdminCount([FromQuery] uint companyGroupID, string hotelName, int price, int roomStatus)
        {
            if (price < 0)
            {
                price = int.MaxValue;
            }

            var guestRooms = myDbContext.GuestRooms
                .AsNoTracking()
                .Include(r => r.HotelInstance)
                .ThenInclude(h => h.CompanyGroup)
                .Where(r => r.HotelInstance.CompanyGroup.ID == companyGroupID && r.HotelInstance.HotelName == hotelName && r.Price <= price)
                .ToList();
            if (roomStatus != -1)
            {
                guestRooms = guestRooms.Where(r => r.RoomStatus == (RoomStatus)roomStatus).ToList();
            }
            return guestRooms.Count;
        }

        [HttpPut("[action]")]
        public async Task<IActionResult> ModifyGuestRoom([FromBody] GuestRoomDto GuestRoomDto)
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
                var room = await myDbContext.GuestRooms
                .SingleOrDefaultAsync(r => r.ID == GuestRoomDto.ID);
            if (room == null)
            {
                throw new BackendException
                {
                    Code = 6001,
                    ErrorMessage = "房间不存在"
                };
            }
            room.Area = GuestRoomDto.Area;
            room.WindowCount = GuestRoomDto.WindowCount;
            room.BedCount = GuestRoomDto.BedCount;
            room.RoomStatus = GuestRoomDto.RoomStatus;
            room.Price = GuestRoomDto.Price;
            room.Title = GuestRoomDto.Title;
            room.MineralWaterCount = GuestRoomDto.MineralWaterCount;
            room.GuestRoomType = GuestRoomDto.GuestRoomType;
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

        [HttpDelete("[action]")]
        public async Task<IActionResult> DeleteGuestRoom([FromQuery] uint ID)
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
                var room = await myDbContext.GuestRooms
                .AsNoTracking()
                .SingleOrDefaultAsync(r => r.ID == ID);
            if (room == null)
            {
                throw new BackendException
                {
                    Code = 6001,
                    ErrorMessage = "房间不存在"
                };
            }
            myDbContext.GuestRooms.Remove(room);
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

        [HttpPost("[action]")]
        public async Task<IActionResult> AddGuestRoom([FromBody] AddGuestRoomRequestDto addGuestRoomRequest)
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
                var hotel = await myDbContext.HotelInstances
                .SingleAsync(h => h.ID == addGuestRoomRequest.HotelInstanceID);
            var newGuestRoom = new GuestRoom
            {
                Floor = addGuestRoomRequest.Floor,
                Address = addGuestRoomRequest.Address,
                RoomStatus = addGuestRoomRequest.RoomStatus,
                RoomType = addGuestRoomRequest.RoomType,
                HotelInstance = hotel,
                Price = addGuestRoomRequest.Price,
                Title = addGuestRoomRequest.Title,
                Ichnography = addGuestRoomRequest.Ichnography,
                Area = addGuestRoomRequest.Area,
                BedCount = addGuestRoomRequest.BedCount,
                WindowCount = addGuestRoomRequest.WindowCount,
                MineralWaterCount = addGuestRoomRequest.MineralWaterCount,
                CondomCount = addGuestRoomRequest.CondomCount,
                GuestRoomType= addGuestRoomRequest.GuestRoomType
            };
            
            var entry = await myDbContext.GuestRooms.AddAsync(newGuestRoom);
            newGuestRoom = entry.Entity;
            await myDbContext.SaveChangesAsync();

            var resp = new GuestRoomDto
            {
                ID = newGuestRoom.ID,
                Floor = newGuestRoom.Floor,
                Address = newGuestRoom.Address,
                RoomStatus = newGuestRoom.RoomStatus,
                RoomType = newGuestRoom.RoomType,
                HotelInstanceID = newGuestRoom.HotelInstance.ID,
                Price = newGuestRoom.Price,
                Title = newGuestRoom.Title,
                Ichnography = newGuestRoom.Ichnography,
                Area = newGuestRoom.Area,
                BedCount = newGuestRoom.BedCount,
                WindowCount = newGuestRoom.WindowCount,
                MineralWaterCount = newGuestRoom.MineralWaterCount,
                CondomCount = newGuestRoom.CondomCount,
                GuestRoomType = newGuestRoom.GuestRoomType
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
    }
}
