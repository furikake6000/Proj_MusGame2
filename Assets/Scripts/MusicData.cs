using System.IO;
using System.Text.RegularExpressions;

public class MusicData {

    //Values
    public string Wav{ get; } //音楽ファイルへの絶対パス
    public int Bpm { get; } //BPM
    public string Title { get; }    //曲名
    public string Genre { get; }    //ジャンル（メニューでの区分けに使われる）
    public string Artist { get; }   //作曲者名
    public string DataFilePath { get; } //楽譜データファイルへの絶対パス

    //Constructor

    /// <summary>
    /// 全てを指定するコンストラクタです。
    /// </summary>
    /// <param name="wav">オーディオファイルへのパス(wavファイルに限らず)</param>
    /// <param name="bpm">Beat Per Minute</param>
    /// <param name="title">曲名</param>
    /// <param name="genre">ジャンル（メニューでの区分けに使います）</param>
    /// <param name="artist">アーティスト</param>
    /// <param name="dataFilePath">楽譜データファイルへの絶対パス</param>
    public MusicData(string dataFilePath, string wav, string title, int bpm, string genre, string artist)
    {
        this.DataFilePath = dataFilePath;
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
        : this("", "", "", 130, "", "") { }

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

        //検索用正規表現（ヘッダー部）
        Regex regTitle = new Regex(@"^#TITLE\s.+$"); //TITLE 一文字以上の文字列
        Regex regGenre = new Regex(@"^#GENRE\s.+$"); //GENRE 一文字以上の文字列
        Regex regArtist = new Regex(@"^#ARTIST\s.+$");   //ARTIST 一文字以上の文字列
        Regex regBpm = new Regex(@"^#BPM\s\d+$");    //BPM 一文字以上の数列（任意の自然数）
        Regex regWav = new Regex(@"^#WAV[0-9A-Fa-f][0-9A-Za-z]\s.+\.(wav|ogg|mp3)$");    //WAV オーディオファイル読み込み（wav/ogg/mp3ファイル）
        //検索用正規表現（データ部）
        Regex regObjDef = new Regex(@"^#\d{3}[1-6][1-9]:([0-9A-Fa-f][0-9A-Za-z])+$");  //各種オブジェクト定義（定義部は36進数）
        Regex regBackDef = new Regex(@"^#\d{3}01:([0-9A-Fa-f][0-9A-Za-z])+$");  //バックコーラス定義（定義部は36進数）
        Regex regShortenTremolo = new Regex(@"^#\d{3}02:\d+(\.\d+)?$");   //小節の短縮（定義部は小数含む正数）
        Regex regChangeBpm = new Regex(@"^#\d{3}03:([0-9A-Fa-f]{2})+$");   //BPMの変更（定義部は16進数）

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
                    Title = dataLine.Substring(6);
                }else 
                if (regGenre.IsMatch(dataLine))
                {
                    //GENRE
                    Genre = dataLine.Substring(6);
                }else 
                if (regArtist.IsMatch(dataLine))
                {
                    //ARTIST
                    Artist = dataLine.Substring(7);
                }else 
                if (regBpm.IsMatch(dataLine))
                {
                    //BPM
                    Bpm = int.Parse(dataLine.Substring(5));
                }else
                if (regWav.IsMatch(dataLine))
                {
                    //WAV
                    Wav = Path.GetDirectoryName(dataFilePath) + dataLine.Substring(6);
                }
                //...(以降データ部の読み取りが続く)
            }
        }
    }
}
