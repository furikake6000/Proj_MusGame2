using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MusicNote {
    float _reachTime;   //ノーツが対応ボタンに来るタイミング
    float _duration;    //ノーツが流れはじめてからreachTimeまでの時間
    string _wavID;

    public float ReachTime{
        get{ return _reachTime; }

        set{ _reachTime = value; }
    }

    public float Duration{
        get { return _duration; }

        set { _duration = value; }
    }

    public string WavID{
        get{ return _wavID; }

        set{ _wavID = value; }
    }

}
