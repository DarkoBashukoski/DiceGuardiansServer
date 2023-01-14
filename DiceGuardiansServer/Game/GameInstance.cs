using DiceGuardiansServer.Game.Collisions;
using DiceGuardiansServer.Game.Entities;
using DiceGuardiansServer.Networking;
using Microsoft.Xna.Framework;
using Riptide;

namespace DiceGuardiansServer.Game;

public class GameInstance {
    private int[,] _board;

    private readonly Player _player1;
    private readonly Player _player2;

    public GameInstance(Player player1, Player player2) {
        _player1 = player1;
        _player2 = player2;

        _board = new int[13, 19];
        _board[6, 0] = 2;
        _board[6, 18] = 1;

        //Message m = Message.Create(MessageSendMode.Reliable, ServerToClientId.CreateMatch);
        //for (int i = 0; i < _board.GetLength(0); i++) {
        //    for (int j = 0; j < _board.GetLength(1); j++) {
        //        m.AddInt(_board[i, j]);
        //    }
        //}

        //NetworkManager.GetServer().Send(m, player1.GetId());
        //NetworkManager.GetServer().Send(m, player2.GetId());
    }

    public bool PlaceTile(int[,] pathPiece, Vector2 pieceCenter, Vector2 mapPos, int playerIndex) {
        if (!BoardCollisions.IsPlacementPossible(_board, pathPiece, mapPos, pieceCenter, playerIndex)) {
            return false;
        }

        Vector2 topLeft = Vector2.Subtract(mapPos, pieceCenter);
        for (int i = 0; i < pathPiece.GetLength(0); i++) {
            for (int j = 0; j < pathPiece.GetLength(1); j++) {
                if (pathPiece[i, j] == 0) {
                    continue;
                }

                Vector2 mapCoords = Vector2.Add(topLeft, new Vector2(i, j));
                _board[(int)mapCoords.X, (int)mapCoords.Y] = playerIndex;
            }
        }
        return true;
    }

    public Player GetPlayer1() {
        return _player1;
    }

    public Player GetPlayer2() {
        return _player2;
    }

    public void SetBoard(int[,] board) {
        _board = board;
    }
}