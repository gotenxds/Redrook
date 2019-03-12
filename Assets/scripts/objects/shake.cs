using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class shake : MonoBehaviour
{
    Tween tween;
    private void Awake()
    {
        tween = transform
                .DOShakePosition(.5f, new Vector3(.05f, 0, 0), 20, 0)
                .SetAutoKill(false)
                .Pause();
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (!tween.IsPlaying())
        {
            tween.Restart();
        }
    }
}
