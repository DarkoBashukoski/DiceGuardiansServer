using DiceGuardiansServer.Collection;
using DiceGuardiansServer.Database;
using DiceGuardiansServer.Game.Board;
using DiceGuardiansServer.Matchmaking;
using DiceGuardiansServer.Networking;
using Riptide.Utils;

RiptideLogger.Initialize(Console.WriteLine, Console.WriteLine, Console.WriteLine, Console.WriteLine, false);

NetworkManager.Start();
DatabaseManager.Start();
DatabaseInitializer.init();
AllCards.InitializeCardInfo();
Matchmaker.Start();
AllPieces.Start();

while (true) {
    NetworkManager.Update();
    Matchmaker.CreateMatches();
}