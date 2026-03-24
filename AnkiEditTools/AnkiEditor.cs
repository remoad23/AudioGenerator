namespace AnkiEditTools;

public class AnkiEditor
{
    public async Task<List<CardInfo>> GetDeckData(string deckName)
    {
        var client = new AnkiClient();

        // 1. Find all card IDs in the deck and any subdecks (using the * wildcard)
        var cardIds = await client.SendAsync<List<long>>("findCards", new { 
            query = $"deck:\"{deckName}*\"" 
        });

        // 2. Get the detailed info for those cards
        var cardsInfo = await client.SendAsync<List<CardInfo>>("cardsInfo", new { 
            cards = cardIds 
        });


        return cardsInfo;
    }
    
    public async Task UpdateNote(long noteId, string front, string noun, string phrase,
        string verbimperfective, 
        string verbperfective, 
        string verbimperfectiveperfective, 
        string audioFile)
    {
        var client = new AnkiClient();

        var fieldsToUpdate = new Dictionary<string, string>
        {
            { "Front", front },
            { "Noun", noun },
            { "Phrase", phrase },
            { "Verb (Imperfective)", verbimperfective },
            { "Verb (Perfective)", verbperfective },
            { "Verb (Imperfective/Perfective)", verbimperfectiveperfective },
            { "Example Sentence", "" },
            { "Audio", audioFile }
        };
        
        var updateParams = new
        {
            note = new
            {
                id = noteId,
                fields = fieldsToUpdate
            }
        };

        var response = await client.SendAsync<object>("updateNoteFields", updateParams);
        Console.WriteLine("Note updated successfully!");
    }
}