using Microsoft.AspNetCore.SignalR;

using SignalRTestServer.Models;

using System.Threading.Tasks;

namespace SignalRTestServer.Hubs
{
    public class MapEntitiesHub : Hub
    {
        public async Task SendAreaPoint(AreaPoint areaPoint)
        {
            await Clients.All.SendAsync("SendAreaPoint", areaPoint);
        }

        public async Task SendArea(Area area)
        {
            await Clients.All.SendAsync("SendArea", area);
        }

        public async Task DeleteAreaPoint(AreaPoint areaPoint)
        {
            await Clients.All.SendAsync("DeleteAreaPoint", areaPoint);
        }

        public async Task DeleteArea(Area area)
        {
            await Clients.All.SendAsync("DeleteArea", area);
        }
    }
}
