using System;
using UnityEngine;
using UnityEngine.InputSystem;

namespace GGJ.InGame.Player
{
    public class PlayerInputManager : MonoBehaviour
    {
        public Vector2 MoveInput { get; private set; }
        public event Action OnInteract;

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
            inputActions.Player.Interact.performed += OnInteractPerformed;
        }

        private void OnDisable()
        {
            inputActions.Player.Move.performed -= OnMovePerformed;
            inputActions.Player.Move.canceled -= OnMoveCanceled;
            inputActions.Player.Interact.performed -= OnInteractPerformed;
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

        private void OnInteractPerformed(InputAction.CallbackContext context)
        {
            OnInteract?.Invoke();
        }
    }
}
