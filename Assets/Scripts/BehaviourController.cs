using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BehaviourController : MonoBehaviour
{
    public static bool isInteractable = false;
    public static bool isFreeToSpawn = false;
    public static bool isRotate = false;

    private void Start()
    {
        SetDefault();
    }

    private static void EnableSpawn()
    {
        isInteractable = false;
        isFreeToSpawn = true;
        isRotate = false;
    }
    
    public static void ToInteract()
    {
        EnableSpawn();
        isInteractable = true;
        isFreeToSpawn = false;
    }
    
    public static void ToSpawn()
    {
        EnableSpawn();
    }

    public static void ToRotate()
    {
        isRotate = !isRotate;
    }

    public static void SetDefault()
    {
        isInteractable = false;
        isFreeToSpawn = false;
        isRotate = false;
    }
}
