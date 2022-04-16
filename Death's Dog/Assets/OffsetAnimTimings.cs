using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OffsetAnimTimings : MonoBehaviour
{
    private Animator anim;
    private void Awake()
    {
        anim = GetComponent<Animator>();
        StartCoroutine(StartAnim());
    }
    IEnumerator StartAnim()
    {
        anim.enabled = false;
        float waitAmount = Random.Range(.01f, .8f);
        float slowAmount = Random.Range(.9f, 1.1f);
        yield return new WaitForSeconds(waitAmount);
        anim.speed = slowAmount;
        anim.enabled = true;
    }
}
