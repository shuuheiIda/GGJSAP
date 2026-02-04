using UnityEngine;
using GGJ.InGame.Events;

namespace GGJ.InGame.Player
{
    /// <summary>
    /// プレイヤーの制御を統括するコントローラー
    /// </summary>
    public class PlayerController : MonoBehaviour
    {
        [SerializeField] private PlayerMovement movement;
        [SerializeField] private PlayerSpriteController spriteController;

        private PlayerInputManager inputManager;
        private PlayerCollider playerCollider;
        private Rigidbody2D rb;

        private void Start()
        {
            inputManager = GetComponent<PlayerInputManager>();
            playerCollider = GetComponent<PlayerCollider>();
            rb = GetComponent<Rigidbody2D>();
            
            var spriteRenderer = GetComponent<SpriteRenderer>();
            spriteController?.Initialize(spriteRenderer);
            
            inputManager.OnInteract += HandleInteract;
        }

        private void OnDestroy()
        {
            if (inputManager != null)
                inputManager.OnInteract -= HandleInteract;
        }

        private void Update()
        {
            Vector2 moveInput = inputManager.MoveInput;
            
            movement.Move(moveInput, rb);
            spriteController?.UpdateSpriteByMovement(moveInput);
        }

        private void HandleInteract()
        {
            var nearestObject = playerCollider.GetNearestObject();
            if (nearestObject == null) return;
            
            GameEvents.RaiseNpcInteractionStarted(nearestObject.gameObject);
        }
    }
}
