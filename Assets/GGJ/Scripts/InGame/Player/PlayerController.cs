using UnityEngine;
using UnityEngine.InputSystem;

namespace GGJ.InGame.Player
{
    public class PlayerController : MonoBehaviour
    {
        [SerializeField] private PlayerMovement movement;

        private PlayerInput playerInput;
        private Rigidbody2D rb;
        private PlayerInputManager inputManager;

        private void Start()
        {
            playerInput = GetComponent<PlayerInput>();
            rb = GetComponent<Rigidbody2D>();
            
            inputManager = new PlayerInputManager();
            inputManager.Initialize(playerInput);
        }

        private void Update()
        {
            Vector2 input = inputManager.GetMoveInput();
            movement.Move(input, rb);
        }
    }
}
