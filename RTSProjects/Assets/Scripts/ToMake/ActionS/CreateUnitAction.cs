using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CreateUnitAction : ActionBehavior
{

    public GameObject Prefab;
    public int Cost = 0;
    private Component player;

    // Use this for initialization
    void Start()
    {
        player = GetComponentInParent<Player>();
    }

    public override System.Action GetClickAction()
    {
        return delegate ()
        {
            if (GetComponentInParent<Player>().food < Cost)
            {
                Debug.Log("Not enough, this costs " + Cost);
                return;
            }
            var go = (GameObject)GameObject.Instantiate(
                Prefab,
                transform.position,
                Quaternion.identity);
            go.AddComponent<Player>();
            go.AddComponent<ActionSelect>();
            GetComponentInParent<Player>().food -= Cost;
        };
    }
}
