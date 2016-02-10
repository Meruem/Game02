using System;
using System.Collections;
using Assets.Scripts;
using Assets.Scripts.Weapons;
using UnityEngine;
using Random = UnityEngine.Random;

[RequireComponent(typeof(MoveScript), typeof(TakeDamageTrigger))]
public class MonsterScript : MonoBehaviour 
{
    public int MaxLives = 3;
    public BulletPrototype BulletPrototype;

    private MoveScript _moveScript;
    private float _direction;
    private int _lives;
    private TakeDamageTrigger _takeDamageTrigger;
    private GameObject _dynamicGameObjects;

    //{
    //    Damage = 1,
    //    Speed = 6,
    //    PrefabName = "MonsterBullet"
    //};

    public void Awake()
    {
        _moveScript = GetComponent<MoveScript>();
        _takeDamageTrigger = GetComponent<TakeDamageTrigger>();
        _dynamicGameObjects = GameObject.Find("Dynamic Objects");
    }
    
    public void Start ()
	{
        _lives = MaxLives;
        _takeDamageTrigger.OnTakeDamage += OnTakeDamage;
	    StartCoroutine(ChangeDirection());
        StartCoroutine(StartShooting());
	}

    private void OnTakeDamage(int damage)
    {
        _lives -= damage;
        if (_lives <= 0)
        {
            _takeDamageTrigger.OnTakeDamage -= OnTakeDamage;
            Destroy(gameObject);
        }
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
                BulletObjectFactory.CreateBullet(transform.position, Random.Range(0, 16) * 360f / 16, BulletPrototype, (int)Layers.MonsterBulletes, _dynamicGameObjects.transform);
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
            var vector = Math2.AngleRadToVector(_direction)*_moveScript.MaxSpeed;
            _moveScript.Move(vector);
            yield return new WaitForSeconds(Random.Range(1, 4));
        }
    }
}
