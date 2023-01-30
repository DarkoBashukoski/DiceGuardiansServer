using DiceGuardiansServer.Database;
using DiceGuardiansServer.Database.Models;

namespace DiceGuardiansServer.Collection; 

public static class AllCards {
    private static Dictionary<long, Card> _allCards;

    public static void InitializeCardInfo() {
        _allCards = new Dictionary<long, Card>();
        
        foreach (Card c in DatabaseManager.GetAllCards()) {
            _allCards[c.GetCardId()] = c;
        }
    }

    public static Card GetCard(long cardId) {
        return _allCards[cardId];
    }

    public static int Count() {
        return _allCards.Count;
    }

    public static Card[] SortedById() {
        return _allCards.Values.OrderBy(x => x.GetCardId()).ToArray();
    }
}