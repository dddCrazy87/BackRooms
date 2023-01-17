using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMotor : MonoBehaviour
{
    private CharacterController Controller;
    private Vector3 PlayerVelocity;
    private bool isGrounded;
    [SerializeField] private float Speed, JumpHeight, Gravity;

    private void Start() {
        Controller = GetComponent<CharacterController>();
    }

    private void Update() {
        isGrounded = Controller.isGrounded;
    }

    public void ProcessMove(Vector2 Input) {
        Vector3 MoveDir = Vector3.zero;
        MoveDir.x = Input.x; MoveDir.z = Input.y;
        Controller.Move(transform.TransformDirection(MoveDir * Speed * Time.deltaTime));
        if(isGrounded && PlayerVelocity.y < 0)
              PlayerVelocity.y  = Gravity * Time.deltaTime;
        else  PlayerVelocity.y += Gravity * Time.deltaTime;
        Controller.Move(PlayerVelocity * Time.deltaTime);
    }

    public void Jump() {
        if(!isGrounded) return;
        PlayerVelocity.y = Mathf.Sqrt(-1 * JumpHeight * JumpHeight * Gravity);
    }
}
