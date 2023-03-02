using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
using TMPro;
using Photon.Realtime;

public class Room : MonoBehaviour
{
    // ���[����
    public Text buttonText;

    // ���[�����
    private RoomInfo info;

    public void RegisterRoomDetails(RoomInfo info)
    {
        // ���[�����i�[
        this.info = info;

        // ���[����
        buttonText.text = this.info.Name;
    }

    public void OpenRoom()
    {
        PhotonManager.instance.JoinRoom(info);
    }
}
