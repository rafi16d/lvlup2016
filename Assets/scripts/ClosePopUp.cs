using UnityEngine;
using System.Collections;

public class ClosePopUp : MonoBehaviour {

    public GameObject popup;
    // Use this for initialization
    void Start() {
        //GameObject popup = GameObject.Find("Modal");
    }

    void PopUpClose() {
        popup.SetActive(false);
    }
}
