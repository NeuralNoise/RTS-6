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
            MouseActivity();       
        }
	}

    /*  TODO:
        - Reimplement camera to rotate on center of the camera's world position while holding middle-mouse and 
            shuffling mouse left/right (i.e. Divinity: Original camera-movement).
        - Adjust camera movement with Arrow-keys.
        - Adjust 'ScrollWidth'-values to be more precise; outer edge of the screen.
        - Adjust 'ScrollSpeed' to feel better (not too fast, not too slow).        
        - Reimplement camera-movement for movement on terrain (height difference) instead of flat plane.            
    */
    private void MoveCamera() {
        float xpos = Input.mousePosition.x;
        float ypos = Input.mousePosition.y;
        Vector3 movement = new Vector3(0, 0, 0);

        bool UpArrow = Input.GetKey(KeyCode.UpArrow);
        bool DownArrow = Input.GetKey(KeyCode.DownArrow);
        bool LeftArrow = Input.GetKey(KeyCode.LeftArrow);
        bool RightArrow = Input.GetKey(KeyCode.RightArrow);

        // x-axis camera movement
        if (LeftArrow) {
            movement.x -= ResourceManager.ScrollSpeed;
        }
        else if (RightArrow) {
            movement.x += ResourceManager.ScrollSpeed;
        }
        // z-axis camera movement
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

        // Reset camera to default rotation
        Quaternion currentRotation = Camera.main.transform.rotation;
        Quaternion targetRotation = ResourceManager.CameraDefaultRotation;
        if (Input.GetKeyDown(KeyCode.Home)) {
                      
            Camera.main.transform.rotation = Quaternion.Slerp(currentRotation, targetRotation, Time.time * 0.1f);
        }

        // Make sure movement is in the direction the camera is pointing
        // but ignore the vertical tilt  of the camera to get a sensible scrolling
        movement = Camera.main.transform.TransformDirection(movement);
        movement.y = 0;

        //TESTING
        //RaycastHit hit;
        //Ray ray = Camera.main.ScreenPointToRay(new Vector3(Screen.width / 2, Screen.height / 2, 0));
        //Physics.Raycast(ray, out hit);                
        //Debug.DrawRay(Camera.main.transform.position, Camera.main.transform.TransformDirection(Vector3.forward) * 1000, Color.green);
        //Debug.Log(hit.distance);

        // Camera zoom
        movement.y -= ResourceManager.ScrollSpeed * Input.GetAxis("Mouse ScrollWheel");

        // Calculate desired camera position based on received input
        Vector3 origin = Camera.main.transform.position;
        Vector3 destination = origin;
        destination.x += movement.x;
        destination.y += movement.y;
        destination.z += movement.z;

        // Limit vertical camera movement between a minimum and maximum distance
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
    
    // Change bad rotation, lawl
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

    private void MouseActivity() {
        if (Input.GetMouseButtonDown(0)) {
            LeftMouseClick();            
        }else if (Input.GetMouseButtonDown(1)) {
            RightMouseClick();            
        }
    }

    private void LeftMouseClick() {
        if (player.hud.MouseInBounds()) {
            GameObject hitObject = FindHitObject();
            Vector3 hitPoint = FindHitPoint();

            if (hitObject && hitPoint != ResourceManager.InvalidPosition) {
                if (player.SelectedObject) {
                    player.SelectedObject.MouseClick(hitObject, hitPoint, player);                    
                }else if (hitObject.name != "Ground") {
                    WorldObject worldObject = hitObject.transform.root.GetComponent<WorldObject>();

                    if (worldObject) {
                        // we already know the player has no selected object
                        player.SelectedObject = worldObject;
                        worldObject.SetSelection(true);
                    }
                }
            }
        }
    }

    private void RightMouseClick() {
        if (player.hud.MouseInBounds() && !Input.GetKey(KeyCode.LeftAlt) && player.SelectedObject) {
            player.SelectedObject.SetSelection(false);
            player.SelectedObject = null;
        }
    }

    private GameObject FindHitObject() {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;
        if (Physics.Raycast(ray, out hit)) {
            return hit.collider.gameObject;
        }

        return null;
    }

    private Vector3 FindHitPoint() {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;
        if (Physics.Raycast(ray, out hit)) {
            return hit.point;
        }

        return ResourceManager.InvalidPosition;
    }
}