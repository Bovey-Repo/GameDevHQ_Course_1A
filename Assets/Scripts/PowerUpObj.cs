using UnityEngine;

[System.Serializable]
public class PowerUpObj
{
    public GameObject prefab;
    public int chance;
    public string description;

    public PowerUpObj(GameObject obj, int r, string d)
    {
        this.prefab = obj;
        this.chance = r;
        this.description = d;
    }
}
