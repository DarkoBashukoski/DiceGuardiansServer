using System.Text;
using DiceGuardiansServer.Database.Models;
using DiceGuardiansServer.Database.SubModels;

namespace DiceGuardiansServer.Game.Player; 

public class CrestPool {
    private readonly Dictionary<Crest, int> _pool;

    public CrestPool() {
        _pool = new Dictionary<Crest, int> {
            [Crest.SUMMON] = 0,
            [Crest.ATTACK] = 0,
            [Crest.MOVEMENT] = 0,
            [Crest.DEFENSE] = 0,
            [Crest.MAGIC] = 0,
            [Crest.TRAP] = 0
        };
    }

    public void SpendCrest(Crest type, int count) {
        _pool[type] -= count;
    }

    public string roll(Card card) {
        DiceFace rolledFace = card.GetDiceFaces()[Random.Shared.Next(0, 6)];
        _pool[rolledFace.GetCrest()] += rolledFace.GetCount();
        return rolledFace.ToString();
    }

    public override string ToString() {
        StringBuilder sb = new StringBuilder();
        sb.Append("Summon: ").Append(_pool[Crest.SUMMON]).Append('\n');
        sb.Append("Attack: ").Append(_pool[Crest.ATTACK]).Append('\n');
        sb.Append("Movement: ").Append(_pool[Crest.MOVEMENT]).Append('\n');
        sb.Append("Defense: ").Append(_pool[Crest.DEFENSE]).Append('\n');
        sb.Append("Magic: ").Append(_pool[Crest.MAGIC]).Append('\n');
        sb.Append("Trap: ").Append(_pool[Crest.TRAP]).Append('\n');

        return sb.ToString();
    }
}