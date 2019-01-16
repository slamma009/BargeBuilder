using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GodView : AnchorSystem
{
    private Camera GodCamera;

    private void Start()
    {
        GodCamera = gameObject.GetComponent<Camera>();
        Setup();
        EnablePlacingMode(true);
    }


    // Update is called once per frame
    void Update()
    {
        if (PlacingModeEnabled && !Input.GetKey(KeyCode.Mouse1))
        {
            // Rotate the ghost avatar above the hand
            GhostAvatar.transform.Rotate(Vector3.up * Time.deltaTime * 30);

            float rotateSpeed = Input.GetKey(KeyCode.R) ? 1 : 0;
            RotateGhostItem(rotateSpeed, Input.GetKeyUp(KeyCode.R));

            RaycastLogic(GodCamera.ScreenPointToRay(Input.mousePosition), Input.GetKeyUp(KeyCode.Mouse0), Input.GetKeyDown(KeyCode.Mouse0));


            if (Input.GetKeyDown(KeyCode.X))
            {
                DestructionMode = !DestructionMode;

                GhostObject.transform.position = GhostObjectHiddenPosition;
                //GhostDeleteMarker.transform.position = GhostObjectHiddenPosition;
            }

            // Check to exit placing mode
            if ( Input.GetKeyDown(KeyCode.F))
            {
                EnablePlacingMode(false);
            }
            

            if (Input.GetKeyDown(KeyCode.V))
            {
                ChangePrefabs();

            }
        }
        else
        {
            // Check to exit placing mode
            if (Input.GetKeyDown(KeyCode.F))
            {
                EnablePlacingMode(true);
            }

        }
    }


    
}
