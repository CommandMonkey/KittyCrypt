using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class BossRoom : Room
{
    [SerializeField] private Transform boss;

    RatBossBehaviour bossScript;
    
    private GameCamera gameCamera;
    private LevelManager levelManager;

    bool started = false;

    protected override void RoomStart()
    {
        gameCamera = FindObjectOfType<GameCamera>();
        onPlayerEnter.AddListener(OnPlayerEnter);
        bossScript = boss.GetComponent<RatBossBehaviour>();
        levelManager = FindObjectOfType<LevelManager>();
    }

    private void OnPlayerEnter()
    {
        boss.gameObject.SetActive(true);
        StartCoroutine(BossCutsceneRoutine());
    }

    public void focusCamOnBoss()
    {
        gameCamera.SetPrimaryTarget(boss);
        gameCamera.SetSecondaryTarget(null);
    }
    
    public void focusCamOnBossAndPlayer()
    {
        gameCamera.SetPrimaryTarget(player.transform);
        gameCamera.SetSecondaryTarget(boss);
    }

    IEnumerator BossCutsceneRoutine()
    {
        if (started) yield break;
        started = true;

        levelManager.SetState(LevelManager.LevelState.Paused);
        levelManager.musicManager.StopMusic();

        focusCamOnBoss();
        yield return new WaitForSeconds(2f);
        bossScript.PlayShadowedCaneAnim();

        yield return new WaitForSeconds(.4f); // First cane stomp
        bossScript.PlayBoomSFX();
        yield return new WaitForSeconds(0.4f);
        gameCamera.DoCameraShake(); 

        yield return new WaitForSeconds(1f);// second cane stomp
        bossScript.PlayBoomSFX();
        yield return new WaitForSeconds(0.4f);
        gameCamera.DoCameraShake(); 

        yield return new WaitForSeconds(1f);// last cane stomp
        bossScript.PlayBoomSFX();
        yield return new WaitForSeconds(0.4f);
        gameCamera.DoCameraShake(); 

        yield return new WaitForSeconds(3f); // fade in
        gameCamera.DoCameraShake(2f); // initiate shake
        yield return new WaitForSeconds(2f);

        focusCamOnBossAndPlayer();
        levelManager.SetState(LevelManager.LevelState.Running);
        bossScript.StartBoss();
    }
}
