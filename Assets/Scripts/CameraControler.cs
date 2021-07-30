using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using UnityEngine.InputSystem;
using System;

public class CameraControler : NetworkBehaviour
{
    [SerializeField] Transform cameraTransform = null;
    [SerializeField] float speed = 20;
    [SerializeField] float screenBorderThickness = 10f;
    [SerializeField] Vector2 screenXLimits = Vector2.zero;
    [SerializeField] Vector2 screenZLimits = Vector2.zero;

    private Controlss controls;
    Vector2 previousInput;

    public override void OnStartAuthority()
    {
        if(!hasAuthority){return;}
        cameraTransform.gameObject.SetActive(true);
        controls = new Controlss();
        controls.Player.MoveCamera.performed += SetPreviousInput;
        controls.Player.MoveCamera.canceled += SetPreviousInput;
        controls.Enable();
    }
    [ClientCallback]
    private void Update() {
        if(!hasAuthority || !Application.isFocused){return;}
        UpdateCameraPosition();
    }

    private void UpdateCameraPosition()
    {
        Vector3 pos = cameraTransform.position;
        if(previousInput == Vector2.zero){
            Vector3 cursorMove = Vector3.zero;
            Vector2 cursorPosition = Mouse.current.position.ReadValue();
            if(cursorPosition.y>Screen.height - screenBorderThickness){
                cursorMove.z +=1;
            }
            else if(cursorPosition.y<screenBorderThickness){
                cursorMove.z -=1;
            }
            if(cursorPosition.x>Screen.width - screenBorderThickness){
                cursorMove.x +=1;
            }
            else if(cursorPosition.x<screenBorderThickness){
                cursorMove.x -=1;
            }
            pos += cursorMove.normalized*speed*Time.deltaTime;
        }
        else{
            pos += new Vector3(previousInput.x,0,previousInput.y)*speed*Time.deltaTime;
        }
        pos.x = Mathf.Clamp(pos.x,screenXLimits.x, screenXLimits.y);
        pos.z = Mathf.Clamp(pos.z,screenZLimits.x, screenZLimits.y);
        cameraTransform.position = pos;
    }

    void SetPreviousInput(InputAction.CallbackContext ctx){
        previousInput = ctx.ReadValue<Vector2>();
    }
}
