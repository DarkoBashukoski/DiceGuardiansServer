using DiceGuardiansServer.Collection;
using DiceGuardiansServer.Database;
using DiceGuardiansServer.Database.Models;

namespace DiceGuardiansServer.Game.Player; 

public class DeckManager {
    private readonly List<Card> _deck;
    private readonly List<Card> _graveyard;
    
    public DeckManager(long userId) {
        _deck = new List<Card>();
        _graveyard = new List<Card>();
        
        foreach (CardsInDeck cid in DatabaseManager.GetDeckList(userId)) {
            for (int i = 0; i < cid.GetCount(); i++) {
                _deck.Add(AllCards.GetCard(cid.GetCardId()));
            }
        }
    }

    public void MoveToGraveyard(int index) {
        Card c = _deck[index];
        _deck.RemoveAt(index);
        _graveyard.Add(c);
    }

    public Card GetCardAtIndex(int index) {
        return _deck[index];
    }

    public int GetDeckSize() {
        return _deck.Count;
    }

    public int GetGraveyardSize() {
        return _graveyard.Count;
    }

    public List<Card> GetDeck() {
        return _deck;
    }

    public void RemoveCard(long cardId) {
        foreach (var c in _deck.Where(c => c.GetCardId() == cardId)) {
            _graveyard.Add(c);
            _deck.Remove(c);
            break;
        }
    }
}