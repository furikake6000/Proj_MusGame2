using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MusicNote {
    int _location;      //ノーツの位置
    float _reachTime;   //ノーツが対応ボタンに来るタイミング
    float _duration;    //ノーツが流れはじめてからreachTimeまでの時間
    string _wavID;      //使うWavID（00-99,AA-ZZまでの大文字英数字37進数2文字）

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

    public int Location
    {
        get
        {
            return _location;
        }

        set
        {
            _location = value;
        }
    }

    //Constructor
    public MusicNote(int location, string wavID, float reachTime, float duration)
    {
        _location = location;
        _reachTime = reachTime;
        _duration = duration;

        //WavIDのフォーマット検査
        if(wavID.Length == 2 &&
            ((wavID[0] >= '0' && wavID[0] <= '9') || (wavID[0] >= 'a' && wavID[0] <= 'z') || (wavID[0] >= 'A' && wavID[0] <= 'Z')) &&
            ((wavID[1] >= '0' && wavID[1] <= '9') || (wavID[1] >= 'a' && wavID[1] <= 'z') || (wavID[1] >= 'A' && wavID[1] <= 'Z')))
        {
            //Format ok
            _wavID = wavID.ToUpper();
        }
        else
        {
            //Format error
            throw new FormatException();
        }
    }
}

public class MusicNoteGenTimeComparerRule : IComparer<MusicNote>
{
    public int Compare(MusicNote a, MusicNote b)
    {
        float birthTimeDiffer = (b.ReachTime - b.Duration) - (a.ReachTime - a.Duration);
        if (birthTimeDiffer > 0.0f)
        {
            // a will be generated before b
            return 1;
        }
        else if(birthTimeDiffer < 0.0f)
        {
            // a will be generated after b
            return -1;
        }
        else
        {
            // a and b will be generated at the same time
            return 0;
        }
    }
}