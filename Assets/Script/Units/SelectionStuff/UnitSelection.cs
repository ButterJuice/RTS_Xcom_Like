using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
/*
Some part of this function use the code provided in this video (and its addendum) especially thing that concern the selection box: 
https://www.youtube.com/watch?v=OL1QgwaDsqo
https://www.youtube.com/watch?v=33RQEzFoFIM
https://github.com/pickles976/RTS_selection/blob/master/global_selection.cs


*/
public class UnitSelection : MonoBehaviour
{
    [SerializeField] private LayerMask layerMask = new LayerMask();
    private Camera mainCamera;

    public List<Unit> selectedUnits { get; } = new List<Unit>();//{get;} Stop this variable from being set elsewhere
    private bool dragSelect;
    //ColliderVariable
    MeshCollider selectionBox;
    Mesh selectionMesh;
    Vector3 p1;
    Vector3 p2;
    //the corners of our 2d selection box
    Vector2[] corners;
    //the vertices of our meshCollider
    Vector3[] verts;
    Vector3[] vecs;


    private void Start()
    {
        mainCamera = Camera.main;
        dragSelect = false;
    }

    private void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            p1 = Input.mousePosition;
        }
        if (Input.GetMouseButton(0))
        {
            //minimum size of the selection box
            if ((p1 - Input.mousePosition).magnitude > 10)
            {
                dragSelect = true;
            }
        }
        if (Input.GetMouseButtonUp(0))
        {
            if (dragSelect == false)
            {
                ClickSelect();
            }
            else 
            {
                BoxSelect();
            }

            dragSelect = false;


        }

    }

    private void ClickSelect()
    {
        Ray ray = Camera.main.ScreenPointToRay(p1);
        if (Physics.Raycast(ray, out RaycastHit hit, 50000.0f))
        {
            if (!Input.GetKey(KeyCode.LeftShift))
            {
                DeselectAll();
            }
            if (!hit.collider.TryGetComponent<Unit>(out Unit unit)) return;
            if (!unit.isOwned) return;
            if (Input.GetKey(KeyCode.LeftShift))
            {
                selectedUnits.Add(unit);

                foreach (Unit selectedUnit in selectedUnits)
                {
                    selectedUnit.Select();
                }
            }
            else
            {
                DeselectAll();
                selectedUnits.Add(unit);
                unit.Select();

            }
        }
    }

    private void BoxSelect()
    {

        verts = new Vector3[4];
        vecs = new Vector3[4];
        int i = 0;
        p2 = Input.mousePosition;
        corners = GetBoundingBox(p1, p2);

        foreach (Vector2 corner in corners)
        {
            Ray ray = Camera.main.ScreenPointToRay(corner);

            if (Physics.Raycast(ray, out RaycastHit hit, 50000.0f, 1 << 6)) //1 << 6 le layer du ground
            {
                verts[i] = new Vector3(hit.point.x, hit.point.y, hit.point.z);
                //  vecs[i] = mainCamera.transform.position - hit.point;
                vecs[i] = ray.origin - hit.point;
                Debug.DrawLine(mainCamera.ScreenToWorldPoint(corner), hit.point, Color.blue, 1.0f);
            }
            i++;
        }

        //generate the mesh
        selectionMesh = GenerateSelectionMesh(verts, vecs);

        selectionBox = gameObject.AddComponent<MeshCollider>();
        selectionBox.sharedMesh = selectionMesh;
        selectionBox.convex = true;
        selectionBox.isTrigger = true;

        if (!Input.GetKey(KeyCode.LeftShift))
        {
            DeselectAll();
        }

        Destroy(selectionBox, 0.02f);
    }
    //fonction called up to several time per frame in unity
    private void OnGUI()
    {
        if (dragSelect == true)
        {
            var rect = Utils.GetScreenRect(p1, Input.mousePosition);
            Utils.DrawScreenRect(rect, new Color(0.3f, 0.7f, 0.3f, 0.25f));
            Utils.DrawScreenRectBorder(rect, 2, new Color(0.3f, 0.9f, 0.3f, 0.95f));
        }
    }

    //create a bounding box (4 corners in order) from the start and end mouse position
    Vector2[] GetBoundingBox(Vector2 p1, Vector2 p2)
    {
        // Min and Max to get 2 corners of rectangle regardless of drag direction.
        var bottomLeft = Vector3.Min(p1, p2);
        var topRight = Vector3.Max(p1, p2);

        // 0 = top left; 1 = top right; 2 = bottom left; 3 = bottom right;
        Vector2[] corners =
        {
            new Vector2(bottomLeft.x, topRight.y),
            new Vector2(topRight.x, topRight.y),
            new Vector2(bottomLeft.x, bottomLeft.y),
            new Vector2(topRight.x, bottomLeft.y)
        };
        return corners;

    }

    private void DeselectAll()
    {
        foreach (Unit selectedUnit in selectedUnits)
        {
            selectedUnit.Deselect();
        }
        selectedUnits.Clear();
    }

    //generate a mesh from the 4 bottom points
    Mesh GenerateSelectionMesh(Vector3[] corners, Vector3[] vecs)
    {
        Vector3[] verts = new Vector3[8];
        int[] tris = { 0, 1, 2, 2, 1, 3, 4, 6, 0, 0, 6, 2, 6, 7, 2, 2, 7, 3, 7, 5, 3, 3, 5, 1, 5, 0, 1, 1, 4, 0, 4, 5, 6, 6, 5, 7 }; //map the tris of our cube

        for (int i = 0; i < 4; i++)
        {
            verts[i] = corners[i];
        }

        for (int j = 4; j < 8; j++)
        {
            verts[j] = corners[j - 4] + vecs[j - 4];
        }

        Mesh selectionMesh = new Mesh();
        selectionMesh.vertices = verts;
        selectionMesh.triangles = tris;

        return selectionMesh;
    }


    private void OnTriggerEnter(Collider other)
    {
        if (!other.TryGetComponent<Unit>(out Unit unit)) return;
        if (!unit.isOwned) return;
        selectedUnits.Add(unit);
        unit.Select();
    }
}
