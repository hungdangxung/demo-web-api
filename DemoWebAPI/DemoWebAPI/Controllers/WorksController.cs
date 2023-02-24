using DemoWebAPI.Data;
using DemoWebAPI.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace DemoWebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class WorksController : ControllerBase
    {
        private readonly MyDbContext _context;
        public WorksController(MyDbContext context)
        {
            _context = context;
        }
        [HttpGet]
        public IActionResult GetAllWork()
        {
            var works = _context.Works.ToList();
            return Ok(works);
        }
        [HttpGet("{id}")]
        public IActionResult GetWorkById(int id)
        {
            try
            {
                var work = _context.Works.FirstOrDefault(w => w.WorkId == id);
                return Ok(work);
            }
            catch (Exception)
            {
                return BadRequest();
            }
        }
        [HttpPost]
        public IActionResult CreateWork(WorkModel model)
        {
            try
            {
                var work = _context.Works.FirstOrDefault(w => w.WorkId == model.WorkId);
                if (work == null)
                {
                    var work1 = new Work
                    {
                        //WorkId = model.WorkId,
                        WorkName = model.WorkName,
                        Description = model.Description,
                        Date = model.Date,
                        IsStatus = model.IsStatus,
                        Id = model.Id
                    };
                    _context.Works.Add(work1);
                    _context.SaveChanges();
                    return Ok(new
                    {
                        Success = true,
                        Data = work1
                    });
                }
                return BadRequest("Work already exists");
            }
            catch (Exception)
            {
                return BadRequest();
            }
        }
        [HttpPut("{id}")]
        public IActionResult UpdateWork(int id, WorkModel work1)
        {
            try
            {
                var work = _context.Works.FirstOrDefault(w => w.WorkId == work1.WorkId);
                if (work == null)
                {
                    return NotFound();
                }
                //WorkId = work1.WorkId;
                work.WorkName = work1.WorkName;
                work.Description = work1.Description;
                work.Date = work1.Date;
                work.IsStatus = work1.IsStatus;
                work.Id = work1.Id;
                _context.SaveChanges();
                return Ok();
            }
            catch (Exception)
            {
                return BadRequest();
            }
        }
        [HttpDelete("{id}")]
        public IActionResult DeleteWork(int id)
        {
            try
            {
                var work = _context.Works.FirstOrDefault(w => w.WorkId == id);
                if (work == null)
                {
                    return NotFound();
                }
                _context.Works.Remove(work);
                _context.SaveChanges();
                return Ok();
            }
            catch (Exception)
            {
                return BadRequest();
            }
        }
    }
}
