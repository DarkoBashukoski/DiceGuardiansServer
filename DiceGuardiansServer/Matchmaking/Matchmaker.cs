using DiceGuardiansServer.Game;
using DiceGuardiansServer.Game.Entities;
using DiceGuardiansServer.Networking;
using Riptide;

namespace DiceGuardiansServer.Matchmaking; 

public static class Matchmaker {
    private static HashSet<Player> _players;
    private static Dictionary<Player, GameInstance> _gameInstances;

    public static void Start() {
        _players = new HashSet<Player>();
        _gameInstances = new Dictionary<Player, GameInstance>();
    }

    public static void AddPlayer(Player player) {
        _players.Add(player);
    }

    public static void RemovePlayer(Player player) {
        if (_gameInstances.ContainsKey(player)) {
            Player player1 = _gameInstances[player].GetPlayer1();
            Player player2 = _gameInstances[player].GetPlayer2();
            _gameInstances.Remove(player1);
            _gameInstances.Remove(player2);
            //TODO disconnected player handling
        }
        else {
            _players.Remove(player);
        }
    }

    public static void CreateMatches() {
        while (_players.Count > 1) {
            Player player1 = _players.First();
            _players.Remove(player1);
            Player player2 = _players.First();
            _players.Remove(player2);
            GameInstance game = new GameInstance(player1, player2);
            Console.WriteLine(player1.GetId());
            Console.WriteLine(player2.GetId());
            _gameInstances.Add(player1, game);
            _gameInstances.Add(player2, game);
        }
    }

    //[MessageHandler((ushort)ServerToClientId.PlaceTile)]
    //private static void placeTile(ushort uid, Message m) {
    //    GameInstance g = _gameInstances[_gameInstances.Keys.First()];
    //    foreach (Player p in _gameInstances.Keys) {
    //        if (p.GetId() == uid) {
    //            g = _gameInstances[p];
    //            break;
    //        }
    //    }
    //    int [,] _board = new int[13, 19];
    //    for (int i = 0; i < 13; i++) {
    //        for (int j = 0; j < 19; j++) {
    //            _board[i, j] = m.GetInt();
    //        }
    //    }
    //    g.SetBoard(_board);
    //    NetworkManager.GetServer().Send(m, g.GetPlayer1().GetId());
    //    NetworkManager.GetServer().Send(m, g.GetPlayer2().GetId());
    //}
}