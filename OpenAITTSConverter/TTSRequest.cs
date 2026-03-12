namespace OpenAITTSConverter;

public class TTSRequest
{
    public Language LanguageOfText { get; set; }
    public Voice VoiceModelOfAudio { get; set; }
    public string[] TextsToConvertToAudio { get; set; }
    public string ApiKey { get; set; }

    public string GetVoiceByEnum(Voice voice)
    {
        return voice switch
        {
            Voice.Alloy => "alloy",
            Voice.Ash => "ash",
            Voice.Ballad => "ballad",
            Voice.Coral => "coral",
            Voice.Echo => "echo",
            Voice.Fable => "fable",
            Voice.Nova => "nova",
            Voice.Onyx => "onyx",
            Voice.Sage => "sage",
            Voice.Shimmer => "shimmer",
            Voice.Verse => "verse",
            _ => "alloy"
        };
    }
    
    public string GetLanguage(Language language)
    {
        return language switch
        {
            Language.English   => "en",
            Language.Ukrainian => "uk",
            Language.Spanish   => "es",
            Language.French    => "fr",
            Language.German    => "de",
            Language.Italian   => "it",
            Language.Portuguese=> "pt",
            Language.Dutch     => "nl",
            Language.Russian   => "ru",
            Language.Japanese  => "ja",
            Language.Korean    => "ko",
            Language.Chinese   => "zh",
            _ => "en" 
        };
    }
}