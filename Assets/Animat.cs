using UnityEngine;
using System.Collections;

public class Animat : MonoBehaviour {
    StochasticAssembly brain;
    public float speed = 1.0f;
    public float rangeVision = 20.0f;

    GameObject target = null;
    float targetDist = float.MaxValue;
    bool moving = false;

    private static float maxEnergy = 1000f;
    public float energy = maxEnergy;

	// Use this for initialization
	void Start () {
        brain = new StochasticAssembly();
	}

    void Eat() {
        if (target != null) { // If we have no target then waste the action
            if (target.tag == "Food") { // If our target is food then eat it.
                brain.Reinforce(10 / (Cost(energy)));
                energy += target.transform.GetComponent<Food>().GetEnergy();
            } else if (target.tag == "Animat") { // If our target is an animat then attack it.
                target.GetComponent<Animat>().energy -= maxEnergy / 10;
                brain.Reinforce(10 / (Cost(energy)) - maxEnergy / 10);
            }
            target = null;
        }
    }

    void Move() {
        Vector3 moveVector;
        if (target == null) { // Move randomly if we dont have a target
            moveVector = new Vector3(Random.Range(-1f, 1f), Random.Range(-1f, 1f), 0f);
        } else { // Move towards the target
            moveVector = Vector3.MoveTowards(transform.position, target.transform.position, speed) - transform.position;
        }

        // Perform our computations for turning and movement
        transform.Translate(moveVector);

        float newAngle = Vector3.Angle(new Vector3(1f, 0f, 0f), moveVector);
        if (moveVector.y < 0) {
            newAngle *= -1;
        }

        transform.rotation = Quaternion.Euler(0f, 0f, newAngle);
    }

    void LookForFood() {
        LookForTag("Food");
    }

    void LookForAnimat() {
        LookForTag("Animat");
    }

    void LookForObstacle() {
        LookForTag("Obstacle");
    }

    void LookForTag(string tag) {
        GameObject[] objects = GameObject.FindGameObjectsWithTag(tag);

        // Finds min distance gameObject that is tagged to what we're looking for.
        GameObject candidate = target;
        float distCurrent = targetDist > rangeVision ? rangeVision + 1 : targetDist;
        foreach (GameObject g in objects) {
            if (g == this.gameObject) {
                continue;
            }

            float distCandidate = Vector3.Distance(g.transform.position, this.transform.position);
            if (distCandidate < distCurrent) {
                candidate = g;
                distCurrent = distCandidate;
            }
        }

        if (candidate != null) {
            target = candidate;
            targetDist = distCurrent;
        }
    }

	// Update is called once per frame
	void Update () {
        // If current animat should be dead, destroy it.
        if (energy <= 1f) {
            GetComponentInParent<WorldController>().animatCount -= 1;
            Destroy(gameObject);
            return;
        }

        energy = Mathf.Clamp(energy - Cost(energy), 1, float.MaxValue);

        // Fill the work cells with random values
        for (uint idx = 0; idx < StochasticAssembly.WORK_CELL_SIZE; ++idx) {
            brain.WorkCells[idx] = (uint) (int) Random.Range(0, StochasticAssembly.N_OPS);
        }

        uint f = brain.Execute();
        switch (f) {
            case 0: Move(); break;
            case 1: Eat(); break;
            case 2: LookForFood(); break;
            case 3: LookForAnimat(); break;
        }
	}

    float Cost(float e) {
        return Mathf.Sqrt(energy)/10;
    }
}
