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
    List<bpmData> _bpm; //初期BPM
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
    public List<bpmData> Bpm { get{ return _bpm; } }
    public string Midifile { get{ return _midifile; } }
    public int Playlevel { get{ return _playlevel; } }
    public int Rank { get{ return _rank; } }
    public int VolWav { get{ return _volWav; } }
    public int Total { get{ return _total; } }
    public string StageImage { get { return _stageImage; } }

    //配列データ（0-Zの36進数2桁のTKeyを持つ。TKeyでは大文字を使うものとする）
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
        _bpm = new List<bpmData>();
        _bpm.Add(new bpmData(0, 130));  //デフォルトとして130のbpmを追加
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
                if(dataLine.Length > 0 && dataLine[0] == '#')
                {
                    //「 (半角スペース)」、「:(コロン)」によって文字列をコマンドと内容物に分割
                    char[] separators = { ' ', ':' };
                    int sepIndex = dataLine.IndexOfAny(separators);
                    string command = dataLine.Substring(1, sepIndex - 1).Trim().ToUpper();  //コマンド部。検索を用意にするため大文字に統一
                    string content = dataLine.Substring(sepIndex + 1).Trim();   //内容部。

                    //splitedData[0]にコマンド名が記載されているはずなので読取
                    switch (command)
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
                            //これによってデフォルトとあわせ二つのbpmDataが出来るが、常に後者を優先するため問題なし
                            _bpm.Add(new bpmData(0f, float.Parse(content)));
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

                            //判定
                            if (Regex.IsMatch(command, @"^\d{5}$") && Regex.IsMatch(content, @"([0-9A-Z]{2})+"))
                            {
                                //メインデータ部（音符の定義）
                                int measure = int.Parse(command.Substring(0, 3));
                                int channel = int.Parse(command.Substring(3, 2));

                                //BPM変更検知
                                if (channel == 3)
                                {
                                    int notesDensity = content.Length / 2;
                                    for (int i = 0; i < notesDensity; i++)
                                    {
                                        //2文字ずつ読み込み
                                        int newBpm = int.Parse(content.Substring(i * 2, 2));
                                        //現在触っている譜面上時間を取得
                                        float nowTimeOnScore = measure + ((float)i / notesDensity);

                                        //BPM変更を追加
                                        _bpm.Add(new bpmData(nowTimeOnScore, newBpm));
                                    }
                                }
                                //BPM変更検知（BPM値として宣言された値）
                                if (channel == 8)
                                {
                                    int notesDensity = content.Length / 2;
                                    for (int i = 0; i < notesDensity; i++)
                                    {
                                        //2文字ずつ読み込み
                                        string bpmID = content.Substring(i * 2, 2);
                                        //現在触っている音符の譜面上時間を取得
                                        float nowTimeOnScore = measure + ((float)i / notesDensity);

                                        //BPM変更を追加
                                        _bpm.Add(new bpmData(nowTimeOnScore, _bpmEx[bpmID]));
                                        /*
                                         * ここで問題アリ！！！
                                         * BPMEXの宣言前にこの命令が出されたら要素無しでエラー！！
                                         * （大抵の場合事前に宣言してあるが...）
                                         * しゅうせいするべし
                                         */
                                    }
                                }
                            }
                            else if (Regex.IsMatch(command, @"^WAV[0-9A-Z]{2}$"))
                            {
                                //WAVxx（音声ファイルの定義）
                                _wav.Add(command.Substring(3, 2), Path.GetDirectoryName(_dataFilePath) + content);
                            }
                            else if (Regex.IsMatch(command, @"^BMP[0-9A-Z]{2}$"))
                            {
                                //BMPxx（画像ファイルの定義）
                                _bmp.Add(command.Substring(3, 2), Path.GetDirectoryName(_dataFilePath) + content);
                            }
                            else if (Regex.IsMatch(command, @"^BPM[0-9A-Z]{2}$"))
                            {
                                //BPMxx（拡張bpmの定義）
                                _bpmEx.Add(command.Substring(3, 2), float.Parse(content));
                            }
                            else
                            {
                                //どこにもひっかからない謎のコマンド
                            }
                            break;
                    }
                }
            }
        }
    }

    /// <summary>
    /// データ部を解析しノーツ配列を返す
    /// </summary>
    /// <returns>ノーツ配列</returns>
    public List<MusicNote> loadMusicNotes()
    {
        List<MusicNote> notes = new List<MusicNote>();

        using (StreamReader dataFileStream = new StreamReader(DataFilePath, true))
        {
            while (dataFileStream.Peek() >= 0)
            {
                //一行ずつ読み込む
                string dataLine = dataFileStream.ReadLine().ToUpper();

                //データ部のみ抜き出して読み込み
                if (Regex.IsMatch(dataLine, @"^#\d{5}:([0-9A-Z]{2})+$"))
                {
                    //メインデータ部（音符の定義）
                    int measure = int.Parse(dataLine.Substring(1, 3));
                    int channel = int.Parse(dataLine.Substring(4, 2));
                    string content = dataLine.Substring(7);

                    //音符配置（チャンネルが0(オート音符)または10～19(1P譜面)ならば）
                    if (channel == 0 || (channel >= 10 && channel <= 19))
                    {
                        int notesDensity = content.Length / 2;
                        for (int i=0; i<notesDensity; i++)
                        {
                            //2文字ずつ読み込み
                            string wavID = content.Substring(i * 2, 2);
                            //現在触っている音符の譜面上時間を取得
                            float nowTimeOnScore = measure + ((float)i / notesDensity);

                            MusicNote newNote = new MusicNote(channel, wavID, nowTimeOnScore, MusicNote.NOTE_DULATION_DEFAULT);
                        }
                    }
                }
            }
        }

        return notes;
    }

}

//BPM途中変更に関するデータ
public struct bpmData
{
    public float bpm;   //変更された後のBPM
    public float measure;      //変更される位置（譜面上時間）

    public bpmData(float measure, float bpm)
    {
        this.bpm = bpm;
        this.measure = measure;
    }
}