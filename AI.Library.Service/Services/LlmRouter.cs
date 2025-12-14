using System.Threading.Tasks;

namespace AI.Library.Service.Services
{
    public class LlmRouter
    {
        private readonly LlmService _llm;

        public LlmRouter(LlmService llm)
        {
            _llm = llm;
        }

        public async Task<string> AnswerAsync(string query, string context)
        {
            var q = query.ToLower();

            // Daha detay isteyen sorular → GroqLoose
            if (q.Contains("detay") ||
                q.Contains("uzun") ||
                q.Contains("özet") ||
                q.Contains("açıkla") ||
                q.Contains("neden") ||
                q.Contains("anlat"))
            {
                return await _llm.GroqLoose(query, context);
            }

            // Normal soru → GroqStrict
            return await _llm.GroqStrict(query, context);
        }
    }
}
