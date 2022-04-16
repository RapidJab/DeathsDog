using System.Collections;
using System.Collections.Generic;
using UnityEngine; 

public class NextLevel : MonoBehaviour
{
    SceneTransition transition;
    private void Awake()
    {
        transition = Camera.main.GetComponentInChildren<SceneTransition>();
    }
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.collider.gameObject != VariableManager.Instance.player)
            return;
        StartCoroutine(transition.ExitScene());
    }
}
