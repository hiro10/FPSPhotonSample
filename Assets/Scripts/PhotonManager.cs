using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
using TMPro;
using Photon.Realtime;

public class PhotonManager : MonoBehaviourPunCallbacks
{
    // static変数
    public static PhotonManager instance;

    // ロードパネル格納用
    [SerializeField] GameObject loadPanel;

    // ロードテキスト
    [SerializeField] TextMeshProUGUI loadText;

    // ボタンの親オブジェクト
    [SerializeField] GameObject buttons;

    // ルームパネル
    public GameObject createRoomPanel;

    // ルーム名格納用
    public TextMeshProUGUI enterRoomName;

    // ルームパネル
    public GameObject RoomPanel;

    // ルームネーム
    public TextMeshProUGUI roomName;

    // エラーパネル
    public GameObject errorPanel;

    // エラーテキスト1
    public TextMeshProUGUI errorText;

    // ルーム一覧
    public GameObject roomListPanel;

    // ルームボタン
    public Room originalRoomButton;

    // ルームボタンの親
    public GameObject roomButtonContent;

    // ルーム情報を扱う辞書
    Dictionary<string, RoomInfo> roomsList = new Dictionary<string, RoomInfo>();

    // ルームボタンを使うリスト
    private List<Room> allRomButtons = new List<Room>();

    // プレイヤー名
    public Text playerNameText;

    //
    private List<Text> allPlayerNames = new List<Text>();

    public GameObject playerNameContent;

    // 名前入力パネル
    [SerializeField] private GameObject nameInputPanel;

    // 名前入力表示テキスト
    [SerializeField] private TextMeshProUGUI placeholdText;

    // 入力フィールド
    [SerializeField] private TMP_InputField  nameInput;

　　//
    private bool setName; 

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
        createRoomPanel.SetActive(false);
        RoomPanel.SetActive(false);
        errorPanel.SetActive(false);
        roomListPanel.SetActive(false);
        nameInputPanel.SetActive(false);
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

        // 辞書の初期化
        roomsList.Clear();

        //仮処理
        PhotonNetwork.NickName = Random.Range(0, 10000).ToString();

        ConfirmationName();
    
    }

    /// <summary>
    /// ルームを作るボタン絵を押した時
    /// </summary>
    public void OpenCreateRoomPanel()
    {
        CloseMenuUi();
        createRoomPanel.SetActive(true);
    }

    /// <summary>
    /// ルームを作成ボタン用
    /// </summary>
    public void CreateRoomButton()
    {
        // enterRoomName.textがなにも入力されていないか
        // 入力されてない場合trueが返ってくる
        if (!string.IsNullOrEmpty(enterRoomName.text))
        {
            RoomOptions options = new RoomOptions();
            // プレイヤーの人数
            options.MaxPlayers = 8;

            // ルーム作成(第一引数ルーム名、第二引数　ルームの条件)
            PhotonNetwork.CreateRoom(enterRoomName.text, options);

            CloseMenuUi();

            // ロードパネル表示
            loadText.text = "Createing Room";
            loadPanel.SetActive(true);
        }
    }

    /// <summary>
    /// ルーム参加時に呼ばれる関数
    /// </summary>
    public override void OnJoinedRoom()
    {
        CloseMenuUi();
        RoomPanel.SetActive(true);

        // 現在参加しているルームの名前の取得
        roomName.text = PhotonNetwork.CurrentRoom.Name;

        // ルームにいるプレイヤー情報を取得
        GetAllPlayer();
    }

    /// <summary>
    /// ルーム退出関数
    /// </summary>
    public void LeavRoom()
    {
        // ルームから退出
        PhotonNetwork.LeaveRoom();

        CloseMenuUi();

        loadText.text = "LeaveRoom.....";
        loadPanel.SetActive(true);
    }

    /// <summary>
    /// ルーム退出時に呼ばれる関数
    /// </summary>
    public override void OnLeftRoom()
    {
        LobbyMenuDisplay();
    }

    /// <summary>
    /// ルーム作成ができなかった時の呼ばれる関数
    /// </summary>
    public override void OnCreateRoomFailed(short returnCode, string message)
    {
        CloseMenuUi();

        errorText.text = "ルーム作成に失敗しました。"+message;

        errorPanel.SetActive(true);
    }

    /// <summary>
    /// ルーム一覧を開く関数
    /// </summary>
    public void FindRoom()
    {
        CloseMenuUi();
        roomListPanel.SetActive(true);
    }

    /// <summary>
    /// ルームリストに更新があった場合に呼ばれる関数
    /// </summary>
    public override void OnRoomListUpdate(List<RoomInfo> roomList)
    {
        //ルームボタンUIの初期化
        RoomUiinitialization();
        // 辞書に登録
        UpdateRoomList(roomList);
    }

    /// <summary>
    /// ルームを辞書に登録
    /// </summary>
    public void UpdateRoomList(List<RoomInfo> roomList)
    {
        //辞書にルームを登録
        for (int i = 0; i < roomList.Count; i++)
        {
            RoomInfo info = roomList[i];

            // RemovedFromListは満室の場合はtrueが帰る
            if (info.RemovedFromList)
            {
                roomsList.Remove(info.Name);
            }
            else
            {
                // 辞書に登録
                roomsList[info.Name] = info;
            }
        }
        RoomListDisplay(roomsList);
    }

    /// <summary>
    /// ルームボタンを表示
    /// </summary>
    public void  RoomListDisplay(Dictionary<string,RoomInfo> catchedRoomList)
    {
        foreach (var roomInfo in catchedRoomList)
        {
            // ボタン作成
            Room newButton = Instantiate(originalRoomButton);

            // 生成したボタンにルーム情報設定
            newButton.RegisterRoomDetails(roomInfo.Value);

            // 親の設定
            newButton.transform.SetParent(roomButtonContent.transform);

            allRomButtons.Add(newButton);
        }
    }

    public void RoomUiinitialization()
    {
        // ルームUI分ループが回る
        foreach  (Room rm in allRomButtons)
        {
            Destroy(rm.gameObject);
        }

        // リストの初期化
        allRomButtons.Clear();
    }

    /// <summary>
    /// ルームにはいる関数
    /// </summary>
    public void JoinRoom(RoomInfo roomInfo)
    {
        // ルームに参加
        PhotonNetwork.JoinRoom(roomInfo.Name);

        // Ui
        CloseMenuUi();

        //
        loadText.text = "ルームに参加中";
        loadPanel.SetActive(true);
    }

    /// <summary>
    /// ルームにいるプレイヤーの取得
    /// </summary>
    public void GetAllPlayer()
    {
        // 名前UI初期化
        InitializePlayerList();

        PlayerDisplay();
    }

    public void InitializePlayerList()
    {
        foreach(var rm in allPlayerNames)
        {
            Destroy(rm.gameObject);
        }
        allPlayerNames.Clear();
    }

   
    public void PlayerDisplay()
    {
        foreach(var players in PhotonNetwork.PlayerList)
        {
            // Ui生成
            PlayerTextGeneration(players);
        }
    }

    public void PlayerTextGeneration(Player players)
    {
        // UI生成
        Text newPlayerText = Instantiate(playerNameText);

        newPlayerText.text = players.NickName;

        //親の設定
        newPlayerText.transform.SetParent(playerNameContent.transform);

        // リストに登録
        allPlayerNames.Add(newPlayerText);
    }

    /// <summary>
    /// 名前が入力済みか確認してUI更新
    /// </summary>
    private void ConfirmationName()
    {
        // 名前が設定されていない
        if(!setName)
        {
            CloseMenuUi();
            nameInputPanel.SetActive(true);

            if(PlayerPrefs.HasKey("playerName"))
            {
                placeholdText.text = PlayerPrefs.GetString("playerName");
                nameInput.text = PlayerPrefs.GetString("playerName");
            }
        }
        else
        {
            PhotonNetwork.NickName = PlayerPrefs.GetString("playerName");
        }
    }

    /// <summary>
    /// 名前登録関数
    /// </summary>
    public void SetName()
    {
        // 入力フィールドに文字があるか
        if(!string.IsNullOrEmpty(nameInput.text))
        {
            PhotonNetwork.NickName = nameInput.text;

            // 保存
            PlayerPrefs.SetString("playerName",nameInput.text);

            LobbyMenuDisplay();

            setName = true;
        }
    }

    public override void OnPlayerEnteredRoom(Player newPlayer)
    {
        PlayerTextGeneration(newPlayer);
    }

    public override void OnPlayerLeftRoom(Player otherPlayer)
    {
        GetAllPlayer();
    }
}
