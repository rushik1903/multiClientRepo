using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using RiptideNetworking;

[RequireComponent(typeof(CharacterController))]

public class PlayerMovement : MonoBehaviour
{
    // [SerializeField] private Player player;
    [SerializeField] private Joystick joystick;
    [SerializeField] private CharacterController controller;
    [SerializeField] private Transform camProxy;
    [SerializeField] private float gravity;
    [SerializeField] private float movementSpeed;
    [SerializeField] private float jumpHeight;

    private float gravityAcceleration;
    private float moveSpeed;
    private float jumspeed;

    private bool[] inputs;
    private float yVelocity;
    private bool didTeleport;

    // private void OnValidate() {
    //     if(controller==null){
    //         controller=GetComponent<CharacterController>();
    //     }
    //     if(player==null){
    //         player=GetComponent<Player>();
    //     }

    //     Initialize();
    // }

    private void Start(){
        Initialize();
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

        // if(System.Math.Abs(joystick.Horizontal)>System.Math.Abs(joystick.Vertical)){
        //     if(joystick.Horizontal>0){
        //         inputs[3]=true;
        //     }else{
        //         inputs[2]=true;
        //     }
        // }else if(System.Math.Abs(joystick.Horizontal)<System.Math.Abs(joystick.Vertical)){
        //     if(joystick.Vertical>0){
        //         inputs[0]=true;
        //     }else{
        //         inputs[1]=true;
        //     }
        // }
        //MobileJumpSprint();
    }
    // void MobileJumpSprint(){
    //     OnClickJump();
    //     OnClickSprint();
    // }
    public void OnClickJump(){
        inputs[4]=true;
    }
    public void OnClickSprint(){
        inputs[5]=true;
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
        Move(inputDirection, inputs[4],inputs[5]);
    }

    private void Initialize(){
        gravityAcceleration = gravity*Time.fixedDeltaTime;
        moveSpeed = movementSpeed*Time.fixedDeltaTime;
        jumspeed = Mathf.Sqrt(jumpHeight*-2f*gravityAcceleration);
    }

    private void Move(Vector2 inputDirection, bool jump, bool sprint){
        Vector3 moveDirection = Vector3.Normalize(camProxy.right*inputDirection.x+Vector3.Normalize(FlattenVector3(camProxy.forward))*inputDirection.y);
        moveDirection *= moveSpeed;
        if(sprint){
            moveDirection*=2f;
        }
        if(controller.isGrounded){
            yVelocity=0f;
            if(jump){
                yVelocity=jumspeed;
            }
        }
        
        yVelocity+=gravityAcceleration;

        moveDirection.y=yVelocity;
        controller.Move(moveDirection);
        //Debug.Log(controller.transform.position);
        
        for(int i=0;i<6;i++){
            inputs[i]=false;
        }
    }

    private Vector3 FlattenVector3(Vector3 vector){
        vector.y=0;
        return vector;
    }

    public void SetInput(bool[] inputs, Vector3 forward){
        this.inputs=inputs;
        camProxy.forward=forward;
    }

    #region Messages

    private void SendInput(){
        Message message = Message.Create(MessageSendMode.unreliable, (ushort)ClientToServerId.input);
        message.AddBools(inputs, false);
        message.AddVector3(camProxy.forward);
        NetworkManager.Singleton.Client.Send(message);
    }
    #endregion
}
