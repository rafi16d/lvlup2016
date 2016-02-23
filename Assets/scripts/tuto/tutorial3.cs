using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class tutorial3 : MonoBehaviour {

    public float maxdistance3 = 3;

    public GameObject player3;


    public string titletuto3;
    public string texttuto3;
    public Text tutovalue3;
    public Text tutovaluetitle3;
    public GameObject tutoscreen3;

    private GameObject thisobject;
    private float distance;
    private bool AlreadyView3 = false;

    // Use this for initialization
    void Start() {
        thisobject = this.gameObject;
        StartCoroutine(PauseCoroutine());
    }

    // Update is called once per frame
    void FixedUpdate() {
        if (!AlreadyView3) {

            distance = Vector3.Distance(player3.transform.position, thisobject.transform.position);

            if (distance < maxdistance3) {
                tutoscreen3.SetActive(true);
                Time.timeScale = 0f;
                tutovalue3.text = texttuto3;
                tutovaluetitle3.text = titletuto3;
            }
        }
    }

    IEnumerator PauseCoroutine() {
        while (true) {
            if (Input.GetButton("Submit")) {
                if (distance < maxdistance3) {
                    Time.timeScale = 1;
                    tutoscreen3.SetActive(false);
                    AlreadyView3 = true;
                }
            }
            yield return null;
        }
    }
}
