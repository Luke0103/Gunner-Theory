using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Realtime;

public class NetworkManager : MonoBehaviour, IConnectionCallbacks
{
    public static NetworkManager Instance { get; private set; }

    private readonly LoadBalancingClient client = new LoadBalancingClient();

    public LoadBalancingClient GetClient { get { return client; } }

    public bool IsMasterClient { get { return client.LocalPlayer.IsMasterClient; } }

    private void Awake()
    {
        Instance = this;
        DontDestroyOnLoad(this);
    }

    private void Start()
    {
        client.AddCallbackTarget(this);
        client.ClientType = ClientAppType.Realtime;
        client.ConnectUsingSettings(new AppSettings() { AppIdRealtime = "afaab8ef-7c38-46a7-ab51-a6fa9d7d333c", FixedRegion = "kr" });
    }

    private void Update()
    {
        client.Service();
    }

    private void OnDisable()
    {
        client.RemoveCallbackTarget(this);
    }

    public void InstantiateObject(GameObject gameObject, int actorNumber)
    {
        
    }

    #region IConnectionCallbacks
    public void OnConnected()
    {
        
    }

    public void OnConnectedToMaster()
    {
        
    }

    public void OnDisconnected(DisconnectCause cause)
    {
        
    }

    public void OnRegionListReceived(RegionHandler regionHandler)
    {
        
    }

    public void OnCustomAuthenticationResponse(Dictionary<string, object> data)
    {
        
    }

    public void OnCustomAuthenticationFailed(string debugMessage)
    {
        
    }
    #endregion
}
