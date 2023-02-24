using DemoWebAPI.Data;
using DemoWebAPI.Models;

namespace DemoWebAPI.Services
{
    public class FilterWorkRepository : IFilterWorkRepository
    {
        private readonly MyDbContext _context;
        public FilterWorkRepository(MyDbContext context)
        {
            _context = context;
        }
        public List<WorkModel> FilterAll(bool? status, DateTime? date)
        {
            var allWorks = _context.Works.AsQueryable();
            #region filter
            if (status != null)
            {
                allWorks = allWorks.Where(w => w.IsStatus == status);
            }
            if (date != null)
            {
                allWorks = allWorks.Where(w => w.Date == date);
            }
            #endregion
            var result = allWorks.Select(w => new WorkModel
            {
                WorkId = w.WorkId,
                WorkName = w.WorkName,
                Description = w.Description,
                Date = w.Date,
                IsStatus = w.IsStatus,
                Id = w.User.Id
            });
            return result.ToList();
        }
    }
}
