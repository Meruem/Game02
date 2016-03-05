using Assets.Scripts.Messages;
using Assets.Scripts.Misc;
using Game02.Assets.Scripts.Messages;
using UnityEngine;
using UnityEngine.UI;

public class UIScript : MonoBehaviour
{
    public Text AmmoText;
    public Text LivesText;
    public Text FPSText;
    public Text EnergyText;

    private float _deltaTime;

    public void Start()
	{
	    PubSub.GlobalPubSub.Subscribe<HealthChangedMessage>(m => UpdateLives(((HealthChangedMessage)m).NewHealth));
        PubSub.GlobalPubSub.Subscribe<EnergyChangedMessage>(m => UpdateEnergy(((EnergyChangedMessage)m).NewValue));
        PubSub.GlobalPubSub.Subscribe<AmmoChangedMessage>(m => 
	    	{ 
	    		var mes = (AmmoChangedMessage)m;
       			UpdateAmmo(mes.NewAmount);
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

    public void UpdateEnergy(int newValue)
    {
        if (EnergyText == null) return;

        EnergyText.text = string.Format("Energy: {0}", newValue);
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
