using SplititActor.Data.Actor;
using SplititActor.Dto.Actor;

namespace SplititActor.Service.Actor
{
    public interface IActorService
    {
        ActorResponse GetAllActors(string? nameFilter, int? minRank, int? maxRank, int page, int pageSize);
        ActorEntity GetActorById(int actorId);
        void DeleteActorById(int actorId);
        void AddNewActor(ActorEntity actor);
        void UpdateActor(ActorEntity actor);
    }
}