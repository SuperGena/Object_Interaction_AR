using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Toggle))]
public class ToggleButton : MonoBehaviour
{
   private Toggle _toggle;
   
   private void Start()
   {
      _toggle = GetComponent<Toggle>();
   }

   private void OnEnable()
   {
      _toggle.isOn = false;
   }

   private void OnDisable()
   {
      _toggle.isOn = false;
   }
}
