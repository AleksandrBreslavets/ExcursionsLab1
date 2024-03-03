using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ExcursionsInfrastructure.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ChartController : ControllerBase
    {
        private readonly ExcursionsDbContext _context;
        public ChartController(ExcursionsDbContext context)
        {
            _context = context;
        }

        [HttpGet("JsonData")]
        public JsonResult JsonData()
        {
            var excursions = _context.Excursions.Include(e => e.Visitors).ToList();
            List<object> excurVisitor= new List<object>();
            excurVisitor.Add(new[] { "Екскурсія", "Кількість відвідувачів" });
            foreach(var e in excursions)
            {
                excurVisitor.Add(new object[] { e.Name, e.Visitors.Count() });
            }

            return new JsonResult(excurVisitor);
        }
    }
}
