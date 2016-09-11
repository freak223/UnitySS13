using UnityEngine;
using System.Collections;

public class MouseController : MonoBehaviour
{
    public float fastSpeedMultiplier = 2;
    public float keyScrollSpeed = 2;

    public float zoomSpeed = 1f;
    public float zoomMax = 25f;
    public float zoomMin = 50f;

    private Vector3 lastFramePosition;
    private int tileSelection = 1;
    public GameObject circleCursor;

    Vector3 currFramePosition;
    Vector3 dragStartPosition;

    void Start() {
        
    }

    void Update() {
        CheckKeyboardScroll();
        CheckZoom();
        CheckMouseScroll();
        UpdateDragging();
        circleCursor.transform.position = new Vector3(Mathf.RoundToInt(currFramePosition.x), Mathf.RoundToInt(currFramePosition.y), 0);
        if (Input.GetKeyDown(KeyCode.Alpha1))
            tileSelection = 1;
        else if (Input.GetKeyDown(KeyCode.Alpha2))
            tileSelection = 2;
        else if (Input.GetKeyDown(KeyCode.Alpha3))
            tileSelection = 0;
    }

    private void CheckKeyboardScroll() {
        float translationX = Input.GetAxis("Horizontal");
        float translationY = Input.GetAxis("Vertical");

        if (Input.GetKey(KeyCode.LeftShift))
            Camera.main.transform.Translate(translationX * fastSpeedMultiplier * keyScrollSpeed, translationY * fastSpeedMultiplier * keyScrollSpeed, 0);
        else
            Camera.main.transform.Translate(translationX * keyScrollSpeed, translationY * keyScrollSpeed, 0);
    }

    private void CheckZoom() {
        if (Input.GetAxis("Mouse ScrollWheel") > 0 && Camera.main.orthographicSize > zoomMax) // Zoom out
            Camera.main.orthographicSize -= zoomSpeed;

        if (Input.GetAxis("Mouse ScrollWheel") < 0 && Camera.main.orthographicSize < zoomMin) // Zoom in
            Camera.main.orthographicSize += zoomSpeed;
    }

    private void CheckMouseScroll() {
        currFramePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        //currFramePosition.z = 0;

        if (Input.GetMouseButton(2) || Input.GetMouseButton(1))
        {
            Vector3 diff = lastFramePosition - currFramePosition;
            Camera.main.transform.Translate(diff);
        }

        lastFramePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
    }

    void UpdateDragging() {
        // Start Drag
        if (Input.GetMouseButtonDown(0))
        {
            dragStartPosition = currFramePosition;
        }

        // End Drag
        if (Input.GetMouseButtonUp(0))
        {
            int start_x = Mathf.RoundToInt(dragStartPosition.x);
            int end_x = Mathf.RoundToInt(currFramePosition.x);
            int start_y = Mathf.RoundToInt(dragStartPosition.y);
            int end_y = Mathf.RoundToInt(currFramePosition.y);

            // We may be dragging in the "wrong" direction, so flip things if needed.
            if (end_x < start_x)
            {
                int tmp = end_x;
                end_x = start_x;
                start_x = tmp;
            }
            if (end_y < start_y)
            {
                int tmp = end_y;
                end_y = start_y;
                start_y = tmp;
            }

            // Loop through all the tiles
            for (int x = start_x; x <= end_x; x++)
            {
                for (int y = start_y; y <= end_y; y++)
                {
                    Tile t = WorldController.Instance.World.GetTileAt(x, y);
                    if (t != null)
                    {
                        if (tileSelection == 1)
                            t.Type = Tile.TileType.EmptyFloor;
                        else if (tileSelection == 2)
                            t.Type = Tile.TileType.TiledFloor;
                        else
                            t.Type = Tile.TileType.Empty;
                    }
                }
            }
        }
    }
}
