using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class BossRoom : Room
{
    [SerializeField] private Transform boss;

    private GameCamera gameCamera;
    private RatBossBehaviour bossScript;
    private LevelExitInteractable levelExit;

    bool started = false;

    protected override void RoomStart()
    {
        gameCamera = FindObjectOfType<GameCamera>();
        bossScript = boss.GetComponent<RatBossBehaviour>();
        levelExit = GetComponentInChildren<LevelExitInteractable>();

        onPlayerEnter.AddListener(OnPlayerEnter);

        bossScript.bossRoom = this;
    }

    private void OnPlayerEnter()
    {
        boss.gameObject.SetActive(true);
        StartCoroutine(BossCutsceneRoutine());
        roomManager.CloseDoors();
    }

    public void OnBossDead()
    {
        levelExit.SetInteractable();
        Invoke("focusCamOnPlayer", 1f);
        roomManager.OpenDoors();
    }


    public void focusCamOnBoss()
    {
        gameCamera.SetPrimaryTarget(boss);
        gameCamera.SetSecondaryTarget(null);
    }

    public void focusCamOnPlayer()
    {
        gameCamera.SetPrimaryTarget(player.transform);
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
        levelExit.SetUnInteractable();

        gameSession.SetState(GameSession.GameState.Paused);
        gameSession.musicManager.StopMusic();

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
        gameSession.SetState(GameSession.GameState.Running);
        bossScript.StartBoss();
    }
}
