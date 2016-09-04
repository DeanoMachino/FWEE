using UnityEngine;
using System.Collections;

public class GameManager : MonoBehaviour {

    public static GameManager Instance;

    public AudioClip earthMusic;
    public AudioClip waterMusic;
    public AudioClip fireMusic;
    public AudioClip electricMusic;

	// Use this for initialization
	void Start () {
        GameObject music = new GameObject("Music");
        music.AddComponent<AudioSource>();
        switch (Random.Range(0, 3)) {
            case 0:
                music.GetComponent<AudioSource>().clip = earthMusic;
                break;
            case 1:
                music.GetComponent<AudioSource>().clip = waterMusic;
                break;
            case 2:
                music.GetComponent<AudioSource>().clip = fireMusic;
                break;
            case 3:
                music.GetComponent<AudioSource>().clip = electricMusic;
                break;
            default:
                goto case 0;
        }

        music.GetComponent<AudioSource>().loop = true;

        music.GetComponent<AudioSource>().Play();
	}
	
	// Update is called once per frame
	void Update () {
	
	}

    void Awake() {
        Instance = this;
    }
}
