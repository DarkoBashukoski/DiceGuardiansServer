namespace DiceGuardiansServer.Database.Models; 

public class CardIsOwnedByUser {
    private long _cardId;
    private long _userId;
    private int _count;

    public CardIsOwnedByUser(long cardId, long userId, int count) {
        _cardId = cardId;
        _userId = userId;
        _count = count;
    }

    public long GetCardId() {return _cardId;}
    public long GetUserId() {return _userId;}
    public int GetCount() {return _count;}
}