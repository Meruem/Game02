using System;
using Assets.Scripts;
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

	// Use this for initialization
	void Start ()
	{
        _lives = MaxLives;
	    _moveScript = GetComponent<MoveScript>();
	    _takeDamageTrigger = GetComponent<TakeDamageTrigger>();
        _takeDamageTrigger.OnTakeDamage += OnTakeDamage;
        ChangeDirection();
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

    // Update is called once per frame
	void Update () 
    {
        //FireBullet();
            //_nextDirectionChange = gameTime.TotalGameTime + TimeSpan.FromSeconds(_random.Next(1, 5));

	}

    private void ChangeDirection()
    {
        float angle = Random.Range(0, 16) * (float)Math.PI * 2f / 16;
        _direction = angle;
        var vector = Math2.AngleRadToVector(_direction)*_moveScript.MaxSpeed;
        _moveScript.Move(vector);
        Invoke("ChangeDirection", Random.Range(1,4));
    }
}
