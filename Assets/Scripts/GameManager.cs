using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Photon.Pun;

public class GameManager : MonoBehaviour
{
    /// <summary>
    /// 開始処理
    /// </summary>
    private void Awake()
    {
        // ネットにつながっているかの判定
        if (!PhotonNetwork.IsConnected)
        {
            SceneManager.LoadScene(0);
        }
    }
}
