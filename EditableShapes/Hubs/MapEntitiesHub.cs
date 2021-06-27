using EditableShapes.Models;
using EditableShapes.Models.Dto;
using Microsoft.AspNetCore.SignalR.Client;

namespace EditableShapes.Hubs
{
    public class MapEntitiesHub
    {
        private MapEntitiesHub()
        {
            Init();
        }

        public delegate void AreaReceiveHandler(AreaDto areaDto);
        public delegate void AreaPointReceiveHandler(AreaPointDto areaPointDto);

        public event AreaReceiveHandler OnAreaReceived;
        public event AreaPointReceiveHandler OnAreaPointReceived;

        public event AreaReceiveHandler OnAreaDeleted;
        public event AreaPointReceiveHandler OnAreaPointDeleted;

        private async void Init()
        {
            _connection = new HubConnectionBuilder().WithUrl("https://localhost:44390/MapEntitiesHub").Build();

            _connection.On<AreaPointDto>("SendAreaPoint", SendAreaPointHandler);
            _connection.On<AreaDto>("SendArea", SendAreaHandler);

            _connection.On<AreaPointDto>("DeleteAreaPoint", DeleteAreaPointHandler);
            _connection.On<AreaDto>("DeleteArea", DeleteAreaHandler);

            await _connection.StartAsync();
        }

        private void DeleteAreaHandler(AreaDto obj)
        {
            OnAreaDeleted?.Invoke(obj);
        }

        private void DeleteAreaPointHandler(AreaPointDto obj)
        {
            OnAreaPointDeleted?.Invoke(obj);
        }

        private void SendAreaPointHandler(AreaPointDto obj)
        {
            OnAreaPointReceived?.Invoke(obj);
        }

        private void SendAreaHandler(AreaDto obj)
        {
            OnAreaReceived?.Invoke(obj);
        }

        private static HubConnection _connection;

        private static MapEntitiesHub _instance;

        public static MapEntitiesHub GetInstance()
        {
            if (_instance is null)
            {
                _instance = new MapEntitiesHub();
            }

            return _instance;
        }
    }
}
