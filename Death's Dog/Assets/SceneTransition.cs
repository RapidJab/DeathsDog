using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class SceneTransition : MonoBehaviour
{
    [SerializeField] private AudioClip transitionSound;
    Animator anim;

    // Start is called before the first frame update
    void Awake()
    {
        anim = GetComponent<Animator>();
    }
    public IEnumerator ExitScene()
    {
        MusicManager.Instance.ChangeMusic(1.5f);
        anim.SetTrigger("ToSolid");
        MusicManager.Instance.PlaySound(transitionSound, .5f);
        yield return new WaitForSeconds(1.5f);
        SceneManager.LoadScene(VariableManager.Instance.nextScene);
    }

    public IEnumerator ExitToStartScene()
    {
        MusicManager.Instance.ChangeMusic(1.5f);
        anim.SetTrigger("ToSolid");
        MusicManager.Instance.PlaySound(transitionSound, .5f);
        yield return new WaitForSeconds(1.5f);
        SceneManager.LoadScene("StartScreen");
    }
} 
