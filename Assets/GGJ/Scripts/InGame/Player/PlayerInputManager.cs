using UnityEngine;
using UnityEngine.InputSystem;

namespace GGJ.InGame.Player
{
    public class PlayerInputManager : MonoBehaviour
    {
        public Vector2 MoveInput { get; private set; }

        private PlayerInput inputActions;

        private void Awake()
        {
            inputActions = new PlayerInput();
        }

        private void OnEnable()
        {
            inputActions.Enable();
            inputActions.Player.Move.performed += OnMovePerformed;
            inputActions.Player.Move.canceled += OnMoveCanceled;
        }

        private void OnDisable()
        {
            inputActions.Player.Move.performed -= OnMovePerformed;
            inputActions.Player.Move.canceled -= OnMoveCanceled;
            inputActions.Disable();
        }

        private void OnMovePerformed(InputAction.CallbackContext context)
        {
            MoveInput = context.ReadValue<Vector2>();
        }

        private void OnMoveCanceled(InputAction.CallbackContext context)
        {
            MoveInput = Vector2.zero;
        }
    }
}
