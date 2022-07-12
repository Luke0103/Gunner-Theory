using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using Photon.Realtime;
using ExitGames.Client.Photon;

public class QuickMatch : MatchmakingManager
{
    [SerializeField] GameObject quickMatchPanel;
    [SerializeField] Text quickMatchStatusText;

    private void Start()
    {
        SubscribeToCallbacks();
    }

    private void OnDisable()
    {
        NetworkManager.Instance.GetClient.OpLeaveRoom(false);
        UnSubscribeToCallbacks();
    }

    public override void OnPlayerEnteredRoom(Photon.Realtime.Player newPlayer)
    {
        currentPlayerCount = NetworkManager.Instance.GetClient.CurrentRoom.PlayerCount;
        UpdateQuickMatchStatusText();

        Debug.Log("Other Joined");
    }

    public override void OnPlayerLeftRoom(Photon.Realtime.Player otherPlayer)
    {
        try
        {
            currentPlayerCount = NetworkManager.Instance.GetClient.CurrentRoom.PlayerCount;
            UpdateQuickMatchStatusText();
        }
        catch
        {

        }
        Debug.Log("Other Left");
    }

    public override void OnRoomPropertiesUpdate(ExitGames.Client.Photon.Hashtable propertiesThatChanged)
    {
        Debug.Log(propertiesThatChanged.ToStringFull());
    }

    //�κ� �÷��̾� ����
    public override void OnJoinedRoom()
    {
        currentPlayerCount = NetworkManager.Instance.GetClient.CurrentRoom.PlayerCount;
        UpdateQuickMatchStatusText(); //��Ȳ ������Ʈ

        //�ִ� �÷��̾� �� ����
        if (currentPlayerCount == DefaultValues.MaxPlayers)
        {
            RaiseEventOptions eventOptions = new RaiseEventOptions();
            eventOptions.Receivers = ReceiverGroup.All;
            NetworkManager.Instance.GetClient.OpRaiseEvent
                ((byte)DefaultValues.EventCode.START_GAME, 0, eventOptions, SendOptions.SendReliable);
        }
    }

    //�ڵ� ��ġ����ŷ
    public void QuickMatchConnect()
    {
        ExitGames.Client.Photon.Hashtable property = new ExitGames.Client.Photon.Hashtable();
        property["MatchType"] = "Quick";

        OpJoinRandomRoomParams joinParams = new OpJoinRandomRoomParams();
        joinParams.ExpectedCustomRoomProperties = new ExitGames.Client.Photon.Hashtable();
        joinParams.ExpectedMaxPlayers = (byte)DefaultValues.MaxPlayers;
        joinParams.ExpectedCustomRoomProperties = property;

        EnterRoomParams enterParams = new EnterRoomParams();
        enterParams.RoomOptions = new RoomOptions();
        enterParams.RoomOptions.IsOpen = true;
        enterParams.RoomOptions.IsVisible = true;
        enterParams.RoomOptions.MaxPlayers = (byte)DefaultValues.MaxPlayers;
        enterParams.RoomOptions.CustomRoomProperties = property;
        enterParams.RoomOptions.CustomRoomPropertiesForLobby = new string[]
        {
            "MatchType"
        };

        quickMatchPanel.SetActive(true);

        NetworkManager.Instance.GetClient.OpJoinRandomOrCreateRoom(joinParams, enterParams);
    }

    public void UpdateQuickMatchStatusText()
    {
        quickMatchStatusText.text = string.Format("{0} / {1}", currentPlayerCount, DefaultValues.MaxPlayers);
    }
}
