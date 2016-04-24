using UnityEngine;
using System.Collections;

public class Food : MonoBehaviour {

    public int energy = 10;
    private int biteSize = 10;

    private bool isGrowing = false;

    private SpriteRenderer rdr;

	// Use this for initialization
	void Start () {
        rdr = GetComponent<SpriteRenderer>();
	}

    public int GetEnergy() {
        if (!isGrowing) {
            DecreaseEnergyBy(biteSize);
            return biteSize;
        }
        return 0;
    }

    public bool IsEdible() {
        return !isGrowing;
    }

    void IncreaseEnergyBy(int e) {
        energy = Mathf.Clamp(energy + e, 0, 100);
    }

    void DecreaseEnergyBy(int e) {
        energy = Mathf.Clamp(energy - e, 0, 100);
    }

	// Update is called once per frame
	void Update () {
        if (isGrowing) {
            IncreaseEnergyBy(1);

            if (energy == 100) {
                isGrowing = false;
                rdr.enabled = true;
            }
        } else if (energy == 0) {
            rdr.enabled = false;
            isGrowing = true;
        }
	}
}
