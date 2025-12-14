using System.IO;
using Tesseract;
using SixLabors.ImageSharp.PixelFormats;
using SixLabors.ImageSharp.Formats.Png;
using SixLabors.ImageSharp.Processing;

// ImageSharp Image’ı seçiyoruz
using Image = SixLabors.ImageSharp.Image;

namespace AI.Library.Service.Services
{
    public class OcrService
    {
        public string ExtractText(byte[] imageBytes)
        {
            // ImageSharp ile resmi yükle
            using var image = Image.Load<Rgba32>(imageBytes);

            // OCR doğruluğu için grileştir
            image.Mutate(x => x.Grayscale());

            // ImageSharp → MemoryStream
            using var ms = new MemoryStream();
            image.Save(ms, new PngEncoder());
            ms.Position = 0;

            // Tesseract OCR
            string tessPath = Path.Combine(Directory.GetCurrentDirectory(), "tessdata");
            using var engine = new TesseractEngine(tessPath, "tur", EngineMode.Default);

            using var pix = Pix.LoadFromMemory(ms.ToArray());
            using var page = engine.Process(pix);

            return page.GetText().Trim();
        }
    }
}
