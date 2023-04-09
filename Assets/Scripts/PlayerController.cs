using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class PlayerController : MonoBehaviourPunCallbacks
{
    // カメラの親オブジェクト
    public Transform viewPoint;

    // 視点移動速度
    public float mouseSensitivity = 1f;

    // ユーザーのマウス入力格納
    private Vector2 mouseInput;

    // y軸回転格納用
    private float verticalMouseInput;

    // カメラか効能
    private Camera cam;

    // 入力値格納
    private Vector3 moveDirection;

    // 進む方向格納
    private Vector3 movement;

    // 実際の移動速度
    private float activeMoveSpeed = 4f;

    // ジャンプ力
    public Vector3 jumpForce = new Vector3(0, 6, 0);

    // レイを飛ばすオブジェクト
    public Transform groundCheakPoint;

    // 地面レイヤー
    public LayerMask groundLayer;

    // リジッドボディ
    Rigidbody rigidbody;

    // 歩きの速度
    public float walkSpeed = 4f;

    // 走りの速度
    public float runSpeed = 8f;

    // カーソルの表示判定
    private bool cursurLock = true;

    // 武器格納リスト
    public List<Gun> guns = new List<Gun>();

    // 選択中の武器
    private int selectedGun = 0;

    // 射撃間隔
    private float shotTimer;

    // 所持弾薬
    [Tooltip("所持弾薬")]
    public int[] ammunition;

    // 最高所持弾薬
    [Tooltip("最高所持弾薬")]
    public int[] maxAmmunition;
    
    // マガジン内の段数
    [Tooltip("マガジン内の段数")]
    public int[] ammoCLip;
    
    // マガジンに入る最大の数
    [Tooltip("マガジンに入る最大の数")]
    public int[] maxAmmoClip;

    private UiManager uiManager;

    private SpownManager spownManager;

    private void Awake()
    {
        spownManager = GameObject.FindGameObjectWithTag("SpownManager").GetComponent<SpownManager>();
        uiManager = GameObject.FindGameObjectWithTag("UiManager").GetComponent<UiManager>();
    }

    /// <summary>
    /// 開始処理
    /// </summary>
    private void Start()
    {
        rigidbody = GetComponent<Rigidbody>();
        // カメラの取得
        cam = Camera.main;

        UpdateCurSurLock();

    }

    /// <summary>
    /// 更新処理
    /// </summary>
    private void Update()
    {
        // このオブジェクトの管理者が自分だたらtrueが帰る
        if(!photonView.IsMine)
        {
            return;
        }

        //視点移動処理
        PlayerRotate();

        // 移動関数
        PlayerMove();

        if (isGround())
        {
            // 走る関数
            Run();

            // ジャンプ関数
            Jump();
        }
        // カーソル表示関数
        UpdateCurSurLock();

        // 武器の変更キー感知関数
        SwitchingGuns();

        // 覗き込み
        Aim();

        // 射撃ボタン検知
        Fire();

        // リロード関数
        Reload();
    }

    private void FixedUpdate()
    {
        // このオブジェクトの管理者が自分だたらtrueが帰る
        if (!photonView.IsMine)
        {
            return;
        }

        // テキストの更新関数の呼び出し
        uiManager.SettingBulletsText(ammoCLip[selectedGun], ammunition[selectedGun]);
    }

    /// <summary>
    /// 視点移動用関数
    /// </summary>
    public void PlayerRotate()
    {
        // 変数にユーザーのマウスの動きを格納
        mouseInput = new Vector2(Input.GetAxisRaw("Mouse X")*mouseSensitivity,
            Input.GetAxisRaw("Mouse Y") * mouseSensitivity);

        // プレイヤーに反映(eulerAngle = オイラー角として返される)
        transform.rotation = Quaternion.Euler(transform.eulerAngles.x,
            transform.eulerAngles.y + mouseInput.x,
            transform.eulerAngles.z);

        // y軸の値に現在の値を足す
        verticalMouseInput += mouseInput.y;

        // 数値を丸める(上限と下限の設定)
        verticalMouseInput = Mathf.Clamp(verticalMouseInput,-60f,60f);

        //viewPointに丸めた数値を反映
        viewPoint.rotation = Quaternion.Euler(-verticalMouseInput, viewPoint.transform.rotation.eulerAngles.y, transform.rotation.eulerAngles.z);
    }

    /// <summary>
    /// カメラの位置の調整
    /// </summary>
    private void LateUpdate()
    {
        // このオブジェクトの管理者が自分だたらtrueが帰る
        if (!photonView.IsMine)
        {
            return;
        }

        cam.transform.position = viewPoint.position;

        // 回転の反映
        cam.transform.rotation = viewPoint.rotation;
    }

    /// <summary>
    /// プレイヤーの移動処理
    /// </summary>
    public void PlayerMove()
    {
        // 移動用のキー入力を検知して値を格納
        moveDirection = new Vector3(Input.GetAxisRaw("Horizontal"),0f, Input.GetAxisRaw("Vertical"));

        // 進む方向を変数に格納
        movement = ((transform.forward * moveDirection.z) + (transform.right * moveDirection.x)).normalized;

        // 現在地に反映
        transform.position += movement * activeMoveSpeed * Time.deltaTime;
    }

    /// <summary>
    /// ジャンプ関数
    /// </summary>
    public void Jump()
    {
        // 地面についていてjumpのボタンが押されたとき
        if(Input.GetKeyDown(KeyCode.Space))
        {
            // ForceMode.Impulseは瞬時に力を加える
            rigidbody.AddForce(jumpForce,ForceMode.Impulse);
        }
    }

    /// <summary>
    /// 地面判定
    /// </summary>
    /// <returns>true 地面にいる　false 地面にいない</returns>
    public bool isGround()
    {
        //判定してBOOLを返す(どこから飛ばすか,どの方向に(真下), 長さ,地面レイヤー)
        return Physics.Raycast(groundCheakPoint.position, Vector3.down, 0.25f,groundLayer);
    }

    /// <summary>
    /// 走る処理
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
    /// カーソルの表示非表示処理
    /// </summary>
    private void UpdateCurSurLock()
    {
        if(Input.GetKeyDown(KeyCode.Escape))
        {
            // 表示
            cursurLock = false;
        }
        else if(Input.GetMouseButton(0))
        {
            // カーソル非表示
            cursurLock = true;
        }

        if(cursurLock)
        {
            // カーソルを中央に固定し非表示
            Cursor.lockState = CursorLockMode.Locked;
        }
        else
        {
            Cursor.lockState = CursorLockMode.None;
        }
    }

    /// <summary>
    /// 武器の変更
    /// </summary>
    private void SwitchingGuns()
    {
        // ホイールで銃の切り替え
        if (Input.GetAxisRaw("Mouse ScrollWheel") > 0f) 
        {
            selectedGun++;

            // 銃のリストがオーバーしてないか
            if (selectedGun >= guns.Count)
            {
                selectedGun = 0;
            }

            // 銃を切り替える関数
            SwitchGun();
        }
        else if (Input.GetAxisRaw("Mouse ScrollWheel") < 0f)
        {
            selectedGun--;

            // 銃のリストがオーバーしてないか
            if (selectedGun <0)
            {
                selectedGun = guns.Count-1;
            }
            // 銃を切り替える関数
            SwitchGun();
        }

        // 数値キーの場合
        for (int i = 0; i < guns.Count; i++)
        {
            // 数値キーを押したか
            if(Input.GetKeyDown((i+1).ToString()))// 1キー
            {
                // 銃を切り替える関数
                selectedGun = i;
                SwitchGun();
            }
        }

    }

    /// <summary>
    /// 銃の切り替え
    /// </summary>
    private void  SwitchGun()
    {
        foreach(Gun gun in guns)
        {
            gun.gameObject.SetActive(false);
        }
        // リストにある特定の銃が表示される
        guns[selectedGun].gameObject.SetActive(true);
    }

    /// <summary>
    /// 覗き込みの処理
    /// </summary>
    private void Aim()
    {
        // 右クリックの検知
        if(Input.GetMouseButton(1))
        {
            //guns[selectedGun].GunLockOn(true);
            // 開始値から目的値まで補完数値分近づく
            cam.fieldOfView = Mathf.Lerp(cam.fieldOfView, 
                guns[selectedGun].adsZoom, 
                guns[selectedGun].adsSpeed * Time.deltaTime);
        }
        else
        {
            //guns[selectedGun].GunLockOn(false);
            // 開始値から目的値まで補完数値分近づく
            cam.fieldOfView = Mathf.Lerp(cam.fieldOfView,
                60f,
                guns[selectedGun].adsSpeed * Time.deltaTime);
        }

    }

    /// <summary>
    /// 射撃ボタンの検知
    /// </summary>
    private void Fire()
    {
        // 打てるか
        if (Input.GetMouseButton(0) && ammoCLip[selectedGun] > 0 && Time.time > shotTimer)
        {
            // 弾を打ち出す関数
            FiringBullet();
        }
    }

    /// <summary>
    /// 弾を打ち出す
    /// </summary>
    public void FiringBullet()
    {
        // 弾を減らす
        ammoCLip[selectedGun]--;

        // 光線を作る
        Ray ray = cam.ViewportPointToRay(new Vector2(0.5f, 0.5f));

        if (Physics.Raycast(ray,out RaycastHit hit))
        {
            //Debug.Log("当たったオブジェクト" + hit.collider.gameObject.name);
            // 弾痕
            GameObject bulletInpactObject = Instantiate(guns[selectedGun].bulletInpact, hit.point+(hit.normal*0.02f), Quaternion.LookRotation(hit.normal,Vector3.up));

            Destroy(bulletInpactObject, 10f);
        }
        // ゲームの経過時間にインターバル時間を足している（連続で打てなくするため）
        shotTimer = Time.time + guns[selectedGun].shotInterval;
    }

    /// <summary>
    /// リロード関数
    /// </summary>
    private void Reload()
    {
        // リロードボタンが押されたか
        if(Input.GetKeyDown(KeyCode.R))
        {
            // リロードで補充する弾数を取得
            int amountNeed = maxAmmoClip[selectedGun] - ammoCLip[selectedGun];

            int ammoAve = amountNeed < ammunition[selectedGun] ? amountNeed : ammunition[selectedGun];

            if (amountNeed != 0 && ammunition[selectedGun] != 0) 
            {
                // 所持弾薬から減らす
                ammunition[selectedGun] -= ammoAve;

                // 弾薬の争点
                ammoCLip[selectedGun] += ammoAve;
            }
        }
    }
}