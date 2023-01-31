using System.Data;
using System.Data.SQLite;
using DiceGuardiansServer.Database.Models;

namespace DiceGuardiansServer.Database; 

public static class DatabaseManager {
    private const string ConnectionString = "Data Source=DiceGuardians.db";
    private static SQLiteConnection? _con;

    public static void Start() {
        _con = new SQLiteConnection(ConnectionString);
        _con.Open();
        CreateTables();
    }

    private static void CreateTables() {
        CreateUserTable();
        CreateDeckTable();
        CreateCardTable();
        CreateCardsInDeckTable();
        CreateCardIsOwnedByUserTable();
    }

    public static void CreateUser(string userName, string passwordHash) {
        SQLiteCommand cmd = new SQLiteCommand(_con);
        User u = new User(userName, passwordHash);
        
        cmd.CommandText = @"INSERT INTO User(userName, passwordHash, mmr, gamesPlayed, gamesWon) 
                            VALUES(@userName, @passwordHash, @mmr, @gamesPlayed, @gamesWon)";
        cmd.Parameters.AddWithValue("@userName", u.GetUserName());
        cmd.Parameters.AddWithValue("@passwordHash", u.GetPasswordHash());
        cmd.Parameters.AddWithValue("@mmr", u.GetMmr());
        cmd.Parameters.AddWithValue("@gamesPlayed", u.GetGamesPlayed());
        cmd.Parameters.AddWithValue("@gamesWon", u.GetGamesWon());
        cmd.Prepare();
        cmd.ExecuteNonQuery();
    }

    public static User GetUser(string userName) {
        SQLiteCommand cmd = new SQLiteCommand(_con);
        cmd.CommandText = @"SELECT * FROM User WHERE userName=@userName LIMIT 1";
        cmd.Parameters.AddWithValue("@userName", userName);
        SQLiteDataReader reader = cmd.ExecuteReader();

        if (!reader.HasRows) {
            throw new EntryDoesNotExistException($"User with name {userName} does not exist");
        }
        
        reader.Read();

        User u = new User(
            reader.GetInt32(0),
            reader.GetString(1),
            reader.GetString(2),
            reader.GetInt32(3),
            reader.GetInt32(4),
            reader.GetInt32(5)
        );
        
        return u;
    }

    public static User GetUser(long userId) {
        SQLiteCommand cmd = new SQLiteCommand(_con);
        cmd.CommandText = @"SELECT * FROM User WHERE userId=@userId LIMIT 1";
        cmd.Parameters.AddWithValue("@userId", userId);
        SQLiteDataReader reader = cmd.ExecuteReader();

        if (!reader.HasRows) {
            throw new EntryDoesNotExistException($"User with id {userId} does not exist");
        }
        
        reader.Read();

        User u = new User(
            reader.GetInt32(0),
            reader.GetString(1),
            reader.GetString(2),
            reader.GetInt32(3),
            reader.GetInt32(4),
            reader.GetInt32(5)
        );
        
        return u;
    }

    public static void CreateCard(long id, string name, int cost, int attack, int defense, int health, string cardText, string diceFaces) {
        SQLiteCommand cmd = new SQLiteCommand(_con);
        Card c = new Card(id, name, cost, attack, defense, health, cardText, diceFaces);
        
        cmd.CommandText = @"INSERT INTO Card(cardId, name, cost, attack, defense, health, cardText, diceFaces) 
                            VALUES(@cardId, @name, @cost, @attack, @defense, @health, @cardText, @diceFaces)";
        cmd.Parameters.AddWithValue("@cardId", c.GetCardId());
        cmd.Parameters.AddWithValue("@name", c.GetName());
        cmd.Parameters.AddWithValue("@cost", c.GetCost());
        cmd.Parameters.AddWithValue("@attack", c.GetAttack());
        cmd.Parameters.AddWithValue("@defense", c.GetDefense());
        cmd.Parameters.AddWithValue("@health", c.GetHealth());
        cmd.Parameters.AddWithValue("@cardText", c.GetCardText());
        cmd.Parameters.AddWithValue("@diceFaces", c.GetDiceFacesString());
        cmd.Prepare();
        cmd.ExecuteNonQuery();
    }
    
    public static Card GetCard(long cardId) {
        SQLiteCommand cmd = new SQLiteCommand(_con);
        cmd.CommandText = @"SELECT * FROM Card WHERE cardId=@cardId LIMIT 1";
        cmd.Parameters.AddWithValue("@cardId", cardId);
        SQLiteDataReader reader = cmd.ExecuteReader();

        if (!reader.HasRows) {
            throw new EntryDoesNotExistException($"Card with id {cardId} does not exist");
        }
        
        reader.Read();

        Card c = new Card(
            reader.GetInt32(0),
            reader.GetString(1),
            reader.GetInt32(2),
            reader.GetInt32(3),
            reader.GetInt32(4),
            reader.GetInt32(5),
            reader.GetString(6),
            reader.GetString(7)
        );
        
        return c;
    }

    public static List<Card> GetAllCards() {
        SQLiteCommand cmd = new SQLiteCommand(_con);
        cmd.CommandText = @"SELECT * FROM Card";
        SQLiteDataReader reader = cmd.ExecuteReader();

        List<Card> output = new List<Card>();

        while (reader.Read()) {
            output.Add(new Card(
                reader.GetInt32(0),
                reader.GetString(1),
                reader.GetInt32(2),
                reader.GetInt32(3),
                reader.GetInt32(4),
                reader.GetInt32(5),
                reader.GetString(6),
                reader.GetString(7)
            ));
        }

        return output;
    }

    //TODO rework to work with multiple decks
    //currently works with only one deck per user, because the client also only supports one deck
    public static void StoreDeck(long userId, string name, Dictionary<long, int> cardQuantities) {
        int total = 0;
        foreach (KeyValuePair<long, int> kvp in cardQuantities) {
            total += kvp.Value;
        }
        if (total != 15) {
            throw new InvalidDeckSizeException();
        }
        
        SQLiteCommand cmd = new SQLiteCommand(_con);
        cmd.CommandText = @"DELETE FROM CardsInDeck 
                            WHERE deckId = (
                                SELECT deckId 
                                FROM Deck 
                                WHERE userId = @userId
                                LIMIT 1
                            )";
        cmd.Parameters.AddWithValue("@userId", userId);
        cmd.Prepare();
        cmd.ExecuteNonQuery();
        
        cmd.CommandText = @"INSERT INTO Deck(userId, name) 
                            VALUES(@userId, @name);
                            SELECT last_insert_rowid() as last_id;";
        cmd.Parameters.AddWithValue("@userId", userId);
        cmd.Parameters.AddWithValue("@name", name);
        cmd.Prepare();
        long deckId = (long) cmd.ExecuteScalar();
        
        cmd.CommandText = @"INSERT INTO CardsInDeck(deckId, cardId, count) 
                            VALUES(@deckId, @cardId, @count)";
        
        cmd.Parameters.Add("@deckId", DbType.Int64);
        cmd.Parameters.Add("@cardId", DbType.Int64);
        cmd.Parameters.Add("@count", DbType.Int32);
        cmd.Prepare();
        
        foreach (KeyValuePair<long, int> kvp in cardQuantities) {
            cmd.Parameters["@deckId"].Value = deckId;
            cmd.Parameters["@cardId"].Value = kvp.Key;
            cmd.Parameters["@count"].Value = kvp.Value;
            cmd.ExecuteNonQuery();
        }
    }
    
    public static void AddCardToUser(long cardId, long userId, int count) {
        SQLiteCommand cmd = new SQLiteCommand(_con);
        CardIsOwnedByUser c = new CardIsOwnedByUser(cardId, userId, count);

        cmd.CommandText = @"UPDATE CardIsOwnedByUser SET count = @count
                            WHERE cardId = @cardId AND userId = @userId;
                            SELECT changes() as rows;";

        cmd.Parameters.AddWithValue("@cardId", c.GetCardId());
        cmd.Parameters.AddWithValue("@userId", c.GetUserId());
        cmd.Parameters.AddWithValue("@count", c.GetCount());
        cmd.Prepare();

        var rows = cmd.ExecuteScalar();
        if (Convert.ToInt32(rows) != 0) return;
        
        cmd.CommandText = @"INSERT INTO CardIsOwnedByUser(cardId, userId, count)
                            VALUES (@cardId, @userId, @count)";
        cmd.ExecuteNonQuery();
    }

    //TODO rewrite function to give proper data types instead of dict
    public static List<CardIsOwnedByUser> GetUserCollection(long userId) {
        List<CardIsOwnedByUser> collection = new List<CardIsOwnedByUser>();
        
        SQLiteCommand cmd = new SQLiteCommand(_con);
        cmd.CommandText = @"SELECT * FROM CardIsOwnedByUser WHERE userId=@userId";
        cmd.Parameters.AddWithValue("@userId", userId);
        SQLiteDataReader reader = cmd.ExecuteReader();

        while (reader.Read()) {
            collection.Add(new CardIsOwnedByUser(
                reader.GetInt64(0),
                reader.GetInt64(1),
                reader.GetInt32(2)
            ));
        }

        return collection;
    }
    
    public static List<Deck> GetUserDecks(long userId) {
        List<Deck> decks = new List<Deck>();
        
        SQLiteCommand cmd = new SQLiteCommand(_con);
        cmd.CommandText = @"SELECT * FROM Deck WHERE userId=@userId";
        cmd.Parameters.AddWithValue("@userId", userId);
        SQLiteDataReader reader = cmd.ExecuteReader();

        while (reader.Read()) {
            decks.Add(new Deck(
                reader.GetInt64(0),
                reader.GetInt64(1),
                reader.GetString(2)
            ));
        }

        return decks;
    }

    public static List<CardsInDeck> GetDeckList(long deckId) {
        List<CardsInDeck> cards = new List<CardsInDeck>();
        
        SQLiteCommand cmd = new SQLiteCommand(_con);
        cmd.CommandText = @"SELECT * FROM CardsInDeck WHERE deckId=@deckId";
        cmd.Parameters.AddWithValue("@deckId", deckId);
        SQLiteDataReader reader = cmd.ExecuteReader();

        while (reader.Read()) {
            cards.Add(new CardsInDeck(
                reader.GetInt64(0),
                reader.GetInt64(1),
                reader.GetInt32(2)
            ));
        }

        return cards;
    }

    private static void CreateUserTable() {
        SQLiteCommand cmd = new SQLiteCommand(_con);
        
        cmd.CommandText = "DROP TABLE IF EXISTS User";
        cmd.ExecuteNonQuery();
        cmd.CommandText = @"CREATE TABLE User(
                                userId INTEGER PRIMARY KEY,
                                userName TEXT,
                                passwordHash TEXT,
                                mmr INTEGER,
                                gamesPlayed INTEGER,
                                gamesWon INTEGER
                            )";
        cmd.ExecuteNonQuery();
    }

    private static void CreateDeckTable() {
        SQLiteCommand cmd = new SQLiteCommand(_con);
        
        cmd.CommandText = "DROP TABLE IF EXISTS Deck";
        cmd.ExecuteNonQuery();
        cmd.CommandText = @"CREATE TABLE Deck(
                                deckId INTEGER PRIMARY KEY,
                                userId INTEGER,
                                name TEXT,
                                FOREIGN KEY (userId) REFERENCES User(userId)
                            )";
        cmd.ExecuteNonQuery();
    }

    private static void CreateCardTable() {
        SQLiteCommand cmd = new SQLiteCommand(_con);
        
        cmd.CommandText = "DROP TABLE IF EXISTS Card";
        cmd.ExecuteNonQuery();
        cmd.CommandText = @"CREATE TABLE Card(
                                cardId INTEGER PRIMARY KEY,
                                name TEXT,
                                cost INTEGER,
                                attack INTEGER,
                                defense INTEGER,
                                health INTEGER,
                                cardText TEXT,
                                diceFaces TEXT
                            )";
        cmd.ExecuteNonQuery();
    }
    
    private static void CreateCardsInDeckTable() {
        SQLiteCommand cmd = new SQLiteCommand(_con);
        
        cmd.CommandText = "DROP TABLE IF EXISTS CardsInDeck";
        cmd.ExecuteNonQuery();
        cmd.CommandText = @"CREATE TABLE CardsInDeck(
                                deckId INTEGER,
                                cardId INTEGER,
                                count INTEGER,
                                PRIMARY KEY (deckId, cardId),
                                FOREIGN KEY (deckId) REFERENCES Deck(deckId),
                                FOREIGN KEY (cardId) REFERENCES Card(cardId)
                            )";
        cmd.ExecuteNonQuery();
    }
    
    private static void CreateCardIsOwnedByUserTable() {
        SQLiteCommand cmd = new SQLiteCommand(_con);
        
        cmd.CommandText = "DROP TABLE IF EXISTS CardIsOwnedByUser";
        cmd.ExecuteNonQuery();
        cmd.CommandText = @"CREATE TABLE CardIsOwnedByUser(
                                cardId INTEGER,
                                userId INTEGER,
                                count INTEGER,
                                PRIMARY KEY (cardId, userId),
                                FOREIGN KEY (cardId) REFERENCES Card(cardId),
                                FOREIGN KEY (userId) REFERENCES User(userId)
                            )";
        cmd.ExecuteNonQuery();
    }
}