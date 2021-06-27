using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

using SignalRTestServer.Data;
using SignalRTestServer.Models;

using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR;
using SignalRTestServer.Hubs;

namespace SignalRTestServer.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AreaPointsController : ControllerBase
    {
        public AreaPointsController(ServerContext context, IHubContext<MapEntitiesHub> mapEntitiesHub)
        {
            _context = context;
            _mapEntitiesHub = mapEntitiesHub;
        }

        private readonly ServerContext _context;

        private readonly IHubContext<MapEntitiesHub> _mapEntitiesHub;

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] AreaPoint areaPoint)
        {
            if (areaPoint is null)
                return NotFound();

            Area area = await _context.Areas.FirstOrDefaultAsync(a => a.Id == areaPoint.AreaId);

            if (area is null)
                return NotFound();

            await _context.AreaPoints.AddAsync(areaPoint);

            await _context.SaveChangesAsync();

            await _mapEntitiesHub.Clients.All.SendAsync("SendAreaPoint", areaPoint);

            return Ok(areaPoint);
        }

        [HttpGet]
        public async Task<IActionResult> Read(int areaId)
        {
            IQueryable<AreaPoint> areaPoints = _context.AreaPoints.Where(a => a.AreaId == areaId).AsNoTracking();

            if (areaPoints is null || !areaPoints.Any())
            {
                return NoContent();
            }

            return Ok(areaPoints);
        }

        [HttpPut]
        public async Task<IActionResult> Update([FromBody] AreaPoint areaPoint)
        {
            if (areaPoint is null)
                return NotFound();

            _context.AreaPoints.Update(areaPoint);

            await _context.SaveChangesAsync();

            await _mapEntitiesHub.Clients.All.SendAsync("SendAreaPoint", areaPoint);

            return Ok(areaPoint);
        }

        [HttpDelete]
        public async Task<IActionResult> Delete(int id)
        {
            AreaPoint firstAreaPoint = await _context.AreaPoints.FirstOrDefaultAsync(a => a.Id == id);

            if (firstAreaPoint is null)
                return NotFound();

            _context.AreaPoints.Remove(firstAreaPoint);

            await _context.SaveChangesAsync();

            await _mapEntitiesHub.Clients.All.SendAsync("DeleteAreaPoint", firstAreaPoint);

            return Ok(firstAreaPoint);
        }
    }
}
