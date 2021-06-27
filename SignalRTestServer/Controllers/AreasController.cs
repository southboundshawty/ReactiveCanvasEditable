using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

using SignalRTestServer.Data;
using SignalRTestServer.Hubs;
using SignalRTestServer.Models;

using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR;

namespace SignalRTestServer.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AreasController : ControllerBase
    {
        public AreasController(ServerContext context, IHubContext<MapEntitiesHub> mapEntitiesHub)
        {
            _context = context;
            _mapEntitiesHub = mapEntitiesHub;
        }

        private readonly ServerContext _context;
        
        private readonly IHubContext<MapEntitiesHub> _mapEntitiesHub;

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] Area area)
        {
            if (area is null)
                return NotFound();

            await _context.Areas.AddAsync(area);

            await _context.SaveChangesAsync();

            await _mapEntitiesHub.Clients.All.SendAsync("SendArea", area);

            return Ok(area);
        }

        [HttpGet]
        public async Task<IActionResult> Read()
        {
            IQueryable<Area> areas = _context.Areas.AsNoTracking();

            if (areas is null || !areas.Any())
            {
                return NoContent();
            }

            return Ok(areas);
        }

        [HttpPut]
        public async Task<IActionResult> Update([FromBody] Area area)
        {
            if (area is null)
                return NotFound();

            _context.Areas.Update(area);

            await _context.SaveChangesAsync();

            await _mapEntitiesHub.Clients.All.SendAsync("SendArea", area);

            return Ok(area);
        }

        [HttpDelete]
        public async Task<IActionResult> Delete(int id)
        {
            Area firstArea = await _context.Areas.FirstOrDefaultAsync(a => a.Id == id);

            if (firstArea is null)
                return NotFound();

            _context.Areas.Remove(firstArea);

            await _context.SaveChangesAsync();

            await _mapEntitiesHub.Clients.All.SendAsync("DeleteArea", firstArea);

            return Ok(firstArea);
        }
    }
}
