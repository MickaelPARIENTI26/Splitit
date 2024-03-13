namespace SplititActor.Dto.Actor
{
    /// <summary>
    /// Represents the response containing a list of actors.
    /// </summary>
    public class ActorResponse
    {    
        /// <summary>
         /// Gets or sets the total number of actors.
         /// </summary>
        public int TotalActors { get; set; }
        /// <summary>
        /// Gets or sets the total number of pages.
        /// </summary>
        public int TotalPages { get; set; }
        /// <summary>
        /// Gets or sets the list of actors.
        /// </summary>
        public IEnumerable<Actor> Actors { get; set; }
    }
}
