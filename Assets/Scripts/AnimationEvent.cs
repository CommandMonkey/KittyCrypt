using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class AnimationEvent: MonoBehaviour
{
    public UnityEvent OnAnimationEventTriggerd, OnAttackPreformed;

    public void TriggerEvent()
    {
        OnAnimationEventTriggerd?.Invoke();
    }

    public void TriggerAttack()
    {
        OnAttackPreformed?.Invoke();
    }
}
