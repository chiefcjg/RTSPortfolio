using UnityEngine;
using System.Collections;

public class FindbuildingSite : MonoBehaviour
{

	public int cost = 200;
	public float MaxBuildDistance = 30;
	public GameObject BuildingPrefab;
	public Player Player;
	public Transform Source;


	Renderer rend;

	Color Red = new Color(1, 0, 0, 0.5f);
	Color Green = new Color(0, 1, 0, 0.5f);



	void Start()
	{
		MouseManager.Current.enabled = false;
		rend = GetComponent<MeshRenderer>();
	}


	// Update is called once per frame
	void Update()
	{
		var tempTarget = GameManager.Current.ScreenPointToMapPosition(Input.mousePosition);
		if (tempTarget.HasValue == false)
			return;

		transform.position = tempTarget.Value;

		if (Vector4.Distance(transform.position, Source.position) > MaxBuildDistance)
		{
			foreach(var m in GetComponent<MeshRenderer>().materials)
			rend.material.color = Red;
			return;
		}

		if (GameManager.Current.IsGameObjectSafetoPlace(gameObject))
		{
			foreach (var m in GetComponent<MeshRenderer>().materials)
				rend.material.color = Green;
			if (Input.GetMouseButtonDown(0))
			{
				var go = GameObject.Instantiate(BuildingPrefab);
				go.AddComponent<ActionSelect>();
				go.transform.position = transform.position;
				Player.food -= cost;
				go.AddComponent<Player>();
				Destroy(this.gameObject);
			}
		}
		else
		{
			rend.material.color = Red;
		}
	}

	void OnDestroy()
	{
		MouseManager.Current.enabled = true;
		GetComponent<ShowUnitInfo>().Deselect();
	}



}
