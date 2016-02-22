using UnityEngine;
using System.Collections;

public class Firewall : MonoBehaviour {

    public Sprite firewall1;
    public Sprite firewall2;
    public Sprite firewall3;
    public Sprite firewall4;

    private int lastState = 0;
    private static SpriteRenderer renderer;

    public static Sprite Firewall1;
    public static Sprite Firewall2;
    public static Sprite Firewall3;
    public static Sprite Firewall4;
    private BoxCollider2D bc2d;
    public static BoxCollider2D bc2d2;

    private static int currStateLevel = 1;

    void Start() {
        this.bc2d = this.GetComponent<BoxCollider2D>();
        Firewall.setSprites(firewall1, firewall2, firewall3, firewall4, this.GetComponent<SpriteRenderer>(), this.bc2d);
        currStateLevel = 1;
    }

    void Update() {
        if (Core.getState() != this.lastState) {
            this.lastState = Core.getState();
            Firewall.downgradeState();
        }
    }

    public static void setSprites(Sprite firewall1, Sprite firewall2, Sprite firewall3, Sprite firewall4, SpriteRenderer renderer, BoxCollider2D bc2d) {
        Firewall.Firewall1 = firewall1;
        Firewall.Firewall2 = firewall2;
        Firewall.Firewall3 = firewall3;
        Firewall.Firewall4 = firewall4;
        Firewall.renderer = renderer;
        Firewall.bc2d2 = bc2d;
    }

    public static void downgradeState() {
        switch (++currStateLevel) {
            case 1:
                Firewall.renderer.sprite = Firewall1;
                break;
            case 2:
                Firewall.renderer.sprite = Firewall2;
                break;
            case 3:
                Firewall.renderer.sprite = Firewall3;
                break;
            case 4:
                Firewall.renderer.sprite = Firewall4;
                Firewall.bc2d2.size = Vector2.zero;
                break;
        }

    }
}
