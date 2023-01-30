using DiceGuardiansServer.Database.SubModels;

namespace DiceGuardiansServer.Database.Models; 

public class Card {
    private long _cardId;
    private string _name;
    private int _attack;
    private int _defense;
    private int _health;
    private string _cardText;
    private DiceFace[] _diceFaces;

    public Card(long cardId, string name, int attack, int defense, int health, string cardText, string diceFaces) {
        _cardId = cardId;
        _name = name;
        _attack = attack;
        _defense = defense;
        _health = health;
        _cardText = cardText;
        _diceFaces = ParseDiceFacesString(diceFaces);
    }

    private DiceFace[] ParseDiceFacesString(string diceFaces) {
        string[] faces = diceFaces.Split(" ");
        DiceFace[] output = new DiceFace[6];

        for (int i = 0; i < 6; i++) {
            string[] f = faces[i].Split("x");
            output[i] = new DiceFace(DiceFace.getTypeFromLetter(f[1]), Convert.ToInt32(f[0]));
        }

        return output;
    }

    public string GetDiceFacesString() {
        return String.Join(" ", _diceFaces.Select(x => x.ToString()));
    }

    public long GetCardId() {return _cardId;}
    public string GetName() {return _name;}
    public int GetAttack() {return _attack;}
    public int GetDefense() {return _defense;}
    public int GetHealth() {return _health;}
    public string GetCardText() {return _cardText;}
    public DiceFace[] GetDiceFaces() {return _diceFaces;}
}