using DiceGuardiansServer.Database.Models;

namespace DiceGuardiansServer.Game.Player; 

public class HumanPlayer {
    private readonly ushort _netId;
    private readonly User _user;
    private DeckManager _deckManager;
    private CrestPool _crestPool;

    private bool _hasSummoned;

    public HumanPlayer(ushort netId, User user) {
        _netId = netId;
        _user = user;
        _deckManager = new DeckManager(user.GetUserId());
        _crestPool = new CrestPool();
        _hasSummoned = false;
    }

    public ushort GetNetworkId() {
        return _netId;
    }

    public User GetUser() {
        return _user;
    }

    public DeckManager GetDeckManager() {
        return _deckManager;
    }

    public CrestPool GetCrestPool() {
        return _crestPool;
    }
}