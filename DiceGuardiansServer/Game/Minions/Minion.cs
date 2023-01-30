using DiceGuardiansServer.Database.Models;

namespace DiceGuardiansServer.Game.Minions; 

public class Minion { //TODO
    private long _cardId;
    private string _name;
    private int _maxHealth;
    private int _currentHealth;
    private int _attack;
    private int _defense;

    public Minion(Card card) {
        _cardId = card.GetCardId();
        _attack = card.GetAttack();
        _maxHealth = card.GetHealth();
        _currentHealth = card.GetHealth();
        _defense = card.GetDefense();
    }
}