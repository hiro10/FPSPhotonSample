using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
using TMPro;
using Photon.Realtime;

public class Room : MonoBehaviour
{
    // ルーム名
    public Text buttonText;

    // ルーム情報
    private RoomInfo info;

    public void RegisterRoomDetails(RoomInfo info)
    {
        // ルーム情報格納
        this.info = info;

        // ルーム名
        buttonText.text = this.info.Name;
    }

    public void OpenRoom()
    {
        PhotonManager.instance.JoinRoom(info);
    }
}
