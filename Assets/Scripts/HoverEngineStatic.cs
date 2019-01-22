using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Valve.VR;

public class HoverEngineStatic : MonoBehaviour
{
    public float HoverHeightMin = 2.5f;
    public float HoverHeightMax = 10f;
    private float HoverHeight = 2.5f;
    public float Speed = 0.2f;
    public float RotateSpeed = 2;
    public float HoverSpeed = 1;
    public float HoverRange = 0.2f;


    private float PowerInput;
    private float TurnInput;
    private float HeightInput;

    [HideInInspector]
    public bool HeightEnabled = false;

    [HideInInspector]
    public bool EnableEngine = false;

    [HideInInspector]
    public bool EngineIsRightHand = false;

    [HideInInspector]
    public bool HeightIsRightHand = false;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    private void Update()
    {
        Vector2 leftInput = SteamVR_Input._default.inActions.ThumbStick.GetAxis(SteamVR_Input_Sources.LeftHand);
        Vector2 rightInput = SteamVR_Input._default.inActions.ThumbStick.GetAxis(SteamVR_Input_Sources.RightHand);
        if (EnableEngine) 
        {
            PowerInput = EngineIsRightHand ? rightInput.y :leftInput.y;
            TurnInput = EngineIsRightHand ? rightInput.x : leftInput.x; 
        }
        else
        {
            PowerInput = 0;
            TurnInput = 0;
        }

        if (HeightEnabled)
        {
            HeightInput = HeightIsRightHand ? rightInput.y : leftInput.y;
        } 
        else
        {
            HeightInput = 0;
        }
    }
    // Update is called once per frame
    void FixedUpdate()
    {
        HoverHeight = HoverHeight + HeightInput * Time.deltaTime;
        if(HoverHeight > HoverHeightMax)
        {
            HoverHeight = HoverHeightMax;
        } else if(HoverHeight < HoverHeightMin)
        {
            HoverHeight = HoverHeightMin;
        }
        this.gameObject.transform.position += transform.forward * Speed * PowerInput;
        this.gameObject.transform.Rotate(Vector3.up * RotateSpeed * TurnInput);

        float time = Mathf.PingPong(Time.time * HoverSpeed, 1);
        Vector3 pos = transform.position;
        pos.y = HoverHeight;
        transform.position = Vector3.Lerp(pos + new Vector3(0, HoverRange, 0), pos + new Vector3(0, -1 * HoverRange, 0), time);
    }
}
