using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Photon.Pun;

public class GameManager : MonoBehaviour
{
    /// <summary>
    /// �J�n����
    /// </summary>
    private void Awake()
    {
        // �l�b�g�ɂȂ����Ă��邩�̔���
        if (!PhotonNetwork.IsConnected)
        {
            SceneManager.LoadScene(0);
        }
    }
}
