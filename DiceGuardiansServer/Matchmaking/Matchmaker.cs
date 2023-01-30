using DiceGuardiansServer.Database;
using DiceGuardiansServer.Database.Models;
using DiceGuardiansServer.Game;
using DiceGuardiansServer.Game.GameEvents;
using DiceGuardiansServer.Game.Player;
using DiceGuardiansServer.Networking;
using Riptide;

namespace DiceGuardiansServer.Matchmaking; 

public static class Matchmaker {
    private static Dictionary<ushort, HumanPlayer> _lookingForGame = null!;
    private static Dictionary<ushort, GameInstance> _gameInstances = null!;

    public static void Start() {
        _lookingForGame = new Dictionary<ushort, HumanPlayer>();
        _gameInstances = new Dictionary<ushort, GameInstance>();
    }

    public static void CreateMatches() {
        while (_lookingForGame.Count > 1) {
            HumanPlayer player1 = _lookingForGame.Values.First();
            _lookingForGame.Remove(player1.GetNetworkId());
            HumanPlayer player2 = _lookingForGame.Values.First();
            _lookingForGame.Remove(player2.GetNetworkId());
            GameInstance game = new GameInstance(player1, player2);

            _gameInstances.Add(player1.GetNetworkId(), game);
            _gameInstances.Add(player2.GetNetworkId(), game);

            player1 = game.GetPlayer1();
            player2 = game.GetPlayer2();
            
            Console.WriteLine(player1.GetUser().GetUserName());
            Console.WriteLine(player2.GetUser().GetUserName());

            Message m1 = Message.Create(MessageSendMode.Reliable, ServerToClientId.StartGame);
            m1.AddInt(1);
            Message m2 = Message.Create(MessageSendMode.Reliable, ServerToClientId.StartGame);
            m2.AddInt(0);

            m1.AddLong(player1.GetUser().GetUserId());
            m1.AddString(player1.GetUser().GetUserName());
            m1.AddInt(player1.GetUser().GetMmr());
            m1.AddInt(player1.GetUser().GetGamesPlayed());
            m1.AddInt(player1.GetUser().GetGamesWon());
            
            m1.AddLong(player2.GetUser().GetUserId());
            m1.AddString(player2.GetUser().GetUserName());
            m1.AddInt(player2.GetUser().GetMmr());
            m1.AddInt(player2.GetUser().GetGamesPlayed());
            m1.AddInt(player2.GetUser().GetGamesWon());
            
            m2.AddLong(player2.GetUser().GetUserId());
            m2.AddString(player2.GetUser().GetUserName());
            m2.AddInt(player2.GetUser().GetMmr());
            m2.AddInt(player2.GetUser().GetGamesPlayed());
            m2.AddInt(player2.GetUser().GetGamesWon());
            
            m2.AddLong(player1.GetUser().GetUserId());
            m2.AddString(player1.GetUser().GetUserName());
            m2.AddInt(player1.GetUser().GetMmr());
            m2.AddInt(player1.GetUser().GetGamesPlayed());
            m2.AddInt(player1.GetUser().GetGamesWon());
            
            List<Deck> decks1 = DatabaseManager.GetUserDecks(player1.GetUser().GetUserId());
            List<CardsInDeck> list1 = DatabaseManager.GetDeckList(decks1[0].GetDeckId());

            m1.AddInt(list1.Count);
            foreach (CardsInDeck cid in list1) {
                m1.AddLong(cid.GetCardId());
                m1.AddInt(cid.GetCount());
            }
            
            List<Deck> decks2 = DatabaseManager.GetUserDecks(player2.GetUser().GetUserId());
            List<CardsInDeck> list2 = DatabaseManager.GetDeckList(decks2[0].GetDeckId());

            m2.AddInt(list2.Count);
            foreach (CardsInDeck cid in list2) {
                m2.AddLong(cid.GetCardId());
                m2.AddInt(cid.GetCount());
            }

            NetworkManager.GetServer().Send(m1, player1.GetNetworkId());
            NetworkManager.GetServer().Send(m2, player2.GetNetworkId());
            
            GameEventManager.NextStepEvent(game, Step.STANDBY);
        }
    }

    public static void CheckIfAvailableOpponent(User u, ushort netId) { // TODO remove debug code
        if (_lookingForGame.Count < 1) {
            _lookingForGame[netId] = new HumanPlayer(netId, u);
        }
    }

    [MessageHandler((ushort) ClientToServerId.StartMatchmaking)]
    public static void AddToMatchmaker(ushort uid, Message message) {
        long userId = message.GetLong();
        User u = DatabaseManager.GetUser(userId);
        HumanPlayer player = new HumanPlayer(uid, u);
        _lookingForGame[uid] = player;
        Console.WriteLine(_lookingForGame.Count);
    }

    [MessageHandler((ushort) ClientToServerId.CancelMatchmaking)]
    public static void CancelMatchmaking(ushort uid, Message message) {
        _lookingForGame.Remove(uid);
    }
    
    [MessageHandler((ushort) ClientToServerId.SelectDiceRoll)]
    public static void SelectDiceRoll(ushort uid, Message message) {
        _gameInstances[uid].TriggerDiceRoll(uid, message);
    }
}