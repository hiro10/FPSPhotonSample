using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
using TMPro;
using Photon.Realtime;

public class PhotonManager : MonoBehaviourPunCallbacks
{
    // static�ϐ�
    public static PhotonManager instance;

    // ���[�h�p�l���i�[�p
    [SerializeField] GameObject loadPanel;

    // ���[�h�e�L�X�g
    [SerializeField] TextMeshProUGUI loadText;

    // �{�^���̐e�I�u�W�F�N�g
    [SerializeField] GameObject buttons;

    // ���[���p�l��
    public GameObject createRoomPanel;

    // ���[�����i�[�p
    public TextMeshProUGUI enterRoomName;

    // ���[���p�l��
    public GameObject RoomPanel;

    // ���[���l�[��
    public TextMeshProUGUI roomName;

    // �G���[�p�l��
    public GameObject errorPanel;

    // �G���[�e�L�X�g1
    public TextMeshProUGUI errorText;

    // ���[���ꗗ
    public GameObject roomListPanel;

    // ���[���{�^��
    public Room originalRoomButton;

    // ���[���{�^���̐e
    public GameObject roomButtonContent;

    // ���[��������������
    Dictionary<string, RoomInfo> roomsList = new Dictionary<string, RoomInfo>();

    // ���[���{�^�����g�����X�g
    private List<Room> allRomButtons = new List<Room>();

    // �v���C���[��
    public Text playerNameText;

    //
    private List<Text> allPlayerNames = new List<Text>();

    public GameObject playerNameContent;

    // ���O���̓p�l��
    [SerializeField] private GameObject nameInputPanel;

    // ���O���͕\���e�L�X�g
    [SerializeField] private TextMeshProUGUI placeholdText;

    // ���̓t�B�[���h
    [SerializeField] private TMP_InputField  nameInput;

�@�@//
    private bool setName; 

    /// <summary>
    /// �J�n����
    /// </summary>
    private void Awake()
    {
        instance = this;
    }

    private void Start()
    {
        CloseMenuUi();

        // �p�l���ƃe�L�X�g�̍X�V
        loadPanel.SetActive(true);
        loadText.text = "Connecting To Network...";

        //********************
        /// �l�b�g���[�N�ɐڑ�
        //********************
        // ���݂Ȃ����Ă��邩�̔���
        ///��PhotonNetwork.IsConnected�Ƃ� 
        ///true : �l�b�g���[�N�ɂȂ����Ă���
        ///false : �l�b�g���[�N�ɂȂ����Ă��Ȃ�
        if (PhotonNetwork.IsConnected==false)
        {
            // �l�b�g���[�N�ɐڑ�
            PhotonNetwork.ConnectUsingSettings();

        }
    }

    /// <summary>
    /// UI�����ׂĔ�\����
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
    /// ���r�[UI��\������֐�
    /// </summary>
    public void LobbyMenuDisplay()
    {
        CloseMenuUi();
        buttons.SetActive(true);
    }

    /// <summary>
    /// �}�X�^�[�T�[�o�[�ɐڑ����ꂽ�Ƃ��ɌĂ΂��֐�
    /// </summary>
    public override void OnConnectedToMaster()
    {
        // ���r�[�ɐڑ�����
        PhotonNetwork.JoinLobby();

        // �e�L�X�g�̍X�V
        loadText.text = "Join In Lobby...";
    }

    /// <summary>
    /// ���r�[�ɐڑ����ꂽ�Ƃ��ɌĂ΂��֐�
    /// </summary>
    public override void OnJoinedLobby()
    {
        LobbyMenuDisplay();

        // �����̏�����
        roomsList.Clear();

        //������
        PhotonNetwork.NickName = Random.Range(0, 10000).ToString();

        ConfirmationName();
    
    }

    /// <summary>
    /// ���[�������{�^���G����������
    /// </summary>
    public void OpenCreateRoomPanel()
    {
        CloseMenuUi();
        createRoomPanel.SetActive(true);
    }

    /// <summary>
    /// ���[�����쐬�{�^���p
    /// </summary>
    public void CreateRoomButton()
    {
        // enterRoomName.text���Ȃɂ����͂���Ă��Ȃ���
        // ���͂���ĂȂ��ꍇtrue���Ԃ��Ă���
        if (!string.IsNullOrEmpty(enterRoomName.text))
        {
            RoomOptions options = new RoomOptions();
            // �v���C���[�̐l��
            options.MaxPlayers = 8;

            // ���[���쐬(���������[�����A�������@���[���̏���)
            PhotonNetwork.CreateRoom(enterRoomName.text, options);

            CloseMenuUi();

            // ���[�h�p�l���\��
            loadText.text = "Createing Room";
            loadPanel.SetActive(true);
        }
    }

    /// <summary>
    /// ���[���Q�����ɌĂ΂��֐�
    /// </summary>
    public override void OnJoinedRoom()
    {
        CloseMenuUi();
        RoomPanel.SetActive(true);

        // ���ݎQ�����Ă��郋�[���̖��O�̎擾
        roomName.text = PhotonNetwork.CurrentRoom.Name;

        // ���[���ɂ���v���C���[�����擾
        GetAllPlayer();
    }

    /// <summary>
    /// ���[���ޏo�֐�
    /// </summary>
    public void LeavRoom()
    {
        // ���[������ޏo
        PhotonNetwork.LeaveRoom();

        CloseMenuUi();

        loadText.text = "LeaveRoom.....";
        loadPanel.SetActive(true);
    }

    /// <summary>
    /// ���[���ޏo���ɌĂ΂��֐�
    /// </summary>
    public override void OnLeftRoom()
    {
        LobbyMenuDisplay();
    }

    /// <summary>
    /// ���[���쐬���ł��Ȃ��������̌Ă΂��֐�
    /// </summary>
    public override void OnCreateRoomFailed(short returnCode, string message)
    {
        CloseMenuUi();

        errorText.text = "���[���쐬�Ɏ��s���܂����B"+message;

        errorPanel.SetActive(true);
    }

    /// <summary>
    /// ���[���ꗗ���J���֐�
    /// </summary>
    public void FindRoom()
    {
        CloseMenuUi();
        roomListPanel.SetActive(true);
    }

    /// <summary>
    /// ���[�����X�g�ɍX�V���������ꍇ�ɌĂ΂��֐�
    /// </summary>
    public override void OnRoomListUpdate(List<RoomInfo> roomList)
    {
        //���[���{�^��UI�̏�����
        RoomUiinitialization();
        // �����ɓo�^
        UpdateRoomList(roomList);
    }

    /// <summary>
    /// ���[���������ɓo�^
    /// </summary>
    public void UpdateRoomList(List<RoomInfo> roomList)
    {
        //�����Ƀ��[����o�^
        for (int i = 0; i < roomList.Count; i++)
        {
            RoomInfo info = roomList[i];

            // RemovedFromList�͖����̏ꍇ��true���A��
            if (info.RemovedFromList)
            {
                roomsList.Remove(info.Name);
            }
            else
            {
                // �����ɓo�^
                roomsList[info.Name] = info;
            }
        }
        RoomListDisplay(roomsList);
    }

    /// <summary>
    /// ���[���{�^����\��
    /// </summary>
    public void  RoomListDisplay(Dictionary<string,RoomInfo> catchedRoomList)
    {
        foreach (var roomInfo in catchedRoomList)
        {
            // �{�^���쐬
            Room newButton = Instantiate(originalRoomButton);

            // ���������{�^���Ƀ��[�����ݒ�
            newButton.RegisterRoomDetails(roomInfo.Value);

            // �e�̐ݒ�
            newButton.transform.SetParent(roomButtonContent.transform);

            allRomButtons.Add(newButton);
        }
    }

    public void RoomUiinitialization()
    {
        // ���[��UI�����[�v�����
        foreach  (Room rm in allRomButtons)
        {
            Destroy(rm.gameObject);
        }

        // ���X�g�̏�����
        allRomButtons.Clear();
    }

    /// <summary>
    /// ���[���ɂ͂���֐�
    /// </summary>
    public void JoinRoom(RoomInfo roomInfo)
    {
        // ���[���ɎQ��
        PhotonNetwork.JoinRoom(roomInfo.Name);

        // Ui
        CloseMenuUi();

        //
        loadText.text = "���[���ɎQ����";
        loadPanel.SetActive(true);
    }

    /// <summary>
    /// ���[���ɂ���v���C���[�̎擾
    /// </summary>
    public void GetAllPlayer()
    {
        // ���OUI������
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
            // Ui����
            PlayerTextGeneration(players);
        }
    }

    public void PlayerTextGeneration(Player players)
    {
        // UI����
        Text newPlayerText = Instantiate(playerNameText);

        newPlayerText.text = players.NickName;

        //�e�̐ݒ�
        newPlayerText.transform.SetParent(playerNameContent.transform);

        // ���X�g�ɓo�^
        allPlayerNames.Add(newPlayerText);
    }

    /// <summary>
    /// ���O�����͍ς݂��m�F����UI�X�V
    /// </summary>
    private void ConfirmationName()
    {
        // ���O���ݒ肳��Ă��Ȃ�
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
    /// ���O�o�^�֐�
    /// </summary>
    public void SetName()
    {
        // ���̓t�B�[���h�ɕ��������邩
        if(!string.IsNullOrEmpty(nameInput.text))
        {
            PhotonNetwork.NickName = nameInput.text;

            // �ۑ�
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
