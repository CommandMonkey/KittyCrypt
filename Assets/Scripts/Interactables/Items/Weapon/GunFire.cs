using Cinemachine;
using UnityEngine;
using UnityEngine.Rendering.Universal;
using UnityEngine.InputSystem;
using System.Collections;
using TMPro;
using UnityEngine.UI;

public class GunFire : MonoBehaviour
{
    //Configurable parameters damage
    [Header("General Options")]
    [SerializeField] WeaponSettingsObject settings;



    //Cached references

    TMP_Text ammoUI;

    public class GunFireRuntimeData
    {
        public GunFireRuntimeData(int bulletsFired, float reloadTimer)
        {
            this.bulletsFired = bulletsFired;
        }
        public int bulletsFired;
        public float reloadTimer;
        public float fireRateCooldownTimer;
        public bool fireRateCoolingDown;
        public bool reloading;
    }

    //Private variables
    GunFireRuntimeData runtimeData;

    

    Quaternion randomBulletSpread;
    RaycastHit2D bulletHit;

    UserInput userInput;
    Player player;
    Animator virtualCameraAnimator;
    AudioSource gunSource;
    Light2D nuzzleLight;
    Image reloadImage;

    private void Start()
    {
        ammoUI = GameObject.FindGameObjectWithTag("AmmoRemainingText").GetComponent<TMP_Text>();
        
        reloadTimer = settings.reloadTime;
        fireRateCooldownTimer = settings.fireRate;

        userInput = FindObjectOfType<UserInput>();
        player = FindObjectOfType<Player>();
        virtualCameraAnimator = FindObjectOfType<CinemachineVirtualCamera>().GetComponent<Animator>();
        gunSource = FindObjectOfType<LevelManager>().gameObject.GetComponent<AudioSource>();
        nuzzleLight = GetComponent<Light2D>();
        reloadImage = player.reloadCircle.GetComponent<Image>();

        userInput.onReload.AddListener(Reload);
        userInput.onFire.AddListener(OnFire);

        if (nuzzleLight != null)
        {
            nuzzleLight.enabled = false;
        }

    }

    // Update is called once per frame
    void Update()
    {
        if (player.isDead) {  return; }
        if (ShouldReload() && !reloading) Reload();
        FireRateCooldown();
    }

    void OnFire()
    {
        if (ProjectileFire()) return;
        else if (BurstFire()) return;
        else if (RaycastFire()) return;
    }

    private void OnDisable()
    {
        player.reloadCircle.gameObject.SetActive(false);
    }

    bool ProjectileFire()
    {
        if(settings.weaponType != WeaponSettingsObject.WeaponType.ProjectileFire || fireRateCoolingDown || reloading) { return false; }
        Debug.Log("Fire");

        // Fire
        ShootBullet();
        GunFeedbackEffects();
        fireRateCoolingDown = true;


        // Ammo
        SetAmmoUI();

        return true;
    }

    bool BurstFire()
    {
        if(settings.weaponType != WeaponSettingsObject.WeaponType.BurstFire || fireRateCoolingDown || reloading) { return false; }
        Debug.Log("Fire");

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
        if (settings.weaponType != WeaponSettingsObject.WeaponType.RaycastFire || fireRateCoolingDown || reloading) { return false; }
        Debug.Log("Fire");

        // Fire
        bulletHit = Physics2D.Raycast(transform.position, transform.right, Mathf.Infinity, ~settings.ignoreLayerMask);
        if (bulletHit)
        {
            bulletsFired++;
            GunFeedbackEffects();
            fireRateCoolingDown = true;

            var smoke = Instantiate(settings.hitEffect, bulletHit.point, Quaternion.identity);
            Destroy(smoke, settings.destroyHitEffectAfter);

            var line = Instantiate(settings.bulletTrail, transform.position, Quaternion.identity);
            Destroy(line, settings.destroyTrailAfter);

            if (bulletHit.collider.gameObject.transform.tag == "Enemy")
            {
                RushingEnemyBehavior enemyScript = bulletHit.collider.gameObject.GetComponent<RushingEnemyBehavior>();

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
        bulletsFired++;
    }

    void GunFeedbackEffects()
    {
        gunSource.PlayOneShot(settings.fireAudio);
        virtualCameraAnimator.SetTrigger("CameraShake");
        player.exteriorVelocity += -(Vector2)transform.right * settings.knockback;

        if (nuzzleLight == null) { return; }
        StopCoroutine(NuzzleFlash());
        StartCoroutine(NuzzleFlash());
    }

    bool ShouldReload()
    {
        if(settings.bulletsBeforeReload == 0) { return false; }

        if (bulletsFired >= settings.bulletsBeforeReload)
            return true;
        else
            return false;
    }

    void FireRateCooldown()
    {
        if(!fireRateCoolingDown) { return; }

        fireRateCooldownTimer -= Time.deltaTime;
        if(fireRateCooldownTimer <= 0 )
        {
            fireRateCoolingDown = false;
            fireRateCooldownTimer = settings.fireRate;
        }
    }

    void Reload()
    {
        if (!reloading && bulletsFired != 0)
        {
            reloading = true;
            StartCoroutine(ReloadRoutine());
        }

    }

    IEnumerator ReloadRoutine()
    {

        // Play Reload SFX
        gunSource.PlayOneShot(settings.reloadAudio);
        reloadAudioPlayed = true;

        // Update Reload Animation circle
        while (reloading) 
        {
            player.reloadCircle.SetActive(true);
            reloadImage.fillAmount = 1f - (reloadTimer / settings.reloadTime);
            reloadTimer -= Time.deltaTime;
            if (reloadTimer <= 0)
            {
                OnReloadDone();
                break;
            }
            yield return new WaitForEndOfFrame();  
        }


        void OnReloadDone()
        {
            // Reset Values
            bulletsFired = 0;
            reloading = false;
            reloadTimer = settings.reloadTime;
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
        ammoUI.text = (settings.bulletsBeforeReload - bulletsFired).ToString() + "/" + settings.bulletsBeforeReload.ToString() ;
    }

    void SetAmmoBurstUI()
    {
        ammoUI.text = Mathf.Max(0, (1 - bulletsFired / settings.bulletsBeforeReload)).ToString() + "/" + 1.ToString();
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
}
