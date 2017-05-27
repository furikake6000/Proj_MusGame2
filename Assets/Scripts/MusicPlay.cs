using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

[RequireComponent(typeof(AudioSource))]
public class MusicPlay : MonoBehaviour {

    //定数群
    const string MUSIC_DIR = "Musics/"; //StreamingAssetsフォルダ内の音楽ファイル群の位置

    //Inspector指定変数群
    [SerializeField]
    Text debugLog;   //ログ表示用テキストボックス
    [SerializeField]
    Text trackDisplay;   //トラック番号表示用テキストボックス
    [SerializeField]
    Text seekDisplay;   //再生位置表示用テキストボックス

    //グローバル変数群
    string[] musics;    //音楽ファイル一覧取得
    int selectedTrack = 0;
    AudioSource audioSource;

    // Use this for initialization
    void Start () {
        //audioSource初期化
        audioSource = gameObject.GetComponent<AudioSource>();

        //musicFiles初期化
        musics = new string[0];
        //音楽を一括取得
        string[] patterns = new string[] { "*.ogg", "*.wav" , "*.mp3" };
        foreach (string pattern in patterns)
        {
            //StreamingAssetsフォルダ内のパターンにマッチしたファイル名の配列を取得
            string[] matchFiles = Directory.GetFiles(Application.streamingAssetsPath + "\\" + MUSIC_DIR, pattern);
            //musicFilesの末尾に追加
            Array.Resize<string>(ref musics, musics.Length + matchFiles.Length);
            Array.Copy(matchFiles, 0, musics, musics.Length - matchFiles.Length, matchFiles.Length);
        }

        foreach (string musicFile in musics)
        {
            debugLog.text += Path.GetFileName(musicFile) + "\n";
        }
        debugLog.text += musics.Length + " Files loaded.\n";

    }

    private void Update()
    {
        //表示更新
        RefreshDisplay();
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
        WWW www = new WWW("file:///" + fileName);
        //読み込み完了まで待機
        yield return www;

        audioSource.clip = www.GetAudioClip(true,true);

        audioSource.Play();
    }

    //Playボタンクリック
    public void PressPlayButton()
    {
        //音楽読み込み&開始
        StartCoroutine("StreamPlayAudioFile", musics[selectedTrack]);
    }

    //トラック進ボタンクリック
    public void PressFormerTrackButton()
    {
        //音楽止めとく
        if (audioSource.isPlaying) audioSource.Stop();

        selectedTrack--;
        if (selectedTrack < 0) selectedTrack = musics.Length - 1;
    }

    //トラック戻ボタンクリック
    public void PressLatterTrackButton()
    {
        //音楽止めとく
        if(audioSource.isPlaying)audioSource.Stop();

        selectedTrack++;
        if (selectedTrack > musics.Length - 1) selectedTrack = 0;
    }

    //表示更新
    void RefreshDisplay()
    {
        //トラックテキスト更新
        trackDisplay.text = "Track " + (selectedTrack + 1) + "/" + musics.Length;

        //現在再生位置を取得(0-1正規化)
        float nowSeek = (audioSource.isPlaying) ? (audioSource.time / audioSource.clip.length) : 0.0f;
        //テキストに反映
        string seekBarText = "";    //新しいテキスト
        const int BARNUM = 40;  //バーの数
        for (int i = 0; i < BARNUM; i++)
        {
            seekBarText += (nowSeek <= ((float)i / BARNUM)) ? "□" : "■";
        }
        seekBarText += "\n";
        seekDisplay.text = seekBarText;
    }

    //float型を分秒表示に
    string FloatToMinSecString(float time)
    {
        return ((int)(time / 60)).ToString("D2") + ":" + ((int)(time % 60)).ToString("D2");
    }
}
