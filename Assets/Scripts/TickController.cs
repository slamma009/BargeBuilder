using System;
using UnityEngine;

public class TickController : MonoBehaviour
{

    public int TicksPerSecond = 5;
    private float _TicksDelta = 0.0f;

    public static EventHandler<TickArgs> TickEvent;

    private void Start()
    {
        _TicksDelta = 1f / TicksPerSecond;
    }

    private float _SavedDeltaTime = 0;
    private void Update()
    {
        _SavedDeltaTime += Time.deltaTime;

        if (_SavedDeltaTime >= _TicksDelta)
        {
            if(TickEvent != null)
                TickEvent.Invoke(this, new TickArgs() { DeltaTime = _SavedDeltaTime });

            _SavedDeltaTime -= _TicksDelta;
        }
    }
}
public class TickArgs : EventArgs
{
    public float DeltaTime;
}

