namespace AI.Library.Service.Services;

public class TtsService
{
    public byte[] GenerateVoice(string text)
    {
        return System.Text.Encoding.UTF8.GetBytes("VOICE:" + text);
    }
}
