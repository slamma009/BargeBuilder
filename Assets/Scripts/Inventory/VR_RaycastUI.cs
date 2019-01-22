using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using Valve.VR;
using Valve.VR.InteractionSystem;


public class VR_RaycastUI : MonoBehaviour
{
    public Camera MainCamera;
    public Transform RayOrigin;
    public Transform Test;
    public float AngleCorrection = 15;
    [SerializeField]
    private LayerMask RaycastLayermask;

    //image UI prefab to be used for the cursor
    public GameObject uiCursor;
    public PointerEventData lastpointerData;
    public PointerEventData currentPointerData;

    public Vector3 lastHitPoint;

    public bool dragging;
    public Vector2 screenDelta = Vector2.zero;

    public bool lastState = false;

    public MonoBehaviour selected;

    public GameObject cursorInstance;
    
    public float castDistance = 100;
    

    private void OnDisable()
    {
        if (cursorInstance != null)
        {
            Destroy(cursorInstance);
            lastState = false;
        }
    }
    HashSet<GameObject> PreviouElements = new HashSet<GameObject>();
    // Update is called once per frame
    void Update()
    {
        currentPointerData = new PointerEventData(EventSystem.current);

        // if there is no pointer data, just use make a copy of the current data;
        lastpointerData = lastpointerData ?? currentPointerData;

        //the canvas needs to have a box collider 
        //on the correct layer for it to be hit by the raycast
        RaycastHit hit;

        Vector3 rayDir = RayOrigin.transform.right;

        //Finger correction. 30 on x, 15 on y. 0.035 distance.
        rayDir = Quaternion.AngleAxis(30, RayOrigin.transform.forward) * rayDir; 
        rayDir = Quaternion.AngleAxis(15, RayOrigin.transform.up) * rayDir;
        
        Ray ray = new Ray(RayOrigin.position, rayDir); //this.GetComponent<Camera>().ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(ray, out hit, castDistance, RaycastLayermask))
        {
            //for mouse emulation
            Vector2 screenPoint = MainCamera.WorldToScreenPoint(hit.point);
            
            //filling in the curstor data for the canvas
            currentPointerData.button = PointerEventData.InputButton.Left;
            currentPointerData.delta = (screenPoint - lastpointerData.position);
            currentPointerData.position = screenPoint;
            
            //if it was not hitting last frame instantiate the cursor at the hit position
            if (lastState == false)
            {
                // slacking last hit on the starting hit point
                lastHitPoint = hit.point;
                var parentCanvas = hit.collider.GetComponentInParent<Canvas>().transform;

                //creating the prefab
                if (uiCursor)
                {
                    cursorInstance = Instantiate(uiCursor, parentCanvas);
                    //cursorInstance.transform.SetAsFirstSibling();
                }

            }

            //if there is a cursor, update it
            if (cursorInstance)
                cursorInstance.transform.position = hit.point + hit.normal * .01f;

            //Debug.DrawRay(hit.point, hit.normal * .05f, Color.blue);
            //Debug.DrawLine(lastHitPoint, hit.point, Color.red);

            //after all that, using the generated mouse pointer event data to raycast onto the UI canvas
            List<RaycastResult> uiHits = new List<RaycastResult>();
            EventSystem.current.RaycastAll(currentPointerData, uiHits);

            //getting input states for passing to the UIBehavior
            //var hand = GetComponent<Hand>(); // interactUIAction.GetState(hand.handType);
           

            //print(currentPointerData);

            //loop though all the items hit by the UI raycast
            foreach (RaycastResult hitUi in uiHits)
            {
                bool interactDown = false;
                bool interactHeld = false;
                if (PreviouElements.Contains(hitUi.gameObject))
                {
                    interactHeld = true;
                } else
                {
                    PreviouElements.Add(hitUi.gameObject);
                    interactDown = true;
                }
                currentPointerData.pointerCurrentRaycast = hitUi;
                currentPointerData.pointerPressRaycast = hitUi;

                //getting the base class for all the interactable UI behaviors
                //using componentsInParent so that you get the button when clicking the label
                var uiBehaviours = hitUi.gameObject.GetComponentsInParent<UIBehaviour>();

                foreach (UIBehaviour behaviour in uiBehaviours)
                {
                    if (interactDown && selected == null)
                    {
                        //if it can cast to a click handler, call the handler function
                        IPointerClickHandler pointerClickHandler = behaviour as IPointerClickHandler;
                        if(pointerClickHandler != null)
                            pointerClickHandler.OnPointerClick(currentPointerData);

                        //if it's a goggle swap the value and call it's submit
                        if (behaviour is Toggle)
                        {
                            Toggle toggle = behaviour as Toggle;
                            toggle.isOn = !toggle.isOn;
                            toggle.OnSubmit(currentPointerData);
                        }
                        //(behaviour as ICancelHandler)?.OnCancel(currentPointerData);

                        //handlers for pointer down, selection, and updating the selection
                        if (behaviour is IPointerDownHandler)
                        {
                            IPointerDownHandler handler = behaviour as IPointerDownHandler;
                            handler.OnPointerDown(currentPointerData);

                            if (behaviour is ISelectHandler )
                            {
                                ISelectHandler sHandler = behaviour as ISelectHandler;
                                sHandler.OnSelect(currentPointerData);
                                if (behaviour is IUpdateSelectedHandler )
                                {
                                    IUpdateSelectedHandler ush = behaviour as IUpdateSelectedHandler;
                                    ush.OnUpdateSelected(currentPointerData);
                                }
                                selected = sHandler as MonoBehaviour;
                            }
                        }

                        //(behaviour as ISubmitHandler)?.OnSubmit(currentPointerData);

                    }

                    //(behaviour as IPointerEnterHandler)?.OnPointerEnter(currentPointerData);
                    //(behaviour as IPointerExitHandler)?.OnPointerExit(currentPointerData);

                }

                //handling sliders and other dragable controls
                if (interactHeld)
                {
                    IDragHandler dragHandler = selected as IDragHandler;
                    if(dragHandler != null)
                        dragHandler.OnDrag(currentPointerData);
                }

            }

            GameObject[] excluded = PreviouElements.Where(x => uiHits.Where(y => y.gameObject != x).Count() == 0).ToArray();
            foreach (GameObject obj in excluded)
            {
                //for when they need a point up event
                if (selected != null && selected.gameObject == obj)
                {
                    IPointerUpHandler pointerHandler = selected as IPointerUpHandler;
                    if (pointerHandler != null)
                        pointerHandler.OnPointerUp(currentPointerData);
                    selected = null;
                }
                PreviouElements.Remove(obj);
            }

            //updating state variables for the next itteration
            lastHitPoint = hit.point;
            lastState = true;
        }
        else //no physics hit
        {
            lastState = false;
            Destroy(cursorInstance);
            GameObject[] excluded = PreviouElements.ToArray();
            foreach (GameObject obj in excluded)
            {
                //for when they need a point up event
                if (selected != null && selected.gameObject == obj)
                {
                    IPointerUpHandler pointerHandler = selected as IPointerUpHandler;
                    if (pointerHandler != null)
                        pointerHandler.OnPointerUp(currentPointerData);
                    selected = null;
                }
                PreviouElements.Remove(obj);
            }
        }

        lastpointerData = currentPointerData;
    }

}