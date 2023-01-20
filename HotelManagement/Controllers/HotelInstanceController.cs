using HotelManagement.DTO;
using HotelManagement.ErrorHandling;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Models;
using Models.Tables;
using SecurityUtils;
using System.Collections;
using System.Text.RegularExpressions;

using static Microsoft.EntityFrameworkCore.DbLoggerCategory.Database;

namespace HotelManagement.Controllers
{
    /// <summary>
    /// 实际上，匹配的路径就是
    /// api/test
    /// </summary>
    [ApiController, Route("api/[controller]")]
    public class HotelInstanceController : ControllerBase 
    {
        private readonly MyDbContext myDbContext;

        public HotelInstanceController(MyDbContext myDbContext)
        {
            this.myDbContext = myDbContext;
        }

        /// <summary>
        /// 返回所有的酒店并分页
        /// </summary>
        /// <author>Zhou Ziyi</author>
        /// <param name="pageSize"></param>
        /// <param name="currentPage"></param>
        /// <returns></returns>
        [HttpGet("[action]")]
        public async Task<IActionResult> GetAllHotels([FromQuery] int pageSize, int currentPage)
        {
            var position = pageSize * (currentPage - 1);
            var hotels = await myDbContext.HotelInstances
                .AsNoTracking()
                .Include( h => h.CompanyGroup)
                .OrderBy(h => h.HotelName)
                .Skip(position)
                .Take(pageSize)
                .ToListAsync();
            var resp = hotels.Select(item =>
            {
                return new HotelInstanceDto
                {
                    HotelId = item.ID,
                    HotelName = item.HotelName,
                    HotelAddress = item.HotelAddress,
                    City = item.City,
                    ContactList = item.ContactList,
                    CompanyName = item.CompanyGroup.GroupName
                };
            });

            return Ok(resp);
        }

        [HttpGet("[action]")]
        public async Task<IActionResult> GetHotelsByCondition([FromQuery]int pageSize, int currentPage, string city="", string keys="")
        {
            var position = pageSize * (currentPage - 1);
            //HotelInstance? hotels = null;
            if (city.Equals("%"))
            {
                var hotels = await myDbContext.HotelInstances
                .AsNoTracking()
                .Include(h => h.CompanyGroup)
                .Where(h => (h.HotelAddress.Contains(keys) || h.HotelName.Contains(keys)))
                .OrderBy(h => h.HotelName)
                .Skip(position)
                .Take(pageSize)
                .ToListAsync();
                var resp = hotels.Select(item =>
                {
                    return new HotelInstanceDto
                    {
                        HotelId = item.ID,
                        HotelName = item.HotelName,
                        HotelAddress = item.HotelAddress,
                        City = item.City,
                        ContactList = item.ContactList,
                        CompanyName = item.CompanyGroup.GroupName
                    };
                });

                return Ok(resp);
            }
            else if(keys.Equals("%"))
            {
               var hotels = await myDbContext.HotelInstances
                .AsNoTracking()
                .Include(h => h.CompanyGroup)
                .Where(h => h.City == city)
                .OrderBy(h => h.HotelName)
                .Skip(position)
                .Take(pageSize)
                .ToListAsync();
                var resp = hotels.Select(item =>
                {
                    return new HotelInstanceDto
                    {
                        HotelId = item.ID,
                        HotelName = item.HotelName,
                        HotelAddress = item.HotelAddress,
                        City = item.City,
                        ContactList = item.ContactList,
                        CompanyName = item.CompanyGroup.GroupName
                    };
                });

                return Ok(resp);
            }
            else
            {
               var hotels = await myDbContext.HotelInstances
                    .AsNoTracking()
                    .Include(h => h.CompanyGroup)
                    .Where(h => h.City == city && (h.HotelAddress.Contains(keys) || h.HotelName.Contains(keys)))
                    .OrderBy(h => h.HotelName)
                    .Skip(position)
                    .Take(pageSize)
                    .ToListAsync();
               
                var resp = hotels.Select(item =>
                {
                    return new HotelInstanceDto
                    {
                        HotelId = item.ID,
                        HotelName = item.HotelName,
                        HotelAddress = item.HotelAddress,
                        City = item.City,
                        ContactList = item.ContactList,
                        CompanyName = item.CompanyGroup.GroupName
                    };
                });

                return Ok(resp);
            }
            
        }

        [HttpGet("[action]")]
        public int GetAllCount()
        {
            var number = myDbContext.HotelInstances
                .AsNoTracking()
                .Include(h => h.CompanyGroup)
                .Count();
            return number;
        }

        [HttpGet("[action]")]
        public int GetCountByCondition([FromQuery] string city, string keys)
        {
            if (city.Equals("%"))
            {
                var number = myDbContext.HotelInstances
                .AsNoTracking()
                .Include(h => h.CompanyGroup)
                .Where(h => h.HotelAddress.Contains(keys) || h.HotelName.Contains(keys))
                .Count();
                return number;
            }
            else if(keys.Equals("%"))
            {
                var number = myDbContext.HotelInstances
                    .AsNoTracking()
                    .Include(h => h.CompanyGroup)
                    .Where(h => h.City == city)
                    .Count();
                return number;
            }
            else
            {
                var number = myDbContext.HotelInstances
                .AsNoTracking()
                .Include(h => h.CompanyGroup)
                .Where(h => h.City == city && (h.HotelAddress.Contains(keys) || h.HotelName.Contains(keys)))
                .Count();
                return number;
            }
        }

        /// <summary>
        /// 实际你匹配的路径是
        /// GET api/test/getHotels
        /// </summary>
        /// <param name="corpId"></param>
        /// <returns></returns>
        [HttpGet("[action]")]
        public async Task<IActionResult> GetHotels([FromQuery] uint corpId)
        {
            var hotels = await myDbContext.HotelInstances
                .AsNoTracking()
                .Include(hotel => hotel.CompanyGroup)
                .Where(hotel => hotel.CompanyGroup.ID == corpId)
                .ToListAsync();

            var resp = hotels.Select(item =>
            {
                return new HotelInstanceDto
                {
                    HotelId = item.ID,
                    HotelName = item.HotelName,
                    HotelAddress = item.HotelAddress,
                    City = item.City,
                    ContactList = item.ContactList,
                    CompanyName = item.CompanyGroup.GroupName
                };
            });

            return Ok(resp);
        }

        /// <summary>
        /// 2022.12.03 TODO: 更改
        /// </summary>
        /// <param name="addHotelRequest"></param>
        /// <returns></returns>
        [HttpPost("[action]")]
        public async Task<IActionResult> AddHotel([FromBody] AddHotelRequestDto addHotelRequest)
        {
            var company = await myDbContext.CompanyGroups
                .SingleOrDefaultAsync(company => company.ID == addHotelRequest.CompanyId);

            if (company == null)
            {
                return BadRequest();
            }

            var newHotel = new HotelInstance
            {
                CompanyGroup = company,
                HotelName = addHotelRequest.HotelInstance.HotelName,
                HotelAddress = addHotelRequest.HotelInstance.HotelAddress,
                ContactList = addHotelRequest.HotelInstance.ContactList,
                City = addHotelRequest.HotelInstance.City
       
            };

            var entry = await myDbContext.HotelInstances.AddAsync(newHotel);
            newHotel = entry.Entity;
            await myDbContext.SaveChangesAsync();

            var resp = new HotelInstanceDto
            {
                HotelId = newHotel.ID,
                HotelName = newHotel.HotelName,
                HotelAddress = newHotel.HotelAddress,
                City = newHotel.City,
                ContactList = newHotel.ContactList,
                CompanyName = company.GroupName
            };

            return Ok(resp);
        }

        [HttpPut("[action]")]
        public async Task<IActionResult> ModifyHotel([FromBody] HotelInstanceDto hotelInstance)
        {
            var hotel = await myDbContext.HotelInstances
                .Include(h => h.CompanyGroup)
                .SingleOrDefaultAsync(h => h.ID == hotelInstance.HotelId);

            if (hotel == null)
            {
                return NotFound();
            }

            hotel.HotelName = hotelInstance.HotelName;
            hotel.HotelAddress = hotelInstance.HotelAddress;
            hotel.ContactList = hotelInstance.ContactList;

            await myDbContext.SaveChangesAsync();

            var resp = new HotelInstanceDto
            {
                HotelId = hotel.ID,
                HotelName = hotel.HotelName,
                HotelAddress = hotel.HotelAddress,
                City = hotel.City,
                ContactList = hotel.ContactList,
                CompanyName = hotel.CompanyGroup.GroupName
            };

            return Ok(resp);
        }

        [HttpDelete("[action]")]
        public async Task<IActionResult> DeleteHotel([FromQuery] uint hotelId)
        {
            var hotel = await myDbContext.HotelInstances
                .SingleOrDefaultAsync(h => h.ID == hotelId);

            if (hotel == null)
            {
                return NotFound();
            }

            myDbContext.HotelInstances.Remove(hotel);
            await myDbContext.SaveChangesAsync();

            return Ok();
        }

        [HttpGet("[action]")]
        public async Task<IActionResult> SubscribeHotel([FromQuery] string? account, string hotelName)
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
                var user = await myDbContext.Users.SingleOrDefaultAsync(u => u.Account == account);
            if (user == null)
            {
                throw new BackendException
                {
                    Code = 10000,
                    ErrorMessage = "账号不存在，请先注册或登录"
                };
            }
            var hotel = await myDbContext.HotelInstances.SingleOrDefaultAsync(h => h.HotelName == hotelName);
            if (hotel == null)
            {
                throw new BackendException
                {
                    Code = 10001,
                    ErrorMessage = "酒店不存在"
                };
            }
            var subscription = user.Subscription.ToList();
            if (subscription.Contains(hotel.ID))
            {
                throw new BackendException
                {
                    Code = 10002,
                    ErrorMessage = "酒店已收藏"
                };
            }
            subscription.Add(hotel.ID);
            user.Subscription = subscription.ToArray();


            //添加信息通知
            for(int i = 0 ;i < user.Info.Length; i++)
            {
                if(user.Info[i]==null)
                {
                    Console.WriteLine("我看到了");
                    user.Info[i] = user.NickName + "，您关注了" + hotelName + "。$$" + DateTimeOffset.Now.ToString();
                    break;
                }
            }
            myDbContext.Users.Update(user);
            await myDbContext.SaveChangesAsync();
            return Ok(hotelName);
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
        public async Task<IActionResult> UnsubscribeHotel([FromQuery] string? account, string hotelName)
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
                var user = await myDbContext.Users.SingleOrDefaultAsync(u => u.Account == account);
            if (user == null)
            {
                throw new BackendException
                {
                    Code = 10000,
                    ErrorMessage = "账号不存在，请先注册或登录"
                };
            }
            var hotel = await myDbContext.HotelInstances.SingleOrDefaultAsync(h => h.HotelName == hotelName);
            if (hotel == null)
            {
                throw new BackendException
                {
                    Code = 10001,
                    ErrorMessage = "酒店不存在"
                };
            }
            if (!user.Subscription.ToArray().Contains(hotel.ID)) {

                throw new BackendException
                {
                    Code = 10003,
                    ErrorMessage = "酒店未收藏"
                };
            }
            user.Subscription = user.Subscription.Where(i => i != hotel.ID).ToArray();

            for (int i = 0; i < user.Info.Length; i++)
            {
                if (user.Info[i] == null)
                {
                    user.Info[i] = user.NickName + "，您取消关注了" + hotelName + "。$$" + DateTimeOffset.Now.ToString();
                    Console.WriteLine(user.Info[i]);   
                    break;
                }
            }
            myDbContext.Users.Update(user);
            await myDbContext.SaveChangesAsync();
            return Ok(hotelName);
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
        public async Task<IActionResult> GetHotelsSubscribed([FromQuery] int pageSize, int currentPage, string? account)
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
            var user = await myDbContext.Users.SingleOrDefaultAsync(u => u.Account == account);
            if (user == null)
            {
                throw new BackendException
                {
                    Code = 10000,
                    ErrorMessage = "账号不存在，请先注册或登录"
                };
            }
            var hotels = new List<HotelInstance>();
            foreach (uint hotelId in user.Subscription)
            {
                var hotel = await myDbContext.HotelInstances.Include(h => h.CompanyGroup).SingleOrDefaultAsync<HotelInstance>(h => h.ID == hotelId);
                if (hotel == null)
                {
                    continue;
                }
                hotels.Add(hotel);
            }
            hotels = hotels.Skip(position).Take(pageSize).ToList();
            var resp = hotels.Select(item =>
            {
                return new HotelInstanceDto
                {
                    HotelId = item.ID,
                    HotelName = item.HotelName,
                    HotelAddress = item.HotelAddress,
                    City = item.City,
                    ContactList = item.ContactList,
                    CompanyName = item.CompanyGroup.GroupName
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
        public async Task<IActionResult> GetHotelsCountSubscribed([FromQuery] string? account)
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
                var user = await myDbContext.Users.SingleOrDefaultAsync(u => u.Account == account);
            if (user == null)
            {
                throw new BackendException
                {
                    Code = 10000,
                    ErrorMessage = "账号不存在，请先注册或登录"
                };
            }
            var hotels = new List<HotelInstance>();
            foreach (uint hotelId in user.Subscription)
            {
                var hotel = await myDbContext.HotelInstances.SingleOrDefaultAsync<HotelInstance>(h => h.ID == hotelId);
                if (hotel == null)
                {
                    continue;
                }
                hotels.Add(hotel);
            }
            return Ok(hotels.Count);
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

        public bool IdentityIDCheck(string id)
        {
            var pattern = @"^\d{17}(?:\d|X)$";
            var birth = id.Substring(6, 8).Insert(6, "-").Insert(4, "-");
          
            int[] arr_weight =  { 7, 9, 10, 5, 8, 4, 2, 1, 6, 3, 7, 9, 10, 5, 8, 4, 2 };   
            string[] id_last = { "1", "0", "X", "9", "8", "7", "6", "5", "4", "3", "2" };  
            var sum = 0;
            for (int i = 0; i < 17; i++)
            {
                sum += arr_weight[i] * int.Parse(id[i].ToString());
            }
            var result =  sum % 11;  // 实际校验位的值

            if (Regex.IsMatch(id, pattern))                
            {
                if (DateTime.TryParse(birth, out _))          
                {
                    if (id_last[result] == id[17].ToString())  
                    {
                        return true;
                    }
                    else
                    {
                        return false;
                    }
                }
                else
                {
                    return false;
                }
            }
            else
            {
                return false;
            }
        }

        public bool PassportIDCheck(string id)
        {
            string pattern = @"^1[45][0-9]{7}|([P|p|S|s]\d{7})|([S|s|G|g]\d{8})|([Gg|Tt|Ss|Ll|Qq|Dd|Aa|Ff]\d{8})|([H|h|M|m]\d{8，10})$";
            if (Regex.IsMatch(id, pattern))
            {
                return true;
            }
            else
            {
                return false;
            }
        }

    }
}
