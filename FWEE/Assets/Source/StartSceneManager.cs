using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

public class StartSceneManager : MonoBehaviour {

    public GameObject Loading;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
        if (Input.GetKeyDown(KeyCode.Space)) {
            Loading.SetActive(true);
            SceneManager.LoadScene(1);
        }
	}
}
