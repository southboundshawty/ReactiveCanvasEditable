namespace SignalRTestServer.Models
{
    public class AreaPoint
    {
        public int Id { get; set; }

        public int X { get; set; }

        public int Y { get; set; }

        public int AreaId { get; set; }

        public Area Area { get; set; }
    }
}
