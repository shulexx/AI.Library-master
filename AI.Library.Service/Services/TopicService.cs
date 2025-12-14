using System.Collections.Generic;
using System.Linq;

namespace AI.Library.Service.Services
{
    public class TopicService
    {
        private readonly EmbedService _embed;

        public TopicService(EmbedService embed)
        {
            _embed = embed;
        }

        public List<BookRecord> RecommendByTopic(string topic)
        {
            topic = topic.ToLower().Trim();

            return _embed.Books
                .Where(b => b.Subject != null && b.Subject.ToLower().Contains(topic))
                .Take(5)
                .ToList();
        }
    }
}
