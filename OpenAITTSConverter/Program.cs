using System.Collections;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using OpenAITTSConverter;

var request = await SetupInput();

foreach (var text in request.TextsToConvertToAudio)
{
    var body = new
    {
        model = "gpt-4o-mini-tts",
        voice = request.GetVoiceByEnum(request.VoiceModelOfAudio),
        input = text,
        language =  request.GetLanguage(request.LanguageOfText),
        instructions= $"Audio for the words i sent you in the {request.LanguageOfText} Language"
    };
    
    await RequestAndSaveAudioFile(body, text);
}

Console.WriteLine("Audios saved in C:/Audiostranslated");

async Task RequestAndSaveAudioFile(object body,string filenameOutput)
{
    var client = new HttpClient();
    client.DefaultRequestHeaders.Authorization =
        new AuthenticationHeaderValue("Bearer", request.ApiKey);
    
    var json = JsonSerializer.Serialize(body);

    var response = await client.PostAsync(
        "https://api.openai.com/v1/audio/speech",
        new StringContent(json, Encoding.UTF8, "application/json")
    );
    
    var bytes = await response.Content.ReadAsByteArrayAsync();
    var pathToSave = Path.Combine(Environment.SystemDirectory,"audiostranslated",$"{filenameOutput}.mp3") ;
    await File.WriteAllBytesAsync(pathToSave, bytes);
}

async Task<TTSRequest> SetupInput()
{
    var request = new TTSRequest();
    
    Console.WriteLine("Please Enter API-Key");
    request.ApiKey = Console.ReadLine() ?? throw new ArgumentNullException(nameof(request.ApiKey));

    request.LanguageOfText = Language.Ukrainian;    
    
    Console.WriteLine("Please give the entire Filepath to the file \n example: sometext/folder/file.txt");
    var path = Console.ReadLine() ?? throw new ArgumentNullException("invalid entry");
    
    request.TextsToConvertToAudio = File.ReadAllLines(path);
    return request;
}