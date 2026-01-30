using UnityEngine;

namespace GGJ.InGame.Player
{
    /// <summary>
    /// プレイヤーの制御を統括するコントローラー
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
            if (nearestObject != null)
            {
                // ここはテスト用（ここで時間を止める処理や、会話Panelを表示したりする依存関係が大きくなりそうな場合はイベント駆動も検討）
                nearestObject.gameObject.SetActive(false);
            }
        }
    }
}
