using AdsService.Services.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace AdsService.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AdsController : ControllerBase
    {
        private readonly IAdsRepository _adsRepository;

        public AdsController(IAdsRepository adsRepository)
        {
            _adsRepository = adsRepository;
        }

        [HttpPost("upload")]
        public IActionResult UploadAds(IFormFile file)
        {
            if (file == null || file.Length == 0)
            {
                return BadRequest("File is required");
            }

            try
            {
                using var stream = file.OpenReadStream();
                _adsRepository.LoadData(stream);
                return Ok("Data loaded successfully");
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error loading data: {ex.Message}");
            }
        }

        [HttpGet("search")]
        public IActionResult SearchAds([FromQuery] string location)
        {
            if (string.IsNullOrEmpty(location))
            {
                return BadRequest("Location parameter is required");
            }

            try
            {
                var ads = _adsRepository.FindAds(location);
                return Ok(ads);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error searching ads: {ex.Message}");
            }
        }
    }
}
