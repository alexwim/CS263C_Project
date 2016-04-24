using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class StochasticAssembly {
    public static uint WORK_CELL_SIZE = 32;
    public uint[] WorkCells = new uint[WORK_CELL_SIZE];

    public static uint N_OPS = 7;
    static uint PROG_CELL_SIZE = N_OPS * N_OPS + N_OPS + 1;
    static uint RESTART = N_OPS - 3, INC_PROB = N_OPS - 2, END_SELF_MOD = N_OPS - 1;

    uint IP = 0;
    float reward = 0;
    uint[] ProgramCells = new uint[PROG_CELL_SIZE];
    float[][] P_DIST = new float[PROG_CELL_SIZE][];

    bool inSMS = false;
    uint disabledUntil = 0;

    class SMS {
        public uint time;
        public float reinforcement;
        public Stack<uint> cell;
        public Stack<float[]> dists;
    }
    Stack<SMS> History = new Stack<SMS>();

    public StochasticAssembly() {
        // Initialize probability distribution memory
        for (uint i = 0; i < PROG_CELL_SIZE; ++i) {
            P_DIST[i] = new float[N_OPS];
        }

        // Initialize to maximum entropy
        float defaultValue = 1f / N_OPS;
        for (uint i = 0; i < PROG_CELL_SIZE; ++i) {
            for (uint j = 0; j < N_OPS; ++j) {
                P_DIST[i][j] = defaultValue;
            }
        }
    }

    public uint Execute() {
        uint inst = Choose(IP);
        ProgramCells[IP] = inst;
        //Debug.Log(inst);

        if (inst == RESTART) {
            Restart();
        } else if (inst == INC_PROB) {
            // Invoke IncProb only if we have enough room to select arguments
            if (IP > PROG_CELL_SIZE - 3 - 1) {
                Restart();
            } else {
                IncProb(Choose(IP + 1), Choose(IP + 2), Choose(IP + 3));
            }
        } else if (inst == END_SELF_MOD) {
            EndSelfMod();
        }

        return inst;
    }

    uint Choose(uint dist) {
        double sum = 0f;
        for (uint idx = 0; idx < N_OPS; ++idx) {
            sum += P_DIST[dist][idx];
        }
        double rand = Random.value * sum;

        for (uint idx = 0; idx < N_OPS; ++idx) {
            if (rand < P_DIST[dist][idx]) {
                return idx;
            } else {
                rand -= P_DIST[dist][idx];
            }
        }

        // Return last element in case rounding errors prevented it from being picked.
        return N_OPS - 1;
    }

    float QFunction(uint T, float R) {
        return (reward - R)/(Time.frameCount - T);
    }

    void Backtrack() {
        while (History.Count > 0) {
            SMS last;
            // Stack has two or more elements
            if (History.Count > 1) {
                last = History.Pop();
                SMS lastB = History.Peek();
                if (QFunction(last.time, last.reinforcement) > QFunction(lastB.time, lastB.reinforcement)) {
                    History.Push(last);
                    break;
                }
                
            // Stack has a single element
            } else {
                last = History.Pop();
                if (QFunction(last.time, last.reinforcement) > (float) last.reinforcement / (float) last.time) {
                    History.Push(last);
                    break;
                }
            }

            // At this point we need to undo the last SMS
            while (last.cell.Count > 0) {
                P_DIST[last.cell.Pop()] = last.dists.Pop();
            }
        }
    }

    void PushNewSMS() {
        SMS seq = new SMS();
        seq.time = (uint) Time.frameCount;
        seq.reinforcement = reward;
        seq.cell = new Stack<uint>();
        seq.dists = new Stack<float[]>();

        History.Push(seq);
        inSMS = true;
    }

    void PushMod(uint modCell) {
        SMS seq = History.Peek();
        seq.cell.Push(modCell);
        seq.dists.Push(P_DIST[modCell]);
    }

    void Restart() {
        IP = 0;
    }

    static float Gamma = 0.15f, epsilon = 0.001f;
    void IncProb(uint a1, uint a2, uint a3) {
        // Check to start a new SMS
        if (!inSMS) {
            if (disabledUntil == 0) {
                Backtrack();
                PushNewSMS();
            } else {
                return;
            }
        }

        // Compute index
        uint i = WorkCells[a1] * N_OPS + WorkCells[a2], j = WorkCells[a3];

        // Keep track of our modification
        PushMod(i);

        // Modify probability
        P_DIST[i][j] *= (1f + Gamma);

        // Recompute sum for normalization
        float sum = 0f;
        for (uint idx = 0; idx < N_OPS; ++idx) {
            sum += P_DIST[i][idx];
        }
        // Renormalize
        for (uint idx = 0; idx < N_OPS; ++idx) {
            Mathf.Clamp(P_DIST[i][idx]/sum, epsilon, float.MaxValue);
        }
    }

    void EndSelfMod() {
        inSMS = false;
        disabledUntil = 5; // Paper recommends this value.
    }

    public void Reinforce(float reinforcement) {
        reward += reinforcement;
        if (disabledUntil > 0 && reinforcement != 0) {
            --disabledUntil;
        }
    }
}
