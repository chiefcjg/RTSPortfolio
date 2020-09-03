using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Player : MonoBehaviour
{
    public string Name;
    public Color AccentColor;

    public bool isMe;

    [Header("Units")]
    public List<Unit> units = new List<Unit>();

    [Header("Resources")]
    public int food;

    [Header("Components")]
    public GameObject unitPrefab;
    public Transform unitSpawnPos;


    public int TechnologyLevel = 0;

    // events
    [System.Serializable]
    public class UnitCreatedEvent : UnityEvent<Unit> { }
    public UnitCreatedEvent onUnitCreated;
    public UnitCreatedEvent onUnit1Created;

    public readonly int unitCost = 50;

    public static Player me;

    void Awake ()
    {
        if(isMe)
            me = this;
    }

    void Start ()
    {
        if(isMe)
        {
            GameUI.instance.UpdateUnitCountText(units.Count);
            GameUI.instance.UpdateFoodText(food);

            CameraController.instance.FocusOnPosition(unitSpawnPos.position);
        }

        food += unitCost;
        CreateNewUnit();
    }

    // called when a unit gathers a certain resource
    public void GainResource (ResourceType resourceType, int amount)
    {
        switch(resourceType)
        {
            case ResourceType.Food:
            {
                food += amount;

                if(isMe)
                    GameUI.instance.UpdateFoodText(food);
                break;
            }
        }
    }

    // creates a new unit for the player
    public void CreateNewUnit ()
    {
        if(food - unitCost < 0)
            return;

        GameObject unitObj = Instantiate(unitPrefab, unitSpawnPos.position, Quaternion.identity, transform);
        Unit unit = unitObj.GetComponent<Unit>();

        units.Add(unit);
        unit.player = this;
        food -= unitCost;

        if(onUnitCreated != null)
            onUnitCreated.Invoke(unit);

        if(isMe)
        {
            GameUI.instance.UpdateUnitCountText(units.Count);
            GameUI.instance.UpdateFoodText(food);
        }
    }

    // is this my unit?
    public bool IsMyUnit (Unit unit)
    {
        return units.Contains(unit);
    }

    public void IncreaseTechnology()
    {
        if (TechnologyLevel == 0)
        {
            foreach (Unit unit in units)
            {
                unit.curHp = unit.curHp;
                unit.maxHp = unit.maxHp;
            }
        }
        if (TechnologyLevel == 1)
        {
            foreach (Unit unit in units)
            {
                unit.curHp = unit.curHp + 10;
                unit.maxHp = unit.maxHp + 10;
            }
        }
        if (TechnologyLevel == 2)
        {
            foreach (Unit unit in units)
            {
                unit.curHp = unit.curHp + 15;
                unit.maxHp = unit.maxHp + 15;
            }
        }
        if (TechnologyLevel == 3)
        {
            foreach (Unit unit in units)
            {
                unit.curHp = unit.curHp + 20;
                unit.maxHp = unit.maxHp + 20;
            }
        }
    }
}