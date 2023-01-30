namespace DiceGuardiansServer.Database.Models; 

public class Deck {
    private long _deckId;
    private long _userId;
    private string _name;

    public Deck(long deckId, long userId, string name) {
        _deckId = deckId;
        _userId = userId;
        _name = name;
    }
    
    public long GetDeckId() {return _deckId;}
    public long GetUserId() {return _userId;}
    public string GetName() {return _name;}
}