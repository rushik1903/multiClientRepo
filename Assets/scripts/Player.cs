using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RiptideNetworking;

public class Player : MonoBehaviour
{
    public static Dictionary<ushort, Player> list = new Dictionary<ushort, Player>();

    public ushort Id{ get; private set; }
    public bool IsLocal{ get; private set; }

    [SerializeField] private Transform camTransform;
    [SerializeField] private Interpolator interpolator;
    //
    // [SerializeField] private GameObject localPlayer;
    //

    private string username;

    private void OnDestroy(){
        list.Remove(Id);
    }

    private void Move(ushort tick, bool isTeleport, Vector3 newPosition, Vector3 forward){
        interpolator.NewUpdate(tick, isTeleport, newPosition);
        // transform.position = newPosition;    dierectly teleporting the player to incoming location
        if(!IsLocal){
            camTransform.forward = forward;
        }
    }

    public static void Spawn(ushort id, string username, Vector3 position){
        Player player;
        GameObject playerObject;
        if(id==NetworkManager.Singleton.Client.Id){
            //playerObject = Instantiate(GameLogic.Singleton.LocalPlayerPrefab, position, Quaternion.identity);
            playerObject = GameObject.FindGameObjectsWithTag("localPlayer")[0];
            player = playerObject.GetComponent<Player>();
            playerObject.GetComponent<CharacterController>().enabled=false;
            playerObject.transform.position = position;
            playerObject.GetComponent<CharacterController>().enabled=true;
            player.IsLocal = true;
        }else{
            player = Instantiate(GameLogic.Singleton.PlayerPrefab, position, Quaternion.identity).GetComponent<Player>();
            player.IsLocal = false;
        }

        player.name = $"Player {id} {(string.IsNullOrEmpty(username) ? "Guest" : username)}";
        player.Id=id;
        player.username=username;

        list.Add(id, player);
    }

    [MessageHandler((ushort)ServerToClientId.playerSpawned)]

    private static void SpawnPlayer(Message message){
        Spawn(message.GetUShort(), message.GetString(), message.GetVector3());
    }


    [MessageHandler((ushort)ServerToClientId.playerMovement)]

    private static void PlayerMovement(Message message){
        if(list.TryGetValue(message.GetUShort(),out Player player)){
            //if(player.IsLocal==false){
                player.Move(message.GetUShort(), message.GetBool(), message.GetVector3(), message.GetVector3());
            //}
        }
    }
}
