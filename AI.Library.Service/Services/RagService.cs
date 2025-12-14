using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace AI.Library.Service.Services
{
    public class RagService
    {
        private readonly EmbedService _embed;

        public RagService(EmbedService embed)
        {
            _embed = embed;
        }

        private float Cosine(List<float> a, List<float> b)
        {
            float dot = 0, magA = 0, magB = 0;

            for (int i = 0; i < a.Count; i++)
            {
                dot += a[i] * b[i];
                magA += a[i] * a[i];
                magB += b[i] * b[i];
            }

            return dot / ((float)Math.Sqrt(magA) * (float)Math.Sqrt(magB));
        }

        private async Task<List<float>> GetQueryEmbedding(string text)
        {
            using var client = new HttpClient();

            var res = await client.PostAsJsonAsync("http://localhost:5005/embed", new { text });
            var responseString = await res.Content.ReadAsStringAsync();

            dynamic parsed = JsonConvert.DeserializeObject(responseString);
            return ((IEnumerable<object>)parsed.embedding)
                .Select(x => (float)Convert.ToDouble(x))
                .ToList();
        }

        public async Task<List<BookRecord>> Search(string query)
        {
            var qEmbed = await GetQueryEmbedding(query);

            var scored = _embed.Embeddings.Select(e => new
            {
                book = _embed.Books.First(b => b.Isbn == e.Isbn),
                score = Cosine(qEmbed, e.Embedding)
            })
            .OrderByDescending(x => x.score)
            .Take(5)
            .Select(x => x.book)
            .ToList();

            return scored;
        }

        public string BuildContext(List<BookRecord> list)
        {
            return string.Join("\n\n", list.Select(b =>
                $"Başlık: {b.Title}\nYazar: {b.Author}\nÖzet: {b.Summary}"));
        }

        public List<BookRecord> GetSimilarBooks(BookRecord book, int count = 3)
        {
            var target = _embed.Embeddings.FirstOrDefault(e => e.Isbn == book.Isbn);
            if (target == null) return new List<BookRecord>();

            var scored = _embed.Embeddings
                .Where(e => e.Isbn != book.Isbn)
                .Select(e => new
                {
                    book = _embed.Books.First(b => b.Isbn == e.Isbn),
                    score = Cosine(target.Embedding, e.Embedding)
                })
                .OrderByDescending(x => x.score)
                .Take(count)
                .Select(x => x.book)
                .ToList();

            return scored;
        }

        public List<BookRecord> RecommendFromBook(BookRecord book, int count = 5)
        {
            var targetEmbed = _embed.Embeddings.FirstOrDefault(e => e.Isbn == book.Isbn);
            if (targetEmbed == null)
                return new List<BookRecord>();

            var subject = book.Subject?.ToLower() ?? "";
            var keywords = book.Keywords?.Select(k => k.ToLower()).ToList() ?? new List<string>();

            var scored = _embed.Embeddings
                .Where(e => e.Isbn != book.Isbn)
                .Select(e =>
                {
                    var b = _embed.Books.First(x => x.Isbn == e.Isbn);

                    float embedScore = Cosine(targetEmbed.Embedding, e.Embedding);

                    bool subjectMatch = !string.IsNullOrEmpty(subject) &&
                                        b.Subject?.ToLower().Contains(subject) == true;

                    int keywordMatches = 0;
                    if (b.Keywords != null)
                        keywordMatches = b.Keywords
                            .Select(k => k.ToLower())
                            .Count(k => keywords.Contains(k));

                    float finalScore =
                        embedScore * 0.6f +
                        (subjectMatch ? 0.25f : 0f) +
                        (keywordMatches * 0.15f);

                    return new
                    {
                        Book = b,
                        Score = finalScore
                    };
                })
                .OrderByDescending(x => x.Score)
                .Take(count)
                .Select(x => x.Book)
                .ToList();

            return scored;
        }

    }
}
