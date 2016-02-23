using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class tutorial1 : MonoBehaviour {

    public float maxdistance = 3;

    public GameObject player;


    public string titletuto;
    public string texttuto;
    public Text tutovalue;
    public Text tutovaluetitle;
    public GameObject tutoscreen;

    private GameObject thisobject;
    private float distance;
    private bool AlreadyView = false;

    // Use this for initialization
    void Start() {
        thisobject = this.gameObject;
        StartCoroutine(PauseCoroutine());
    }

    // Update is called once per frame
    void FixedUpdate() {
        if (!AlreadyView) {

            distance = Vector3.Distance(player.transform.position, thisobject.transform.position);

            if (distance < maxdistance) {
                tutoscreen.SetActive(true);
                Time.timeScale = 0f;
                tutovalue.text = texttuto;
                tutovaluetitle.text = titletuto;
            }
        }
    }

    IEnumerator PauseCoroutine() {
        while (true) {
            if (Input.GetButton("Submit")) {
                if (distance < maxdistance) {
                    Time.timeScale = 1;
                    tutoscreen.SetActive(false);
                    AlreadyView = true;
                }
            }
            yield return null;
        }
    }
}
