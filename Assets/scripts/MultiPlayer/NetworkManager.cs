using RiptideNetworking;
using RiptideNetworking.Utils;
using UnityEngine;
using System;
using TMPro;
using UnityEngine.UI;

public enum ServerToClientId : ushort
{
    sync=1,
    playerSpawned,
    playerReconnect,
    playerMovement,
}

public enum ClientToServerId : ushort
{
    name=1,
    input,
}

public class NetworkManager : MonoBehaviour
{
    private static NetworkManager _singleton;
    [SerializeField] private InputField inputPort,inputIp;

    public static NetworkManager Singleton{
        get=>_singleton;
        private set{
            if(_singleton==null){
                _singleton=value;
            }
            else if(_singleton!=value){
                Debug.Log($"{nameof(NetworkManager)} instance already exists, destroying duplicate");
                Destroy(value);
            }
        }
    }

    
    public Client Client {get; private set; }
    private ushort _serverTick;
    public ushort ServerTick{
        get => _serverTick;
        private set{
            _serverTick = value;
            InterpolationTick = (ushort)(value-TicksBetweenPositionUpdates);
        }
    }
    public ushort InterpolationTick{get; private set;}
    private ushort _ticksBetweenPositionUpdates = 2;

    public ushort TicksBetweenPositionUpdates{
        get => _ticksBetweenPositionUpdates;
        private set{
            _ticksBetweenPositionUpdates = value;
            InterpolationTick = (ushort)(ServerTick-value);
        }
    }

    [SerializeField] private string ip;
    [SerializeField] private ushort port;
    [Space(10)]
    [SerializeField] private ushort tickDivergenceTolerance;

    private void Start(){
        RiptideLogger.Initialize(Debug.Log, Debug.Log, Debug.LogWarning, Debug.LogError, false);

        Client = new Client();
        Client.Connected += DidConnect;
        Client.ConnectionFailed += FailedToConnect;
        Client.ClientDisconnected += PlayerLeft;
        Client.Disconnected += DidDisconnect;

        ServerTick = 2;
    }

    private void FixedUpdate(){
        Client.Tick();
        ServerTick++;
    }

    private void OnApplicationQuit() {
        Client.Disconnect();
    }


    private void Awake(){
        Singleton = this;
    }

    public void Connect(){
        if (inputIp.text != "")
        {
            port = Convert.ToUInt16(inputPort.text);
            ip = inputIp.text;
        }
        Client.Connect($"{ip}:{port}");
        Debug.Log(ip);
    }

    private void DidConnect(object sender, EventArgs e){
        UIManager.Singleton.SendName();
    }

    private void FailedToConnect(object sender, EventArgs e){
        UIManager.Singleton.BackToMain();
    }

    private void DidDisconnect(object sender, EventArgs e){
        UIManager.Singleton.BackToMain();
        foreach(Player player in Player.list.Values){
            Destroy(player.gameObject);
        }
    }

    private void PlayerLeft(object sender, ClientDisconnectedEventArgs e){
        if(Player.list.TryGetValue(e.Id,out Player player)){
            //Destroy(player.gameObject);
        }
    }

    private void SetTick(ushort serverTick){
        if(Mathf.Abs(ServerTick - serverTick) > tickDivergenceTolerance){
            // Debug.Log($"Client tick: {ServerTick}->{serverTick}");
            ServerTick = serverTick;
        }
    }

    [MessageHandler((ushort)ServerToClientId.sync)]

    public static void Sync(Message message){
        Singleton.SetTick(message.GetUShort());
    }
}
