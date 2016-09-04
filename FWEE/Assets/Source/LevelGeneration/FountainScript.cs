using UnityEngine;
using System.Collections;

public enum FountainType {
    Earth,
    Water,
    Fire,
    Electric
}

public class FountainScript : MonoBehaviour {

    public Sprite earthFountain;
    public Sprite waterFountain;
    public Sprite fireFountain;
    public Sprite electricFountain;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

    public void SetFountainType(FountainType type) {
        switch (type) {
            case FountainType.Earth:
                GetComponent<SpriteRenderer>().sprite = earthFountain;
                break;
            case FountainType.Water:
                GetComponent<SpriteRenderer>().sprite = waterFountain;
                break;
            case FountainType.Fire:
                GetComponent<SpriteRenderer>().sprite = fireFountain;
                break;
            case FountainType.Electric:
                GetComponent<SpriteRenderer>().sprite = electricFountain;
                break;
        }
    }
}
