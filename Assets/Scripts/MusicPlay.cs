using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

[RequireComponent(typeof(AudioSource))]
public class MusicPlay : MonoBehaviour {

    //グローバル変数群
    MusicData nowMusic;         //現在再生しているMusicのData
    AudioSource audioSource;    //このgameObjectに紐付けられたAudioSource(Required)
    Key[] keys; //鍵（けん）

    public MusicData NowMusic
    {
        get
        {
            return nowMusic;
        }

        set
        {
            nowMusic = value;
        }
    }

    // Use this for initialization
    private void Start () {
        //audioSource初期化
        audioSource = gameObject.GetComponent<AudioSource>();

        
    }

    private void Update()
    {
        //表示更新
        RefreshDisplay();
    }

    /// <summary>
    /// 再生を開始する
    /// </summary>
    private void Play()
    {
        if(nowMusic == null)
        {
            //音楽が設定されていなかったら終了
            return;
        }
        
        //WavMainを再生（非同期処理）
        StartCoroutine("StreamPlayAudioFile", nowMusic.WavMain);
    }

    /// <summary>
    /// 音楽ファイルを非同期的に読み込み
    /// </summary>
    /// <param name="fileName">ファイル名</param>
    /// <returns></returns>
    IEnumerator StreamPlayAudioFile(string fileName)
    {
        //ソース指定し音楽流す
        //音楽ファイルロード
        using(WWW www = new WWW("file:///" + fileName))
        {
            //読み込み完了まで待機
            yield return www;

            audioSource.clip = www.GetAudioClip(true, true);

            audioSource.Play();
        }
    }
    
    //表示更新
    void RefreshDisplay()
    {
        
    }

    //float型を分秒表示に
    string FloatToMinSecString(float time)
    {
        return ((int)(time / 60)).ToString("D2") + ":" + ((int)(time % 60)).ToString("D2");
    }
}
