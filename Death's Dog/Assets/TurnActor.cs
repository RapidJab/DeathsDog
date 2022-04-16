using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TurnActor : MonoBehaviour
{
    public void Turn()
    {
        Debug.Log("Turn");
        Vector3 turn = transform.localScale;
        turn.x *= -1;
        transform.localScale = turn;
    }
}
