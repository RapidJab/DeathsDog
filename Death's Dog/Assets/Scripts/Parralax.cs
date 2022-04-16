using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Parralax : MonoBehaviour
{
    private float lengthX;
    private float startPosX;
    private float offsetY;
     
    [SerializeField] private int PixelsPerUnit;
    private GameObject cam;

    [SerializeField] private float parrallaxEffect;

    // Start is called before the first frame update
    void Start()
    {
        cam = Camera.main.gameObject;
        startPosX = transform.position.x;//cam.transform.position.x;
        lengthX = GetComponent<SpriteRenderer>().bounds.size.x;
        offsetY = transform.position.y - cam.transform.position.y;
    }

    private void LateUpdate()
    {
        float temp = (cam.transform.position.x * (1 - parrallaxEffect));
        float dist = cam.transform.position.x * parrallaxEffect;  

        Vector2 newPosition = new Vector2(startPosX + dist, cam.transform.position.y + offsetY);

        transform.position = PixelPerfectClamp(newPosition, PixelsPerUnit);

        if (temp > startPosX + lengthX)
        {
            startPosX += lengthX;
        }
        else if(temp < startPosX - lengthX)
        {
            startPosX -= lengthX;
        }
    }
    private Vector2 PixelPerfectClamp(Vector3 locationVector, float pixelsPerUnit)
    {
        Vector3 vectorInPixels = new Vector3(Mathf.CeilToInt(locationVector.x * pixelsPerUnit), Mathf.CeilToInt(locationVector.y * pixelsPerUnit), Mathf.CeilToInt(locationVector.z * pixelsPerUnit));
        return vectorInPixels / pixelsPerUnit;
    }
}
