using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Realtime;
using System;
using System.Linq;
using ExitGames.Client.Photon;

public class PlayerSpawner : MonoBehaviour, IOnEventCallback
{
    [SerializeField] private GameObject playerPrefab;
    [SerializeField] private Transform[] spawnPosList;

    [SerializeField] private float spawnYPosOffset;

    private void Start()
    {
        NetworkManager.Instance.GetClient.AddCallbackTarget(this);
        if (NetworkManager.Instance.IsMasterClient)
            SpawnPlayers();
    }

    private void OnDisable()
    {
        NetworkManager.Instance.GetClient.RemoveCallbackTarget(this);
    }

    private int ComparePlayerManager(PlayerManager x, PlayerManager y)
    {
        return x.ActorIndex.CompareTo(y.ActorIndex);
    }

    private void SpawnPlayers()
    {
        for (int i = 0; i < NetworkManager.Instance.GetClient.CurrentRoom.PlayerCount; i++)
        {
            RaiseEventOptions eventOptions = new RaiseEventOptions();
            eventOptions.Receivers = ReceiverGroup.All;

            Dictionary<string, int> info = new Dictionary<string, int>();
            info.Add("SpawnPos", i);
            info.Add("ActorNum", NetworkManager.Instance.GetClient.CurrentRoom.Players.ElementAt(i).Key);

            NetworkManager.Instance.GetClient.OpRaiseEvent((byte)DefaultValues.EventCode.INSTANTIATE, info, eventOptions, SendOptions.SendReliable);
        }

        GameManager.Instance.playerList.Sort(ComparePlayerManager);
    }

    public void OnEvent(EventData photonEvent)
    {   
        if (photonEvent.Code == (byte)DefaultValues.EventCode.INSTANTIATE)
        {   
            int idx = Convert.ToInt32(((Dictionary<string, int>)photonEvent.CustomData)["SpawnPos"]);
            int num = Convert.ToInt32(((Dictionary<string, int>)photonEvent.CustomData)["ActorNum"]);

            GameObject go = Instantiate(playerPrefab, spawnPosList[idx].position + new Vector3(0, spawnYPosOffset, 0), spawnPosList[idx].rotation);
            PlayerManager pm = go.GetComponent<PlayerManager>();
            pm.ActorIndex = idx;
            pm.ActorNumber = num;
            pm.Init();
            GameManager.Instance.playerList.Add(pm);
        }
    }
}
