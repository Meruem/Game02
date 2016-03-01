using System;
using System.Collections;
using Assets.Scripts;
using Assets.Scripts.Messages;
using Assets.Scripts.Misc;
using Assets.Scripts.Visibility;
using Assets.Scripts.Weapons;
using UnityEngine;
using Random = UnityEngine.Random;

[RequireComponent(typeof(IMoveScript))]
public class MonsterController : MonoBehaviour 
{
    public BulletPrototype BulletPrototype;
    public float MaxVisibleDistance = 4;
    public float AttackDistance = 0.6f;
    public float TooCloseDistance = 0.3f;

    private float _direction;
    private GameObject _dynamicGameObjects;
    private GameObject _playerGameObject;
    private int _layerMask;

    private IMoveScript _moveScript;

    public void Awake()
    {
        _dynamicGameObjects = GameObjectEx.Find(GameObjectNames.DynamicObjects);
        _playerGameObject = GameObjectEx.FindGameObjectWithTag(GameObjectTags.Player);

        var shadowLayer = Layers.GetLayer(LayerName.ShadowLayer);
        _layerMask = (1 << shadowLayer) | (1 << Layers.GetLayer(LayerName.Player));
        _moveScript = GetComponent<IMoveScript>();
    }
    
    public void Start ()
	{
        this.GetPubSub().SubscribeInContext<WeaponHitMessage>(m => HandleWeaponHit((WeaponHitMessage)m));
	    StartCoroutine(WanderAround());
        //StartCoroutine(StartShooting());
    }

    private void HandleWeaponHit(WeaponHitMessage weaponHitMessage)
    {
        this.GetPubSub().PublishMessageInContext(new TakeDamageMessage(weaponHitMessage.Damage));
    }

    private IEnumerator StartShooting()
    {
        yield return new WaitForSeconds(Random.Range(1, 3));

        while (true)
        {
            if (BulletPrototype.Prefab == null)
            {
                Debug.LogWarning("Prototype is null.");
            }
            else
            {
                BulletObjectFactory.CreateBullet(transform.position, Random.Range(0, 16) * 360f / 16, BulletPrototype, Layers.GetLayer(LayerName.MonsterBullets), _dynamicGameObjects.transform);
            }
            yield return new WaitForSeconds(Random.Range(1, 3));
        }
    }
    
    private void Move(Vector2 vector)
    {
        _moveScript.MoveMaxSpeed(vector);
    }
    
    private void Move(Vector2 vector, float speed)
    {
        _moveScript.MoveNormal(vector, speed);
    }
    
    private void Attack()
    {
    	this.GetPubSub().PublishMessageInContext(new FireMessage(true));
    }

    IEnumerator WanderAround()
    {
        while (true)
        {
            if (VisibilityHelper.CheckVisibility(transform, _playerGameObject.transform, MaxVisibleDistance, _layerMask))
            {
                var distance = (transform.position - _playerGameObject.transform.position).magnitude;
                if (distance < TooCloseDistance)
                {
                    var vector = -_playerGameObject.transform.position + transform.position;
                    Move(vector);
                }
                else if (distance <= AttackDistance)
                {
                    var vector = _playerGameObject.transform.position - transform.position;
                    Move(vector, 0.1f); // in order to have correct position to player make small step
                    Attack();
                }
                else
                {
                    var vector = _playerGameObject.transform.position - transform.position;
                    Move(vector);
                }
                yield return null;
            }
            else
            {
                float angle = Random.Range(0, 16) * (float)Math.PI * 2f / 16;
                _direction = angle;
                var vector = Math2.AngleRadToVector(_direction);
                Move(vector);

                yield return new WaitForSeconds(Random.Range(1, 3));
            }
        }
    }
}
