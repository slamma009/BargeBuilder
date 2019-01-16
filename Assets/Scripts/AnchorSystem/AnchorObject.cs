using UnityEngine;

[System.Serializable]
public struct AnchorObject 
{
    public GameObject Anchor;

    [HideInInspector]
    private GameObject pConnectedAnchor;

    public delegate void AnchorMethod(AnchorObject obj);
    public AnchorMethod AnchorChanged;

    public GameObject ConnectAnchor
    {
        get
        {
            return pConnectedAnchor;
        }
        set
        {
            pConnectedAnchor = value;

            if (AnchorChanged != null)
            {
                AnchorChanged(this);
            }
        }
    }
}

