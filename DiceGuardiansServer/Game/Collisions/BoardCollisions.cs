using Vector2 = Microsoft.Xna.Framework.Vector2;

namespace DiceGuardiansServer.Game.Collisions; 

public class BoardCollisions {
    public static bool IsPlacementPossible(int[,] tileMap, int[,] pathPiece, Vector2 mapPos, Vector2 pieceCenter, int playerIndex) {
        Vector2 mapLimits = new Vector2(tileMap.GetLength(0) - 1, tileMap.GetLength(1) - 1);
        Vector2 pieceLimits = new Vector2(pathPiece.GetLength(0) - 1, pathPiece.GetLength(1) - 1);
        Vector2 topLeft = Vector2.Subtract(mapPos, pieceCenter);
        Vector2 bottomRight = Vector2.Add(topLeft, pieceLimits);

        bool connected = false;
        if (topLeft.X < 0 || topLeft.Y < 0 || bottomRight.X > mapLimits.X || bottomRight.Y > mapLimits.Y) {return false;}

        for (int i = 0; i < pathPiece.GetLength(0); i++) {
            for (int j = 0; j < pathPiece.GetLength(1); j++) {
                if (pathPiece[i, j] == 0) {continue;}
                Vector2 mapCoords = Vector2.Add(topLeft, new Vector2(i, j));
                if (tileMap[(int) mapCoords.X, (int) mapCoords.Y] != 0) {return false;}
                
                if (connected) {continue;}
                try {
                    if (tileMap[(int)mapCoords.X + 1, (int)mapCoords.Y] == playerIndex) {connected = true; continue;}
                    if (tileMap[(int)mapCoords.X - 1, (int)mapCoords.Y] == playerIndex) {connected = true; continue;}
                    if (tileMap[(int)mapCoords.X, (int)mapCoords.Y + 1] == playerIndex) {connected = true; continue;}
                    if (tileMap[(int)mapCoords.X, (int)mapCoords.Y - 1] == playerIndex) {connected = true;}
                }
                catch (IndexOutOfRangeException) {}
            }
        }
        return connected;
    }
}