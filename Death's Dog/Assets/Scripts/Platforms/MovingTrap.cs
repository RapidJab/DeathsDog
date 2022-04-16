using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovingTrap : MonoBehaviour
{
    private Vector2 originalPos;
    public float movingTime;
    public float movingSpeed;
    private float curTime;
    [HideInInspector] public float curSpeed; 

    private Transform trans;

    private void Awake()
    { 
        trans = gameObject.GetComponent<Transform>();
        originalPos = trans.position;
        curTime = movingTime;
        curSpeed = movingSpeed;
    }
    // Start is called before the first frame update
    void Start()
    { 
        StartCoroutine(MovePlatform());
    }

    private void Update()
    {
        if (VariableManager.Instance.playerMovement.dead) 
        {
            trans.position = originalPos;
        }
    }

    private IEnumerator MovePlatform()
    { 
        curSpeed *= -1;
        curTime = movingTime;
        while(curTime > 0)
        { 
            trans.localPosition = new Vector2(trans.localPosition.x + (curSpeed * Time.deltaTime), trans.localPosition.y); 
            curTime -= Time.deltaTime;
            yield return null;
        }
        StopAllCoroutines();
        StartCoroutine(MovePlatform());
    }

    private IEnumerator PotentialChange()
    {
        yield return new WaitForSeconds(.05f); 
        StopAllCoroutines();
        StartCoroutine(MovePlatform());
    }



    private void OnTriggerEnter2D(Collider2D collision)
    {  
        if (!collision.isTrigger && collision.gameObject != VariableManager.Instance.player)
        {
            StartCoroutine(PotentialChange());
        }
    }
}
