using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MVC6.Controllers.Interfaces;
using MVC6.Models.DataModels;
using MVC6.Models.ViewModels;

namespace MVC6.Controllers.Api
{
    [Authorize(Roles = "SuperUser, SystemUser")]
    [Route("api/[controller]")]
    [ApiController]
    public class GrantApiController : ControllerBase
    {
        private readonly IGrant _grant;

        public GrantApiController(IGrant grant)
        {
            _grant = grant;
        }
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(GrantAjaxInfo))]
        public async Task<IActionResult> GetAsync()
        {
            IEnumerable<User> user = await _grant.GetUserListAsync();

            var grantAjaxInfo = new GrantAjaxInfo() {
                RecordsTotal = user.ToList().Count,
                Data = user.ToArray()
            };

            return Ok(grantAjaxInfo);
        }
    }
}
