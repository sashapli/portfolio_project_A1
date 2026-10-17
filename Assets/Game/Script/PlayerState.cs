using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;


public class PlayerState : MonoBehaviour
{
    public static PlayerState Singalton;

    public bool drag;
    public TMP_Text textDebug;

    private void Awake()
    {
        Singalton = this;
    }
}
