using DiceGuardiansServer.Database.Models;

namespace DiceGuardiansServer.Game.Entities; 

public class Player {
    private ushort _id;
    private User _user;

    public Player(ushort id, User user) {
        _id = id;
        _user = user;
    }

    public ushort GetId() {
        return _id;
    }

    public User GetUser() {
        return _user;
    }
}