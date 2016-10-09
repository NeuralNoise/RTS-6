using UnityEngine;
using System.Collections;

public class Player : MonoBehaviour {
    
    public WorldObject SelectedObject { get; set; }
    public string username;
    public bool human;
    public HUD hud;

    void Start () {
        hud = GetComponentInChildren<HUD>();
	}
	
	void Update () {
	
	}
}
