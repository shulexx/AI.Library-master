using Newtonsoft.Json;
using System.Collections.Generic;

namespace AI.Library.Service.Services
{
    public class EmbedService
    {
        private readonly string _booksPath = "Data/books.json";
        private readonly string _embedPath = "Data/embeddings.json";

        public List<BookRecord> Books { get; set; }
        public List<EmbeddingRecord> Embeddings { get; set; }

        public EmbedService()
        {
            Books = LoadBooks();
            Embeddings = LoadEmbeddings();
        }

        private List<BookRecord> LoadBooks()
        {
            return JsonConvert.DeserializeObject<List<BookRecord>>(File.ReadAllText(_booksPath))!;
        }

        private List<EmbeddingRecord> LoadEmbeddings()
        {
            return JsonConvert.DeserializeObject<List<EmbeddingRecord>>(File.ReadAllText(_embedPath))!;
        }
    }

    public class EmbeddingRecord
    {
        public string Isbn { get; set; }
        public List<float> Embedding { get; set; }
    }

    public class BookRecord
    {
        public string Isbn { get; set; }
        public string Title { get; set; }
        public string Author { get; set; }
        public string Publisher { get; set; }
        public int Year { get; set; }
        public string Subject { get; set; }
        public string Location { get; set; }
        public string Summary { get; set; }
        public List<string> Keywords { get; set; }
    }
}
