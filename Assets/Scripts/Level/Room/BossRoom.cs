using System.Collections;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class BossRoom : Room
{
    [SerializeField] private Transform boss;
    [SerializeField] private GameObject bossDoorPrefab;

    private GameCamera gameCamera;
    private RatBossBehaviour bossScript;
    private LevelExitInteractable levelExit;
    private UICanvas uiCanvas;
    private TextMeshProUGUI bossNameText;

    private BossDoorInteractable bossDoor;

    bool started = false;

    protected new void Start()
    {
        base.Start();

        gameCamera = FindObjectOfType<GameCamera>();
        bossScript = boss.GetComponent<RatBossBehaviour>();
        levelExit = GetComponentInChildren<LevelExitInteractable>();
        uiCanvas = FindObjectOfType<UICanvas>();
        bossNameText = GameObject.Find("RattBossNameText").GetComponent<TextMeshProUGUI>();


        onPlayerEnter.AddListener(OnPlayerEnter);
        roomManager.onRoomSpawningDone.AddListener(OnRoomSpawningDone); // for switching to bossRoom door

        bossScript.bossRoom = this;

        bossNameText.enabled = false;
    }


    private void OnPlayerEnter()
    {
        if (started) return;
        boss.gameObject.SetActive(true);
        bossDoor.CloseDoor();
        StartCoroutine(BossCutsceneRoutine());
    }

    public void OnBossDead()
    {
        levelExit.SetInteractable();
        Invoke("focusCamOnPlayer", 1f);
        bossDoor.OpenDoor();

        uiCanvas.directionPointer.gameObject.SetActive(true);
        uiCanvas.directionPointer.target = levelExit.transform;
    }

    void OnRoomSpawningDone()
    {
        // switching to bossRoom door
        BossDoorInteractable bossDoor =  Instantiate(bossDoorPrefab, previousRoomEntrance.transform.position, Quaternion.identity).GetComponent<BossDoorInteractable>();
        bossDoor.direction = previousRoomEntrance.direction;
        previousRoomEntrance.Die();
        this.bossDoor = bossDoor;
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
        bossNameText.enabled = true;

        gameCamera.DoCameraShake(2f); // initiate shake
        yield return new WaitForSeconds(2f);

        bossNameText.enabled=false;
        focusCamOnBossAndPlayer();
        gameSession.SetState(GameSession.GameState.Running);
        bossScript.StartBoss();
    }
}
