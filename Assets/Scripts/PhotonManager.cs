using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
using TMPro;

public class PhotonManager : MonoBehaviourPunCallbacks
{
    //
    public static PhotonManager instance;

    // ロードパネル格納用
    [SerializeField] GameObject loadPanel;

    // ロードテキスト
    [SerializeField] TextMeshProUGUI loadText;

    // ボタンの親オブジェクト
    [SerializeField] GameObject buttons;

    // 

    /// <summary>
    /// 開始処理
    /// </summary>
    private void Awake()
    {
        instance = this;
    }

    private void Start()
    {
        CloseMenuUi();

        // パネルとテキストの更新
        loadPanel.SetActive(true);
        loadText.text = "Connecting To Network...";

        //********************
        /// ネットワークに接続
        //********************
        // 現在つながっているかの判定
        ///※PhotonNetwork.IsConnectedとは 
        ///true : ネットワークにつながっている
        ///false : ネットワークにつながっていない
        if (PhotonNetwork.IsConnected==false)
        {
            // ネットワークに接続
            PhotonNetwork.ConnectUsingSettings();

        }
    }

    /// <summary>
    /// UIをすべて非表示に
    /// </summary>
    public void CloseMenuUi()
    {
        loadPanel.SetActive(false);
        buttons.SetActive(false);
    }

    /// <summary>
    /// ロビーUIを表示する関数
    /// </summary>
    public void LobbyMenuDisplay()
    {
        CloseMenuUi();
        buttons.SetActive(true);
    }

    /// <summary>
    /// マスターサーバーに接続されたときに呼ばれる関数
    /// </summary>
    public override void OnConnectedToMaster()
    {
        // ロビーに接続する
        PhotonNetwork.JoinLobby();

        // テキストの更新
        loadText.text = "Join In Lobby...";
    }

    /// <summary>
    /// ロビーに接続されたときに呼ばれる関数
    /// </summary>
    public override void OnJoinedLobby()
    {
        LobbyMenuDisplay();

    }
}
