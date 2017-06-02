using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

public class MusicDataManager : MonoBehaviour {

    //定数群
    const string MUSIC_DIR = "Musics/"; //StreamingAssetsフォルダ内の音楽ファイル群の位置

    //グローバル変数群
    public static MusicData[] Musics { get; private set; }    //音楽ファイル一覧取得

    /// <summary>
    /// 指定ディレクトリ下に存在する音楽データの一括読み込み
    /// </summary>
    public static void LoadMusics()
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
        Musics = new MusicData[musicDataPath.Length];
        for (int i = 0; i < Musics.Length; i++)
        {
            //musicDataPathそれぞれに対してmusicsを読み込む
            Musics[i] = new MusicData(musicDataPath[i]);
        }
    }
}
