using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using PacificProgrammingWebApi.Model;
using PacificProgrammingWebApi.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace PacificProgrammingWebApi.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class AvatarController : ControllerBase
    {
        private readonly ClientService _service;

        public AvatarController(ClientService imageService)
        {
            _service = imageService;
        }

        // Endpoint: /avatar?userIdentifier={userIdentifier}
        [HttpGet]
        public async Task<IActionResult> GetAvatarUrl(string userIdentifier)
        {
            if (string.IsNullOrEmpty(userIdentifier))
            {
                return BadRequest("User identifier is required.");
            }

            var url = await _service.GetImageUrlAsync(userIdentifier);

            if (url == null)
            {
                return NotFound("Image not found");
            }

            return Ok(new { url });
        }
    }
}
