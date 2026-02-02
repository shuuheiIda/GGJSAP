using System;
using UnityEngine;
using UnityEngine.InputSystem;
using GGJ.InGame.Events;

namespace GGJ.InGame.Player
{
    /// <summary>
    /// プレイヤー入力を管理し、Input Systemからのイベントを変換する
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
            
            // Playerアクションマップを無効化（UI操作に専念）
            inputActions.Player.Disable();
        }

        private void OnNpcInteractionEnded()
        {
            isInputEnabled = true;
            
            // Playerアクションマップを再度有効化
            inputActions.Player.Enable();
        }
    }
}
