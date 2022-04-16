using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

[CreateAssetMenu(menuName = "Text Line")]
public class TextLine : ScriptableObject
{
    public string line; //What is being said
    public Color boxColor = Color.white; //Color of the textbox
    public Color textColor = Color.white; //Color of the textbox
    public float scrollSpeed = .03f; //How fast the text appears
    public float requiredTime; //Cannot go to next dialogue until requiredTime has passed since textbox appeared
    public string actorName; //Name of the character speaking
    public string animName; //Name of the animation the character does, if any
    public int requiredItems;
    public bool yesNo; 
    public float moveSpeed; 
}
