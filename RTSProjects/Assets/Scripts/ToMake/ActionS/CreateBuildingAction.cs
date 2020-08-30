using UnityEngine;
using System.Collections;

public class CreateBuildingAction : ActionBehavior {

	public int Cost = 0;
	public float MaxBuildDistance = 30;
	public GameObject BuildingPrefab;

	public GameObject GhostBuildingPrefab;
	private GameObject active = null;



	public override System.Action GetClickAction ()
	{
		return delegate() {
			var player = GetComponentInParent<Player>();
			if(player.food < Cost)
			{
				GetComponent<ShowUnitInfo>().Deselect();
				Debug.Log("Not enough, this costs " + Cost);
            }


			var go = GameObject.Instantiate(GhostBuildingPrefab);
			var finder = go.AddComponent<FindbuildingSite>();
			finder.BuildingPrefab = BuildingPrefab;
			finder.MaxBuildDistance = MaxBuildDistance;
			finder.Player = player;
			finder.Source = transform;
			active = go;
		};
	}

	void Update()
	{
		if (active == null)
		{
			return;
		}
			if (Input.GetKeyDown (KeyCode.Escape))
			GameObject.Destroy (active);
	}

	void OnDestroy()
	{
		GetComponent<ShowUnitInfo>().Deselect();
		if (active == null)
			return;
	}

}
