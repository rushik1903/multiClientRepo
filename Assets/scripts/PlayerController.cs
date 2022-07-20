using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RiptideNetworking;

[RequireComponent(typeof(CharacterController))]

public class PlayerController : MonoBehaviour
{
    [SerializeField] private Player localPlayer;
    [SerializeField] private Transform camTransform;
    [SerializeField] private CharacterController localPlayerController;
    [SerializeField] public GameObject localPlayerObject;
    [SerializeField] private Joystick joystick;

    [SerializeField] private float gravity;
    [SerializeField] private float movementSpeed;
    [SerializeField] private float jumpHeight;

    private float gravityAcceleration;
    private float moveSpeed;
    private float jumspeed;

    private bool[] inputs;
    private float yVelocity;
    private bool didTeleport;

    private void OnValidate() {
        // if(localPlayerController==null){
        //     localPlayerController=GetComponent<CharacterController>();
        // }
        // if(localPlayer==null){
        //     localPlayer=GetComponent<Player>();
        // }
        
        localPlayerController = localPlayerObject.GetComponent<CharacterController>();
        // localPlayer = localPlayerObject.GetComponent<Player>();

        Initialize();
    }

    private void Initialize(){
        gravityAcceleration = gravity*Time.fixedDeltaTime;
        moveSpeed = movementSpeed*Time.fixedDeltaTime;
        jumspeed = Mathf.Sqrt(jumpHeight*-2f*gravityAcceleration);
    }

    private void Start(){
        inputs=new bool[6];
    }

    private void Update(){
        if(Input.GetKey(KeyCode.W))
            inputs[0]=true;
        if(Input.GetKey(KeyCode.S))
            inputs[1]=true;
        if(Input.GetKey(KeyCode.A))
            inputs[2]=true;
        if(Input.GetKey(KeyCode.D))
            inputs[3]=true;
        if(Input.GetKey(KeyCode.Space))
            inputs[4]=true;
        if(Input.GetKey(KeyCode.LeftShift))
            inputs[5]=true;

        if(joystick.W)
            inputs[0]=true;
        if(joystick.S)
            inputs[1]=true;
        if(joystick.A)
            inputs[2]=true;
        if(joystick.D)
            inputs[3]=true;
    }
    public void MobileJump(){
        inputs[4] = true;
    }
    public void MobileSprint(){
        inputs[5] = true;
    }

    private void FixedUpdate(){
        SendInput();
        Vector2 inputDirection = Vector2.zero;
        if(inputs[0]){
            inputDirection.y+=1;
        }
        if(inputs[1]){
            inputDirection.y-=1;
        }
        if(inputs[2]){
            inputDirection.x-=1;
        }
        if(inputs[3]){
            inputDirection.x+=1;
        }
        //Move(inputDirection,inputs[4],inputs[5]);
        for (int i=0;i<inputs.Length;i++){
            inputs[i]=false;
        }
    }

    //private void Move(Vector2 inputDirection, bool jump, bool sprint){
    //    Vector3 moveDirection = Vector3.Normalize(camTransform.right*inputDirection.x+Vector3.Normalize(FlattenVector3(camTransform.forward))*inputDirection.y);
    //    //Debug.Log(moveDirection);
    //    moveDirection *= moveSpeed;
    //    if(sprint){
    //        moveDirection*=2f;
    //    }
    //    if(localPlayerController.isGrounded){
    //        yVelocity=0f;
    //        if(jump){
    //            yVelocity=jumspeed;
    //        }
    //    }
    //    yVelocity+=gravityAcceleration;

    //    moveDirection.y=yVelocity;
    //    localPlayerController.Move(moveDirection);
    //}

    //private Vector3 FlattenVector3(Vector3 vector){
    //    vector.y=0;
    //    return vector;
    //}

    #region Messages

    private void SendInput(){
        Message message = Message.Create(MessageSendMode.unreliable, (ushort)ClientToServerId.input);
        message.AddBools(inputs, false);
        message.AddVector3(camTransform.forward);
        NetworkManager.Singleton.Client.Send(message);
    }
    #endregion
}
