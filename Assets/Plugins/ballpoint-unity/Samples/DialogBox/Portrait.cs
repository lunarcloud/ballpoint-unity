using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Portrait : MonoBehaviour
{
    public string speakerName = "None";

    public void ShowIfSpeaker(string value) => gameObject.SetActive(value == speakerName);
}
