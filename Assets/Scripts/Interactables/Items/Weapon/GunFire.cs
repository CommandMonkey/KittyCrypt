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

    //Private variables
    int bulletsFired;
    float reloadTimer;
    float fireRateCooldownTimer;
    bool fireRateCoolingDown = false;
    bool reloading = false;
    bool reloadAudioPlayed = false;

    PlayerInput playerInput;
    Player player;
    Animator virtualCameraAnimator;
    Quaternion randomBulletSpread;
    RaycastHit2D bulletHit;
    AudioSource gunSource;
    Light2D nuzzleLight;
    Image reloadImage;

    private void Start()
    {
        ammoUI = GameObject.FindGameObjectWithTag("AmmoRemainingText").GetComponent<TMP_Text>();
        
        reloadTimer = settings.reloadTime;
        fireRateCooldownTimer = settings.fireRate;
        playerInput = GetComponent<PlayerInput>();
        virtualCameraAnimator = FindObjectOfType<CinemachineVirtualCamera>().GetComponent<Animator>();
        gunSource = FindObjectOfType<LevelManager>().gameObject.GetComponent<AudioSource>();
        player = FindObjectOfType<Player>();
        nuzzleLight = GetComponent<Light2D>();
        reloadImage = player.reloadCircle.GetComponent<Image>();
        if (nuzzleLight != null)
        {
            nuzzleLight.enabled = false;
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (player.isDead) {  return; }
        ProjectileFire();
        BurstFire();
        RaycastFire();
        Reload();
        CheckBulletsFired();
        FireRateCooldown();
    }

    private void OnDisable()
    {
        player.reloadCircle.gameObject.SetActive(false);
    }

    void ProjectileFire()
    {
        if(settings.weaponType != WeaponSettingsObject.WeaponType.ProjectileFire || fireRateCoolingDown || reloading) { return; }

        if (playerInput.actions["Fire"].IsPressed()) 
        {
            randomBulletSpread = GetBulletSpread();
            Instantiate(settings.projectile, transform.position, randomBulletSpread);    
            GunFeedbackEffects();
            fireRateCoolingDown = true;
            bulletsFired++;
        }
        SetAmmoUI();
    }

    void BurstFire()
    {
        if(settings.weaponType != WeaponSettingsObject.WeaponType.BurstFire || fireRateCoolingDown || reloading) { return; }

        if(playerInput.actions["Fire"].IsPressed())
        {
            for (int i = 0; i < settings.bulletsBeforeReload || settings.bulletsBeforeReload == 0; i++)
            {
                randomBulletSpread = GetBulletSpread();
                Instantiate(settings.projectile, transform.position, randomBulletSpread);
                bulletsFired++;
            }
            GunFeedbackEffects();
        }
        SetAmmoBurstUI();   
    }

    void RaycastFire()
    {
        if (settings.weaponType != WeaponSettingsObject.WeaponType.RaycastFire || fireRateCoolingDown || reloading) { return; }

        if (playerInput.actions["Fire"].IsPressed())
        {
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

                    enemyScript.TakeDamage(settings.damagePerBullet);
                }
            }
        }
        SetAmmoUI();
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

    void CheckBulletsFired()
    {
        if(settings.bulletsBeforeReload == 0) { return; }

        if (bulletsFired >= settings.bulletsBeforeReload)
        {
            reloading = true;
        }
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
        if (!reloading) { return; }
        if (!reloadAudioPlayed)
        {
            gunSource.PlayOneShot(settings.reloadAudio);
            reloadAudioPlayed = true;
        }

        player.reloadCircle.SetActive(true);
        reloadImage.fillAmount = (settings.reloadTime / settings.reloadTime) - (reloadTimer / settings.reloadTime);
        reloadTimer -= Time.deltaTime;
        if(reloadTimer <= 0)
        {
            bulletsFired = 0;
            reloading = false;
            reloadAudioPlayed = false;
            reloadTimer = settings.reloadTime;
            player.reloadCircle.SetActive(false);
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
        ammoUI.text = (settings.bulletsBeforeReload / settings.bulletsBeforeReload - bulletsFired / settings.bulletsBeforeReload).ToString() + "/" + (settings.bulletsBeforeReload / settings.bulletsBeforeReload).ToString();
    }
    public float GetProjectileSpeed()
    {
        return settings.projectileSpeed;
    }

    public float GetVanishAfter()
    {
        return settings.projectileVanishAfter;
    }

    public float GetDamagePerBullet()
    {
        return settings.damagePerBullet;
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
