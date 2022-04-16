using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FacePlayer : MonoBehaviour
{
    [SerializeField] private bool facingRight; 
    SpriteRenderer rend; 

    private void Awake()
    { 
        rend = GetComponentInChildren<SpriteRenderer>();
    }
    void Update()
    {
        if (!VariableManager.Instance.playerMovement.sitting)
        {
            if (VariableManager.Instance.player.transform.position.x > transform.position.x && !facingRight)
            {
                rend.flipX = true;
                facingRight = true;
            }
            else if (VariableManager.Instance.player.transform.position.x <= transform.position.x && facingRight)
            {
                rend.flipX = false;
                facingRight = false;
            }
        }
        else
        {
           // rend.flipX = false;
        }
    }

}
