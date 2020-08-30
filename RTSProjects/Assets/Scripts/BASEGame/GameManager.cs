using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using UnityEngine.AI;

public class GameManager : MonoBehaviour
{
    public Player[] players;

    public static GameManager instance;


    public TerrainCollider MapCollider;
    public static GameManager Current = null;




    public RectTransform selectionBox;
    public LayerMask unitLayerMask;

    public List<Unit> selectedUnits = new List<Unit>();
    private Vector2 startPos;

    // components
    private Camera cam;
    private Player player;
    public GameManager()
    {
        Current = this;
    }
    void Awake()
    {
        instance = this;

        // get the components
        cam = Camera.main;
        player = GetComponent<Player>();
    }

    void Update()
    {
        // mouse down
        if (Input.GetMouseButtonDown(0))
        {
            ToggleSelectionVisual(false);
            selectedUnits = new List<Unit>();

            TrySelect(Input.mousePosition);
            startPos = Input.mousePosition;
        }

        // mouse up
        if (Input.GetMouseButtonUp(0))
        {
            ReleaseSelectionBox();
        }

        // mouse held down
        if (Input.GetMouseButton(0))
        {
            UpdateSelectionBox(Input.mousePosition);
        }
    }

    // called when we click on a unit
    void TrySelect(Vector2 screenPos)
    {
        Ray ray = cam.ScreenPointToRay(screenPos);
        RaycastHit hit;


        if (Physics.Raycast(ray, out hit, 100, unitLayerMask))
        {
            Unit unit = hit.collider.GetComponent<Unit>();
            if (player.IsMyUnit(unit))
            {
                selectedUnits.Add(unit);
                unit.ToggleSelectionVisual(true);
            }
        }

    }

    // called when we are creating a selection box
    void UpdateSelectionBox(Vector2 curMousePos)
    {
        if (!selectionBox.gameObject.activeInHierarchy)
            selectionBox.gameObject.SetActive(true);

        float width = curMousePos.x - startPos.x;
        float height = curMousePos.y - startPos.y;

        selectionBox.sizeDelta = new Vector2(Mathf.Abs(width), Mathf.Abs(height));
        selectionBox.anchoredPosition = startPos + new Vector2(width / 2, height / 2);
    }

    // called when we release the selection box
    void ReleaseSelectionBox()
    {
        selectionBox.gameObject.SetActive(false);

        Vector2 min = selectionBox.anchoredPosition - (selectionBox.sizeDelta / 2);
        Vector2 max = selectionBox.anchoredPosition + (selectionBox.sizeDelta / 2);

        foreach (Unit unit in player.units)
        {
            Vector3 screenPos = cam.WorldToScreenPoint(unit.transform.position);

            if (screenPos.x > min.x && screenPos.x < max.x && screenPos.y > min.y && screenPos.y < max.y)
            {
                selectedUnits.Add(unit);
                unit.ToggleSelectionVisual(true);
            }
        }
    }

    // removes all destroyed or missing units from the selected list
    public void RemoveNullUnitsFromSelection()
    {
        for (int x = 0; x < selectedUnits.Count; x++)
        {
            if (selectedUnits[x] == null)
                selectedUnits.RemoveAt(x);

        }
    }

    // toggles the selected units selection visual
    void ToggleSelectionVisual(bool selected)
    {
        foreach (Unit unit in selectedUnits)
        {
            unit.ToggleSelectionVisual(selected);
        }
    }

    // returns whether or not we're selecting a unit or units
    public bool HasUnitsSelected()
    {
        return selectedUnits.Count > 0 ? true : false;

    }

    // returns the selected units
    public Unit[] GetSelectedUnits()
    {
        return selectedUnits.ToArray();
    }


    // returns a random enemy player
    public Player GetRandomEnemyPlayer(Player me)
    {
        Player ranPlayer = players[Random.Range(0, players.Length)];

        while (ranPlayer == me)
        {
            ranPlayer = players[Random.Range(0, players.Length)];
        }

        return ranPlayer;
    }

    // called when a unit dies, check to see if there's one remaining player
    public void UnitDeathCheck()
    {
        int remainingPlayers = 0;
        Player winner = null;

        for (int x = 0; x < players.Length; x++)
        {
            if (players[x].units.Count > 0)
            {
                remainingPlayers++;
                winner = players[x];
            }
        }

        // if there is more than 1 remaining player, return
        if (remainingPlayers != 1)
            return;

        EndScreenUI.instance.SetEndScreen(winner.isMe);
    }

    public Vector3? ScreenPointToMapPosition(Vector2 point)
    {
        var ray = Camera.main.ScreenPointToRay(point);
        RaycastHit hit;
        if (!MapCollider.Raycast(ray, out hit, Mathf.Infinity))
            return null;

        return hit.point;
    }

    public bool IsGameObjectSafetoPlace(GameObject go)
    {
        var verts = go.GetComponent<MeshFilter>().mesh.vertices;

        var obstacles = GameObject.FindObjectsOfType<NavMeshObstacle>();
        var cols = new List<Collider>();

        foreach (var o in obstacles)
        {
            if (o.gameObject != go)
            {
                cols.Add(o.gameObject.GetComponent<Collider>());
            }
        }

        foreach (var v in verts)
        {
            NavMeshHit hit;
            var vReal = go.transform.TransformPoint(v);
            NavMesh.SamplePosition(vReal, out hit, 20, NavMesh.AllAreas);

            bool onXAxis = Mathf.Abs(hit.position.x - vReal.x) < 0.5f;
            bool onZAxis = Mathf.Abs(hit.position.z - vReal.z) < 0.5f;
            bool hitCollider = cols.Any(c => c.bounds.Contains(vReal));

            if (!onXAxis || !onZAxis || hitCollider)
            {
                return false;
            }
        }
        return true;
    }
}