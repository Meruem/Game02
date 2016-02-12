using System;
using System.Collections;
using Assets.Scripts;
using Assets.Scripts.Messages;
using Assets.Scripts.Misc;
using Assets.Scripts.Weapons;
using UnityEngine;
using Random = UnityEngine.Random;

[RequireComponent(typeof(MoveScript))]
public class MonsterScript : MonoBehaviour 
{
    public BulletPrototype BulletPrototype;

    private float _direction;
    private GameObject _dynamicGameObjects;

    public void Awake()
    {
        _dynamicGameObjects = GameObjectEx.Find(GameObjectNames.DynamicObjects);
    }
    
    public void Start ()
	{
	    StartCoroutine(ChangeDirection());
        StartCoroutine(StartShooting());
	}

    private IEnumerator StartShooting()
    {
        yield return new WaitForSeconds(3);

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

    IEnumerator ChangeDirection()
    {
        while (true)
        {
            float angle = Random.Range(0, 16)*(float) Math.PI*2f/16;
            _direction = angle;
            var vector = Math2.AngleRadToVector(_direction);
            this.GetPubSub().PublishMessageInContext(new MoveInDirectionMessage(vector));
            yield return new WaitForSeconds(Random.Range(1, 4));
        }
    }
}
