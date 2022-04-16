using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Sunrise : MonoBehaviour
{
    [SerializeField] private float startY;
    [SerializeField] private float endY;
    [SerializeField] private float riseTime;


    public void BeginRising()
    {
        StartCoroutine(Rising());
    }
    private IEnumerator Rising()
    {
        float time = 0;
        while (time < riseTime)
        {
            if (Time.timeScale == 0)
                time += Time.unscaledDeltaTime;
            else
                time += Time.deltaTime;

            float val = Mathf.Lerp(startY, endY, time / riseTime);
            Vector2 newPos = transform.position;
            newPos.y = val;
            transform.position = newPos;
            yield return null;
        }
    }
}
