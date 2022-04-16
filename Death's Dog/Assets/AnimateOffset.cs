using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimateOffset : MonoBehaviour
{
    [SerializeField] private float enableTime;
    private SpriteRenderer rend;
    private Animator anim;

    private void Awake()
    {
        rend = GetComponent<SpriteRenderer>();
        anim = GetComponent<Animator>();
    }

    private void OnEnable()
    {
        anim.enabled = false;
        rend.enabled = false;
        StartCoroutine(AnimStartWait());
    }

    private IEnumerator AnimStartWait()
    { 
        yield return new WaitForSeconds(enableTime);
        anim.enabled = true;
        rend.enabled = true;
        //StartCoroutine(AnimContinueWait());
    } 
    public void StopBehavior()
    {
        anim.enabled = false;
        rend.enabled = false;
        StopAllCoroutines(); 
    }
}
