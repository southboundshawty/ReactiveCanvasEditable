using System.Collections.Generic;

namespace SignalRTestServer.Models
{
    public class Area
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public bool IsClosed { get; set; }

        public ICollection<AreaPoint> ShapePoints { get; set; }
    }
}
