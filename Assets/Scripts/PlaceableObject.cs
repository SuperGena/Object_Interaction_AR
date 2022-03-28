using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Renderer))]
public class PlaceableObject : MonoBehaviour
{
   public bool Selected
   {
      get
      {
         return selected;
      }
      set
      {
         selected = value;
         
         if (value)
         {
            renderer.material = selectMaterial;
         }
         else
         {
            renderer.material = defaultMaterial;
         }
      }
   }

   public bool selected = false;

   private Renderer renderer;

   public Material defaultMaterial;
   public Material selectMaterial;

   private void Start()
   {
      renderer = transform.GetComponent<Renderer>();
      renderer.material = defaultMaterial;
   }

   public void DestroyObject()
   {
      Destroy(gameObject);
   }
}
