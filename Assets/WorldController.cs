using UnityEngine;
using System.Collections;

public class WorldController : MonoBehaviour {

    public GameObject animat, foodplant;
    public uint maximum;
    public uint animatCount;
    public int seed = 0;

    private Spawner spawner;

	// Use this for initialization
	void Start () {
        Random.seed = seed;
        spawner = transform.GetComponent<Spawner>();
        spawner.MultiSpawn(animat, maximum);
        spawner.MultiSpawn(foodplant, maximum);
        animatCount = maximum;
	}

    void Update() {
        spawner.MultiSpawn(animat, maximum - animatCount);
        animatCount = maximum;
    }
}
