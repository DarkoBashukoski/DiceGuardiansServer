using DiceGuardiansServer.Game.Minions;
using DiceGuardiansServer.Game.Player;

namespace DiceGuardiansServer.Game.Tasks; 

public enum PlayerTaskType {
    CHOOSE_DICE, CONCEDE, END_TURN, MINION_MOVE, MINION_ATTACK, MINION_SUMMON
}

public abstract class PlayerTask {
    protected Minion? _source;
    protected Minion? _target;
    protected GameInstance _game;
    protected HumanPlayer _controller;

    protected bool HasSource() {
        return _source != null;
    }

    protected bool HasTarget() {
        return _target != null;
    }

    public abstract bool Process();
}