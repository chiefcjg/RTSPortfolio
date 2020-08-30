using UnityEngine;
using System.Collections;

public class ShowUnitInfo : Interaction
{
    public string Name;
    public float MaxHealth, CurrentHealth;
    public Sprite ProfilePic;

    bool show = false;

    public void Awake()
    {
        MaxHealth = this.GetComponent<Unit>().maxHp;
        CurrentHealth = this.GetComponent<Unit>().curHp;
    }

    public override void Select()
    {
        CurrentHealth = this.GetComponent<Unit>().curHp;
        show = true;
    }

    void Update()
    {
        if (!show) return;

        InfoManager.Current.SetPic(ProfilePic);
        InfoManager.Current.SetLines(Name, CurrentHealth + "/" + MaxHealth, "Owner: " + GetComponent<Unit>().player.name);
    }

    public override void Deselect()
    {
        InfoManager.Current.ClearPic();
        InfoManager.Current.ClearLines();
        show = false;
    }
}
