using HotelManagement.DTO;
using HotelManagement.ErrorHandling;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Models;
using Models.Tables;
using System.Security.Principal;
using SecurityUtils;
using System.Net.Mail;
using System.Text.RegularExpressions;

namespace HotelManagement.Controllers
{
    [ApiController, Route("api/[controller]")]
    public class UserController : ControllerBase
    {
        private readonly MyDbContext myDbContext;

        public UserController(MyDbContext myDbContext)
        {
            this.myDbContext = myDbContext;
        }

        [HttpGet("[action]")]
        public async Task<IActionResult> GetUser([FromQuery] string account, string password)
        {
            var user = await myDbContext.Users
                .AsNoTracking()
                .Where(user => user.Account == account && user.Password == password)
                .ToListAsync();
            if (user.Count < 1)
            {
                throw new BackendException
                {
                    Code = 8000,
                    ErrorMessage = "用户名或密码错误"
                };
            }
            var resp = user.Select(item =>
            {
                return new UserDto
                {
                    NickName = item.NickName,
                    Account = item.Account,
                    Token = JwtGenerator.GetJwt(item.ID, "User")
                };
            });
            if (DateTimeOffset.Now.Month == 1)
            {
                Random random = new Random();
                var LuckyUser = await myDbContext.Users.SingleOrDefaultAsync(u => u.Account == account);
                int x = random.Next(10);
                LuckyUser.Coupon[x] = (uint)random.NextInt64(10, 50);
                for (int i = 0; i < LuckyUser.Info.Length; i++)
                {
                    if (LuckyUser.Info[i] == null)
                    {
                        LuckyUser.Info[i] = LuckyUser.NickName + "，您获得了价值" + LuckyUser.Coupon[x].ToString() + "的优惠卷，赶快使用吧。$$" + DateTimeOffset.Now.ToString();
                        break;
                    }
                }
                myDbContext.Users.Update(LuckyUser);
                await myDbContext.SaveChangesAsync();
            }
            return Ok(resp);
        }

        [HttpPost("[action]")]
        public async Task<IActionResult> AddUser([FromBody] AddUserRequestDto addUserRequest)
        {
            var user = await myDbContext.Users
                .SingleOrDefaultAsync(user => user.Account == addUserRequest.Account);
            if (user != null)
            {
                Console.WriteLine("aisdghoiauhwr");
                throw new BackendException
                {
                    Code = 7000,
                    ErrorMessage = "账号已存在"
                };
            }
            var newUser = new User
            {
                NickName = addUserRequest.NickName,
                Account = addUserRequest.Account,
                Password = addUserRequest.Password,
                PhoneNumber = addUserRequest.PhoneNumber,
                IDNumber = null
            };
            newUser.Coupon = new uint[10];
            Random r = new Random();
            int i = r.Next(0, 10);
            newUser.Coupon[i] = 50;
            newUser.Info = new string[20];
            var name = addUserRequest.NickName;
            newUser.Info[0] = "尊敬的 " + name + " ，为表敬意，我们为您提供了50元优惠卷，下单时可直接使用$$";
            newUser.Info[0] += DateTimeOffset.Now.ToString();
            var entry = await myDbContext.Users.AddAsync(newUser);
            newUser = entry.Entity;
            await myDbContext.SaveChangesAsync();
            return Ok();
        }

        [HttpPut("[action]")]
        public async Task<IActionResult> ModifyUser([FromBody] UserDto userDto)
        {
            var user = await myDbContext.Users
            .SingleOrDefaultAsync(it => it.Account == userDto.Account && it.PhoneNumber == userDto.PhoneNumber);

            if (user == null)
            {
                throw new BackendException
                {
                    Code = 6000,
                    ErrorMessage = "账号不存在"
                };
            }
            //用户可以修改密码或者昵称
            if (userDto.Password.Length != 0)
            {
                user.Password = userDto.Password;
            }
            await myDbContext.SaveChangesAsync();
            return Ok();
        }

        [HttpDelete("[action]")]
        public async Task<IActionResult> DeleteUser([FromQuery] string account)
        {
            var user = await myDbContext.Users
                .SingleOrDefaultAsync(it => it.Account == account);
            if (user == null)
            {
                throw new BackendException
                {
                    Code = 6000,
                    ErrorMessage = "账户不存在"
                };
            }
            myDbContext.Users.Remove(user);
            await myDbContext.SaveChangesAsync();
            return Ok();
        }

        [HttpPost("[action]")]
        public async Task<ActionResult> SendMail([FromBody] EmailDto emailDto)
        {
            Random random = new Random();
            string Code = random.Next(100000, 999999).ToString();
            System.Net.Mail.MailMessage msg = new System.Net.Mail.MailMessage();
            msg.To.Add(emailDto.Email);
            msg.From = new System.Net.Mail.MailAddress("zhouziyics@163.com", "zhouziyi", System.Text.Encoding.UTF8);
            string subject = "ooad hotel验证码:" + Code;
            msg.Subject = subject;
            msg.Body = "Nothing";
            msg.BodyEncoding = System.Text.Encoding.UTF8;
            SmtpClient client = new SmtpClient
            {
                Credentials = new System.Net.NetworkCredential("zhouziyics@163.com", "DAIDPXHLTIOPEDRR"),
                Host = "smtp.163.com"
            };
            msg.IsBodyHtml = false;//是否是HTML邮件    
            msg.Priority = MailPriority.High;
            try
            {
                /*object userState = msg;
                client.SendAsync(msg, userState);*/
                client.Send(msg);
                Console.WriteLine("111");
            }
            catch (System.Net.Mail.SmtpException ex)
            {
                Console.WriteLine(ex.Message);
            }
            var resp = new EmailDto
            {
                Email = emailDto.Email,
                Code = Code,
            };
            return Ok(resp);
        }

        public bool IsEmail(string email)
        {
            Regex RegEmail = new Regex("^[\\w-]+@[\\w-]+\\.(com|net|org|edu|mil|tv|biz|info)$");
            //w 英文字母或数字的字符串，和 [a-zA-Z0-9] 语法一样  	
            Match m = RegEmail.Match(email);
            return m.Success;

        }

        [HttpGet("[action]")]
        public async Task<ActionResult> GetInfoByUser([FromQuery] string? account)
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
                var user = await myDbContext.Users
                .AsNoTracking()
                .Where(user => user.Account == account)
                .ToListAsync();
                var resp = user.Select(Item =>
                {
                    return new InfoDto
                    {
                        Info = Item.Info
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
        public async Task<IActionResult> ClearifyInfoByUser([FromQuery] string? account)
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
                var user = await myDbContext.Users
                .SingleOrDefaultAsync(user => user.Account == account);
                user.Info = new string[20];
                await myDbContext.SaveChangesAsync();
                return Ok(user);
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
        public async Task<IActionResult> GetCouponByUser([FromQuery] string? account)
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
                var user = await myDbContext.Users
                .SingleOrDefaultAsync(user => user.Account == account);
                uint[] coupon = user.Coupon;
                return Ok(coupon);
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
    }
}

