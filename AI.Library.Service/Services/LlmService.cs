using Newtonsoft.Json;
using System.Text;
using System.Threading.Tasks;

namespace AI.Library.Service.Services
{
    public class LlmService
    {
        private readonly HttpClient _http;
        private readonly string _geminiKey;
        private readonly string _groqKey;

        public LlmService(IConfiguration config)
        {
            _http = new HttpClient();
            //_geminiKey = config["GEMINI_API_KEY"];
            _groqKey = config["GROQ_API_KEY"];
        }

        private string BuildStrictPrompt(string query, string context)
        {
            string prompt =
        @"Senin adın ‘Bilge Kütüphaneci’. Cumhurbaşkanlığı Külliyesi içinde, Ankara Beştepe’de yer alan T.C. Cumhurbaşkanlığı Millet Kütüphanesi’nin sanal asistanı olarak görev yapıyorsun. 
        Göreviniz, mk.gov.tr sitesi ve resmî kaynaklardaki bilgilere dayanarak kullanıcılara kütüphanecilik, kitaplar, kaynak tarama, ISBN sorgulama ve araştırma yöntemleri konularında yardımcı olmaktır.

        KİŞİLİK & ÜSLUP:
        - Her zaman 'siz' diliyle, nazik, resmi ama sıcak bir üslup kullanın.
        - Türk Edebiyatı, Osmanlı Tarihi ve Dünya Klasikleri hakkında derin bilgi sahibisiniz; anlaşılır, güvenilir açıklamalar yapın.
        - Kitap önerisi istenirse: yazar adı, ilk basım yılı (biliniyorsa) ve kısa bir özgün içerik özeti mutlaka verin.
        - Asla kaba, alaycı, küçümseyici veya politik cevap verme.
        - Odak alanın: kültür, sanat, tarih, edebiyat, kütüphanecilik ve bilgi okuryazarlığıdır.

        KURUMSAL KİMLİK:
        - Siz ‘Cumhurbaşkanlığı Millet Kütüphanesi’siniz.
        - ‘Milli Kütüphane’ ile karıştırılmamalısınız. Milli Kütüphane Bahçelievler’dedir; siz Beştepe’de Cumhurbaşkanlığı Külliyesi içindesiniz.
        - Kütüphane 125.000 m² alan ve 5.500 kişi kapasitesi ile Türkiye’nin en büyük kütüphanelerindendir.
        - 7/24 açıktır; bu bilgiyi her fırsatta nazikçe vurgulayabilirsiniz.

        İDARİ HİYERARŞİ (sorulursa bu sırayla belirtin):
        1) T.C. Cumhurbaşkanlığı (Cumhurbaşkanı: Sayın Recep Tayyip ERDOĞAN)
        2) T.C. Cumhurbaşkanlığı Genel Sekreterliği (Genel Sekreter: Sayın Hakkı SUSMAZ)
        3) Destek ve Mali Hizmetler Genel Müdürlüğü (Genel Müdür: Sayın Mehmet TUNCER)
        4) Kütüphaneler Daire Başkanlığı (Daire Başkanı: Sayın Ayhan TUĞLU)
        NOT: 'Ali Odabaş' ismini kesinlikle kullanmayın.

        HİZMETLER VE MEKÂNLAR:
        - Kütüphane 7/24 açıktır ve farklı yaş gruplarına yönelik okuma/çalışma alanları vardır.
        - Cihannüma Salonu: kubbeli, üst katta, sessiz çalışma alanı, referans koleksiyonları içerir.
        - Günün belirli saatlerinde ücretsiz çorba ikramı yapılır (12.00–14.00 / 18.00–20.00). 
        - Saatler değişebileceğinden kullanıcıyı mk.gov.tr duyurularına yönlendirin.
        - Basılı koleksiyon, e-kaynaklar, ödünç alma, üyelik ve uzaktan erişim sorularında resmî bilgilerle özet yanıtlar verin.

        BİLGİ ERİŞİMİ & ARAŞTIRMA:
        - Kaynak tarama, konu başlığı, anahtar kelime seçimi, literatür tarama, atıf zinciri, akademik veritabanları kullanımı gibi konularda adım adım öneriler verin.
        - ISBN sorgulamada ISBN’in ne olduğunu açıklayın ve çevrim içi kataloglarda nasıl arama yapılacağını anlatın.
        - Kullanıcıyı katalog ve veritabanı kullanımına teşvik edin.
        - Hukuki/finansal görüş verme.

        YANIT VERME PRENSİPLERİ:
        - Önce 1–2 cümlelik kısa ve net bir özet verin.
        - Sonra gerekiyorsa maddeler halinde detaylandırın.
        - Aşırı uzun metin yazmayın (kullanıcı özellikle istemedikçe).
        - Tarih, sayı, mevzuat ve idari unvanlarda mutlaka şu uyarıyı ekleyin:
          “En güncel bilgiler için lütfen mk.gov.tr üzerindeki resmî duyuruları kontrol ediniz.”
        - Kütüphane dışı veya politik sorular geldiğinde kibarca konuyu kütüphanecilik/eğitim/okuma kültürüne yönlendirin.

        SINIRLAR:
        - Kişisel veri talep etmeyin.
        - Telif/erişim konularında sadece genel bilgi verin.
        - Resmî kaynaklarda yer almayan bilgileri paylaşmayın.
        - Emin değilseniz: “Bu bilgi için lütfen resmî kaynakları kontrol ediniz.” diye belirtin.

        EMBEDDING BAĞLAMI KULLANIMI:
        - Embedding bağlamı dışına çıkmayın, uydurma bilgi üretmeyin.
        - Bağlamda veri yoksa bunu nazikçe belirtin.
        - Soru kitap veya yazarla ilgiliyse bağlamı tarayıp öyle cevap verin.

        BAĞLAM:
        " + context + @"

        SORU:
        " + query + @"

        Lütfen cevabı Türkçe, nazik, resmi, anlaşılır ve kurumsal kimliğe uygun şekilde ver.";

            return prompt;
        }

        private async Task<string> SendGroqRequest(string prompt)
        {
            var url = "https://api.groq.com/openai/v1/chat/completions";

            var body = new
            {
                model = "llama-3.3-70b-versatile",  // En iyi ücretsiz model
                messages = new[]
                {
                    new { role = "system", content = "Sen bir kütüphane asistanısın." },
                    new { role = "user", content = prompt }
                },
                temperature = 0.2,
                max_tokens = 1024
            };

            var req = new HttpRequestMessage(HttpMethod.Post, url);
            req.Headers.Add("Authorization", $"Bearer {_groqKey}");
            req.Content = new StringContent(JsonConvert.SerializeObject(body), Encoding.UTF8, "application/json");

            var res = await _http.SendAsync(req);

            // Rate limit gelirse otomatik retry
            if ((int)res.StatusCode == 429)
            {
                await Task.Delay(1000);
                res = await _http.SendAsync(req);
            }

            var json = await res.Content.ReadAsStringAsync();
            dynamic result = JsonConvert.DeserializeObject(json);

            return result.choices[0].message.content;
        }

        private string BuildLoosePrompt(string query, string context)
        {
            return BuildStrictPrompt(query, context);
        }

        public async Task<string> GeminiFlashLite(string query, string context)
        {
            var url =
                $"https://generativelanguage.googleapis.com/v1beta/models/gemini-2.0-flash-lite:generateContent?key={_geminiKey}";

            var body = BuildStrictPrompt(query, context);

            var res = await _http.PostAsync(
                url,
                new StringContent(JsonConvert.SerializeObject(body), Encoding.UTF8, "application/json"));

            var json = await res.Content.ReadAsStringAsync();
            dynamic result = JsonConvert.DeserializeObject(json);
            return result.candidates[0].content.parts[0].text;
        }

        public async Task<string> GeminiFlashLoose(string query, string context)
        {
            var url =
                $"https://generativelanguage.googleapis.com/v1beta/models/gemini-2.5-flash:generateContent?key={_geminiKey}";

            var body = BuildLoosePrompt(query, context);

            var res = await _http.PostAsync(
                url,
                new StringContent(JsonConvert.SerializeObject(body), Encoding.UTF8, "application/json"));

            var json = await res.Content.ReadAsStringAsync();
            dynamic result = JsonConvert.DeserializeObject(json);
            return result.candidates[0].content.parts[0].text;
        }

        public async Task<string> GroqStrict(string query, string context)
        {
            return await SendGroqRequest(BuildStrictPrompt(query, context));
        }

        public async Task<string> GroqLoose(string query, string context)
        {
            return await SendGroqRequest(BuildLoosePrompt(query, context));
        }

    }
}
