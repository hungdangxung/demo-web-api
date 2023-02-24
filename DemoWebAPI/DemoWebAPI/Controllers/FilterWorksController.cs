using DemoWebAPI.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace DemoWebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class FilterWorksController : ControllerBase
    {
        private readonly IFilterWorkRepository _filterWorkRepository;
        public FilterWorksController(IFilterWorkRepository filterWorkRepository)
        {
            _filterWorkRepository = filterWorkRepository;
        }
        [HttpGet]
        public IActionResult FilterWork(bool? status, DateTime? date)
        {
            try
            {
                var result = _filterWorkRepository.FilterAll(status, date);
                return Ok(result);
            }
            catch (Exception)
            {
                return BadRequest("We can't get the work");
            }
        }
    }
}
