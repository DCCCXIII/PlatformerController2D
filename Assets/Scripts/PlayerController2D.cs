using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(PlatformerCharacter2D))]
public class PlayerController2D : MonoBehaviour
{
    private PlatformerCharacter2D character2D;

    private void Awake()
    {
        character2D = gameObject.GetComponent<PlatformerCharacter2D>();
    }

    private void LateUpdate()
    {
        float horizontalInput = Input.GetAxis("Horizontal");
        bool jump = Input.GetButtonDown("Jump");
        character2D.Move(horizontalInput, 0, jump);
    }
}