using System.IO;
using System.Collections.Generic;
using System.Text.RegularExpressions;

/// <summary>
/// 楽曲のヘッダデータをファイルから読み込むクラス
/// </summary>
public class MusicData {

    //Values
    public string WavMain { get; private set; } //メインの音楽ファイル(絶対パス)
    public Dictionary<string, string> Wav { get; private set; }  //キータップ音などの音楽ファイル(ディクショナリ、絶対パス)
    public int Bpm { get; private set; } //BPM
    public string Title { get; private set; }    //曲名
    public string Genre { get; private set; }    //ジャンル（メニューでの区分けに使われる）
    public string Artist { get; private set; }   //作曲者名
    public string DataFilePath { get; private set; } //楽譜データファイルへの絶対パス

    //Constructor

    /// <summary>
    /// 空のコンストラクタです。（デフォルトBPMは130です）
    /// </summary>
    public MusicData() {
        DataFilePath = "";
        WavMain = "";
        Wav = new Dictionary<string, string>;
        Bpm = 130;
        Title = "";
        Genre = "";
        Artist = "";
    }

    /// <summary>
    /// 指定されたデータファイルからヘッダ データを読み込みます。
    /// </summary>
    /// <param name="dataFilePath">データファイルの絶対パス</param>
    public MusicData(string dataFilePath)
        : this()
    {
        //↑とりあえず最初はデフォルトコンストラクタで初期化

        //曲名はデフォルトだとデータファイルと同名になる
        Title = Path.GetFileNameWithoutExtension(dataFilePath);

        //データファイルパス格納
        DataFilePath = dataFilePath;

        //検索用正規表現（ヘッダー部）
        Regex regTitle = new Regex(@"^#TITLE\s.+$"); //TITLE 一文字以上の文字列
        Regex regGenre = new Regex(@"^#GENRE\s.+$"); //GENRE 一文字以上の文字列
        Regex regArtist = new Regex(@"^#ARTIST\s.+$");   //ARTIST 一文字以上の文字列
        Regex regBpm = new Regex(@"^#BPM\s\d+$");    //BPM 一文字以上の数列（任意の自然数）
        Regex regWavMain = new Regex(@"^#WAVMAIN\s.+\.(wav|ogg|mp3)$");    //WAV メインオーディオファイル読み込み（wav/ogg/mp3ファイル）
        Regex regWav = new Regex(@"^#WAV[0-9A-Fa-f][0-9A-Za-z]\s.+\.(wav|ogg|mp3)$");    //WAV オーディオファイル読み込み（wav/ogg/mp3ファイル）

        ////検索用正規表現（データ部）
        //Regex regObjDef = new Regex(@"^#\d{3}[1-6][1-9]:([0-9A-Fa-f][0-9A-Za-z])+$");  //各種オブジェクト定義（定義部は36進数）
        //Regex regBackDef = new Regex(@"^#\d{3}01:([0-9A-Fa-f][0-9A-Za-z])+$");  //バックコーラス定義（定義部は36進数）
        //Regex regShortenTremolo = new Regex(@"^#\d{3}02:\d+(\.\d+)?$");   //小節の短縮（定義部は小数含む正数）
        //Regex regChangeBpm = new Regex(@"^#\d{3}03:([0-9A-Fa-f]{2})+$");   //BPMの変更（定義部は16進数）

        //エンコードは自動判別
        using (StreamReader dataFileStream = new StreamReader(dataFilePath, true))
        {
            while (dataFileStream.Peek() >= 0)
            {
                //一行ずつ読み込む
                string dataLine = dataFileStream.ReadLine();

                //条件分岐部分
                if (regTitle.IsMatch(dataLine))
                {
                    //TITLE
                    Title = dataLine.Substring(6);
                }
                else
                if (regGenre.IsMatch(dataLine))
                {
                    //GENRE
                    Genre = dataLine.Substring(6);
                }
                else
                if (regArtist.IsMatch(dataLine))
                {
                    //ARTIST
                    Artist = dataLine.Substring(7);
                }
                else
                if (regBpm.IsMatch(dataLine))
                {
                    //BPM
                    Bpm = int.Parse(dataLine.Substring(5));
                }
                else
                if (regWavMain.IsMatch(dataLine))
                {
                    //WAVMAIN
                    WavMain = Path.GetDirectoryName(DataFilePath) + dataLine.Substring(8);
                }
                else
                if (regWav.IsMatch(dataLine))
                {
                    //WAVXX(XXには36進数が入る、小文字は大文字に)
                    Wav[dataLine.Substring(3, 2).ToUpper()] = Path.GetDirectoryName(DataFilePath) + dataLine.Substring(6);
                }
            }
        }
    }
}