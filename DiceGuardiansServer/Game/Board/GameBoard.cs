using DiceGuardiansServer.Collection;
using DiceGuardiansServer.Game.Minions;
using Microsoft.Xna.Framework;

namespace DiceGuardiansServer.Game.Board; 

public class GameBoard {
    private static readonly int WIDTH = 13;
    private static readonly int HEIGHT = 19;

    private readonly Tile[,] _board;
    
    private readonly List<Minion> _player1Minions;
    private readonly List<Minion> _player2Minions;
    
    public GameBoard() {
        _board = new Tile[WIDTH, HEIGHT];
        
        for (int i = 0; i < WIDTH; i++) {
            for (int j = 0; j < HEIGHT; j++) {
                _board[i, j] = new Tile();
            }
        }
        
        _board[6, 0].SetTerrain(Terrain.PLAYER_TILE_1);
        _board[6, 0].SetMinion(new Minion(AllCards.GetDiceGuardian()));
        
        _board[6, 18].SetTerrain(Terrain.PLAYER_TILE_2);
        _board[6, 18].SetMinion(new Minion(AllCards.GetDiceGuardian()));
        
        _player1Minions = new List<Minion> { _board[6, 0].GetMinion()! };
        _player2Minions = new List<Minion> { _board[6, 18].GetMinion()! };
    }
   
    
    public void PlaceTile(Piece p, Vector2 mapPos, long cardId, int currentPlayer) {
        Vector2[] relativeCoords = p.GetRelativeCoords(mapPos);

        foreach (Vector2 coords in relativeCoords) {
            _board[(int) coords.X, (int) coords.Y].SetTerrain(currentPlayer == 1 ? Terrain.PLAYER_TILE_1 : Terrain.PLAYER_TILE_2);
        }
        _board[(int) mapPos.X, (int) mapPos.Y].SetMinion(new Minion(AllCards.GetCard(cardId)));
        if (currentPlayer == 1) {
            _player1Minions.Add(_board[(int) mapPos.X, (int) mapPos.Y].GetMinion()!); 
        }
        else {
            _player2Minions.Add(_board[(int) mapPos.X, (int) mapPos.Y].GetMinion()!); 
        }
    }

    public void MoveMinion(Vector2 start, Vector2 end) {
        Minion? m = _board[(int) start.X, (int) start.Y].GetMinion();
        if (m == null) {
            return;
        }

        _board[(int)end.X, (int)end.Y].SetMinion(m);
        _board[(int) start.X, (int) start.Y].SetMinion(null);
    }
}