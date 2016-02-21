using UnityEngine;
using System.Collections;

public class LifeDisplay : MonoBehaviour{

    private static GameObject life2;
    private static GameObject life3;
    private static GameObject life1;

    void Start() {
        LifeDisplay.life1 = this.transform.GetChild(0).gameObject;
        LifeDisplay.life2 = this.transform.GetChild(1).gameObject;
        LifeDisplay.life3 = this.transform.GetChild(2).gameObject;
    }
    
    public static void setLife(int life) {
        switch (life) {
            case 0:
                LifeDisplay.life1.SetActive(false);
                LifeDisplay.life2.SetActive(false);
                LifeDisplay.life3.SetActive(false);
                break;
            case 1:
                LifeDisplay.life1.SetActive(true);
                LifeDisplay.life2.SetActive(false);
                LifeDisplay.life3.SetActive(false);
                break;
            case 2:
                LifeDisplay.life1.SetActive(true);
                LifeDisplay.life2.SetActive(true);
                LifeDisplay.life3.SetActive(false);
                break;
            case 3:
                LifeDisplay.life1.SetActive(true);
                LifeDisplay.life2.SetActive(true);
                LifeDisplay.life3.SetActive(true);
                break;
        }
    }
}
