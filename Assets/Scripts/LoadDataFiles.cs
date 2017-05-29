using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

public class LoadDataFiles : MonoBehaviour {

    //定数群
    const string MUSIC_DIR = "Musics/"; //StreamingAssetsフォルダ内の音楽ファイル群の位置

    //Inspector指定変数群
    [SerializeField]
    Text debugLog;   //ログ表示用テキストボックス

    //グローバル変数群
    MusicData[] musics;    //音楽ファイル一覧取得
    int selectedTrack = 0;

    // Use this for initialization
    void Start () {
        LoadMusics();

        if(musics.Length >= 1)
        {
            debugLog.text =
                "Title: " + musics[0].Title + "\n" +
                "Genre: " + musics[0].Genre + "\n" +
                "Artist: " + musics[0].Artist + "\n" +
                "BPM: " + musics[0].Bpm + "\n";
        }
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    void LoadMusics()
    {
        //音楽を一括取得
        string[] musicDataPath = new string[0]; //データファイル格納用配列
        
        string[] patterns = new string[] { "*.bms" };   //データファイルの形式一覧
        foreach (string pattern in patterns)
        {
            //StreamingAssetsフォルダ内のパターンにマッチしたファイル名の配列を取得
            //(フォルダ下全ファイルを列挙)
            string[] matchFiles = Directory.GetFiles(Application.streamingAssetsPath + "\\" + MUSIC_DIR, pattern, SearchOption.AllDirectories);
            //musicFilesの末尾に追加
            Array.Resize<string>(ref musicDataPath, musicDataPath.Length + matchFiles.Length);
            Array.Copy(matchFiles, 0, musicDataPath, musicDataPath.Length - matchFiles.Length, matchFiles.Length);
        }

        //musics配列を初期化
        musics = new MusicData[musicDataPath.Length];
        for (int i = 0; i < musics.Length; i++)
        {
            //musicDataPathそれぞれに対してmusicsを読み込む
            musics[i] = new MusicData(musicDataPath[i]);
        }
    }
}
