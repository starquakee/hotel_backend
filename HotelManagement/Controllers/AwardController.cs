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
    public class AwardController : ControllerBase
    {
        private readonly MyDbContext myDbContext;

        public AwardController(MyDbContext myDbContext)
        {
            this.myDbContext = myDbContext;
        }

        [HttpGet("[action]")]
        public async Task<IActionResult> GetPoint([FromQuery] string? account)
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
                var user = await myDbContext.Users.AsNoTracking().SingleOrDefaultAsync(u => u.Account == account);
                if (user == null)
                {
                    throw new BackendException
                    {
                        Code = 10004,
                        ErrorMessage = "用户不存在"
                    };
                }
                return Ok(user.Points);
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
        public async Task<IActionResult> AddAward([FromBody] AwardDto awardDto)
        {
            try
            {
                var (id, payload) = HttpContext.GetUserIdFromHeader();
                var user = await myDbContext.Users
                .SingleOrDefaultAsync(it => it.ID == id);
                if (user == null || payload != "User")
                {
                    throw new BackendException
                    {
                        Code = 100001,
                        ErrorMessage = "请先登录"
                    };
                }
                if (user.Account != awardDto.Account)
                {
                    throw new BackendException
                    {
                        Code = 10004,
                        ErrorMessage = "用户不存在"
                    };
                }
                if (user.Points < awardDto.GoodsPoints)
                {
                    throw new BackendException
                    {
                        Code = 20000,
                        ErrorMessage = "积分不够"
                    };
                }
                user.Points -= awardDto.GoodsPoints;
                myDbContext.Users.Update(user);
                var newAward = new Award
                {
                    User = user,
                    Account = awardDto.Account,
                    Consignee = awardDto.Consignee,
                    ContactNumber = awardDto.ContactNumber,
                    DeliveryAddress = awardDto.DeliveryAddress,
                    Goods = awardDto.Goods,
                    GoodsPoints = awardDto.GoodsPoints
                };
                await myDbContext.Awards.AddAsync(newAward);
                await myDbContext.SaveChangesAsync();
                return Ok(user.Points);
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
                    ErrorMessage = ex.Message,
                };
            }
        }

        [HttpGet("[action]")]
        public async Task<IActionResult> GetAwardsByUser([FromQuery] string? account, int pageSize, int currentPage)
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
                var user = await myDbContext.Users.AsNoTracking().SingleOrDefaultAsync(u => u.Account == account);
                if (user == null)
                {
                    throw new BackendException
                    {
                        Code = 10004,
                        ErrorMessage = "用户不存在"
                    };
                }
                var awards = await myDbContext.Awards
                    .AsNoTracking()
                    .Where(a => a.Account == account)
                    .Skip(position)
                    .Take(pageSize)
                    .ToListAsync();
                var resp = awards.Select(item =>
                {
                    return new AwardDto
                    {
                        Account = account,
                        Goods = item.Goods,
                        GoodsPoints = item.GoodsPoints,
                        Consignee = item.Consignee,
                        DeliveryAddress = item.DeliveryAddress,
                        ContactNumber = item.ContactNumber
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
        public async Task<IActionResult> GetAwardsCountByUser([FromQuery] string? account)
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
                var user = await myDbContext.Users.AsNoTracking().SingleOrDefaultAsync(u => u.Account == account);
                if (user == null)
                {
                    throw new BackendException
                    {
                        Code = 10004,
                        ErrorMessage = "用户不存在"
                    };
                }
                var awards = await myDbContext.Awards
                    .AsNoTracking()
                    .Where(a => a.Account == account)
                    .ToListAsync();
                return Ok(awards.Count);
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
        public async Task<IActionResult> GetAllAwards([FromQuery] int pageSize, int currentPage)
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
                var awards = await myDbContext.Awards
                    .AsNoTracking()
                    .Skip(position)
                    .Take(pageSize)
                    .ToListAsync();
                var resp = awards.Select(item =>
                {
                    return new AwardDto
                    {
                        Account = item.Account,
                        Goods = item.Goods,
                        GoodsPoints = item.GoodsPoints,
                        Consignee = item.Consignee,
                        DeliveryAddress = item.DeliveryAddress,
                        ContactNumber = item.ContactNumber
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
        public async Task<IActionResult> GetAllAwardsCount()
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
                        ErrorMessage = "请先登录"
                    };
                }
                var awards = await myDbContext.Awards
                .AsNoTracking()
                .ToListAsync();
                return Ok(awards.Count);
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

    }
}
