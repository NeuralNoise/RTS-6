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
        string selectionName = "";

        GUI.skin = ordersSkin;
        GUI.BeginGroup(new Rect(0, Screen.height - ORDERS_BAR_HEIGHT, Screen.width, ORDERS_BAR_HEIGHT));
        GUI.Box(new Rect(0, 0, Screen.width, ORDERS_BAR_HEIGHT), "");
        
        if (player.SelectedObject) {
            selectionName = player.SelectedObject.objectName;
        }
        if (!selectionName.Equals("")) {
            GUI.Label(new Rect(20, 20, Screen.width, ORDERS_BAR_HEIGHT), selectionName);
        }

        GUI.EndGroup();
    }

    // Top bar
    private void DrawResourceBar() {
        GUI.skin = resourceSkin;
        GUI.BeginGroup(new Rect(0, 0, Screen.width, RESOURCE_BAR_HEIGHT));
        GUI.Box(new Rect(0, 0, Screen.width, RESOURCE_BAR_HEIGHT), "");
        GUI.EndGroup();
    }

    public bool MouseInBounds() {
        // Screen coordinates start in the lower left corner of the screen
        // not the top left of the screen like the drawing coordinates do

        Vector3 mousePos = Input.mousePosition;
        bool insideWidth = mousePos.y >= 0 && mousePos.y <= Screen.height - ORDERS_BAR_HEIGHT;
        bool insideHeight = mousePos.y >= 0 && mousePos.y <= Screen.height - RESOURCE_BAR_HEIGHT;

        return insideWidth && insideHeight;
    }
}
