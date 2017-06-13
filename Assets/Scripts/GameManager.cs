using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static MusicDataManager;

public class GameManager : MonoBehaviour {

	// Use this for initialization
	void Start () {
        LoadMusics();

        for (int i = 0; i < Musics.Length; i++)
        {
            Debug.Log("「" + Musics[i].Title + "」 loaded.");
        }
        Debug.Log(Musics.Length + " musics loaded.");
    }
	
	// Update is called once per frame
	void Update () {
		
	}
}
