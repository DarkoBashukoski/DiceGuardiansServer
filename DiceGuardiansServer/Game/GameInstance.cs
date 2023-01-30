using DiceGuardiansServer.Collection;
using DiceGuardiansServer.Game.Board;
using DiceGuardiansServer.Game.GameEvents;
using DiceGuardiansServer.Game.Player;
using DiceGuardiansServer.Networking;
using Riptide;

namespace DiceGuardiansServer.Game;

public class GameInstance { //TODO will rewrite whole class by the end
    private Step _step;
    
    private GameBoard _board;

    private readonly HumanPlayer _player1;
    private readonly HumanPlayer _player2;
    private HumanPlayer _currentPlayer;

    private HumanPlayer GetOtherPlayer() {
        return _player1 == _currentPlayer ? _player2 : _player1;
    }

    public GameInstance(HumanPlayer player1, HumanPlayer player2) {
        if (Random.Shared.Next(0, 2) == 0) {
            _player1 = player1;
            _player2 = player2;
        } else {
            _player1 = player2;
            _player2 = player1;
        }

        _currentPlayer = _player1;

        _board = new GameBoard();
    }

    #region StateMachine

    public void Standby() {
        Message m1 = Message.Create(MessageSendMode.Reliable, ServerToClientId.BeginStandby);
        Message m2 = Message.Create(MessageSendMode.Reliable, ServerToClientId.BeginStandby);

        m1.AddBool(_player1 == _currentPlayer);
        m2.AddBool(_player2 == _currentPlayer);
        
        NetworkManager.GetServer().Send(m1, _player1.GetNetworkId());
        NetworkManager.GetServer().Send(m2, _player2.GetNetworkId());
        
        Console.WriteLine("Standby Phase");

        GameEventManager.NextStepEvent(this, Step.SELECT_DICE);
    }

    public void SelectDice() {
        Message m1 = Message.Create(MessageSendMode.Reliable, ServerToClientId.BeginDiceSelect);
        NetworkManager.GetServer().Send(m1, _player1.GetNetworkId());
        NetworkManager.GetServer().Send(m1, _player2.GetNetworkId());
        Console.WriteLine("Dice Select Phase");
    }

    public void Main() {
        Message m1 = Message.Create(MessageSendMode.Reliable, ServerToClientId.BeginMain);
        NetworkManager.GetServer().Send(m1, _player1.GetNetworkId());
        NetworkManager.GetServer().Send(m1, _player2.GetNetworkId());
        Console.WriteLine("Main Phase");
        GameEventManager.NextStepEvent(this, Step.END);
    }

    public void End() {
        Message m1 = Message.Create(MessageSendMode.Reliable, ServerToClientId.BeginEnd);
        NetworkManager.GetServer().Send(m1, _player1.GetNetworkId());
        NetworkManager.GetServer().Send(m1, _player2.GetNetworkId());

        _currentPlayer = GetOtherPlayer();
        Console.WriteLine("End Phase");
        GameEventManager.NextStepEvent(this, Step.STANDBY);
    }

    #endregion

    #region PlayerActions

    public void TriggerDiceRoll(ushort netId, Message m) {
        if (_currentPlayer.GetNetworkId() != netId) {
            return;
        }
        Message replyPlayer = Message.Create(MessageSendMode.Reliable, ServerToClientId.DiceRollResult);
        Message replyOpponent = Message.Create(MessageSendMode.Reliable, ServerToClientId.DiceRollResult);

        replyPlayer.AddBool(true);
        replyOpponent.AddBool(false);
        
        for (int i = 0; i < 3; i++) {
            string outcome = _currentPlayer.GetCrestPool().roll(AllCards.GetCard(m.GetLong()));
            replyPlayer.AddString(outcome);
            replyOpponent.AddString(outcome);
        }
        
        NetworkManager.GetServer().Send(replyPlayer, _currentPlayer.GetNetworkId());
        NetworkManager.GetServer().Send(replyOpponent, GetOtherPlayer().GetNetworkId());
        
        GameEventManager.NextStepEvent(this, Step.MAIN);
    }

    #endregion

    public HumanPlayer GetPlayer1() {
        return _player1;
    }

    public HumanPlayer GetPlayer2() {
        return _player2;
    }

    public void SetStep(Step s) {
        _step = s;
    }
}