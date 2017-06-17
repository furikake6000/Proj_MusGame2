using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Note {
    float _time;
    string _wavID;

    public float Time{
        get{ return _time; }

        set{ _time = value; }
    }

    public string WavID{
        get{ return _wavID; }

        set{ _wavID = value; }
    }
}
