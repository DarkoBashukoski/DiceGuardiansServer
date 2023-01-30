using DiceGuardiansServer.Database;
using DiceGuardiansServer.Database.Models;
using DiceGuardiansServer.Networking;
using Riptide;

namespace DiceGuardiansServer.Collection; 

public class CollectionManager {
    [MessageHandler((ushort)ClientToServerId.GetCollection)]
    public static void GetCollection(ushort uid, Message message) {
        long userId = message.GetLong();
        List<CardIsOwnedByUser> collection = DatabaseManager.GetUserCollection(userId);
        
        Message reply = Message.Create(MessageSendMode.Reliable, ServerToClientId.SuccessfulGetCollectionResponse);
        reply.AddInt(collection.Count);
        foreach (CardIsOwnedByUser card in collection) {
            reply.AddLong(card.GetCardId());
            reply.AddInt(card.GetCount());
        }
        
        List<Deck> decks = DatabaseManager.GetUserDecks(userId);
        List<CardsInDeck> list = DatabaseManager.GetDeckList(decks[0].GetDeckId());

        reply.AddInt(list.Count);
        foreach (CardsInDeck cid in list) {
            reply.AddLong(cid.GetCardId());
            reply.AddInt(cid.GetCount());
        }
        NetworkManager.GetServer().Send(reply, uid);
    }
}