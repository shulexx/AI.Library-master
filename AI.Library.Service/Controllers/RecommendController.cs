using AI.Library.Service.Services;
using Microsoft.AspNetCore.Mvc;
using System.Linq;

namespace AI.Library.Service.Controllers
{
    [ApiController]
    [Route("api/recommend")]
    public class RecommendController : ControllerBase
    {
        private readonly RagService _rag;
        private readonly EmbedService _embed;
        private readonly TopicService _topics;

        public RecommendController(RagService rag, EmbedService embed, TopicService topics)
        {
            _rag = rag;
            _embed = embed;
            _topics = topics;
        }

        // 1) Benzer kitap öner
        [HttpGet("similar")]
        public IActionResult Similar([FromQuery] string isbn)
        {
            var book = _embed.Books.FirstOrDefault(b => b.Isbn == isbn);
            if (book == null)
                return NotFound("Kitap bulunamadı.");

            var list = _rag.GetSimilarBooks(book);
            return Ok(list);
        }

        // 2) Konuya göre kitap öner
        [HttpGet("topic")]
        public IActionResult RecommendTopic([FromQuery] string topic)
        {
            var list = _topics.RecommendByTopic(topic);
            return Ok(list);
        }

        [HttpGet("frombook")]
        public IActionResult FromBook([FromQuery] string isbn)
        {
            var book = _embed.Books.FirstOrDefault(b => b.Isbn == isbn);
            if (book == null)
                return NotFound("Kitap bulunamadı.");

            var list = _rag.RecommendFromBook(book);
            return Ok(list);
        }

    }
}
