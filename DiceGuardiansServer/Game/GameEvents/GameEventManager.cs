namespace DiceGuardiansServer.Game.GameEvents; 

public static class GameEventManager {
    public static void NextStepEvent(GameInstance game, Step step) {
        switch (step) {
            case Step.STANDBY:
                game.SetStep(step);
                game.Standby();
                break;
            
            case Step.SELECT_DICE:
                game.SetStep(step);
                game.SelectDice();
                break;
            
            case Step.MAIN:
                game.SetStep(step);
                game.Main();
                break;
            
            case Step.END:
                game.SetStep(step);
                game.End();
                break;
        }
    }
}