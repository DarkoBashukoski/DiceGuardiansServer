using DiceGuardiansServer.Game.Player;

namespace DiceGuardiansServer.Game.Tasks; 

public class EndTurnTask : PlayerTask {
    public EndTurnTask(GameInstance game, HumanPlayer player) {
        _game = game;
        _controller = player;
    }
    
    public override bool Process() {
        //TODO
        return false;
    }
}