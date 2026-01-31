using UnityEngine;
using GGJ.InGame.Events;

namespace GGJ.InGame.Player
{
    /// <summary>
    /// 繝励Ξ繧､繝､繝ｼ縺ｮ蛻ｶ蠕｡繧堤ｵｱ諡ｬ縺吶ｋ繧ｳ繝ｳ繝医Ο繝ｼ繝ｩ繝ｼ
    /// </summary>
    public class PlayerController : MonoBehaviour
    {
        [SerializeField] private PlayerMovement movement;

        private PlayerInputManager inputManager;
        private PlayerCollider playerCollider;
        private Rigidbody2D rb;

        private void Start()
        {
            inputManager = GetComponent<PlayerInputManager>();
            playerCollider = GetComponent<PlayerCollider>();
            rb = GetComponent<Rigidbody2D>();
            
            inputManager.OnInteract += HandleInteract;
        }

        private void OnDestroy()
        {
            if (inputManager != null)
                inputManager.OnInteract -= HandleInteract;
        }

        private void Update()
        {
            movement.Move(inputManager.MoveInput, rb);
        }

        private void HandleInteract()
        {
            var nearestObject = playerCollider.GetNearestObject();
            if (nearestObject == null) return;
            
            GameEvents.RaiseNpcInteractionStarted(nearestObject.gameObject);
        }
    }
}
