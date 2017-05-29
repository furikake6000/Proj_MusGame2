using System.IO;
using UnityEngine;
using System.Text.RegularExpressions;

public class MusicData : MonoBehaviour {

    //Values
    public string Wav{ get; } //音楽ファイルへの絶対パス
    public int Bpm { get; } //BPM
    public string Title { get; }    //曲名
    public string Genre { get; }    //ジャンル（メニューでの区分けに使われる）
    public string Artist { get; }   //作曲者名

    //Constructor

    /// <summary>
    /// 全てを指定するコンストラクタです。
    /// </summary>
    /// <param name="wav">オーディオファイルへのパス(wavファイルに限らず)</param>
    /// <param name="bpm">Beat Per Minute</param>
    /// <param name="title">曲名</param>
    /// <param name="genre">ジャンル（メニューでの区分けに使います）</param>
    /// <param name="artist">アーティスト</param>
    public MusicData(string wav, int bpm, string title, string genre, string artist)
    {
        this.Wav = wav;
        this.Bpm = bpm;
        this.Title = title;
        this.Genre = genre;
        this.Artist = artist;
    }

    /// <summary>
    /// 空のコンストラクタです。（デフォルトBPMは130です）
    /// </summary>
    public MusicData()
        : this("", 130, "", "", "") { }

    /// <summary>
    /// 指定されたデータファイルから全てのデータを読み込みます。
    /// </summary>
    /// <param name="dataFilePath">データファイルの絶対パス</param>
    public MusicData(string dataFilePath)
        : this()
    {
        //↑とりあえず最初はデフォルトコンストラクタで初期化

        //曲名はデフォルトだとデータファイルと同名になる
        Title = Path.GetFileNameWithoutExtension(dataFilePath);

        //検索用正規表現一覧
        Regex regTitle = new Regex(@"^#TITLE\s.+$"); //TITLE 一文字以上の文字列
        Regex regGenre = new Regex(@"^#GENRE\s.+$"); //GENRE 一文字以上の文字列
        Regex regArtist = new Regex(@"^#ARTIST\s.+$");   //ARTIST 一文字以上の文字列
        Regex regBpm = new Regex(@"^#BPM\s\d+$");    //BPM 一文字以上の数列（任意の自然数）
        Regex regWav = new Regex(@"^#WAV[0-9A-Fa-f][0-9A-Za-z]\s.+\.(wav|ogg|mp3)$");    //WAV オーディオファイル読み込み（wav/ogg/mp3ファイル）

        //エンコードは自動判別
        using (StreamReader dataFileStream = new StreamReader(dataFilePath, true))
        {
            while (dataFileStream.Peek() >= 0)
            {
                //一行ずつ読み込み
                string dataLine = dataFileStream.ReadLine();

                if (regTitle.IsMatch(dataLine))
                {
                    //TITLE
                    Title = dataLine.Substring(8);
                }else 
                if (regGenre.IsMatch(dataLine))
                {
                    //GENRE
                    Genre = dataLine.Substring(8);
                }else 
                if (regArtist.IsMatch(dataLine))
                {
                    //ARTIST
                    Artist = dataLine.Substring(9);
                }else 
                if (regBpm.IsMatch(dataLine))
                {
                    //BPM
                    Bpm = int.Parse(dataLine.Substring(6));
                }else
                if (regWav.IsMatch(dataLine))
                {
                    //WAV
                    Wav = Path.GetDirectoryName(dataFilePath) + dataLine.Substring(8);
                }
            }
        }
    }

    //Methods
}
