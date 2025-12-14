using AI.Library.Service.Services;
using Microsoft.AspNetCore.Mvc;
using System.Linq;
using System.Threading.Tasks;

namespace AI.Library.Service.Controllers
{
    [ApiController]
    [Route("api/ask")]
    public class AskController : ControllerBase
    {
        private readonly RagService _rag;
        private readonly LlmRouter _llm;
        private readonly EmbedService _embed;

        private static string LastTopic = null;
        private static BookRecord LastBook = null;

        public AskController(RagService rag, LlmRouter llm, EmbedService embed)
        {
            _rag = rag;
            _llm = llm;
            _embed = embed;
        }

        private string Normalize(string text)
        {
            if (string.IsNullOrWhiteSpace(text)) return "";
            return text.ToLower()
                .Replace("ç", "c")
                .Replace("ğ", "g")
                .Replace("ı", "i")
                .Replace("ö", "o")
                .Replace("ş", "s")
                .Replace("ü", "u")
                .Trim();
        }

        private bool MatchTopic(string query, string topic)
        {
            query = Normalize(query);
            topic = Normalize(topic);

            if (query.Contains(topic)) return true;

            // "tıp konusu" → "tip"
            if (query.Contains(topic + "konu")) return true;
            if (query.Contains(topic + "alan")) return true;
            if (query.Contains(topic + "hakkin")) return true;
            if (query.Contains(topic + "ileilgili")) return true;

            return false;
        }

        [HttpPost]
        public async Task<IActionResult> Ask([FromBody] AskRequest req)
        {
            var qNorm = Normalize(req.Query);
            var allowedTopics = _embed.Books
                .Where(b => b.Keywords != null)
                .SelectMany(b => b.Keywords)
                .Distinct()
                .ToList();

            string detectedTopic = allowedTopics
                .FirstOrDefault(k => MatchTopic(qNorm, k));

            if (detectedTopic == null)
            {
                var answerNoTopic = await _llm.AnswerAsync(req.Query, "");
                return Ok(new
                {
                    answer = answerNoTopic,
                    books = new object[0]
                });
            }

            LastTopic = detectedTopic;

            var foundBooks = await _rag.Search(req.Query);

            if (foundBooks.Any())
            {
                LastBook = foundBooks.First();
            }
            else if (LastBook != null)
            {
                foundBooks = new List<BookRecord> { LastBook };
            }

            if (!foundBooks.Any())
            {
                var answerNoBook = await _llm.AnswerAsync(req.Query, "");
                return Ok(new
                {
                    answer = answerNoBook,
                    books = new object[0]
                });
            }
            var context = _rag.BuildContext(foundBooks);

            var answer = await _llm.AnswerAsync(req.Query, context);

            return Ok(new { answer, books = foundBooks });
        }
    }

    public class AskRequest
    {
        public string Query { get; set; }
    }
}
