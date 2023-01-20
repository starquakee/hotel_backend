using HotelManagement.DTO;
using Microsoft.AspNetCore.Mvc;
using Models;
using Microsoft.EntityFrameworkCore;
using System.Text.RegularExpressions;
using Models.Tables;
using HotelManagement.ErrorHandling;
using System.Globalization;
using SecurityUtils;

namespace HotelManagement.Controllers
{
    [ApiController, Route("api/[controller]")]
    public class EmployeeInstanceController : ControllerBase
    {
        private readonly MyDbContext myDbcontext;
        public EmployeeInstanceController(MyDbContext myDbcontext)
        {
            this.myDbcontext = myDbcontext;
        }
        [HttpGet("[action]")]

        public async Task<IActionResult> GetEmployeesCount()
        {
            try
            {
                var (id, payload) = HttpContext.GetUserIdFromHeader();
                var admin = await myDbcontext.Administrations.SingleOrDefaultAsync(item => item.ID == id);
                if (admin == null || payload != "Admin")
                {
                    throw new BackendException
                    {
                        Code = 99999,
                        ErrorMessage = "请先登录"
                    };
                }
                return Ok(myDbcontext.EmployeeInstances.Count());
            }
            catch (Exception ex)
            {
                if (ex is BackendException)
                {
                    throw;
                }
                else
                {
                    throw new BackendException
                    {
                        Code = 99999,
                        ErrorMessage = "请先登录",
                    };
                }
            }
        }

        [HttpGet("[action]")]
        public async Task<IActionResult> GetAllEmployees([FromQuery] int pageSize, int currentPage)
        {
            try
            {
                var (id, payload) = HttpContext.GetUserIdFromHeader();
                var admin = await myDbcontext.Administrations.SingleOrDefaultAsync(item => item.ID == id);
                if (admin == null || payload != "Admin")
                {
                    throw new BackendException
                    {
                        Code = 100000,
                        ErrorMessage = "请先登录"
                    };
                }
                var position = pageSize * (currentPage - 1);
            var employees = await myDbcontext.EmployeeInstances
                .OrderBy(e => e.EmployeeName)
                .Skip(position)
                .Take(pageSize)
                .ToListAsync();


            var resp = employees.Select(item =>
            {
                return new EmployeeInstanceDto
                {
                    ID = item.ID,
                    EmployeeName = item.EmployeeName,
                    Character = item.Character,
                    PhoneNumber = item.PhoneNumber,
                    IdentityCardId = item.IdentityCardId,
                    IdentityCardType = item.IdentityCardType
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
        public async Task<IActionResult> GetEmployee([FromQuery] string employeeName,IdentityCardType identityCardType,string identityCardId)
        {
            try
            {
                var (id, payload) = HttpContext.GetUserIdFromHeader();
                var admin = await myDbcontext.Administrations.SingleOrDefaultAsync(item => item.ID == id);
                if (admin == null || payload != "Admin")
                {
                    throw new BackendException
                    {
                        Code = 100000,
                        ErrorMessage = "请先登录"
                    };
                }
                var employee = await myDbcontext.EmployeeInstances
                .AsNoTracking()
                .Where(employee => employee.EmployeeName == employeeName && employee.IdentityCardType == identityCardType && employee.IdentityCardId == identityCardId)
                .ToListAsync();

            var resp = employee.Select(item =>
            {
                return new EmployeeInstanceDto
                {
                    ID = item.ID,
                    EmployeeName = item.EmployeeName,
                    Character = item.Character,
                    PhoneNumber = item.PhoneNumber,
                    IdentityCardId = item.IdentityCardId,
                    IdentityCardType = item.IdentityCardType
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

        [HttpPost("[action]")]
        public async Task<IActionResult> AddEmployeeInstance([FromBody] AddEmployeeRequestDto addEmployeeRequest)
        {
            try
            {
                var (id, payload) = HttpContext.GetUserIdFromHeader();
                var admin = await myDbcontext.Administrations.SingleOrDefaultAsync(item => item.ID == id);
                if (admin == null || payload != "Admin")
                {
                    throw new BackendException
                    {
                        Code = 100000,
                        ErrorMessage = "请先登录"
                    };
                }
                var employee = await myDbcontext.EmployeeInstances
                .SingleOrDefaultAsync(employee => employee.IdentityCardId == addEmployeeRequest.IdentityCardId);
            if (employee != null)
            {
                throw new BackendException
                {
                    Code = 777,
                    ErrorMessage = "已存在该员工"
                };
            }
            //如果证件类型是身份证
            if (addEmployeeRequest.IdentityCardType == IdentityCardType.IdentityCard)
            {
                if (!IdentityIDCheck(addEmployeeRequest.IdentityCardId))
                {
                    throw new BackendException
                    {
                        Code = 888,
                        ErrorMessage = "身份证错误"
                    };
                }
            }  //如果证件类型是护照

            else if(addEmployeeRequest.IdentityCardType == IdentityCardType.Passport)
            {
                if (!PassportIDCheck(addEmployeeRequest.IdentityCardId))
                {
                    throw new BackendException
                    {
                        Code = 999,
                        ErrorMessage = "护照错误"
                    };
                }
            }

            var newEmployee = new EmployeeInstance
            { 
                EmployeeName = addEmployeeRequest.EmployeeName,
                IdentityCardId = addEmployeeRequest.IdentityCardId,
                IdentityCardType = addEmployeeRequest.IdentityCardType,
                Character = addEmployeeRequest.Character,
                PhoneNumber = addEmployeeRequest.PhoneNumber
            };
            var entry = await myDbcontext.EmployeeInstances.AddAsync(newEmployee);
            newEmployee = entry.Entity;
            await myDbcontext.SaveChangesAsync();

            var resp = new EmployeeInstanceDto
            {
                ID = newEmployee.ID,
                EmployeeName = newEmployee.EmployeeName,
                IdentityCardId = newEmployee.IdentityCardId,
                IdentityCardType = newEmployee.IdentityCardType,
                Character = newEmployee.Character,
                PhoneNumber = newEmployee.PhoneNumber
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

        [HttpPut("[action]")]
        public async Task<IActionResult> ModifyEmployee([FromBody] EmployeeInstanceDto employeeInstance)
        {
            try
            {
                var (id, payload) = HttpContext.GetUserIdFromHeader();
                var admin = await myDbcontext.Administrations.SingleOrDefaultAsync(item => item.ID == id);
                if (admin == null || payload != "Admin")
                {
                    throw new BackendException
                    {
                        Code = 100000,
                        ErrorMessage = "请先登录"
                    };
                }
                var employee = await myDbcontext.EmployeeInstances
                .SingleOrDefaultAsync(employee => employee.IdentityCardId == employeeInstance.IdentityCardId);
            if(employee == null)
            {
                throw new BackendException
                {
                    Code = 1001,
                    ErrorMessage = "不存在该员工"
                };
            }

            if (employeeInstance.IdentityCardType == IdentityCardType.IdentityCard)
            {
                if (!IdentityIDCheck(employeeInstance.IdentityCardId))
                {
                    throw new BackendException
                    {
                        Code = 888,
                        ErrorMessage = "身份证错误"
                    };
                }
            }

            else if (employeeInstance.IdentityCardType == IdentityCardType.Passport)
            {
                if (!PassportIDCheck(employeeInstance.IdentityCardId))
                {
                    throw new BackendException
                    {
                        Code = 999,
                        ErrorMessage = "护照错误"
                    };
                }
            }
            employee.EmployeeName = employeeInstance.EmployeeName;
            employee.Character = employeeInstance.Character;
            employee.IdentityCardId = employeeInstance.IdentityCardId;
            employee.IdentityCardType = employeeInstance.IdentityCardType;
            employee.PhoneNumber = employeeInstance.PhoneNumber;

            await myDbcontext.SaveChangesAsync();

            var resp = new EmployeeInstanceDto
            {
                ID = employee.ID,
                EmployeeName = employee.EmployeeName,
                Character = employee.Character,
                IdentityCardId = employee.IdentityCardId,
                IdentityCardType = employee.IdentityCardType,
                PhoneNumber = employee.PhoneNumber
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

        [HttpDelete("[action]")]
        public async Task<IActionResult> DeleteEmployee([FromQuery] uint id)
        {
            try
            {
                var (ID, payload) = HttpContext.GetUserIdFromHeader();
                var admin = await myDbcontext.Administrations.SingleOrDefaultAsync(item => item.ID == ID);
                if (admin == null || payload != "Admin")
                {
                    throw new BackendException
                    {
                        Code = 100000,
                        ErrorMessage = "请先登录"
                    };
                }
                var employee = await myDbcontext.EmployeeInstances
                .SingleOrDefaultAsync(employee => employee.ID == id);
            if(employee == null)
            {
                return NotFound();
            }
            myDbcontext.EmployeeInstances.Remove(employee);
            await myDbcontext.SaveChangesAsync();
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

        public bool IdentityIDCheck(string id)
        {
                if(id.Length!=18)
                {
                    return false;
                }
                string pattern = @"^\d{17}(?:\d|X)$";
                string birth = id.Substring(6, 8).Insert(6, "-").Insert(4, "-");

                int[] arr_weight = { 7, 9, 10, 5, 8, 4, 2, 1, 6, 3, 7, 9, 10, 5, 8, 4, 2 };
                string[] id_last = { "1", "0", "X", "9", "8", "7", "6", "5", "4", "3", "2" };
                int sum = 0;
                for (int i = 0; i < 17; i++)
                {
                    sum += arr_weight[i] * int.Parse(id[i].ToString());
                }
                int result = sum % 11;  // 实际校验位的值
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
