using System;
using UnityEngine;
using UnityEngine.InputSystem;
using GGJ.InGame.Events;

namespace GGJ.InGame.Player
{
    /// <summary>
    /// 繝励Ξ繧､繝､繝ｼ蜈･蜉帙ｒ邂｡逅・＠縲！nput System縺九ｉ縺ｮ繧､繝吶Φ繝医ｒ螟画鋤縺吶ｋ
    /// </summary>
    public class PlayerInputManager : MonoBehaviour
    {
        public Vector2 MoveInput { get; private set; }
        public event Action OnInteract;

        private PlayerInput inputActions;
        private bool isInputEnabled = true;

        private void Awake()
        {
            inputActions = new PlayerInput();
            GameEvents.OnNpcInteractionStarted += OnNpcInteractionStarted;
            GameEvents.OnNpcInteractionEnded += OnNpcInteractionEnded;
        }

        private void OnDestroy()
        {
            GameEvents.OnNpcInteractionStarted -= OnNpcInteractionStarted;
            GameEvents.OnNpcInteractionEnded -= OnNpcInteractionEnded;
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
            if (!isInputEnabled) return;
            MoveInput = context.ReadValue<Vector2>();
        }

        private void OnMoveCanceled(InputAction.CallbackContext context) => 
            MoveInput = Vector2.zero;

        private void OnInteractPerformed(InputAction.CallbackContext context)
        {
            if (!isInputEnabled) return;
            OnInteract?.Invoke();
        }

        private void OnNpcInteractionStarted(GameObject npc)
        {
            isInputEnabled = false;
            MoveInput = Vector2.zero;
        }

        private void OnNpcInteractionEnded() => isInputEnabled = true;
    }
}
