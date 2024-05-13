
using UnityEngine;

public class Boss : Enemy
{
    protected GameCamera cam;
    protected AudioSource audioSource;
    protected SpriteRenderer spriteRenderer;
    protected MusicManager musicManager;
    internal virtual void StartBoss() { }
}