using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class PlayerController : MonoBehaviourPunCallbacks
{
    // �J�����̐e�I�u�W�F�N�g
    public Transform viewPoint;

    // ���_�ړ����x
    public float mouseSensitivity = 1f;

    // ���[�U�[�̃}�E�X���͊i�[
    private Vector2 mouseInput;

    // y����]�i�[�p
    private float verticalMouseInput;

    // �J���������\
    private Camera cam;

    // ���͒l�i�[
    private Vector3 moveDirection;

    // �i�ޕ����i�[
    private Vector3 movement;

    // ���ۂ̈ړ����x
    private float activeMoveSpeed = 4f;

    // �W�����v��
    public Vector3 jumpForce = new Vector3(0, 6, 0);

    // ���C���΂��I�u�W�F�N�g
    public Transform groundCheakPoint;

    // �n�ʃ��C���[
    public LayerMask groundLayer;

    // ���W�b�h�{�f�B
    Rigidbody rigidbody;

    // �����̑��x
    public float walkSpeed = 4f;

    // ����̑��x
    public float runSpeed = 8f;

    // �J�[�\���̕\������
    private bool cursurLock = true;

    // ����i�[���X�g
    public List<Gun> guns = new List<Gun>();

    // �I�𒆂̕���
    private int selectedGun = 0;

    // �ˌ��Ԋu
    private float shotTimer;

    // �����e��
    [Tooltip("�����e��")]
    public int[] ammunition;

    // �ō������e��
    [Tooltip("�ō������e��")]
    public int[] maxAmmunition;
    
    // �}�K�W�����̒i��
    [Tooltip("�}�K�W�����̒i��")]
    public int[] ammoCLip;
    
    // �}�K�W���ɓ���ő�̐�
    [Tooltip("�}�K�W���ɓ���ő�̐�")]
    public int[] maxAmmoClip;

    private UiManager uiManager;

    private SpownManager spownManager;

    private void Awake()
    {
        spownManager = GameObject.FindGameObjectWithTag("SpownManager").GetComponent<SpownManager>();
        uiManager = GameObject.FindGameObjectWithTag("UiManager").GetComponent<UiManager>();
    }

    /// <summary>
    /// �J�n����
    /// </summary>
    private void Start()
    {
        rigidbody = GetComponent<Rigidbody>();
        // �J�����̎擾
        cam = Camera.main;

        UpdateCurSurLock();

    }

    /// <summary>
    /// �X�V����
    /// </summary>
    private void Update()
    {
        // ���̃I�u�W�F�N�g�̊Ǘ��҂�����������true���A��
        if(!photonView.IsMine)
        {
            return;
        }

        //���_�ړ�����
        PlayerRotate();

        // �ړ��֐�
        PlayerMove();

        if (isGround())
        {
            // ����֐�
            Run();

            // �W�����v�֐�
            Jump();
        }
        // �J�[�\���\���֐�
        UpdateCurSurLock();

        // ����̕ύX�L�[���m�֐�
        SwitchingGuns();

        // �`������
        Aim();

        // �ˌ��{�^�����m
        Fire();

        // �����[�h�֐�
        Reload();
    }

    private void FixedUpdate()
    {
        // ���̃I�u�W�F�N�g�̊Ǘ��҂�����������true���A��
        if (!photonView.IsMine)
        {
            return;
        }

        // �e�L�X�g�̍X�V�֐��̌Ăяo��
        uiManager.SettingBulletsText(ammoCLip[selectedGun], ammunition[selectedGun]);
    }

    /// <summary>
    /// ���_�ړ��p�֐�
    /// </summary>
    public void PlayerRotate()
    {
        // �ϐ��Ƀ��[�U�[�̃}�E�X�̓������i�[
        mouseInput = new Vector2(Input.GetAxisRaw("Mouse X")*mouseSensitivity,
            Input.GetAxisRaw("Mouse Y") * mouseSensitivity);

        // �v���C���[�ɔ��f(eulerAngle = �I�C���[�p�Ƃ��ĕԂ����)
        transform.rotation = Quaternion.Euler(transform.eulerAngles.x,
            transform.eulerAngles.y + mouseInput.x,
            transform.eulerAngles.z);

        // y���̒l�Ɍ��݂̒l�𑫂�
        verticalMouseInput += mouseInput.y;

        // ���l���ۂ߂�(����Ɖ����̐ݒ�)
        verticalMouseInput = Mathf.Clamp(verticalMouseInput,-60f,60f);

        //viewPoint�Ɋۂ߂����l�𔽉f
        viewPoint.rotation = Quaternion.Euler(-verticalMouseInput, viewPoint.transform.rotation.eulerAngles.y, transform.rotation.eulerAngles.z);
    }

    /// <summary>
    /// �J�����̈ʒu�̒���
    /// </summary>
    private void LateUpdate()
    {
        // ���̃I�u�W�F�N�g�̊Ǘ��҂�����������true���A��
        if (!photonView.IsMine)
        {
            return;
        }

        cam.transform.position = viewPoint.position;

        // ��]�̔��f
        cam.transform.rotation = viewPoint.rotation;
    }

    /// <summary>
    /// �v���C���[�̈ړ�����
    /// </summary>
    public void PlayerMove()
    {
        // �ړ��p�̃L�[���͂����m���Ēl���i�[
        moveDirection = new Vector3(Input.GetAxisRaw("Horizontal"),0f, Input.GetAxisRaw("Vertical"));

        // �i�ޕ�����ϐ��Ɋi�[
        movement = ((transform.forward * moveDirection.z) + (transform.right * moveDirection.x)).normalized;

        // ���ݒn�ɔ��f
        transform.position += movement * activeMoveSpeed * Time.deltaTime;
    }

    /// <summary>
    /// �W�����v�֐�
    /// </summary>
    public void Jump()
    {
        // �n�ʂɂ��Ă���jump�̃{�^���������ꂽ�Ƃ�
        if(Input.GetKeyDown(KeyCode.Space))
        {
            // ForceMode.Impulse�͏u���ɗ͂�������
            rigidbody.AddForce(jumpForce,ForceMode.Impulse);
        }
    }

    /// <summary>
    /// �n�ʔ���
    /// </summary>
    /// <returns>true �n�ʂɂ���@false �n�ʂɂ��Ȃ�</returns>
    public bool isGround()
    {
        //���肵��BOOL��Ԃ�(�ǂ������΂���,�ǂ̕�����(�^��), ����,�n�ʃ��C���[)
        return Physics.Raycast(groundCheakPoint.position, Vector3.down, 0.25f,groundLayer);
    }

    /// <summary>
    /// ���鏈��
    /// </summary>
    public void Run()
    {
        if(Input.GetKey(KeyCode.LeftShift))
        {
            activeMoveSpeed = runSpeed;
        }
        else
        {
            activeMoveSpeed = walkSpeed;
        }
    }

    /// <summary>
    /// �J�[�\���̕\����\������
    /// </summary>
    private void UpdateCurSurLock()
    {
        if(Input.GetKeyDown(KeyCode.Escape))
        {
            // �\��
            cursurLock = false;
        }
        else if(Input.GetMouseButton(0))
        {
            // �J�[�\����\��
            cursurLock = true;
        }

        if(cursurLock)
        {
            // �J�[�\���𒆉��ɌŒ肵��\��
            Cursor.lockState = CursorLockMode.Locked;
        }
        else
        {
            Cursor.lockState = CursorLockMode.None;
        }
    }

    /// <summary>
    /// ����̕ύX
    /// </summary>
    private void SwitchingGuns()
    {
        // �z�C�[���ŏe�̐؂�ւ�
        if (Input.GetAxisRaw("Mouse ScrollWheel") > 0f) 
        {
            selectedGun++;

            // �e�̃��X�g���I�[�o�[���ĂȂ���
            if (selectedGun >= guns.Count)
            {
                selectedGun = 0;
            }

            // �e��؂�ւ���֐�
            SwitchGun();
        }
        else if (Input.GetAxisRaw("Mouse ScrollWheel") < 0f)
        {
            selectedGun--;

            // �e�̃��X�g���I�[�o�[���ĂȂ���
            if (selectedGun <0)
            {
                selectedGun = guns.Count-1;
            }
            // �e��؂�ւ���֐�
            SwitchGun();
        }

        // ���l�L�[�̏ꍇ
        for (int i = 0; i < guns.Count; i++)
        {
            // ���l�L�[����������
            if(Input.GetKeyDown((i+1).ToString()))// 1�L�[
            {
                // �e��؂�ւ���֐�
                selectedGun = i;
                SwitchGun();
            }
        }

    }

    /// <summary>
    /// �e�̐؂�ւ�
    /// </summary>
    private void  SwitchGun()
    {
        foreach(Gun gun in guns)
        {
            gun.gameObject.SetActive(false);
        }
        // ���X�g�ɂ������̏e���\�������
        guns[selectedGun].gameObject.SetActive(true);
    }

    /// <summary>
    /// �`�����݂̏���
    /// </summary>
    private void Aim()
    {
        // �E�N���b�N�̌��m
        if(Input.GetMouseButton(1))
        {
            //guns[selectedGun].GunLockOn(true);
            // �J�n�l����ړI�l�܂ŕ⊮���l���߂Â�
            cam.fieldOfView = Mathf.Lerp(cam.fieldOfView, 
                guns[selectedGun].adsZoom, 
                guns[selectedGun].adsSpeed * Time.deltaTime);
        }
        else
        {
            //guns[selectedGun].GunLockOn(false);
            // �J�n�l����ړI�l�܂ŕ⊮���l���߂Â�
            cam.fieldOfView = Mathf.Lerp(cam.fieldOfView,
                60f,
                guns[selectedGun].adsSpeed * Time.deltaTime);
        }

    }

    /// <summary>
    /// �ˌ��{�^���̌��m
    /// </summary>
    private void Fire()
    {
        // �łĂ邩
        if (Input.GetMouseButton(0) && ammoCLip[selectedGun] > 0 && Time.time > shotTimer)
        {
            // �e��ł��o���֐�
            FiringBullet();
        }
    }

    /// <summary>
    /// �e��ł��o��
    /// </summary>
    public void FiringBullet()
    {
        // �e�����炷
        ammoCLip[selectedGun]--;

        // ���������
        Ray ray = cam.ViewportPointToRay(new Vector2(0.5f, 0.5f));

        if (Physics.Raycast(ray,out RaycastHit hit))
        {
            //Debug.Log("���������I�u�W�F�N�g" + hit.collider.gameObject.name);
            // �e��
            GameObject bulletInpactObject = Instantiate(guns[selectedGun].bulletInpact, hit.point+(hit.normal*0.02f), Quaternion.LookRotation(hit.normal,Vector3.up));

            Destroy(bulletInpactObject, 10f);
        }
        // �Q�[���̌o�ߎ��ԂɃC���^�[�o�����Ԃ𑫂��Ă���i�A���őłĂȂ����邽�߁j
        shotTimer = Time.time + guns[selectedGun].shotInterval;
    }

    /// <summary>
    /// �����[�h�֐�
    /// </summary>
    private void Reload()
    {
        // �����[�h�{�^���������ꂽ��
        if(Input.GetKeyDown(KeyCode.R))
        {
            // �����[�h�ŕ�[����e�����擾
            int amountNeed = maxAmmoClip[selectedGun] - ammoCLip[selectedGun];

            int ammoAve = amountNeed < ammunition[selectedGun] ? amountNeed : ammunition[selectedGun];

            if (amountNeed != 0 && ammunition[selectedGun] != 0) 
            {
                // �����e�򂩂猸�炷
                ammunition[selectedGun] -= ammoAve;

                // �e��̑��_
                ammoCLip[selectedGun] += ammoAve;
            }
        }
    }
}