using DiceGuardiansServer.Database;
using DiceGuardiansServer.Database.Models;
using Riptide;

namespace DiceGuardiansServer.Networking; 

public class LoginManager {
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
        
        reply = Message.Create(MessageSendMode.Reliable, ServerToClientId.SuccessfulLogin);
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
            
            reply = Message.Create(MessageSendMode.Reliable, ServerToClientId.SuccessfulRegister);
            reply.AddString(u.GetUserName());
            reply.AddInt(u.GetMmr());
            reply.AddInt(u.GetGamesPlayed());
            reply.AddInt(u.GetGamesWon());
            NetworkManager.GetServer().Send(reply, uid);
        }
    }
}