using System.Collections;
using Cinemachine;
using TMPro;
using UnityEngine;
using UnityEngine.Rendering.Universal;
using UnityEngine.UI;

public class GunFire : IItem
{
    //Configurable parameters damage
    [Header("General Options")] [SerializeField]
    WeaponSettingsObject settings;

    public bool active = false;


    //Cached reference
    TMP_Text ammoUI;

    [System.Serializable]
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

    //Private variables
    public GunFireRuntimeData runtimeData;

    Quaternion randomBulletSpread;
    RaycastHit2D bulletHit;

    UserInput userInput;
    LevelManager levelManager;
    Player player;
    Animator virtualCameraAnimator;
    AudioSource gunSource;
    Light2D nuzzleLight;
    Image reloadImage;
    

    private void Start()
    {
        ammoUI = GameObject.FindGameObjectWithTag("AmmoRemainingText").GetComponent<TMP_Text>();

        userInput = FindObjectOfType<UserInput>();
        levelManager = FindObjectOfType<LevelManager>();
        player = levelManager.player;
        virtualCameraAnimator = FindObjectOfType<CinemachineVirtualCamera>().GetComponent<Animator>();
        gunSource = FindObjectOfType<LevelManager>().gameObject.GetComponent<AudioSource>();
        nuzzleLight = GetComponent<Light2D>();
        reloadImage = player.reloadCircle.GetComponent<Image>();

        Activate();

        if (runtimeData == null)
        {
            runtimeData = new GunFireRuntimeData(0, settings.reloadTime, settings.fireRate, false, false);
        }

        if (nuzzleLight != null)
        {
            nuzzleLight.enabled = false;
        }
    }

    // Update is called once per frame
    void Update()
    {
        //if (player.isDead) {  return; }
        if (ShouldReload() && !runtimeData.isReloading) Reload();
        FireRateCooldown();
    }

    void OnFire()
    {
        if (levelManager.state != LevelManager.LevelState.Running || runtimeData.isFireRateCoolingDown || runtimeData.isReloading) return;   
        else if (ProjectileFire()) return;
        else if (BurstFire()) return;
        else if (RaycastFire()) return;
    }

    private void OnDisable()
    {
        player.reloadCircle.gameObject.SetActive(false);
    }

    bool ProjectileFire()
    {
        if (settings.weaponType != WeaponSettingsObject.WeaponType.ProjectileFire)
        {
            return false;
        }

        Debug.Log("Fire");

        // Fire
        ShootBullet();
        GunFeedbackEffects();
        runtimeData.isFireRateCoolingDown = true;


        // Ammo
        SetAmmoUI();

        return true;
    }

    bool BurstFire()
    {
        if (settings.weaponType != WeaponSettingsObject.WeaponType.BurstFire) return false;

        // Fire
        for (int i = 0; i < settings.bulletsBeforeReload || settings.bulletsBeforeReload == 0; i++)
        {
            ShootBullet();
        }

        GunFeedbackEffects();

        //Ammo
        SetAmmoBurstUI();

        return true;
    }

    bool RaycastFire()
    {
        if (settings.weaponType != WeaponSettingsObject.WeaponType.RaycastFire)
        {
            return false;
        }

        Debug.Log("Fire");

        // Fire
        bulletHit = Physics2D.Raycast(transform.position, transform.right, Mathf.Infinity, ~settings.ignoreLayerMask);
        if (bulletHit)
        {
            runtimeData.bulletsFired++;
            GunFeedbackEffects();
            runtimeData.isFireRateCoolingDown = true;

            var smoke = Instantiate(settings.hitEffect, bulletHit.point, Quaternion.identity);
            Destroy(smoke, settings.destroyHitEffectAfter);

            var line = Instantiate(settings.bulletTrail, transform.position, Quaternion.identity);
            Destroy(line, settings.destroyTrailAfter);

            Enemy enemyScript = bulletHit.collider.gameObject.GetComponent<Enemy>();

            if (enemyScript != null)
            {
                enemyScript.TakeDamage(settings.bulletDamage);
            }
        }

        // Ammo
        SetAmmoUI();

        return true;
    }

    void ShootBullet()
    {
        Debug.Log("SHOOOOOOT HAJ");
        randomBulletSpread = GetBulletSpread();
        Bullet bullet = Instantiate(settings.projectile, transform.position, randomBulletSpread).GetComponent<Bullet>();
        bullet.Initialize(settings.bulletSpeed, settings.bulletDamage, settings.hitEffect);
        runtimeData.bulletsFired++;
    }

    void GunFeedbackEffects()
    {
        gunSource.PlayOneShot(settings.fireAudio);
        virtualCameraAnimator.SetTrigger("CameraShake");
        player.exteriorVelocity += -(Vector2)transform.right * settings.knockback;

        if (nuzzleLight == null)
        {
            return;
        }

        StopCoroutine(NuzzleFlash());
        StartCoroutine(NuzzleFlash());
    }

    bool ShouldReload()
    {
        if (settings.bulletsBeforeReload == 0)
        {
            return false;
        }

        if (runtimeData.bulletsFired >= settings.bulletsBeforeReload)
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

    void Reload()
    {
        if (!runtimeData.isReloading && runtimeData.bulletsFired != 0)
        {
            runtimeData.isReloading = true;
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
            player.reloadCircle.SetActive(true);
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
            player.reloadCircle.SetActive(false);

            // Ammo Text
            if (settings.weaponType == WeaponSettingsObject.WeaponType.BurstFire)
                SetAmmoBurstUI();
            else
                SetAmmoUI();
        }
    }

    IEnumerator NuzzleFlash()
    {
        nuzzleLight.enabled = true;
        yield return new WaitForSeconds(0.05f);
        nuzzleLight.enabled = false;
    }

    void SetAmmoUI()
    {
        ammoUI.text = (settings.bulletsBeforeReload - runtimeData.bulletsFired).ToString() + "/" +
                      settings.bulletsBeforeReload.ToString();
    }

    void SetAmmoBurstUI()
    {
        ammoUI.text = Mathf.Max(0, (1 - runtimeData.bulletsFired / settings.bulletsBeforeReload)).ToString() + "/" +
                      1.ToString();
    }

    public Vector3 GetBulletHitPoint()
    {
        return bulletHit.point;
    }

    public GameObject GetHitEffect()
    {
        return settings.hitEffect;
    }

    public LayerMask GetIgnoredLayers()
    {
        return settings.ignoreLayerMask;
    }

    public float GetDestroyHitEffectAfter()
    {
        return settings.destroyHitEffectAfter;
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

    public override void Activate()
    {
        if (userInput == null) userInput = FindObjectOfType<UserInput>();
        userInput.onReload.AddListener(Reload);
        userInput.onFireEvent.AddListener(OnFire);
    }

    public override void DeActivate()
    {
        ResetReloading();
        userInput.onFireEvent.RemoveListener(OnFire);
        userInput.onReload.RemoveListener(Reload);
    }

    void ResetReloading()
    {
        StopCoroutine(ReloadRoutine());
        runtimeData.isReloading = false;
        runtimeData.reloadTimer = settings.reloadTime;
        player.reloadCircle.SetActive(false);
    }
}

