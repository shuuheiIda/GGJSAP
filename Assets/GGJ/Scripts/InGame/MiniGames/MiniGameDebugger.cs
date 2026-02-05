using UnityEngine;

namespace GGJ.InGame.MiniGames
{
    /// <summary>
    /// MiniGame単体デバッグ用クラス
    /// PlayしたいMiniGameをアタッチするだけでPlay可能
    /// </summary>
    public class MiniGameDebugger : MonoBehaviour
    {
        [SerializeField] private GameObject miniGameObject; // デバッグしたいミニゲームオブジェクト

        private void Start()
        {
            if (miniGameObject == null)
            {
                Debug.LogError("[MiniGameDebugger] ミニゲームオブジェクトがアタッチされていません！");
                return;
            }

            IMiniGame miniGame = miniGameObject.GetComponent<IMiniGame>();
            if (miniGame == null)
            {
                Debug.LogError($"[MiniGameDebugger] {miniGameObject.name} に IMiniGame が実装されていません！");
                return;
            }

            miniGame.StartMiniGame();
        }
    }
}
