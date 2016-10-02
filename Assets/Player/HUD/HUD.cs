using UnityEngine;
using System.Collections;

public class HUD : MonoBehaviour {

    public GUISkin resourceSkin;
    public GUISkin ordersSkin;
    private const int ORDERS_BAR_HEIGHT = 200;
    private const int RESOURCE_BAR_HEIGHT = 40;

    private Player player;

	void Start () {
        player = transform.root.GetComponent<Player>();
	}
	
	void OnGUI() {
        if (player && player.human) {
            DrawOrdersBar();
            DrawResourceBar();
        }
    }

    // Bottom bar
    private void DrawOrdersBar() {
        GUI.skin = ordersSkin;
        GUI.BeginGroup(new Rect(0, Screen.height - ORDERS_BAR_HEIGHT, Screen.width, ORDERS_BAR_HEIGHT));
        GUI.Box(new Rect(0, 0, Screen.width, ORDERS_BAR_HEIGHT), "");
        GUI.EndGroup();
    }

    // Top bar
    private void DrawResourceBar() {
        GUI.skin = resourceSkin;
        GUI.BeginGroup(new Rect(0, 0, Screen.width, RESOURCE_BAR_HEIGHT));
        GUI.Box(new Rect(0, 0, Screen.width, RESOURCE_BAR_HEIGHT), "");
        GUI.EndGroup();
    }
}
