namespace DiceGuardiansServer.Game.Board; 

public class GameBoard {
    private static readonly int WIDTH = 13;
    private static readonly int HEIGHT = 19;

    private Tile[,] _board;
    
    public GameBoard() {
        _board = new Tile[WIDTH, HEIGHT];
        
        for (int i = 0; i < WIDTH; i++) {
            for (int j = 0; j < HEIGHT; j++) {
                _board[i, j] = new Tile();
            }
        }
        
        _board[6, 0].SetTerrain(Terrain.PLAYER_TILE_1);
        _board[6, 18].SetTerrain(Terrain.PLAYER_TILE_2);
    }
}