using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class Key : MonoBehaviour {

    //グローバル変数群
    AudioSource _audioSource;    //このgameObjectに紐付けられたAudioSource(Required)
    MusicNote[] _notes;  //ノーツ

    private void Start()
    {
        //audioSource初期化
        _audioSource = gameObject.GetComponent<AudioSource>();

    }

    private void Update()
    {
        
    }
}
