using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using Mirror;
using UnityEngine.EventSystems;

public class MiniMapControl : MonoBehaviour, IPointerDownHandler, IDragHandler
{
    [SerializeField] RectTransform minimapRect = null;
    [SerializeField] float mapScale = 20f;
    [SerializeField] float offset = -6;
    Transform playerCameraTransform;
    [ClientCallback]
    private void Update() {
        if(playerCameraTransform!=null){return;}
        if(NetworkClient.connection.identity==null){return;}
        playerCameraTransform = NetworkClient.connection.identity.GetComponent<CameraControler>().GetCamera();
    }

    void MoveCamera(){
        Vector2 mousePos = Mouse.current.position.ReadValue();
        if(!RectTransformUtility.ScreenPointToLocalPointInRectangle(
            minimapRect,
            mousePos,
            null,
            out Vector2 localPoint
        )){return;}
        Vector2  lerp = new Vector2(
            (localPoint.x - minimapRect.rect.x)/minimapRect.rect.width,
            (localPoint.y-minimapRect.rect.y)/minimapRect.rect.height);
        Vector3 newCamPos = new Vector3(
            Mathf.Lerp(-mapScale,mapScale,lerp.x),
            playerCameraTransform.position.y,
            Mathf.Lerp(-mapScale, mapScale, lerp.y)
            );
        playerCameraTransform.position = newCamPos + new Vector3(0,0,offset);
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        MoveCamera();
    }

    public void OnDrag(PointerEventData eventData)
    {
        MoveCamera();
    }
}
