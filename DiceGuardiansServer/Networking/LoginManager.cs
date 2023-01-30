using DiceGuardiansServer.Database;
using DiceGuardiansServer.Database.Models;
using Riptide;

namespace DiceGuardiansServer.Networking; 

public class LoginManager {
    private static Dictionary<long, long> _currentUsers = new();

    public static Dictionary<long, long> GetCurrentUsers() {
        return _currentUsers;
    }

    [MessageHandler((ushort) ClientToServerId.Login)]
    public static void LoginUser(ushort uid, Message message) {
        string userName = message.GetString();
        string password = message.GetString();
        User u;
        Message reply;
        
        try {
            u = DatabaseManager.GetUser(userName);
        }
        catch (EntryDoesNotExistException) {
            reply = Message.Create(MessageSendMode.Reliable, ServerToClientId.InvalidLoginUserDoesNotExist);
            NetworkManager.GetServer().Send(reply, uid);
            return;
        }
        
        if (u.GetPasswordHash() != password) {
            reply = Message.Create(MessageSendMode.Reliable, ServerToClientId.InvalidLoginWrongPassword);
            NetworkManager.GetServer().Send(reply, uid);
            return;
        }

        _currentUsers[uid] = u.GetUserId();
        reply = Message.Create(MessageSendMode.Reliable, ServerToClientId.SuccessfulLogin);
        reply.AddLong(u.GetUserId());
        reply.AddString(u.GetUserName());
        reply.AddInt(u.GetMmr());
        reply.AddInt(u.GetGamesPlayed());
        reply.AddInt(u.GetGamesWon());
        NetworkManager.GetServer().Send(reply, uid);
    }
    
    [MessageHandler((ushort) ClientToServerId.Register)]
    public static void RegisterUser(ushort uid, Message message) {
        string userName = message.GetString();
        string password = message.GetString();
        Message reply;
        
        try {
            DatabaseManager.GetUser(userName);
            reply = Message.Create(MessageSendMode.Reliable, ServerToClientId.InvalidRegisterUsernameAlreadyTaken);
            NetworkManager.GetServer().Send(reply, uid);
        } catch (EntryDoesNotExistException) {
            DatabaseManager.CreateUser(userName, password);
            User u = DatabaseManager.GetUser(userName);

            GiveStarterCollection(u.GetUserId());
            
            reply = Message.Create(MessageSendMode.Reliable, ServerToClientId.SuccessfulRegister);
            reply.AddLong(u.GetUserId());
            reply.AddString(u.GetUserName());
            reply.AddInt(u.GetMmr());
            reply.AddInt(u.GetGamesPlayed());
            reply.AddInt(u.GetGamesWon());
            NetworkManager.GetServer().Send(reply, uid);
        }
    }

    private static void GiveStarterCollection(long userId) {
        Dictionary<long, int> deck = new Dictionary<long, int> {
            [3] = 2, [5] = 3, [8] = 1, [15] = 2,
            [1] = 1, [20] = 3, [7] = 2, [27] = 1
        };

        foreach (KeyValuePair<long, int> kvp in deck) {
            DatabaseManager.AddCardToUser(kvp.Key, userId, kvp.Value);
        }

        DatabaseManager.StoreDeck(userId, "testDeck", deck);
    }
}