using Microsoft.EntityFrameworkCore;

namespace SplititActor.Data.Actor
{
    public class ActorsDbContext : DbContext
    {
        public DbSet<ActorEntity> Actors { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseInMemoryDatabase("ActorsDatabase");
        }
    }
}