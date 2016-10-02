using UnityEngine;
using System.Collections;
using RTS;

public class UserInput : MonoBehaviour {

    private Player player;

    void Start () {
        player = transform.root.GetComponent<Player>();
	}
	
	void Update () {
        if (player.human) {
            MoveCamera();
            RotateCamera();
        }
	}

    /*  TODO:
        - Reimplement camera to rotate on center of the camera's world position while holding middle-mouse and 
            shuffling mouse left/right (i.e. Divinity: Original camera-movement).
        - Adjust camera movement with Arrow-keys.
        - Adjust 'ScrollWidth'-values to be more precise; outer edge of the screen.
        - Adjust 'ScrollSpeed' to feel better (not too fast, not too slow).
        
        [Crucial] - Reimplement camera-movement for movement on terrain (height difference) instead of flat plane.
            - Possible idea: Calculate distance between camera and ground (terrain) with a fixed delta-distance.
    */
    private void MoveCamera() {
        float xpos = Input.mousePosition.x;
        float ypos = Input.mousePosition.y;
        Vector3 movement = new Vector3(0, 0, 0);

        bool UpArrow = Input.GetKey(KeyCode.UpArrow);
        bool DownArrow = Input.GetKey(KeyCode.DownArrow);
        bool LeftArrow = Input.GetKey(KeyCode.LeftArrow);
        bool RightArrow = Input.GetKey(KeyCode.RightArrow);

        // Horizontal key camera movement
        if (LeftArrow) {
            movement.x -= ResourceManager.ScrollSpeed;
        }
        else if (RightArrow) {
            movement.x += ResourceManager.ScrollSpeed;
        }
        // Vertical key camera movement
        if (DownArrow) {
            movement.y -= ResourceManager.ScrollSpeed;
        }
        else if (UpArrow) {
            movement.y += ResourceManager.ScrollSpeed;
        }

        // Horizontal mouse camera movement
        if (xpos >= 0 && xpos < ResourceManager.ScrollWidth) {
            movement.x -= ResourceManager.ScrollSpeed;
        } else if (xpos <= Screen.width && xpos > Screen.width - ResourceManager.ScrollWidth) {
            movement.x += ResourceManager.ScrollSpeed;
        }
        // Vertical mouse camera movement
        if (ypos >= 0 && ypos < ResourceManager.ScrollWidth) {
            movement.z -= ResourceManager.ScrollSpeed;
        } else if (ypos <= Screen.width && ypos > Screen.height - ResourceManager.ScrollWidth) {
            movement.z += ResourceManager.ScrollSpeed;
        }

        // Make sure movement is in the direction the camera is pointing
        // but ignore the vertical tilt  of the camera to get a sensible scrolling
        movement = Camera.main.transform.TransformDirection(movement);
        movement.y = 0;

        // Away from ground movement
        movement.y -= ResourceManager.ScrollSpeed * Input.GetAxis("Mouse ScrollWheel");

        // Calculate desired camera position based on received input
        Vector3 origin = Camera.main.transform.position;
        Vector3 destination = origin;
        destination.x += movement.x;
        destination.y += movement.y;
        destination.z += movement.z;

        // Limit away from ground movement to be between a minimum and maximum distance
        if (destination.y > ResourceManager.MaxCameraHeight) {
            destination.y = ResourceManager.MaxCameraHeight;
        } else if (destination.y < ResourceManager.MinCameraHeight) {
            destination.y = ResourceManager.MinCameraHeight;
        }

        // If a change in position is detected then perform necessary update
        if (destination != origin) {
            Camera.main.transform.position = Vector3.MoveTowards(origin, destination, Time.deltaTime * ResourceManager.ScrollSpeed);
        }
    }

    /*  TODO:
        - Rotate camera with keyboard-keys (e.g. PageUp/PageDown) instead of using mouse.
    */
    private void RotateCamera() {
        Vector3 origin = Camera.main.transform.eulerAngles;
        Vector3 destination = origin;

        // Detect rotation amount if 'ALT'-key is being held and the Right Mouse Button is down
        if ((Input.GetKey(KeyCode.LeftAlt) || Input.GetKey(KeyCode.RightAlt)) && Input.GetMouseButton(1)) {
            destination.x -= Input.GetAxis("Mouse Y") * ResourceManager.RotateAmount;
            destination.y += Input.GetAxis("Mouse X") * ResourceManager.RotateAmount;
        }

        // If a change in position is detected then perform necessary update
        if (destination != origin) {
            Camera.main.transform.eulerAngles = Vector3.MoveTowards(origin, destination, Time.deltaTime * ResourceManager.RotateSpeed);
        }
    }
}
