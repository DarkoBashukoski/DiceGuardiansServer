using DiceGuardiansServer.Database;
using DiceGuardiansServer.Database.Models;
using DiceGuardiansServer.Matchmaking;
using Riptide;

namespace DiceGuardiansServer.Networking; 

public static class NetworkManager {
    private static Server _server = null!;

    public static void Start() {
        _server = new Server();
        _server.Start(7777, 10);
        Matchmaker.Start();
    }

    public static void Update() {
        _server.Update();
        Matchmaker.CreateMatches();
    }

    public static Server GetServer() {
        return _server;
    }
}