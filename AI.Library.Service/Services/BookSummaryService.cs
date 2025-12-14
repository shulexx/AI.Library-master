using System.Threading.Tasks;

namespace AI.Library.Service.Services
{
    public class BookSummaryService
    {
        private readonly RagService _rag;
        private readonly LlmService _llm;

        public BookSummaryService(RagService rag, LlmService llm)
        {
            _rag = rag;
            _llm = llm;
        }

        public async Task<string> Summarize(string text)
        {
            var books = await _rag.Search(text);
            if (!books.Any())
                return "Kitap bulunamadı.";

            var context = _rag.BuildContext(books);

            return await _llm.GeminiFlashLoose("Bu kitabı detaylı özetle.", context);
        }
    }
}
