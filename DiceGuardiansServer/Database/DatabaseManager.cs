using System.Data.SQLite;
using DiceGuardiansServer.Database.Models;

namespace DiceGuardiansServer.Database; 

public static class DatabaseManager {
    private const string ConnectionString = "Data Source=DiceGuardians.db";
    private static SQLiteConnection _con;

    public static void Start() {
        _con = new SQLiteConnection(ConnectionString);
        _con.Open();
        CreateTables();
    }

    private static void CreateTables() {
        SQLiteCommand cmd = new SQLiteCommand(_con);
        
        cmd.CommandText = "DROP TABLE IF EXISTS User";
        cmd.ExecuteNonQuery();
        cmd.CommandText = @"CREATE TABLE User(
                                userId INTEGER PRIMARY KEY,
                                userName TEXT,
                                passwordHash TEXT,
                                mmr INTEGER,
                                gamesPlayed INTEGER,
                                gamesWon INTEGER)";
        cmd.ExecuteNonQuery();
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
}