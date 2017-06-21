using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class MusicKey : MonoBehaviour {

    //グローバル変数群
    AudioSource _audioSource;    //このgameObjectに紐付けられたAudioSource(Required)
    List<MusicNote> _notes = new List<MusicNote>();  //ノーツ

    public List<MusicNote> Notes{ get{ return _notes; } set{ _notes = value;} }

    private void Start()
    {
        //audioSource初期化
        _audioSource = gameObject.GetComponent<AudioSource>();

    }

    private void Update()
    {
        
    }
}
