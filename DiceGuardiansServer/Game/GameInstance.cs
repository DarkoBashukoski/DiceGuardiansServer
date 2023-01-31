using DiceGuardiansServer.Collection;
using DiceGuardiansServer.Database.SubModels;
using DiceGuardiansServer.Game.Board;
using DiceGuardiansServer.Game.GameEvents;
using DiceGuardiansServer.Game.Player;
using DiceGuardiansServer.Networking;
using Microsoft.Xna.Framework;
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
        // if (Random.Shared.Next(0, 2) == 0) {
        //     _player1 = player1;
        //     _player2 = player2;
        // } else {
        //     _player1 = player2;
        //     _player2 = player1;
        // }

        _player1 = player2; //TODO remove
        _player2 = player1;

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
    
    public void TriggerEndTurn(ushort netId, Message m) {
        if (_currentPlayer.GetNetworkId() != netId) {
            return;
        }
        
        GameEventManager.NextStepEvent(this, Step.END);
    }

    public void TriggerPlaceTile(ushort netId, Message m) {
        if (_currentPlayer.GetNetworkId() != netId) {
            return;
        }

        int piece = m.GetInt();
        int rotation = m.GetInt();

        Piece p = AllPieces.GetPiece(piece, rotation);
        Vector2 mapPosition = new Vector2(m.GetInt(), m.GetInt());
        long cardId = m.GetLong();
        
        _board.PlaceTile(p, mapPosition, cardId, _currentPlayer == _player1 ? 1 : 2);
        _currentPlayer.GetCrestPool().SpendCrest(Crest.SUMMON, AllCards.GetCard(cardId).GetCost());
        
        Message reply = Message.Create(MessageSendMode.Reliable, ServerToClientId.PlaceTile);
        reply.AddInt(piece);
        reply.AddInt(rotation);
        reply.AddInt((int) mapPosition.X);
        reply.AddInt((int) mapPosition.Y);
        reply.AddLong(cardId);
        _currentPlayer.GetDeckManager().RemoveCard(cardId);
        
        NetworkManager.GetServer().Send(reply, _player1.GetNetworkId());
        NetworkManager.GetServer().Send(reply, _player2.GetNetworkId());
    }
    
    public void TriggerMoveMinion(ushort netId, Message m) {
        if (_currentPlayer.GetNetworkId() != netId) {
            return;
        }

        Vector2 start = new Vector2(m.GetInt(), m.GetInt());
        Vector2 end = new Vector2(m.GetInt(), m.GetInt());
        int cost = m.GetInt();

        _board.MoveMinion(start, end);
        _currentPlayer.GetCrestPool().SpendCrest(Crest.MOVEMENT, cost);
        
        Message reply = Message.Create(MessageSendMode.Reliable, ServerToClientId.MoveMinion);
        reply.AddInt((int)start.X);
        reply.AddInt((int) start.Y);
        reply.AddInt((int) end.X);
        reply.AddInt((int) end.Y);
        reply.AddInt(cost);
        NetworkManager.GetServer().Send(reply, _player1.GetNetworkId());
        NetworkManager.GetServer().Send(reply, _player2.GetNetworkId());
    }

    #endregion

    #region Getters and Setters

    public HumanPlayer GetPlayer1() {
        return _player1;
    }

    public HumanPlayer GetPlayer2() {
        return _player2;
    }

    public void SetStep(Step s) {
        _step = s;
    }

    #endregion
}