using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using eCommerce.API.EC;
using eCommerce.Library.DTO;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using eCommerce.API.Database;
using Microsoft.Extensions.Logging;
using System.IO;
using System;

namespace eCommerce.API.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class InventoryController : ControllerBase
    {
        private readonly ILogger<InventoryController> _logger;

        public InventoryController(ILogger<InventoryController> logger)
        {
            _logger = logger;
        }

        [HttpGet()]
        public async Task<IEnumerable<ProductDTO>> Get()
        {
            return await new InventoryEC().Get();
        }

        [HttpPost("Search")]
        public async Task<IEnumerable<ProductDTO>> Search([FromBody] Query query)
        {
            if (query == null || string.IsNullOrWhiteSpace(query.QueryString))
            {
                return Enumerable.Empty<ProductDTO>();
            }
            return await new InventoryEC().Search(query.QueryString);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var result = await new InventoryEC().Delete(id);
            if (result == null)
            {
                return NotFound();
            }
            return Ok(result);
        }

        [HttpPost()]
        public async Task<IActionResult> AddOrUpdate([FromBody] ProductDTO p)
        {
            if (p == null)
            {
                return BadRequest("ProductDTO cannot be null.");
            }

            var result = await new InventoryEC().AddOrUpdate(p);
            return Ok(result);
        }

        [HttpPost("Import")]
        public async Task<IActionResult> ImportProducts(IFormFile file)
        {
            if (file == null || file.Length == 0)
            {
                return BadRequest(new { message = "The file field is required." });
            }

            try
            {
                using (var stream = file.OpenReadStream())
                {
                    var filebase = Filebase.Current;
                    filebase.ImportProductsFromCsv(stream);
                }
                return Ok(new { message = "Products imported successfully." });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error importing products.");
                return StatusCode(500, new { message = "An error occurred while importing products." });
            }
        }
    }
}

