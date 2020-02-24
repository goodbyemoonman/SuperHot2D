﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PunchMove : MonoBehaviour {
    Transform parent;
    Vector2 originPos;
    TrailRenderer tr;
    RaycastHit2D[] hits;
    public bool isPlayer;
    int targetLayer;

    private void Awake()
    {
        string target;
        if (isPlayer)
            target = "EnemyCharacter";
        else
            target = "PlayerCharacter";
        targetLayer = (1 << LayerMask.NameToLayer(target));
        tr = GetComponent<TrailRenderer>();
        parent = transform.parent;
        originPos = transform.localPosition;
    }

    private void OnEnable()
    {
        transform.localPosition = originPos;
        StartCoroutine(Move());
    }

    IEnumerator Move()
    {
        hits = null;
        tr.Clear();
        Vector2 origin = transform.position;
        Vector2 target = Vector2.right;
        for(float t = 0; t < 0.1f; )
        {
            transform.localPosition = Vector2.Lerp(originPos , target, t * 10);
            if (isPlayer)
            {
                t += 0.02f;
                yield return new WaitForSecondsRealtime(0.02f);
            }
            else
            {
                t += Time.deltaTime;
                yield return null;
            }
        }

        hits = Physics2D.RaycastAll(origin, ((Vector2)transform.position - origin),
            ((Vector2)transform.position - origin).magnitude, targetLayer);
        Debug.DrawLine(origin, (Vector2)transform.position, Color.blue, 0.5f);

        foreach(RaycastHit2D hit in hits)
        {
            hit.collider.SendMessage("GetDamaged", 1);
        }

        for(float t = 0; t < 0.25f; t += Time.deltaTime)
        {
            transform.localPosition = Vector2.Lerp(target, originPos * -1, t);
            yield return null;
        }
        gameObject.SetActive(false);
    }
}