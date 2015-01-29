using Assets.Scripts;
using UnityEngine;

[RequireComponent(typeof(MoveScript))]
public class CharacterScript : MonoBehaviour
{
    public Transform Bullet;
 
    //adjustment for rotation based on sprite starting orientation.
    private bool _mousePressed;
    private MoveScript _moveScript;

    void Start()
    {
        _moveScript = GetComponent<MoveScript>();
    }

    public void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            _mousePressed = true;
        }
    }
 
    public void FixedUpdate()
    {
        //get the x factor of movement.
        float xMovement = Input.GetAxisRaw("Horizontal");
        //get the y factor of movement.
        float yMovement = Input.GetAxisRaw("Vertical");

        Vector2 movement = new Vector2(xMovement, yMovement);

        if (_mousePressed)
        {
            var pos = Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, Input.mousePosition.z));
            var difference = pos - transform.position;
            difference.z = 0;
            var angle = Mathf.Atan2( difference.y,
                difference.x);
            var normalizedDifference = difference.normalized;
            
            var bullet = (Transform)Instantiate(Bullet, transform.position + normalizedDifference * 1, Quaternion.Euler(0, 0, Mathf.Rad2Deg * angle));
            var bulletScript = bullet.GetComponent<Bullet>();
            bullet.rigidbody2D.velocity = normalizedDifference * bulletScript.Speed;
            _mousePressed = false;
        }

        _moveScript.Move(movement.normalized);
    }
}