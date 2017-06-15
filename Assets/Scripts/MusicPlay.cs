using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

[RequireComponent(typeof(AudioSource))]
public class MusicPlay : MonoBehaviour {

    //変数群
    AudioSource _audioSource;    //このgameObjectに紐付けられたAudioSource(Required)

    MusicData _nowMusic;         //現在再生しているMusicのData

    Key[] _keys; //鍵（けん）

    public MusicData NowMusic {
        get { return _nowMusic; }
        set { _nowMusic = value; }
    }
    
    // Start関数
    private void Start()
    {
        //audioSource初期化
        _audioSource = gameObject.GetComponent<AudioSource>();
        
    }
    
    // Update関数
    private void Update()
    {

    }

    /// <summary>
    /// 音楽ファイル、およびノーツ配置をロード
    /// </summary>
    private void LoadMusic()
    {
        //もしnowMusicが存在しなかったら終了措置
        if (_nowMusic == null) return;
        
        //音楽のロード
        
        //鍵の情報取得

        //ノーツ情報取得、各鍵に配置

    }
    /// <summary>
    /// 音楽ファイルを非同期的に読み込み
    /// </summary>
    /// <param name="fileName">ファイル名</param>
    /// <returns></returns>
    IEnumerator StreamAudioFile(string fileName)
    {
        //ソース指定し音楽流す
        //音楽ファイルロード
        using (WWW www = new WWW("file:///" + fileName))
        {
            //読み込み完了まで待機
            yield return www;

            _audioSource.clip = www.GetAudioClip(true, true);

            _audioSource.Play();
        }
    }

    /// <summary>
    /// 再生を開始する
    /// </summary>
    private void Play()
    {
        if(_nowMusic == null)
        {
            //音楽が設定されていなかったら終了
            return;
        }
        
        ////Midifileをロード（非同期処理）
        //StartCoroutine("StreamAudioFile", _nowMusic.Midifile);
    }

    /// <summary>
    /// 秒を表すfloat型から「分:秒」の形のStringを出力
    /// </summary>
    /// <param name="time">時間（秒単位）</param>
    /// <returns>「分:秒」の形の文字列（秒は2桁表記）</returns>
    string FloatToMinSecString(float time)
    {
        return ((int)(time / 60)).ToString("D2") + ":" + ((int)(time % 60)).ToString("D2");
    }
}
