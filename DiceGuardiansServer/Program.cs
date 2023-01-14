using DiceGuardiansServer.Database;
using DiceGuardiansServer.Networking;
using Riptide.Utils;

RiptideLogger.Initialize(Console.WriteLine, Console.WriteLine, Console.WriteLine, Console.WriteLine, false);

NetworkManager.Start();
DatabaseManager.Start();

while (true) {
    NetworkManager.Update();
}

