using UnityEngine;
using System.Collections.Generic;

public class MoveSelector : MonoBehaviour
{

    // TODO: rename variables to follow C# naming conventions - m_ prefix for member of this object/class variables

    private GameObject moveLocationPrefab;
    private GameObject tileHighlightPrefab;
    private GameObject attackLocationPrefab;

    private GameObject tileHighlight;
    private GameObject movingPiece;

    private List<Vector2Int> moveLocations;
    private List<GameObject> locationHighlights;

    void Start()
    {
        moveLocationPrefab = Resources.Load<GameObject>("Prefabs/Selection-Blue");
        tileHighlightPrefab = Resources.Load<GameObject>("Prefabs/Selection-Yellow");
        attackLocationPrefab = Resources.Load<GameObject>("Prefabs/Selection-Red");

        tileHighlight = Instantiate(
            tileHighlightPrefab,
            Geometry.PointFromGrid(new Vector2Int(0, 0)),
            Quaternion.identity, gameObject.transform
        );
        tileHighlight.SetActive(false);

        this.enabled = false;
    }

    // Update is called once per frame
    void Update()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

        RaycastHit hit;
        if (Physics.Raycast(ray, out hit))
        {
            Vector3 point = hit.point;
            Vector2Int gridPoint = Geometry.GridFromPoint(point);

            tileHighlight.SetActive(true);
            tileHighlight.transform.position = Geometry.PointFromGrid(gridPoint);
            if (Input.GetMouseButtonDown(0))
            {
                // Reference Point 2: check if the player clicked on a tile that is a valid move location
                // check for valid move location
                // If the player clicks on a tile that isn’t a valid move, exit from this function.
                if (!moveLocations.Contains(gridPoint))
                {
                    return;
                }
                if (GameManager.instance.PieceAtGrid(gridPoint) == null)
                {
                    GameManager.instance.Move(movingPiece, gridPoint);
                }
                // Reference Point 3: capture enemy piece
                else
                {
                    GameManager.instance.CapturePieceAt(gridPoint);
                    GameManager.instance.Move(movingPiece, gridPoint);
                }
                ExitState();
            }
        }
        else
        {
            tileHighlight.SetActive(false);
        }
    }

    private void ExitState()
    {
        this.enabled = false;
        tileHighlight.SetActive(false);
        GameManager.instance.DeselectPiece(movingPiece);
        movingPiece = null;
        GameManager.instance.NextPlayer();
        TileSelector selector = GetComponent<TileSelector>();
        selector.EnterState();

        foreach (GameObject highlight in locationHighlights)
        {
            Destroy(highlight);
        }
    }

    public void EnterState(GameObject piece)
    {
        movingPiece = piece;
        this.enabled = true;

        moveLocations = GameManager.instance.MovesForPiece(movingPiece);
        locationHighlights = new List<GameObject>();

        foreach (Vector2Int loc in moveLocations)
        {
            GameObject highlight;
            if (GameManager.instance.PieceAtGrid(loc))
            {
                // place red square for attack location
                highlight = Instantiate(
                    attackLocationPrefab,
                    Geometry.PointFromGrid(loc),
                    Quaternion.identity,
                    gameObject.transform
                );
            }
            else
            {
                // place blue square for move location
                highlight = Instantiate(
                    moveLocationPrefab,
                    Geometry.PointFromGrid(loc),
                    Quaternion.identity,
                    gameObject.transform
                );
            }
            locationHighlights.Add(highlight);
        }

    }


}
