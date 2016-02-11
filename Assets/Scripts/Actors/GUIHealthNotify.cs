namespace Assets.Scripts.Actors
{
    [ReguireComponent(typeof(Health))]
    public class GUIHealthNotify : MonoBehaviour
    {
        public void Start()
        {
            var health = GetComponent<Health>();
            var originalAction = health.OnTakeDamageAction;
            health.OnTakeDamageAction = dmg => 
                {
                    originalAction(dmg);
                    PubSub.GlobalPubSub.Publish(new HealthChangedMessage(health.Lives));
                }
        }
    }
}
