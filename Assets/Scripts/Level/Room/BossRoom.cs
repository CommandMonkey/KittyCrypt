using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class BossRoom : Room
{
    [SerializeField] private Transform boss;
    
    private GameCamera camera;
    private void RoomStart()
    {
        camera = FindObjectOfType<GameCamera>();
        onPlayerEnter.AddListener(OnPlayerEnter);
    }

    private void OnPlayerEnter()
    {
        boss.gameObject.SetActive(true);
    }

    public void focusCamOnBoss()
    {
        camera.SetPrimaryTarget(boss);
        camera.SetSecondaryTarget(null);
    }
    
    public void focusCamOnBossandPlayer()
    {
        camera.SetPrimaryTarget(player.transform);
        camera.SetSecondaryTarget(boss);
    }
}
