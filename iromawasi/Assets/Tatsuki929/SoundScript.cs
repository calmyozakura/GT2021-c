using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundScript : MonoBehaviour
{
    public AudioSource audioSource;
    public AudioClip[] audioClips;
    
    private const short songs = 10;


    public bool Play=false;//再生しているか　falseで未再生

    // Start is called before the first frame update
    void Start()
    {
        //オーディオソースの取得
        audioSource = this.GetComponent<AudioSource>();

        //クリップを動的に取得
        audioClips[0] = Resources.Load<AudioClip>("Electronic_Circuit");
        //audioClips[1] = Resources.Load<AudioClip>("untitled");

        audioSource.clip = audioClips[0];

        audioSource.volume = 0.25f;

        audioSource.Play(); Play = true;

    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))//スペースを押し下げたとき
        {

            if (!Play)
            {   //BGMを再生
                audioSource.Play();
                Play = true;
            }
            else 
            {   //BGMを停止
                audioSource.Stop();
                Play = false;
            }
        }


    }
}
