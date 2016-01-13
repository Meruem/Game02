using UnityEngine;
using UnityEngine.UI;

public class UIScript : MonoBehaviour
{
    public static UIScript Instance;

    public Text AmmoText;
    public Text LivesText;

	// Use this for initialization
	void Start ()
	{
	    Instance = this;
	}

    public void UpdateAmmo(int ammo)
    {
        if (AmmoText == null) return;

        AmmoText.text = string.Format("Ammo: {0}", ammo);
    }

    public void UpdateLives(int lives)
    {
        if (LivesText == null) return;

        LivesText.text = string.Format("Lives: {0}", lives);
    }
}
