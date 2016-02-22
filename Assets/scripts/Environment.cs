using UnityEngine;
using System.Collections;

public class Environment : MonoBehaviour {

    public Sprite happyImage;
    public Sprite[] DarkImage;
    public Sprite[] MissingImage;
    public bool forceMode = false;
    public bool WorldManager = false;

    private float timeImageChange = 0;
    private SpriteRenderer renderer;
    private int state = 0;
    private bool stateChanged = false;
    private bool isTypeOfBlinkDark;
    private float lastTimeChange = 0;
    private int stateOfThis = 0;
    private static int globalState = 0;
    private bool finalState = false;

    void Start() {
        if (this.DarkImage.Length == 0 && !this.WorldManager)
            throw new System.Exception("0 dark image is not allowed.");
        if (this.MissingImage.Length == 0 && !this.WorldManager)
            throw new System.Exception("0 missing image is not allowed.");

        if (!this.WorldManager) {
            this.renderer = this.GetComponent<SpriteRenderer>();
            this.renderer.sprite = this.happyImage;
        }

        if (this.WorldManager)
            Core.resetState();
    }

    void Update() {
        if (this.WorldManager)
            this.updateWorldManager();
        else {
            checkState();
            checkBlink();
        }
    }

    void updateWorldManager() {
        Environment.globalState = Core.getState();
    }

    void checkState() {
        if (this.state != Environment.globalState) {
            this.state = Environment.globalState;
            switch (this.state) {
                case 1:
                    if (Random.Range(0, 9) == 0 && !this.forceMode) // 1/20
                        this.updateState(++this.stateOfThis);
                    this.stateChanged = false;
                    break;
                case 2:
                    if (Random.Range(0, 3) != 0 || this.forceMode) // 15/20
                        this.updateState(++this.stateOfThis);
                    this.stateChanged = false;
                    break;
                case 3:
                    this.finalState = true;
                    this.stateOfThis = 3;
                    this.updateState(2); // 20/20 to missing text.
                    this.stateChanged = false;
                    break;
            }
        }
    }

    void updateState(int State, bool selectType = true) {
        if (State == 1) {
            this.renderer.sprite = this.DarkImage[Random.Range(0, this.DarkImage.Length)];
            if (selectType)
                this.isTypeOfBlinkDark = Random.Range(0, 2) == 1;
        } else if (State == 2) {
            this.renderer.sprite = this.MissingImage[Random.Range(0, this.MissingImage.Length)];
        }
    }

    void checkBlink() {
        if (!this.isTypeOfBlinkDark || this.stateOfThis != 1)
            return;

        if (this.lastTimeChange + this.timeImageChange < Time.time) {
            this.lastTimeChange = Time.time;
            this.updateState(this.stateOfThis, false);
            this.timeImageChange = (float)Random.Range(0, 200) / 100;
        }

    }

    public static void globalStateReset() {
        Environment.globalState = 0;
    }

}
