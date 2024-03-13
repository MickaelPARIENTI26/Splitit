using SplititActor.Data.Actor;
using SplititActor.Dto.Actor;

namespace SplititActor.Service.Actor
{
    public class ActorService : IActorService
    {
        public ActorResponse GetAllActors(string? nameFilter, int? minRank, int? maxRank, int page, int pageSize)
        {
            using (var dbContext = new ActorsDbContext())
            {
                var actorsQuery = dbContext.Actors.ToList().AsQueryable();
                if (!string.IsNullOrEmpty(nameFilter))
                {
                    actorsQuery = actorsQuery.Where(actor => actor.Name.Contains(nameFilter));
                }

                if (minRank.HasValue)
                {
                    actorsQuery = actorsQuery.Where(actor => actor.Rank >= minRank);
                }

                if (maxRank.HasValue)
                {
                    actorsQuery = actorsQuery.Where(actor => actor.Rank <= maxRank);
                }

                var totalActors = actorsQuery.Count();
                var totalPages = (int)Math.Ceiling((double)totalActors / pageSize);

                var actors = actorsQuery
                    .OrderBy(actor => actor.Id)
                    .Skip((page - 1) * pageSize)
                    .Take(pageSize)
                    .Select(actor => new Dto.Actor.Actor { Id = actor.Id, Name = actor.Name })
                    .ToList();

                return new ActorResponse
                {
                    TotalActors = totalActors,
                    TotalPages = totalPages,
                    Actors = actors
                };
            }
        }

        public ActorEntity GetActorById(int actorId)
        {
            using (var dbContext = new ActorsDbContext())
            {
                var actor = dbContext.Actors.Find(actorId);
                return actor ?? throw new ArgumentException($"Actor with ID {actorId} not found.");
            }
        }

        public void AddNewActor(ActorEntity actor)
        {
            using (var dbContext = new ActorsDbContext())
            {
                if (actor.Name != null && actor.Rank > 0)
                {
                    var existingActorWithNewRank = dbContext.Actors.FirstOrDefault(a => a.Id != actor.Id && a.Rank == actor.Rank);
                    if (existingActorWithNewRank != null)
                    {
                        throw new InvalidOperationException($"Another actor with rank {actor.Rank} already exists.");
                    }

                    int newActorId = GetNextAvailableId(dbContext);
                    actor.Id = newActorId;
                    dbContext.Actors.Add(actor);
                    dbContext.SaveChanges();
                }
                else
                {
                    throw new ArgumentException($"Missing data for Name or Rank.");
                }
            }
        }

        public void UpdateActor(ActorEntity actor)
        {
            using (var dbContext = new ActorsDbContext())
            {
                if (actor.Name != null && actor.Rank > 0)
                {
                    var actorToUpdate = dbContext.Actors.Find(actor.Id) ?? throw new ArgumentException($"Actor with ID {actor.Id} not found.");

                    var existingActorWithNewRank = dbContext.Actors.FirstOrDefault(a => a.Id != actor.Id && a.Rank == actor.Rank);
                    if (existingActorWithNewRank != null)
                    {
                        throw new InvalidOperationException($"Another actor with rank {actor.Rank} already exists.");
                    }

                    actorToUpdate.Rank = actor.Rank;
                    actorToUpdate.Details = actor.Details;
                    actorToUpdate.Type = actor.Type;

                    dbContext.SaveChanges();
                }
                else
                {
                    throw new ArgumentException($"Missing data for Name or Rank. Can't update the actor {actor.Id}");
                }
            }
        }

        public void DeleteActorById(int actorId)
        {
            using (var dbContext = new ActorsDbContext())
            {
                var actorToRemove = dbContext.Actors.Find(actorId);
                if (actorToRemove != null)
                {
                    dbContext.Actors.Remove(actorToRemove);
                    dbContext.SaveChanges();
                }
                else
                {
                    throw new InvalidOperationException($"Actor with ID {actorId} not found can't delete id.");
                }
            }
        }

        private static int GetNextAvailableId(ActorsDbContext dbContext)
        {
            int maxId = dbContext.Actors.Max(a => (int?)a.Id) ?? 0;
            return maxId + 1;
        }
    }
}