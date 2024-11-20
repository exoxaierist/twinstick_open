using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

public class MinimapBlock : MonoBehaviour
{
    [Header("Icons")]
    public GameObject shopIcon;
    
    [Header("Connections")]
    public GameObject upConnection;
    public GameObject downConnection;
    public GameObject rightConnection;
    public GameObject leftConnection;

    public GameObject currentRoomIndicator;
    private MetaRoomInfo info;

    private Image[] images;

    public void SetVisual(MetaRoomInfo _info)
    {
        info = _info;
        upConnection.SetActive(info.upConnected);
        downConnection.SetActive(info.downConnected);
        rightConnection.SetActive(info.rightConnected);
        leftConnection.SetActive(info.leftConnected);
        currentRoomIndicator.SetActive(false);

        if (info.type == RoomType.Shop) shopIcon.SetActive(true);
        else shopIcon.SetActive(false);

        images = GetComponentsInChildren<Image>();
    }

    public void IsCurrentRoom(bool state) => currentRoomIndicator.SetActive(state);

    public void OnRoomChange(MetaRoomInfo newRoom)
    {
        float dist = Vector2.Distance(new(info.x, info.y), new(newRoom.x, newRoom.y));
        Color col = Color.Lerp(Color.white, Color.gray, dist / 3);

        foreach (Image image in images) image.color = col;
        GetComponent<Image>().color = col;

        if (info.metaRoom.roomEnterCount == 0) gameObject.SetActive(false);
        else gameObject.SetActive(true);
    }
}
