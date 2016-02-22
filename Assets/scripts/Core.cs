using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;

public class Core : MonoBehaviour {

    [Header("Password")]
    public int id;
    public string passwordValue;

    [Header("Player")]
    public GameObject player;

    [Header("PopUp")]
    public GameObject popup;
    public GameObject popupAvast;
    public Button btnOk;
    public Button btnCancel;
    public Button btnClose;
    public InputField password;
    public GameObject ErrorPass;

    [Header("Sprite")]
    public Sprite corefile1;
    public Sprite corefile2;
    public Sprite corefile3;
    public Sprite corrupted;

    [Header("User Interface")]
    public Text core1_pwd;
    public Text core2_pwd;
    public Text core3_pwd;

    [Header("Sound")]
    public AudioClip coreDestruction;
    public AudioClip WindowsNotif;

    private Rigidbody2D collRd2d;
    private Vector3 _startPosition;
    private bool isCorrupted = false;


    private static Dictionary<int, string> coreData = new Dictionary<int, string>();
    private static int countState;

    // Use this for initialization
    void Start() {
        addCore(id, passwordValue);
        if (!isCorrupted) { _startPosition = transform.position; }
        if (id == 1) {
            gameObject.GetComponent<SpriteRenderer>().sprite = corefile1;
        } else if (id == 2) {
            gameObject.GetComponent<SpriteRenderer>().sprite = corefile2;
        } else if (id == 3) {
            gameObject.GetComponent<SpriteRenderer>().sprite = corefile3;
        }

    }

    // Update is called once per frame
    void Update() {

    }

    // fixedUpdate is called regular
    void FixedUpdate() {
        if (password.text.Length >= 1) {
            if (Input.GetButtonDown("Submit")) { passwordEnter(); }
        }
        if (!isCorrupted) {
            transform.position = _startPosition + new Vector3(0.0f, Mathf.Sin(Time.time) * (Time.deltaTime + 0.1f), 0.0f);
        }

        btnClose.onClick.AddListener(cancel);
        btnCancel.onClick.AddListener(cancel);
    }

    void passwordEnter() {
        if (password.text == getPassword(id)) {
            passwordCorrect();
        } else {
            ErrorPass.SetActive(true);
        }

    }

    void passwordCorrect() {
        PlayerMovement playmovementscript = collRd2d.gameObject.GetComponent<PlayerMovement>();

        playmovementscript.setGameObjectHashCodeAsDestroy(this.gameObject.GetHashCode());
        //Destroy(gameObject);
        gameObject.GetComponent<SpriteRenderer>().sprite = corrupted;
        gameObject.AddComponent<Rigidbody2D>();

        isCorrupted = true;


        collRd2d.constraints = RigidbodyConstraints2D.None | RigidbodyConstraints2D.FreezeRotation;
        popup.SetActive(false);
        password.text = "";
        Core.upgradeState();

        if (Core.getState() == 1) {
            popupAvast.SetActive(true);
        }


        AudioSource audio = GetComponent<AudioSource>();
        audio.clip = this.coreDestruction;
        audio.Play();

        if (id == 1) {
            core1_pwd.text = "corrupt";
            core1_pwd.color = UnityEngine.Color.red;
        } else if (id == 2) {
            core2_pwd.text = "corrupt";
            core2_pwd.color = UnityEngine.Color.red;
        } else if (id == 3) {
            core3_pwd.text = "corrupt";
            core3_pwd.color = UnityEngine.Color.red;
        }
    }


    void cancel() {
        popup.SetActive(false);
        player.GetComponent<Rigidbody2D>().constraints = RigidbodyConstraints2D.None | RigidbodyConstraints2D.FreezeRotation;
        this.transform.Translate(new Vector3(20, 0, 0));


    }


    void OnCollisionEnter2D(Collision2D coll) {
        if (coll.gameObject.tag == "Player" && !isCorrupted) {
            //Play sound
            AudioSource audio = GetComponent<AudioSource>();
            audio.clip = this.WindowsNotif;
            audio.Play();

            // Show PopUp   
            popup.SetActive(true);
            password.ActivateInputField();
            collRd2d = coll.gameObject.GetComponent<Rigidbody2D>();
            collRd2d.constraints = RigidbodyConstraints2D.FreezePositionX | RigidbodyConstraints2D.FreezePositionY | RigidbodyConstraints2D.FreezeRotation;
            if (passwordValue.Length != 0) {

                btnOk.onClick.AddListener(passwordEnter);
            } else {
                StartCoroutine(Wait());
            }

        }

    }

    IEnumerator Wait() {
        password.text = "Copyright Flopy.ch";
        yield return new WaitForSeconds(0.7f);
        passwordCorrect();
    }

    private static void addCore(int id, string pwd) {
        Core.coreData[id] = pwd;
    }

    public static string getPassword(int id) {
        if (coreData.ContainsKey(id))
            return Core.coreData[id];
        else
            return null;
    }

    public static void upgradeState() {
        Core.countState++;

        if (Core.countState == 3)
            GameObject.Find("Main Camera").SendMessage("shake", Mathf.Infinity);
        else
            GameObject.Find("Main Camera").SendMessage("shake", 2);

        GameObject.Find("_Manager").SendMessage("upgradeMusic");
        if (Core.getState() == 1) {
            GameObject.Find("subManager").SendMessage("UvastPlay");
        }
    }

    public static int getState() {
        return Core.countState;
    }

    public static void resetState() {
        Core.countState = 0;
    }
}