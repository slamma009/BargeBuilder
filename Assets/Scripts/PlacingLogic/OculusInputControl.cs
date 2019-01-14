using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class OculusInputControl : MonoBehaviour {

    public InputObject[] Inputs;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
        foreach(InputObject obj in Inputs)
        {
            obj.Pressed = false;

            float inputValue = Input.GetAxis(string.IsNullOrEmpty(obj.InputName) ? obj.Name : obj.InputName);

            if (!obj.IsActive &&  ((obj.inverted && inputValue <= -1 + obj.variance) || (!obj.inverted && inputValue >= 1 - obj.variance )))
            { 
                obj.Pressed = true;
                obj.IsActive = true;
            }

            if(obj.IsActive && ((!obj.inverted && inputValue < 1 - obj.variance) || (obj.inverted && inputValue > -1 + obj.variance)))
            {
                obj.IsActive = false;
            }
        }
	}

    public bool GetButton(string name)
    {

        InputObject obj = Inputs.Where(x => x.Name == name).FirstOrDefault();
        return obj.Pressed;
        
    }
}

[System.Serializable]
public class InputObject
{
    public string Name;

    public string InputName;

    public bool inverted;

    public float variance = 0;

    [HideInInspector]
    public bool Pressed = false;

    [HideInInspector]
    public bool IsActive;
}
