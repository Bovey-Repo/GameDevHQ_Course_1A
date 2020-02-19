public class Weapon
{
    public string name;
    public float fireRate;
    public int ammoCount;

    public Weapon(string name, float fireRate, int ammoCount)
    {
        this.name = name;
        this.fireRate = fireRate;
        this.ammoCount = ammoCount;
    }
}