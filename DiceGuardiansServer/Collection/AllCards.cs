using DiceGuardiansServer.Database;
using DiceGuardiansServer.Database.Models;

namespace DiceGuardiansServer.Collection; 

public static class AllCards {
    private static Dictionary<long, Card> _allCards = null!;
    private static Card? _diceGuardian;

    public static void InitializeCardInfo() {
        _allCards = new Dictionary<long, Card>();
        
        foreach (Card c in DatabaseManager.GetAllCards()) {
            _allCards[c.GetCardId()] = c;
        }
        _diceGuardian = new Card(9999, "Dice Guardian", 0, 1, 0, 30, "", "1xs 1xs 1xs 1xs 1xs 1xs");

    }

    public static Card GetCard(long cardId) {
        return (cardId == 9999 ? _diceGuardian : _allCards[cardId])!;
    }

    public static int Count() {
        return _allCards.Count;
    }

    public static Card[] SortedById() {
        return _allCards.Values.OrderBy(x => x.GetCardId()).ToArray();
    }
    
    public static Card GetDiceGuardian() {
        return _diceGuardian!;
    }
}