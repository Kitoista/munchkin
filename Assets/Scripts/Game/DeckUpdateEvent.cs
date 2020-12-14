using Mirror;

public class DeckUpdateEvent {

    public int oldValue;
    public int newValue;
    public NetworkIdentity topCard;

    public DeckUpdateEvent() { }

    public DeckUpdateEvent(int o, int n, Card t) {
        oldValue = o;
        newValue = n;
        if (t != null) {
            topCard = t.GetComponent<NetworkIdentity>();
        }
    }

}
