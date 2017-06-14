using System.IO;
using System.Collections.Generic;
using System.Text.RegularExpressions;

/// <summary>
/// 楽曲のヘッダデータをファイルから読み込むクラス
/// </summary>
public class MusicData {

    //変数群
    string _dataFilePath;   //楽譜データファイルへの絶対パス
    public string DataFilePath { get{ return _dataFilePath; } }
    
    //ファイルから読み込むデータ群
    //単一データ
    int _player;        //プレイモード
    string _genre;      //ジャンル
    string _title;      //曲名
    string _artist;     //制作者名
    float _bpm;         //初期BPM
    string _midifile;   //バックグラウンドで流す音声ファイルへの絶対パス
    int _playlevel;     //難易度
    int _rank;          //ランク
    int _volWav;        //音量補正(%)
    int _total;         //ゲージの増量
    string _stageImage; //曲開始時の画像

    public int Player { get{ return _player; } }
    public string Genre { get{ return _genre; } }
    public string Title { get{ return _title; } }
    public string Artist { get{ return _artist; } }
    public float Bpm { get{ return _bpm; } }
    public string Midifile { get{ return _midifile; } }
    public int Playlevel { get{ return _playlevel; } }
    public int Rank { get{ return _rank; } }
    public int VolWav { get{ return _volWav; } }
    public int Total { get{ return _total; } }
    public string StageImage { get { return _stageImage; } }

    //配列データ（0-Zの36進数2桁で定義されている。Keyでは大文字を使うものとする）
    Dictionary<string, string> _wav;    //音声ファイルへの絶対パス
    Dictionary<string, string> _bmp;    //表示画像
    Dictionary<string, float> _bpmEx;  //拡張BPM

    public Dictionary<string, string> Wav { get{ return _wav; } }
    public Dictionary<string, string> Bmp { get{ return _bmp; } }
    public Dictionary<string, float> BpmEx { get{ return _bpmEx; } }
    
    //メソッド

    /// <summary>
    /// 空のコンストラクタです。（デフォルトBPMは130です）
    /// </summary>
    public MusicData() {
        _dataFilePath = "";

        _genre = "";
        _title = "";
        _artist = "";
        _bpm = 130;
        _midifile = "";
        _playlevel = 0;
        _rank = 0;
        _volWav = 100;
        _total = 0;

        _wav = new Dictionary<string, string>();
        _bmp = new Dictionary<string, string>();
        _bpmEx = new Dictionary<string, float>();
    }

    /// <summary>
    /// 指定されたデータファイルからヘッダ データを読み込みます。
    /// </summary>
    /// <param name="dataFilePath">データファイルの絶対パス</param>
    public MusicData(string dataFilePath)
        : this()
    {
        //↑とりあえず最初はデフォルトコンストラクタで初期化
        
        //データファイルパス保存
        _dataFilePath = dataFilePath;

        //曲名はデフォルトだとデータファイルと同名になる
        _title = Path.GetFileNameWithoutExtension(dataFilePath);

        //エンコードは自動判別
        using (StreamReader dataFileStream = new StreamReader(dataFilePath, true))
        {
            while (dataFileStream.Peek() >= 0)
            {
                //一行ずつ読み込む
                string dataLine = dataFileStream.ReadLine();

                //一文字目が「#」かどうかでコマンドコードを判別
                if(dataLine[0] == '#')
                {
                    //「 (半角スペース)」、「:(コロン)」によって文字列をコマンドと内容物に分割
                    char[] separators = { ' ', ':' };
                    int sepIndex = dataLine.IndexOfAny(separators);
                    string command = dataLine.Substring(1, sepIndex - 1).Trim();
                    string content = dataLine.Substring(sepIndex + 1).Trim();

                    //splitedData[0]にコマンド名が記載されているはずなので読取
                    switch (command.ToUpper())
                    {
                        case "PLAYER":
                            _player = int.Parse(content);
                            break;
                        case "TITLE":
                            _title = content;
                            break;
                        case "GENRE":
                            _genre = content;
                            break;
                        case "ARTIST":
                            _artist = content;
                            break;
                        case "BPM":
                            _bpm = float.Parse(content);
                            break;
                        case "MIDIFILE":
                            _midifile = Path.GetDirectoryName(_dataFilePath) + content;
                            break;
                        case "PLAYLEVEL":
                            _playlevel = int.Parse(content);
                            break;
                        case "RANK":
                            _rank = int.Parse(content);
                            break;
                        case "VOLWAV":
                            _volWav = int.Parse(content);
                            break;
                        case "TOTAL":
                            _total = int.Parse(content);
                            break;
                        case "STAGEFILE":
                            _stageImage = Path.GetDirectoryName(_dataFilePath) + content;
                            break;
                        default:
                            //switch文で読み取れないコマンドの判別

                            break;
                    }
                }
            }
        }
    }

    /*
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
                    Title = dataLine.Substring(7);
                }
                else
                if (regGenre.IsMatch(dataLine))
                {
                    //GENRE
                    Genre = dataLine.Substring(7);
                }
                else
                if (regArtist.IsMatch(dataLine))
                {
                    //ARTIST
                    Artist = dataLine.Substring(8);
                }
                else
                if (regBpm.IsMatch(dataLine))
                {
                    //BPM
                    Bpm = int.Parse(dataLine.Substring(6));
                }
                else
                if (regWavMain.IsMatch(dataLine))
                {
                    //WAVMAIN
                    WavMain = Path.GetDirectoryName(DataFilePath) + dataLine.Substring(9);
                }
                else
                if (regWav.IsMatch(dataLine))
                {
                    //WAVXX(XXには36進数が入る、小文字は大文字に)
                    Wav[dataLine.Substring(4, 2).ToUpper()] = Path.GetDirectoryName(DataFilePath) + dataLine.Substring(7);
                }
            }
        }
    }*/
}