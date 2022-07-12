using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using Photon.Realtime;
using System;
using ExitGames.Client.Photon;

public class MatchmakingManager : MonoBehaviour, IMatchmakingCallbacks, IInRoomCallbacks, IOnEventCallback
{
    protected int currentPlayerCount = 1;

    private void Awake()
    {
        DontDestroyOnLoad(this);
    }

    #region IMatchmakingCallbacks

    public virtual void OnCreatedRoom()
    {
        
    }

    public void OnCreateRoomFailed(short returnCode, string message)
    {
        
    }

    public void OnFriendListUpdate(List<FriendInfo> friendList)
    {
        
    }

    public virtual void OnJoinedRoom()
    {
        
    }

    public void OnJoinRandomFailed(short returnCode, string message)
    {
        
    }

    public void OnJoinRoomFailed(short returnCode, string message)
    {
        
    }

    public void OnLeftRoom()
    {
        
    }

    #endregion

    #region IInRoomCallbacks

    public virtual void OnPlayerEnteredRoom(Photon.Realtime.Player newPlayer)
    {
        
    }

    public virtual void OnPlayerLeftRoom(Photon.Realtime.Player otherPlayer)
    {
        
    }

    public virtual void OnRoomPropertiesUpdate(ExitGames.Client.Photon.Hashtable propertiesThatChanged)
    {
        
    }

    public void OnPlayerPropertiesUpdate(Photon.Realtime.Player targetPlayer, ExitGames.Client.Photon.Hashtable changedProps)
    {
        
    }

    public void OnMasterClientSwitched(Photon.Realtime.Player newMasterClient)
    {
        
    }

    #endregion

    protected void SubscribeToCallbacks()
    {
        NetworkManager.Instance.GetClient.AddCallbackTarget(this);
    }

    protected void UnSubscribeToCallbacks()
    {
        NetworkManager.Instance.GetClient.RemoveCallbackTarget(this);
    }

    //방(로비) 생성
    public virtual void CreateRoom()
    {
        RoomOptions roomOptions = new RoomOptions();
        roomOptions.MaxPlayers = (byte)DefaultValues.MaxPlayers;

        EnterRoomParams enterRoomParams = new EnterRoomParams();
        enterRoomParams.RoomOptions = roomOptions;

        NetworkManager.Instance.GetClient.OpCreateRoom(enterRoomParams);
    }

    //방(로비) 참가
    public void JoinRoom(string name)
    {
        RoomOptions roomOptions = new RoomOptions();
        roomOptions.MaxPlayers = (byte)DefaultValues.MaxPlayers;

        TypedLobby typedLobby = new TypedLobby();
        typedLobby.Name = name;
        typedLobby.Type = LobbyType.SqlLobby;

        NetworkManager.Instance.GetClient.OpJoinLobby(typedLobby);
    }

    //게임 시작
    public IEnumerator StartGameCoroutine()
    {
        yield return new WaitForSeconds(0.1f);
        SceneManager.LoadScene("MainScene");
    }

    //이벤트 콜백
    public void OnEvent(EventData photonEvent)
    {
        if (photonEvent.Code == (byte)DefaultValues.EventCode.START_GAME)
        {
            StartCoroutine(StartGameCoroutine());
        }
    }
}
