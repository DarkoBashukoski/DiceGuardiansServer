using DiceGuardiansServer.Collection;
using DiceGuardiansServer.Database;
using DiceGuardiansServer.Database.Models;
using DiceGuardiansServer.Game.Board;
using DiceGuardiansServer.Matchmaking;
using DiceGuardiansServer.Networking;
using Riptide;
using Riptide.Utils;

RiptideLogger.Initialize(Console.WriteLine, Console.WriteLine, Console.WriteLine, Console.WriteLine, false);

NetworkManager.Start();
DatabaseManager.Start();
DatabaseInitializer.init();
AllCards.InitializeCardInfo();
Matchmaker.Start();
AllPieces.Start();

User u = DatabaseManager.GetUser(1); // TODO remove debug code
Client testClient = new Client();
testClient.Connect("127.0.0.1:7777");

while (true) {
    NetworkManager.Update();
    Matchmaker.CreateMatches();
    Matchmaker.CheckIfAvailableOpponent(u, testClient.Id); // TODO remove debug code
    testClient.Update();
}