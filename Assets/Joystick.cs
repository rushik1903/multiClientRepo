using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class Joystick : MonoBehaviour
{
    [SerializeField] private Canvas canvas;
    [SerializeField] private GameObject border;
    [SerializeField] private GameObject circle;
    public float horizontal,vertical;
    public bool W,A,S,D;

    void Start(){
        border.SetActive(false);
        circle.SetActive(false);
    }

    public void DragHandler(BaseEventData data){
        PointerEventData pointerData = (PointerEventData)data;
        Vector2 position;
        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            (RectTransform)canvas.transform,
            pointerData.position,
            canvas.worldCamera,
            out position
        );
        float distance = Vector3.Distance(canvas.transform.TransformPoint(position), border.GetComponent<Transform>().position);
        Vector3 tempPosition=new Vector3(canvas.transform.TransformPoint(position).x, canvas.transform.TransformPoint(position).y, 0f);
        if(distance<100){
            circle.GetComponent<Transform>().position = canvas.transform.TransformPoint(position);
        }
        else
        {
            Vector3 newTempPosition = tempPosition;
            newTempPosition=(newTempPosition-border.GetComponent<Transform>().position).normalized*100;
            newTempPosition=newTempPosition+border.GetComponent<Transform>().position;

            circle.GetComponent<Transform>().position = newTempPosition;
        }
        tempPosition = tempPosition-border.GetComponent<Transform>().position;

        ResetValues();

        //changing joystick
        horizontal = tempPosition.normalized.x;
        vertical = tempPosition.normalized.y;

        //changing joystick to WASD
        float ratio = vertical/horizontal;
        if(vertical>0){
            if(ratio > 0){
                if(ratio < 0.414){
                    D = true;
                }else if(ratio < 2.414){
                    W = true;
                    D = true;
                }else{
                    W = true;
                }
            }else{
                if(ratio < -2.414){
                    W = true;
                }else if(ratio < -0.414){
                    W = true;
                    A = true;
                }else{
                    A = true;
                }
            }
        }else{
            if(ratio > 0){
                if(ratio < 0.414){
                    A = true;
                }else if(ratio < 2.414){
                    A = true;
                    S = true;
                }else{
                    S = true;
                }
            }else{
                if(ratio < -2.414){
                    S = true;
                }else if(ratio < -0.414){
                    S = true;
                    D = true;
                }else{
                    D = true;
                }
            }
        }
        string s = W.ToString()+" " + A.ToString()+" " + S.ToString()+" " + D.ToString();
        Debug.Log(s);
    }
    public void Selected(BaseEventData data){
        border.SetActive(true);
        circle.SetActive(true);
        PointerEventData pointerData = (PointerEventData)data;
        Vector2 position;
        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            (RectTransform)canvas.transform,
            pointerData.position,
            canvas.worldCamera,
            out position
        );

        border.GetComponent<Transform>().position = canvas.transform.TransformPoint(position);
        circle.GetComponent<Transform>().position = canvas.transform.TransformPoint(position);
    }
    public void Deselected(){
        ResetValues();
        border.SetActive(false);
        circle.SetActive(false);
    }

    private void ResetValues(){
        horizontal=0;
        vertical=0;
        W = false;
        A = false;
        S = false;
        D = false;
    }
}
