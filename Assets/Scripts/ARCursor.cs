using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;

public class ARCursor : MonoBehaviour
{
    [SerializeField] private Camera arCamera;
    [SerializeField] private GameObject interactionUIPanel;
    [SerializeField] private GameObject changePrefabUIPanel;
    
    private Vector2 touchPosition;

    private PlaceableObject lastSelectedObject;

    private Vector3 initialScale;
    
    private float initialDistance;
    
    
    public GameObject cursorChildObject;
    public PlaceableObject objectToPlace;
    
    public List<PlaceableObject> objectsToPlace;
    
    public ARRaycastManager raycastManager;
    
    static List<ARRaycastHit> hits = new List<ARRaycastHit>();
    
    List<PlaceableObject> addedInstances = new List<PlaceableObject>();
    
    public bool useCursor = true;

    public float rotationSpeed;

    private void Start()
    {
        cursorChildObject.SetActive(useCursor);
    }

    private void LateUpdate()
    {
        if (useCursor)
        {
            UpdateCursor();
        }
        
        if (Input.touchCount > 0)
        {
            Touch touch = Input.GetTouch(0);

            touchPosition = touch.position;

            if (BehaviourController.isFreeToSpawn)
            {
                if (touch.phase == TouchPhase.Began)
                {
                    var isOverUI = touchPosition.IsPointOverUIObject();
                    
                    if (!isOverUI)
                        SpawnOrMoveObject();
                }
            }

            if (BehaviourController.isInteractable)
            {
                var isOverUI = touchPosition.IsPointOverUIObject();

                if (touch.phase == TouchPhase.Moved)
                {
                    if (Input.touchCount == 1)
                    {
                        if (BehaviourController.isRotate && !isOverUI)
                        {
                            RotateObject(touch);
                        }
                        
                        if (!isOverUI && !BehaviourController.isRotate)
                            SpawnOrMoveObject();
                    }
                }
                
                if (Input.touchCount == 2 && !BehaviourController.isRotate)
                {
                    ScaleHandler();
                }
            }
        }
    }

    private void Update()
    {
        if (Input.touchCount > 0)
        {
            Touch touch = Input.GetTouch(0);

            touchPosition = touch.position;

            if (BehaviourController.isFreeToSpawn)
                if (touch.phase == TouchPhase.Began)
                {
                    ObjectSelectionHandler();
                }
        }
    }

    public void ChangeInteractableObject()
    {
        lastSelectedObject.Selected = false;
        lastSelectedObject = null;

        interactionUIPanel.SetActive(false);
        
        BehaviourController.ToSpawn();
        changePrefabUIPanel.gameObject.SetActive(true);
    }
    
    public void DeleteObject()
    {        
        addedInstances.Remove(lastSelectedObject);
        lastSelectedObject.selected = false;
        lastSelectedObject.DestroyObject();
        
        ChangeInteractableObject();
    }

    public void SetObjectToSpawn(PlaceableObject item)
    {
        objectToPlace = item;
    }

    void RotateObject(Touch touch)
    {
        float xPos = Input.GetAxis("Mouse X") * rotationSpeed * Time.deltaTime;
        float yPos = Input.GetAxis("Mouse Y") * rotationSpeed * Time.deltaTime;
        
        lastSelectedObject.transform.Rotate(new Vector3(yPos, -xPos, 0));
    }

    void SpawnOrMoveObject()
    {
        if (raycastManager.Raycast(touchPosition, hits, TrackableType.PlaneWithinPolygon))
        {
            Pose hitPose = hits[0].pose;

            if (lastSelectedObject == null && !BehaviourController.isInteractable)
            {
                lastSelectedObject = Instantiate(objectToPlace, new Vector3(hitPose.position.x, hitPose.position.y + objectToPlace.transform.localScale.y / 2, hitPose.position.z), hitPose.rotation).GetComponent<PlaceableObject>();
                addedInstances.Add(lastSelectedObject);
            }
            else
            {
                if (lastSelectedObject.Selected)
                {
                    MovementHandler(hitPose);
                }
            }
        }
    }

    void ObjectSelectionHandler()
    {
        Ray ray = arCamera.ScreenPointToRay(touchPosition);
        RaycastHit hitObject;
        
        if (Physics.Raycast(ray, out hitObject))
        {
            var selectedObject = hitObject.transform.GetComponent<PlaceableObject>();

            if (selectedObject == null)
            {
                if (!BehaviourController.isInteractable)
                {
                    lastSelectedObject.Selected = false;
                    lastSelectedObject = null;
                }
                
                var isOverUI = touchPosition.IsPointOverUIObject();
                    
                if (!isOverUI)
                    SpawnOrMoveObject();
            }
            else
            {
                lastSelectedObject = selectedObject;
                lastSelectedObject.Selected = true;
                
                foreach (PlaceableObject item in addedInstances)
                {
                    item.Selected = item == lastSelectedObject;
                }
                
                BehaviourController.ToInteract();
                ShowInteractionUIPanel();
            }
        }
    }

    void ShowInteractionUIPanel()
    {
        if (!interactionUIPanel.gameObject.activeSelf)
        {
            interactionUIPanel.SetActive(true);
        }
        
        changePrefabUIPanel.gameObject.SetActive(false);
    }

    void MovementHandler(Pose hitPose)
    {
        lastSelectedObject.transform.position = new Vector3(hitPose.position.x, hitPose.position.y + objectToPlace.transform.localScale.y / 2, hitPose.position.z);
    }

    void ScaleHandler()
    {
        var touchOne = Input.GetTouch(0);
        var touchTwo = Input.GetTouch(1);
                    
        if (touchOne.phase == TouchPhase.Ended || touchOne.phase == TouchPhase.Canceled ||
            touchTwo.phase == TouchPhase.Ended || touchTwo.phase == TouchPhase.Canceled)
        {
            return;
        }

        if (touchOne.phase == TouchPhase.Began || touchTwo.phase == TouchPhase.Began)
        {
            initialDistance = Vector2.Distance(touchOne.position, touchTwo.position);
            initialScale = lastSelectedObject.transform.localScale;
        }
        else
        {
            var currentDistance = Vector2.Distance(touchOne.position, touchTwo.position);
            
            if (Mathf.Approximately(initialDistance,0))
            {
                return;
            }

            var factor = currentDistance / initialDistance;
            lastSelectedObject.transform.localScale = initialScale * factor;
        }
    }

    void UpdateCursor()
    {
        Vector2 screenPosition = Camera.main.ViewportToScreenPoint(new Vector2(0.5f, 0.5f));
        List<ARRaycastHit> hit = new List<ARRaycastHit>();
        raycastManager.Raycast(screenPosition, hit, TrackableType.Planes);

        if (hit.Count > 0)
        {
            transform.position = hit[0].pose.position;
            transform.rotation = hit[0].pose.rotation;
        }
    }
}
