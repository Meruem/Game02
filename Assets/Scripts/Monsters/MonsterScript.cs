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

    private MoveScript _moveScript;
    private float _direction;
    private int _lives;
    private TakeDamageTrigger _takeDamageTrigger;

    private readonly BulletPrototype _bulletPrototype = new BulletPrototype
    {
        Damage = 1,
        Speed = 6,
        PrefabName = "MonsterBullet"
    };

    // Use this for initialization
    public void Start ()
	{
        _lives = MaxLives;
	    _moveScript = GetComponent<MoveScript>();
	    _takeDamageTrigger = GetComponent<TakeDamageTrigger>();
        _takeDamageTrigger.OnTakeDamage += OnTakeDamage;
	    StartCoroutine(ChangeDirection());
        StartCoroutine(StartShooting());
	}

    private void OnTakeDamage(int damage)
    {
        Debug.Log(string.Format("Damage taken: {0}", damage));
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
            var bullet = BulletObjectFactory.CreateBullet(transform.position, Random.Range(0, 16) * 360f / 16, _bulletPrototype);
            bullet.gameObject.layer = (int)Layers.MonsterBulletes;
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
