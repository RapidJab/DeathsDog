using System.Collections;
using System.Collections.Generic;
using UnityEngine; 

public class WindTrap : MonoBehaviour
{ 
    [SerializeField] private GameObject windEffect; //Wind template object
    [SerializeField] private float windSpawnTime; //How fast the wind should spawn
    [SerializeField] private int windObjectAmount = 20; //How many wind effects should spawn

    [SerializeField] private float velocityLimit; //Fastest the player can be pushed
    [SerializeField] private float maxForce; //Strongest the force can be
    [SerializeField] private float minForce; //Weakest the force can be

    private Transform origin;
    BoxCollider2D col;
    Vector2 cubeSize;
    Vector2 cubeCenter; 
    private int curWind = 0;
    GameObject[] allWind; 
    private Vector2 windDir;
    private Bounds windBounds;

    private void Awake()
    { 
        col = GetComponent<BoxCollider2D>();
        cubeCenter = col.bounds.center;
        windBounds = col.bounds;
        origin = transform.parent;
        // Multiply by scale because it does affect the size of the collider
        cubeSize.x = col.size.x;
        cubeSize.y = col.size.y;
        windDir = -windEffect.transform.right;
    }
    private void Start()
    {
        windEffect.SetActive(false);
        allWind = new GameObject[windObjectAmount];
        for(int i = 0; i < windObjectAmount; i++)
        { 
            allWind[i] = Instantiate(windEffect, transform);
            allWind[i].SetActive(false);
        }
        StartCoroutine(SpawnWind()); 
    } 

    private IEnumerator SpawnWind()
    { 
        yield return new WaitForSeconds(windSpawnTime);
        RandomSpawn(allWind[curWind]);
        if(curWind < allWind.Length - 1)
        { 
            curWind++;
        }
        else
        {
            curWind = 0;
        }
        StartCoroutine(SpawnWind());
    }

    private void RandomSpawn(GameObject wind)
    {
        bool goodPoint = false;
        Vector2 randomPos = Vector2.zero;
        while (!goodPoint)
        {
            float randomX = Random.Range((cubeCenter.x - (windBounds.extents.x * .7f)), (cubeCenter.x + (windBounds.extents.x * .7f)));
            float randomY = Random.Range((cubeCenter.y - (windBounds.extents.y * .7f)), (cubeCenter.y + (windBounds.extents.y * .7f)));
            randomPos = new Vector2(randomX, randomY);
            if (Physics2D.OverlapPoint(randomPos, LayerMask.GetMask("Wind")))
            {
                Debug.Log("impossible");
                goodPoint = true;
            }
        } 

        wind.transform.position = randomPos;  
        wind.SetActive(true);
    }

    private void OnTriggerStay2D(Collider2D other)
    { 
        if (other.CompareTag("Player") && !VariableManager.Instance.playerMovement.dead)
        { 
            float distance = Vector2.Distance(transform.position, other.transform.position);
            float t = (1 - (distance / col.size.y)); 
            Vector2 temp = other.gameObject.GetComponent<Rigidbody2D>().velocity + Vector2.Lerp(minForce * windDir, maxForce * windDir, t);
            if (temp.magnitude > velocityLimit)
            {
                temp = temp.normalized * velocityLimit;
            }
            VariableManager.Instance.playerMovement.moveModifier = temp - other.gameObject.GetComponent<Rigidbody2D>().velocity;
        }
    }


     
} 