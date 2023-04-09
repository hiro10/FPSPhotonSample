using UnityEngine;
using Photon.Pun;

public class SpownManager : MonoBehaviour
{
    // �X�|�[���|�C���g�i�[�z��
    public Transform[] spownPoints;

    // ��������v���C���[�I�u�W�F�N�g
    [SerializeField] private GameObject playerPrefab;

    // ���������v���C���[�I�u�W�F�N�g
    private GameObject player;

    // �X�|�[���܂ł̃C���^�[�o��
    public float respownInterval = 5f;

    /// <summary>
    /// �J�n����
    /// </summary>
    private void Start()
    {
        foreach(Transform position in spownPoints)
        {
            position.gameObject.SetActive(false);
        }

        // �����֐��Ăяo��
        if(PhotonNetwork.IsConnected)
        {
            SpownPlayer();
        }
    }

    /// <summary>
    /// �����_���ɃX�|�[���|�C���g�̈��I��
    /// </summary>
    public Transform GetSpownPoint()
    {
        return spownPoints[Random.Range(0, spownPoints.Length)];
    }

    /// <summary>
    /// �v���C���[�𐶐�����
    /// </summary>
    private void SpownPlayer()
    {
        // �����_���ȃX�|�[���ʒu
        Transform spownPoint = GetSpownPoint();

        // �l�b�g���[�N�I�u�W�F�N�g�̐���
        player = PhotonNetwork.Instantiate(playerPrefab.name, spownPoint.position, spownPoint.rotation);
    }
}
