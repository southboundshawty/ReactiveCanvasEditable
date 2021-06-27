using Microsoft.EntityFrameworkCore;

using SignalRTestServer.Models;

namespace SignalRTestServer.Data
{
    public class ServerContext : DbContext
    {
        public ServerContext(DbContextOptions options) : base(options) { }

        public DbSet<Area> Areas { get; set; }

        public DbSet<AreaPoint> AreaPoints { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<AreaPoint>()
                .HasOne<Area>()
                .WithMany(g => g.ShapePoints)
                .HasForeignKey(s => s.AreaId);

            //base.OnModelCreating(modelBuilder);
        }
    }
}
