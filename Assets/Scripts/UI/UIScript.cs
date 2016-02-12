using Assets.Scripts.Messages;
using Assets.Scripts.Misc;
using Assets.Scripts.Weapons;
using Game02.Assets.Scripts.Messages;
using UnityEngine;
using UnityEngine.UI;

public class UIScript : MonoBehaviour
{
    public static UIScript Instance;

    public Text AmmoText;
    public Text LivesText;
    public Text FPSText;

    private float _deltaTime;

    public void Start()
	{
	    Instance = this;
	    
	    PubSub.GlobalPubSub.Subscribe<HealthChangedMessage>(m => UpdateLives(((HealthChangedMessage)m).NewHealth));
	    PubSub.GlobalPubSub.Subscribe<AmmoChangedMessage>(m => 
	    	{ 
	    		var mes = (AmmoChangedMessage)m;
	    		if (mes.Type == AmmoType.Bullets)
	    		{
	    			UpdateAmmo(mes.NewAmount);
	    		}
	    	});	
		
	}

    public void Update()
    {
        _deltaTime += (Time.deltaTime - _deltaTime) * 0.1f;
        if (FPSText != null)
        {
            FPSText.text = string.Format("FPS: {0}", 1.0f/_deltaTime);
        }
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
