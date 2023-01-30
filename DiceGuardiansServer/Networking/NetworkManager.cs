using Riptide;

namespace DiceGuardiansServer.Networking; 

public static class NetworkManager {
    private static Server _server = null!;

    public static void Start() {
        _server = new Server();
        _server.Start(7777, 1000);
    }

    public static void Update() {
        _server.Update();
    }

    public static Server GetServer() {
        return _server;
    }
}