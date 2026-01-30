using UnityEngine;

namespace GGJ.InGame.Player
{
    public class PlayerController : MonoBehaviour
    {
        [SerializeField] private PlayerMovement movement;

        private PlayerInputManager inputManager;
        private Rigidbody2D rb;

        private void Start()
        {
            inputManager = GetComponent<PlayerInputManager>();
            rb = GetComponent<Rigidbody2D>();
        }

        private void Update()
        {
            movement.Move(inputManager.MoveInput, rb);
        }
    }
}
