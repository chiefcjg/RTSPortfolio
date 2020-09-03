using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using UnityEngine.AI;

public class GameManager : MonoBehaviour
{
    public Player[] Players;

    public static GameManager instance;


    public TerrainCollider MapCollider;
    public static GameManager Current = null;

    public GameManager()
    {
        instance = this;
        Current = this;
    }

    // returns a random enemy player
    public Player GetRandomEnemyPlayer(Player me)
    {
        Player ranPlayer = Players[Random.Range(0, Players.Length)];

        while (ranPlayer == me)
        {
            ranPlayer = Players[Random.Range(0, Players.Length)];
        }

        return ranPlayer;
    }

    // called when a unit dies, check to see if there's one remaining player
    public void UnitDeathCheck()
    {
        int remainingPlayers = 0;
        Player winner = null;

        for (int x = 0; x < Players.Length; x++)
        {
            if (Players[x].units.Count > 0)
            {
                remainingPlayers++;
                winner = Players[x];
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