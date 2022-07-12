using System.Collections;
using System.Collections.Generic;

public class Card
{
    protected string name;
    protected string desc;

    public Card()
    {
        name = "DefaultName";
        desc = "DefaultDesc";
    }

    public string GetName() { return name; }
    public string GetDesc() { return desc; }

    public virtual void CardEvent()
    {
        Dictionary<string, string> eventContent = new Dictionary<string, string>();
        eventContent["CardName"] = name;
        eventContent["CardDesc"] = desc;

        NetworkManager.Instance.GetClient.OpRaiseEvent
            ((byte)DefaultValues.EventCode.CARD_USED, eventContent, 
            Photon.Realtime.RaiseEventOptions.Default, ExitGames.Client.Photon.SendOptions.SendReliable);
    }
}
