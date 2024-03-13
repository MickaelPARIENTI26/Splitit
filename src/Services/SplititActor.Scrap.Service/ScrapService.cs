using HtmlAgilityPack;
using Microsoft.Extensions.Configuration;
using SplititActor.Common.Configurations;
using SplititActor.Data.Actor;

namespace SplititActor.Service.Scrap
{
    public class ScrapService : IScrapService
    {
        private static int lastGeneratedId = 0;
        private readonly IConfiguration _configuration;

        public ScrapService(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public void ScrapAllActorByProvider(string provider)
        {
            var providerConfig = GetProviderConfig(provider);
            if (providerConfig != null)
            {
                var web = new HtmlWeb();
                var doc = web.Load(providerConfig.UrlProvider);

                var actorNodes = doc.DocumentNode.SelectNodes(providerConfig.ActorNodeXPath);

                if (actorNodes == null)
                    return;

                using (var dbContext = new ActorsDbContext())
                {
                    foreach (var actorNode in actorNodes)
                    {
                        var actor = CreateActorFromNode(actorNode, providerConfig);
                        if (actor != null && actor.Name != null)
                        {
                            dbContext.Actors.Add(actor);
                        }
                    }
                    dbContext.SaveChanges();
                }
            }
            else
            {
                throw new ArgumentException($"Can't find the provider in configuration {provider}");
            }
        }

        private ProviderInfo GetProviderConfig(string provider)
        {
            var providerConfig = _configuration.GetSection("ProviderConfigurations").Get<ProviderConfigurations>();
            return provider.ToLower() switch
            {
                "imdb" => providerConfig?.Imdb,
                "other" => providerConfig?.Other,
                _ => null,
            };
        }

        private ActorEntity CreateActorFromNode(HtmlNode actorNode, ProviderInfo providerConfig)
        {
            return new ActorEntity
            {
                Id = GenerateUniqueId(),
                Name = GetNodeText(actorNode, providerConfig.NameNodeXPath),
                Rank = GetRank(actorNode, providerConfig.RankNodeXPath),
                Details = GetNodeText(actorNode, providerConfig.DetailsNodeXPath),
                Type = GetActorType(actorNode, providerConfig.ActorTypeXPath)
            };
        }

        private int GenerateUniqueId()
        {
            return ++lastGeneratedId;
        }

        private string GetNodeText(HtmlNode parentNode, string xpath)
        {
            var node = parentNode.SelectSingleNode(xpath);
            return node?.InnerText.Trim();
        }

        private int GetRank(HtmlNode actorNode, string rankNodeXPath)
        {
            var rankNode = actorNode.SelectSingleNode(rankNodeXPath);
            if (rankNode != null && int.TryParse(rankNode.InnerText.Trim().Replace(".", ""), out int rank))
            {
                return rank;
            }
            return 0; // Default value if rank cannot be parsed
        }

        private string GetActorType(HtmlNode actorNode, string actorTypeXPath)
        {
            var typeNode = actorNode.SelectSingleNode(actorTypeXPath);
            if (typeNode != null)
            {
                var typeText = typeNode.InnerText.Trim();
                // Remove text after |
                int pipeIndex = typeText.IndexOf('|');
                if (pipeIndex != -1)
                {
                    typeText = typeText[..pipeIndex].Trim();
                }
                return typeText;
            }
            return string.Empty;
        }
    }
}