namespace DiceGuardiansServer.Database; 

public static class DatabaseInitializer {
    public static void init() {
        DatabaseManager.CreateUser("Jako Bate", "123");

        for (int i = 0; i < 30; i++) {
            DatabaseManager.CreateCard(i, $"test-card-{i}", 4, 3, 2, "testString", "1xs 2xr 1xa 1xt 1xd 1xm");
        }
        
        DatabaseManager.AddCardToUser(3, 1, 2);
        DatabaseManager.AddCardToUser(5, 1, 3);
        DatabaseManager.AddCardToUser(8, 1, 1);
        DatabaseManager.AddCardToUser(15, 1, 2);
        DatabaseManager.AddCardToUser(1, 1, 1);
        DatabaseManager.AddCardToUser(20, 1, 3);
        DatabaseManager.AddCardToUser(7, 1, 2);
        DatabaseManager.AddCardToUser(27, 1, 1);

        Dictionary<long, int> deck = new Dictionary<long, int> {
            [3] = 2,
            [5] = 3,
            [8] = 1,
            [15] = 2,
            [1] = 1,
            [20] = 3,
            [7] = 2,
            [27] = 1
        };
        
        DatabaseManager.StoreDeck(1, "testDeck", deck);
    }
}