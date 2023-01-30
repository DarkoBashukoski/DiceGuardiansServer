namespace DiceGuardiansServer.Database.Models; 

public class CardsInDeck {
    private long _deckId;
    private long _cardId;
    private int _count;

    public CardsInDeck(long deckId, long cardId, int count) {
        _deckId = deckId;
        _cardId = cardId;
        _count = count;
    }

    public long GetDeckId() {return _deckId;}
    public long GetCardId() {return _cardId;}
    public int GetCount() {return _count;}
}