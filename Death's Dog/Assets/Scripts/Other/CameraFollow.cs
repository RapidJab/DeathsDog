using UnityEngine;
using System.Collections;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
using UnityEngine.Experimental.Rendering.Universal;

public class CameraFollow : MonoBehaviour
{
	[SerializeField] public GameObject target;
	[SerializeField] private int PixelsPerUnit;
	[SerializeField] private Vector3 offset;
	[SerializeField] private float followSpeed;
	private float interpVelocity;
	private Vector3 targetPos;
	private Volume volume;
	[SerializeField] private float vignetteChange = .02f;
	private PixelPerfectCamera pixelCam;
	private float defaultZoom;
	private SpriteRenderer dogSprite;
	[SerializeField] private float dogAlphaChange;
	public Coroutine curEnumerator; 
	[SerializeField] private float memoryFollowSpeed;
	private float defaultFollowSpeed; 
	private PerfectPixelWithZoom ppwz;
	[SerializeField] private GameObject transition;

	void Start()
	{
		ppwz = GetComponent<PerfectPixelWithZoom>();
		pixelCam = GetComponent<PixelPerfectCamera>();
		defaultZoom = ppwz.zoomCurrentValue;
		targetPos = transform.position;
		volume = GetComponent<Volume>();
		dogSprite = VariableManager.Instance.player.GetComponent<SpriteRenderer>();
		defaultFollowSpeed = followSpeed;
		transition.SetActive(true);
	}
	void FixedUpdate()
	{
		if (target) //Make sure a target exists
		{
			Vector3 posNoZ = transform.position;
			posNoZ.z = target.transform.position.z;

			Vector3 targetDirection = (target.transform.position - posNoZ);

			interpVelocity = targetDirection.magnitude * 15f;

			targetPos = transform.position + (targetDirection.normalized * interpVelocity * Time.deltaTime);

			Vector3 newPos = Vector3.Lerp(transform.position, targetPos + offset, followSpeed);
			transform.position = PixelPerfectClamp(newPos, PixelsPerUnit);
		} else
		{
			transform.position = PixelPerfectClamp(transform.position, PixelsPerUnit);
		}
	} 

	private Vector3 PixelPerfectClamp(Vector3 moveVector, float pixelsPerUnit)
	{
		Vector3 vectorInPixels = new Vector3(Mathf.CeilToInt(moveVector.x * pixelsPerUnit), Mathf.CeilToInt(moveVector.y * pixelsPerUnit), Mathf.CeilToInt(moveVector.z * pixelsPerUnit));
		return vectorInPixels / pixelsPerUnit;
	}
	public IEnumerator ToMemory(Transform newTarget, int zoom)
	{ 
		bool actualMemory = VariableManager.Instance.dialogue.curMemory;
		target = null;
		target = newTarget.gameObject; 
		followSpeed = memoryFollowSpeed;
		if (actualMemory)
		{ 
			MusicManager.Instance.ChangeMusic(1.5f, MusicManager.Instance.memoryMusic);
			foreach (SpriteRenderer rend in VariableManager.Instance.dialogue.curMemory.memoryObjects)
			{
				VariableManager.Instance.dialogue.curMemory.startCoroutines.Add(VariableManager.Instance.dialogue.curMemory.StartCoroutine(VariableManager.Instance.dialogue.curMemory.EnterScene(rend)));
			}
		} else
        {
			VariableManager.Instance.dialogue.curCharacter.startCoroutines.Add(VariableManager.Instance.dialogue.curCharacter.StartCoroutine(VariableManager.Instance.dialogue.curCharacter.EnterScene()));
		}
		int ppuChange = (zoom > ppwz.pixelsPerUnitScale ? 3 : -3);
		while (!actualMemory ? (ppwz.pixelsPerUnitScale != zoom) : (volume.weight < 1 || ppwz.pixelsPerUnitScale != zoom || dogSprite.color.a > 0.5f))
		{
			if (actualMemory && dogSprite.color.a > .5f)
			{
				Color tempColor = dogSprite.color;
				tempColor.a -= dogAlphaChange;
				dogSprite.color = tempColor;
			}
			if (actualMemory && volume.weight < 1)
			{
				volume.weight += vignetteChange;
			}
			if (ppuChange < 0 && zoom < ppwz.pixelsPerUnitScale || ppuChange > 0 && zoom > ppwz.pixelsPerUnitScale)
			{
				ppwz.ZoomIn();
				ppwz.ZoomIn();
			}  
			yield return null;
		}
	}
	public IEnumerator ToReality(Transform newTarget)
	{ 
		bool actualMemory = VariableManager.Instance.dialogue.curMemory;
		if (actualMemory)
		{
			MusicManager.Instance.ChangeMusic(1.5f, MusicManager.Instance.normalMusic);
			for (int i = 0; i < VariableManager.Instance.dialogue.curMemory.memoryObjects.Length; i++)
			{
				VariableManager.Instance.dialogue.curMemory.endCoroutines.Add(VariableManager.Instance.dialogue.curMemory.StartCoroutine(VariableManager.Instance.dialogue.curMemory.ExitScene(VariableManager.Instance.dialogue.curMemory.memoryObjects[i], VariableManager.Instance.dialogue.curMemory.objectsPos[i], VariableManager.Instance.dialogue.curMemory.xScale[i])));
			} 
        }
        else
        {
			VariableManager.Instance.dialogue.curCharacter.endCoroutines.Add(VariableManager.Instance.dialogue.curCharacter.StartCoroutine(VariableManager.Instance.dialogue.curCharacter.ExitScene()));
		}
		target = null;
		target = newTarget.gameObject;
		int ppuChange = (defaultZoom > ppwz.pixelsPerUnitScale ? 3 : -3); 
		while (!actualMemory ? (ppwz.pixelsPerUnitScale != defaultZoom) : (volume.weight > 0 || ppwz.pixelsPerUnitScale != defaultZoom || dogSprite.color.a < 1))
		{
			if(actualMemory && dogSprite.color.a < 1)
            {
				Color tempColor = dogSprite.color;
				tempColor.a += dogAlphaChange;
				dogSprite.color = tempColor; 
			} 
			if (actualMemory && volume.weight > 0)
			{
				volume.weight -= vignetteChange;
				followSpeed = defaultFollowSpeed;
			}
			if (ppuChange < 0 && defaultZoom < ppwz.pixelsPerUnitScale || ppuChange > 0 && defaultZoom > ppwz.pixelsPerUnitScale)
			{
				ppwz.ZoomOut(); 
			}  
			yield return null;
		} 

	}
}