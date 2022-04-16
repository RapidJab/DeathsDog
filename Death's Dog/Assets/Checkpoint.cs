using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Checkpoint : MonoBehaviour
{
    public Vector3 lastPos; 
     
    private void OnTriggerStay2D(Collider2D collision)
    {
        if (!collision.isTrigger && VariableManager.Instance.playerMovement.groundJump)
        {
            VariableManager.Instance.curCheckpoint = this;
            lastPos = VariableManager.Instance.player.transform.position;
        }
    }
}
