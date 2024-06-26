using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Rendering.Universal;
using UnityEngine.UI;

public class GunFire<T> : Item where T : GunSettingsObject
{
    //Configurable parameters damage
    [Header("General Options")]
    [SerializeField]
    protected T settings;

    public class GunFireRuntimeData
    {
        public GunFireRuntimeData(int bulletsFired, float reloadTimer, float fireRateCooldownTimer,
                                  bool isFireRateCoolingDown, bool isReloading)
        {
            this.bulletsFired = bulletsFired;
            this.reloadTimer = reloadTimer;
            this.fireRateCooldownTimer = fireRateCooldownTimer;
            this.isFireRateCoolingDown = isFireRateCoolingDown;
            this.isReloading = isReloading;
        }

        public int bulletsFired;
        public float reloadTimer;
        public float fireRateCooldownTimer;
        public bool isFireRateCoolingDown;
        public bool isReloading;
    }

    // Private variables
    protected GunFireRuntimeData runtimeData;
    Quaternion randomBulletSpread;

    // Cached reference
    protected GameSession gameSession;
    Player player;
    UserInput userInput;
    PlayerInput playerInput;
    Crosshair crosshair;
    AudioSource gunSource;

    UICanvas uiCanvas;
    protected TMP_Text ammoUI;

    protected Light2D nuzzleLight;




    protected void Start()
    {
        gameSession = GameSession.Instance;
        player = gameSession.Player;
        userInput = gameSession.userInput;
        playerInput = gameSession.playerInput;
        crosshair = gameSession.crosshair;
        gunSource = gameSession.GetComponent<AudioSource>();

        uiCanvas = FindObjectOfType<UICanvas>();
        ammoUI = uiCanvas.ammoText;

        nuzzleLight = GameHelper.GetComponentInAllChildren<Light2D>(transform);


        gameSession.onSceneloaded.AddListener(OnSceneLoaded);


        runtimeData = new GunFireRuntimeData(0, settings.reloadTime, settings.fireRate, false, false);

        Activate();

        if (nuzzleLight != null)
        {
            nuzzleLight.enabled = false;
        }
    }


    // Update is called once per frame
    protected void Update()
    {
        //if (player.isDead) {  return; }
        if (ShouldReload() && !runtimeData.isReloading) Reload();
        FireRateCooldown();

    }

    private void OnDestroy()
    {
        DeActivate();
    }

    void OnSceneLoaded()
    {
        gameSession = GameSession.Instance;
        uiCanvas = FindObjectOfType<UICanvas>();
        ammoUI = uiCanvas.ammoText;

        Debug.Log("NEW SCENE");

        DeActivate();
        Activate();
    }

    public override void Activate()
    {
        userInput = GameSession.Instance.userInput;
        userInput.onReload.AddListener(Reload);
        userInput.onFire.AddListener(OnFire);

        if (!runtimeData.isReloading)
        {
            gameSession.reloadCircle.gameObject.SetActive(false);
        }

        SetAmmoUI();
    }

    public override void DeActivate()
    {
        ResetReloading();
        runtimeData.isFireRateCoolingDown = false;
        userInput.onFire.RemoveListener(OnFire);
        userInput.onReload.RemoveListener(Reload);
    }

    void OnFire()
    {
        if (GameSession.state != GameSession.GameState.Running || runtimeData.isFireRateCoolingDown || runtimeData.isReloading || player.isDead) return;
        
        // Cat angry face
        if (!gameSession.playerIsShooting && gameObject.activeInHierarchy)
        {
            StartCoroutine(SetCatAngry());
        }

             

        WeaponFire();

        // Ammo
        SetAmmoUI();
    }


    protected void ShootBullet(GameObject projectile, float bulletSpeed)
    {
        randomBulletSpread = GetBulletSpread();
        Bullet bullet = Instantiate(projectile, transform.position, randomBulletSpread).GetComponent<Bullet>();
        bullet.Initialize(bulletSpeed, settings.damage, settings.hitEffect);
    }

    IEnumerator SetCatAngry()
    {
        gameSession.playerIsShooting = true;
        yield return new WaitForSeconds(0.01f);
        gameSession.playerIsShooting = false;
    }

    protected void GunFeedbackEffects()
    {
        gunSource.PlayOneShot(settings.fireAudio, settings.audioVolume);
        gameSession.GameCamera.DoCameraShake();
        player.exteriorVelocity += -(Vector2)transform.right * settings.playerKnockback;

        if (nuzzleLight == null)
        {
            return;
        }

        StopCoroutine(NuzzleFlash());
        StartCoroutine(NuzzleFlash());
    }

    bool ShouldReload()
    {
        if (settings.shotsBeforeReload == 0)
        {
            return false;
        }

        if (runtimeData.bulletsFired >= settings.shotsBeforeReload)
            return true;
        else
            return false;
    }

    void FireRateCooldown()
    {
        if (!runtimeData.isFireRateCoolingDown)
        {
            return;
        }

        runtimeData.fireRateCooldownTimer -= Time.deltaTime;
        if (runtimeData.fireRateCooldownTimer <= 0)
        {
            runtimeData.isFireRateCoolingDown = false;
            runtimeData.fireRateCooldownTimer = settings.fireRate;
        }
    }

    protected void Reload()
    {
        if (!runtimeData.isReloading && runtimeData.bulletsFired != 0)
        {
            runtimeData.isReloading = true;
            if (playerInput.currentControlScheme != "Keyboard and mouse")
            {
                crosshair.gameObject.SetActive(false);
            }
            else
            {
                Cursor.visible = false;
            }
            StartCoroutine(ReloadRoutine());
        }
    }

    IEnumerator ReloadRoutine()
    {
        // Play Reload SFX
        gunSource.PlayOneShot(settings.reloadAudio);

        // Update Reload Animation circle
        while (runtimeData.isReloading)
        {
            gameSession.reloadCircle.gameObject.SetActive(true);

            Image reloadImage = gameSession.reloadCircle.GetComponentInChildren<Image>();

            reloadImage.fillAmount = 1f - (runtimeData.reloadTimer / settings.reloadTime);
            runtimeData.reloadTimer -= Time.deltaTime;
            if (runtimeData.reloadTimer <= 0)
            {
                OnReloadDone();
                break;
            }

            yield return new WaitForEndOfFrame();
        }


        void OnReloadDone()
        {
            // Reset Values
            runtimeData.bulletsFired = 0;
            runtimeData.isReloading = false;
            runtimeData.reloadTimer = settings.reloadTime;
            if (playerInput.currentControlScheme != "Keyboard and mouse")
            {
                crosshair.gameObject.SetActive(true);
            }
            else
            {
                Cursor.visible = true;
            }
            gameSession.reloadCircle.gameObject.SetActive(false);
            SetAmmoUI();

        }
    }

    void ResetReloading()
    {
        StopCoroutine(ReloadRoutine());
        runtimeData.isReloading = false;
        runtimeData.reloadTimer = settings.reloadTime;
    }

    IEnumerator NuzzleFlash()
    {
        nuzzleLight.enabled = true;
        yield return new WaitForSeconds(0.05f);
        nuzzleLight.enabled = false;
    }


    private Quaternion GetBulletSpread()
    {
        Vector3 angles = transform.rotation.eulerAngles;
        Vector3 newAngles = new Vector3(
            angles.x,
            angles.y,
            angles.z + Random.Range(-settings.bulletSpreadRange, settings.bulletSpreadRange));
        return Quaternion.Euler(newAngles);
    }


    protected virtual void SetAmmoUI()
    {
        
        ammoUI.text = (settings.shotsBeforeReload - runtimeData.bulletsFired).ToString() + "/" +
                      settings.shotsBeforeReload.ToString();
        Debug.Log("Setting Ammo UI " + itemName + " " + ammoUI.text);
    }

    protected virtual void WeaponFire() { }
}

