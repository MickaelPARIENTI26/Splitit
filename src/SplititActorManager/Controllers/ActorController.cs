using Microsoft.AspNetCore.Mvc;
using SplititActor.Data.Actor;
using SplititActor.Dto.Actor;
using SplititActor.Service.Actor;
using Swashbuckle.AspNetCore.Annotations;
using System.Net;

namespace SplititActorManager.Controllers
{
    [Produces("application/json")]
    [Route("Actor")]
    [ApiController]
    public class ActorController : ControllerBase
    {
        private readonly IActorService _actorService;

        public ActorController(IActorService actorService)
        {
            _actorService = actorService;
        }

        /// <summary>
        /// Retrieves a list of actors based on optional filters such as name, rank range, page, and page size.
        /// </summary>
        /// <remarks>
        /// Sample request:
        /// GET /actors?nameFilter=Tom&minRank=5&maxRank=10&page=2&pageSize=5
        /// </remarks>
        /// <param name="nameFilter">Optional. Filters actors by name.</param>
        /// <param name="minRank">Optional. Filters actors by minimum rank.</param>
        /// <param name="maxRank">Optional. Filters actors by maximum rank.</param>
        /// <param name="page">Optional. Specifies the page number.</param>
        /// <param name="pageSize">Optional. Specifies the page size.</param>
        /// <returns>Returns a list of actors.</returns>
        [HttpGet]
        [ProducesResponseType(typeof(ActorResponse), (int)HttpStatusCode.OK)]
        [SwaggerResponse((int)HttpStatusCode.OK, "OK", typeof(ActorResponse))]
        public IActionResult GetActors([FromQuery] string? nameFilter, [FromQuery] int? minRank, [FromQuery] int? maxRank, [FromQuery] int page = 1, [FromQuery] int pageSize = 10)
        {
            try
            {
                var actors = _actorService.GetAllActors(nameFilter, minRank, maxRank, page, pageSize);
                return Ok(actors);
            }
            catch (ArgumentException ex)
            {
                return NotFound(ex.Message);
            }
        }

        /// <summary>
        /// Retrieves an actor by ID.
        /// </summary>
        /// <remarks>
        /// Sample request:
        /// GET /actors/123
        /// </remarks>
        /// <param name="actorId">The ID of the actor to retrieve.</param>
        /// <returns>Returns the actor with the specified ID.</returns>
        [HttpGet("{actorId}")]
        [ProducesResponseType(typeof(ActorEntity), (int)HttpStatusCode.OK)]
        [SwaggerResponse((int)HttpStatusCode.OK, "OK", typeof(ActorEntity))]
        public IActionResult GetActorById(int actorId)
        {
            try
            {
                var actor = _actorService.GetActorById(actorId);
                return Ok(actor);
            }
            catch (ArgumentException ex)
            {
                return NotFound(ex.Message);
            }
        }

        /// <summary>
        /// Adds a new actor.
        /// </summary>
        /// <remarks>
        /// Sample request:
        /// POST /actors
        /// {
        ///   "id": 123,
        ///   "name": "Tom Cruise",
        ///   "type": "Actor",
        ///   "rank": 1,
        ///   "details": "Tom Cruise is an American actor and producer."
        /// }
        /// </remarks>
        /// <param name="actor">The actor to add.</param>
        /// <returns>Returns 200 OK if the actor is added successfully.</returns>
        [HttpPost]
        [ProducesResponseType((int)HttpStatusCode.NoContent)]
        public IActionResult AddActor(ActorEntity actor)
        {
            try
            {
                _actorService.AddNewActor(actor);
                return Ok();
            }
            catch (ArgumentException ex)
            {
                return NotFound(ex.Message);
            }
            catch (InvalidOperationException ex)
            {
                return Conflict(ex.Message);
            }
        }

        /// <summary>
        /// Updates an existing actor.
        /// </summary>
        /// <remarks>
        /// Sample request:
        /// PUT /actors
        /// {
        ///   "id": 123,
        ///   "name": "Updated Name",
        ///   "type": "Actor",
        ///   "rank": 1,
        ///   "details": "Updated details about the actor."
        /// }
        /// </remarks>
        /// <param name="actor">The actor with updated information.</param>
        /// <returns>Returns 204 No Content if the actor is updated successfully.</returns>
        [HttpPut]
        [ProducesResponseType((int)HttpStatusCode.NoContent)]
        public IActionResult UpdateActor([FromBody] ActorEntity actor)
        {
            if (actor.Id != actor.Id)
            {
                return BadRequest("Actor ID in the request body does not match the ID in the URL.");
            }

            try
            {
                _actorService.UpdateActor(actor);
                return NoContent();
            }
            catch (ArgumentException ex)
            {
                return NotFound(ex.Message);
            }
            catch (InvalidOperationException ex)
            {
                return Conflict(ex.Message);
            }
        }

        /// <summary>
        /// Deletes an actor by ID.
        /// </summary>
        /// <remarks>
        /// Sample request:
        /// DELETE /actors/123
        /// </remarks>
        /// <param name="actorId">The ID of the actor to delete.</param>
        /// <returns>Returns 204 No Content if the actor is deleted successfully.</returns>
        [HttpDelete("{actorId}")]
        [ProducesResponseType((int)HttpStatusCode.NoContent)]
        public IActionResult DeleteActorById(int actorId)
        {
            try
            {
                _actorService.DeleteActorById(actorId);
                return NoContent();
            }
            catch (ArgumentException ex)
            {
                return NotFound(ex.Message);
            }
        }
    }
}
