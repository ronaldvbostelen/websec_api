using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Client_tech_resversi_api.Assets.Extensions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace Client_tech_resversi_api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ForbiddenController : Controller
    {
        private readonly ILogger<ForbiddenController> _logger;
        public ForbiddenController(ILogger<ForbiddenController> logger)
        {
            _logger = logger;
        }

        [HttpGet]
        public ActionResult Get()
        {
            _logger.LogMsg( HttpContext,"FORBIDDEN");

            return StatusCode(401);
        }
    }
}