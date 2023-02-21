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

    // ���[�h�p�l���i�[�p
    [SerializeField] GameObject loadPanel;

    // ���[�h�e�L�X�g
    [SerializeField] TextMeshProUGUI loadText;

    // �{�^���̐e�I�u�W�F�N�g
    [SerializeField] GameObject buttons;

    // 

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

    }
}
