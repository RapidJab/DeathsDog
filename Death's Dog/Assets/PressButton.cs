using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PressButton : MonoBehaviour
{
    [SerializeField] bool yes;
    Button button;

    private void Awake()
    {
        button = GetComponent<Button>();
    }

    // Update is called once per frame
    void Update()
    {
        if (yes)
        {
            if (VariableManager.Instance.input.currentActionMap.FindAction("Yes").GetButton())
            {
                button.onClick.Invoke();
            }
        }
        else
        {
            if (VariableManager.Instance.input.currentActionMap.FindAction("No").GetButton())
            {
                button.onClick.Invoke();
            }
        }
        
    }
}
