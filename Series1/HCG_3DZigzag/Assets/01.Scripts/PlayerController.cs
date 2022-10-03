using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    private Movement _movement;
    private float _limitDeathY;

    private void Awake()
    {
        _movement = GetComponent<Movement>();
        _movement.MoveTo(Vector3.right);

        _limitDeathY = transform.position.y - transform.localPosition.y * 0.3f;
    }

    private void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Vector3 direction = _movement.MoveDirection == Vector3.forward ? Vector3.right : Vector3.forward;
            _movement.MoveTo(direction);
        }

        if (transform.position.y < _limitDeathY)
        {
            Debug.Log("GameOver");
        }
    }
}
