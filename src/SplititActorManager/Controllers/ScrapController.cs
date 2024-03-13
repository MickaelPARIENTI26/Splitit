using Microsoft.AspNetCore.Mvc;
using SplititActor.Service.Scrap;
using System.Net;

namespace SplititActorManager.Controllers
{
    [Produces("application/json")]
    [Route("Scrap")]
    [ApiController]
    public class ScrapController : ControllerBase
    {
        private readonly IScrapService _scrapService;

        public ScrapController(IScrapService scrapService)
        {
            _scrapService = scrapService;
        }

        /// <summary>
        /// Scrapes all actors from a website provider.
        /// </summary>
        /// <remarks>
        /// Sample request:
        /// POST /actors/scrap/{provider}
        /// </remarks>
        /// <param name="provider">The name of the website provider to scrape actors from.</param>
        /// <returns>Returns 200 OK if the scraping operation is successful.</returns>
        [HttpPost]
        [Route("{provider}")]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        public IActionResult ScrapActor([FromRoute] string provider)
        {
            try
            {
                _scrapService.ScrapAllActorByProvider(provider);
                return Ok();
            }
            catch (Exception)
            {
                return NotFound();
            }
        }
    }
}
