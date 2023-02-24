using DemoWebAPI.Models;

namespace DemoWebAPI.Services
{
    public interface IFilterWorkRepository
    {
        List<WorkModel> FilterAll(bool? status, DateTime? date);
    }
}
