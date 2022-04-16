using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ReaperManager : MonoBehaviour
{
    public GameObject[] reapers; 
    [SerializeField] [ShowOnly] private GameObject primeReaper;
    [SerializeField] private float transitionTime;
    private bool inTransition;

    private void Awake()
    { 
        for(int i = 0; i < reapers.Length; i++)
        { 
            reapers[i].SetActive(false); 
        }
    }

    private void SetReaper()
    {
        float maxDist = 10000;
        GameObject newReaper = primeReaper;
        foreach(GameObject reaper in reapers)
        { 
            float curDist = Vector2.Distance(reaper.transform.position, VariableManager.Instance.player.transform.position);
            if (curDist < maxDist)
            { 
                newReaper = reaper;
                maxDist = curDist;
            }
        }
        if(newReaper != primeReaper)
        { 
            if (primeReaper)
            {
                StartCoroutine(Transition(newReaper));
            }
            else
            { 
                primeReaper = newReaper;
                primeReaper.SetActive(true);
            } 
        } 
    }

    private void Update()
    {
        if (!inTransition)
        {
            SetReaper();
        }
    }

    private IEnumerator Transition(GameObject newReaper)
    {
        inTransition = true;
        primeReaper.GetComponent<Animator>().SetTrigger("IsDisappear");
        yield return new WaitForSeconds(transitionTime);
        primeReaper = newReaper;
        primeReaper.SetActive(true);
        inTransition = false;
    }
}
