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
    public class CompanyGroupController : ControllerBase
    {
        private readonly MyDbContext myDbContext;


        public CompanyGroupController(MyDbContext myDbContext)
        {
            this.myDbContext = myDbContext;
        }

        [HttpGet("[action]")]
        public async Task<IActionResult> GetCompanyGroup([FromQuery] string groupName)
        {
            var companyGroup = await myDbContext.CompanyGroups
                .AsNoTracking()
                .Where(companyGroup => companyGroup.GroupName == groupName)
                .ToListAsync();
            if (companyGroup.Count < 1)
            {
                throw new BackendException
                {
                    Code = 9000,
                    ErrorMessage = "GroupName错误"
                };
            }
            var resp = companyGroup.Select(item =>
            {
                return new CompanyGroupDto
                {
                    ID = item.ID,
                    GroupName = item.GroupName,
                    HotelInstances = item.HotelInstances

                };
            });

            return Ok(resp);
        }


        [HttpPost("[action]")]
        public async Task<IActionResult> AddCompanyGroup([FromBody] AddCompanyGroupRequestDto addCompanyGroupRequest)
        {

            var newCompanyGroup = new CompanyGroup
            {
                GroupName = addCompanyGroupRequest.GroupName,
                HotelInstances = addCompanyGroupRequest.HotelInstances
            };

            var entry = await myDbContext.CompanyGroups.AddAsync(newCompanyGroup);
            newCompanyGroup = entry.Entity;
            await myDbContext.SaveChangesAsync();

            var resp = new CompanyGroupDto
            {

                GroupName = newCompanyGroup.GroupName,
                HotelInstances = newCompanyGroup.HotelInstances,
                ID = newCompanyGroup.ID
            };

            return Ok(resp);
        }


        [HttpPut("[action]")]
        public async Task<IActionResult> ModifyCompanyGroup([FromBody] CompanyGroupDto companyGroupDto)
        {
            var companyGroup = await myDbContext.CompanyGroups
                .SingleOrDefaultAsync(it => it.GroupName == companyGroupDto.GroupName);
            if (companyGroup == null)
            {
                throw new BackendException
                {
                    Code = 6000,
                    ErrorMessage = "CompanyGroup不存在"
                };
            }
            

            if (companyGroupDto.HotelInstances != null)
            {
                companyGroup.HotelInstances = companyGroupDto.HotelInstances;
            }
            await myDbContext.SaveChangesAsync();
            var resp = new CompanyGroupDto
            {
                GroupName = companyGroupDto.GroupName,
                HotelInstances = companyGroupDto.HotelInstances,
                ID = companyGroupDto.ID
            };
            return Ok(resp);
        }

        [HttpDelete("[action]")]
        public async Task<IActionResult> DeleteCompanyGroup([FromQuery] string groupName)
        {
            var companyGroup = await myDbContext.CompanyGroups
                .SingleOrDefaultAsync(it => it.GroupName == groupName);

            if (companyGroup == null)
            {
                throw new BackendException
                {
                    Code = 6000,
                    ErrorMessage = "GroupName不存在"
                };
            }
            myDbContext.CompanyGroups.Remove(companyGroup);
            await myDbContext.SaveChangesAsync();

            return Ok();
        }

    }
}
