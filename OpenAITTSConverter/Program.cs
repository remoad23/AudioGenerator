using System.Collections;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using AnkiEditTools;
using OpenAITTSConverter;

var request = await SetupInput();


//string currentKapitel = "";
 //
 // foreach (var text in request.TextsToConvertToAudio)
 // {
 //     if (text.Contains("Modul"))
 //     {
 //        currentKapitel = text.Replace("Modul", "Kapitel");    
 //     }
 //     
 //     var body = new
 //     {
 //         model = "gpt-4o-mini-tts",
 //         voice = request.GetVoiceByEnum(request.VoiceModelOfAudio),
 //         input = text,
 //         language =  request.GetLanguage(request.LanguageOfText),
 //         instructions= $"Audio for the words i sent you in the {request.LanguageOfText} Language"
 //     };
 //     
 //     await RequestAndSaveAudioFile(body, text,currentKapitel);
 // }
 //
 // var audioPath = Path.Combine("C:/", "audiostranslated");
 // Console.WriteLine($"Audios saved in {audioPath}");


var ankiEditor = new AnkiEditor();

var cardsInfo = await ankiEditor.GetDeckData("яблуко ukrainisch B2-C1");

foreach (var card in cardsInfo)
{
    var text = "";
    

    // Noun Field
    if (card.Fields.TryGetValue("Noun", out var noun) && !string.IsNullOrEmpty(noun.Value))
    {
        text += noun.Value;
    }

    // Phrase Field
    if (card.Fields.TryGetValue("Phrase", out var phrase) && !string.IsNullOrEmpty(phrase.Value))
    {
        text += phrase.Value;
    }

    // Verb (Imperfective)
    if (card.Fields.TryGetValue("Verb (Imperfective)", out var vImp) && !string.IsNullOrEmpty(vImp.Value))
    {
        text += vImp.Value;
    }

    // Verb (Perfective)
    if (card.Fields.TryGetValue("Verb (Perfective)", out var vPerf) && !string.IsNullOrEmpty(vPerf.Value))
    {
        text += "," + vPerf.Value;
    }
    
    // Verb (Imperfective/Perfective)
    if (card.Fields.TryGetValue("Verb (Imperfective/Perfective", out var vImpfPerf) && !string.IsNullOrEmpty(vImpfPerf.Value))
    {
        text += vImpfPerf.Value;
    }
    
    var body = new
    {
        model = "gpt-4o-mini-tts",
        voice = request.GetVoiceByEnum(request.VoiceModelOfAudio),
        input = text,
        language =  request.GetLanguage(request.LanguageOfText),
        instructions= $"Audio for the words i sent you in the {request.LanguageOfText} Language"
    };

    try
    {
        var filename = await RequestAndSaveAudioFile(body, text, card.CardId.ToString());
        Console.WriteLine($"Note ID: {card.Note} | Front: {card.Fields["Front"].Value}");

        var audioConcate = "";


        audioConcate += $"[sound:{filename}.mp3]";


        await ankiEditor.UpdateNote(card.Note, card.Fields["Front"].Value,
            card.Fields?["Noun"].Value ?? "",
            card.Fields?["Phrase"].Value ?? "",
            card.Fields?["Verb (Imperfective)"].Value ?? "",
            card.Fields?["Verb (Perfective)"].Value ?? "",
            card.Fields?["Verb (Imperfective/Perfective)"].Value ?? "",
            audioConcate);
    }
    catch (Exception e)
    {
        Console.WriteLine(e.Message);
        Console.WriteLine("ERROR FOR CARD: " + text);
    }

}





// currentKapitelToAddToFileName wird auch weiter oben mit die Id der Karte verwendet
async Task<string> RequestAndSaveAudioFile(object body,string filenameOutput,string currentKapitelToAddToFileName)
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
    
    Console.WriteLine("audio apicall response: "+ response.StatusCode);
    var pathToSave = Path.Combine("C:/","audiostranslated",$"{filenameOutput}_{currentKapitelToAddToFileName}.mp3");
    Console.WriteLine("Path: "+pathToSave);
  
    await File.WriteAllBytesAsync(pathToSave, bytes);
    
    return $"{filenameOutput}_{currentKapitelToAddToFileName}";
}

async Task<TTSRequest> SetupInput()
{
    var request = new TTSRequest();
    
    Console.WriteLine("Please Enter API-Key");
    request.ApiKey = Console.ReadLine() ?? throw new ArgumentNullException(nameof(request.ApiKey));

    Console.WriteLine("Please select a Language.Type a number.");
    Console.WriteLine($"""
                      0 English = {Language.English}
                      1 Ukrainian = {Language.Ukrainian}
                      2 Spanish = {Language.Spanish}
                      3 French = {Language.French}
                      4 German = {Language.German}
                      5 Italian = {Language.Italian}
                      6 Portuguese = {Language.Portuguese}
                      7 Dutch = {Language.Dutch}
                      8 Russian = {Language.Russian}
                      9 Japanese = {Language.Japanese}
                      10 Korean = {Language.Korean}
                      11 Chinese = = {Language.Chinese}
                      """);

    var maxValueOfEnum = Enum.GetValues(typeof(Language)).Cast<Language>().Max();
    var language = Console.ReadLine();
    
    if (!int.TryParse(language, out int languageInt) || languageInt >  (int)maxValueOfEnum)
    {
        throw new ArgumentException("Invalid Language");
    }
    
    request.LanguageOfText = (Language)languageInt;    
    
    Console.WriteLine("Please give the entire Filepath to the file \n example: sometext/folder/file.txt");
    var path = Console.ReadLine() ?? throw new ArgumentNullException("invalid entry");
    
    var lines = File.ReadAllLines(path);
    
    char[] dashCharacters = { '-', '–', '—','-' }; // Standard, En Dash, and Em Dash
    request.TextsToConvertToAudio = lines.Select(line => line.Split(dashCharacters)[0]).ToArray(); 
    return request;
}