using UnityEngine;

public class Credits : MonoBehaviour {
    private float offset;
    public float speed = 29.0f;
    public GUIStyle style;
    public Rect viewArea;

    private void Start() {
        this.offset = this.viewArea.height;
    }

    private void Update() {
        this.offset -= Time.deltaTime * this.speed;
    }

    private void OnGUI() {
        GUI.BeginGroup(this.viewArea);

        var position = new Rect(500, this.offset, 500, 500);
        var text = @"Lorem ipsum dolor sit amet, consectetur adipiscing elit.
 Quisque a mauris sit amet neque posuere molestie at laoreet lorem.
 Suspendisse accumsan pretium ante, sit amet tincidunt tortor tempor ac.
  
  
 Sed condimentum mi id nisi egestas non vulputate urna porttitor.
 Mauris sed mauris vitae velit imperdiet vulputate ut nec velit.
 Maecenas convallis posuere velit, quis interdum justo mattis vel.
  
  
  
 Aliquam hendrerit ullamcorper dui, a laoreet dolor ornare sit amet.
 Praesent sed odio purus, a convallis tellus.
 Nulla porttitor arcu vel ipsum luctus euismod.
  
  
  
 Duis tincidunt vehicula nisl, nec venenatis velit convallis non.
 Sed semper metus egestas libero venenatis imperdiet.
 Pellentesque venenatis orci nisi, vel fringilla dolor.
  
  
  
 Nam at lacus massa, commodo pellentesque velit.
 In accumsan velit sed nisi aliquam tristique.
 Ut eu quam tellus, eu egestas diam.
  
  
 Maecenas vel dui vitae orci accumsan molestie.
 Donec pulvinar metus nec turpis rutrum quis gravida ante dignissim.
 Ut quis justo quis nisl eleifend ornare non at ipsum.";

        GUI.Label(position, text, this.style);

        GUI.EndGroup();
    }
}
