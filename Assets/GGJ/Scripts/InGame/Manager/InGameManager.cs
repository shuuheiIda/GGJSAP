using UnityEngine;
using GGJ.Core;
using GGJ.Manager;
using GGJ.InGame.Player;

namespace GGJ.InGame.Manager
{
    /// <summary>
    /// インゲームシーン固有の処理を管理するマネージャー
    /// 時間制限管理はGameManagerに移植済み
    /// </summary>
    public class InGameManager : Singleton<InGameManager>
    {
        [Header("ポーズUI")]
        [SerializeField] private GameObject pauseUI;

        /// <summary>ゲームがポーズ中かどうか</summary>
        public bool IsPaused { get; private set; }
        
        protected override bool UseDontDestroyOnLoad => false;

        protected override void Init()
        {
            // インゲームシーン固有の初期化処理
            // 制限時間管理はGameManagerが担当
            if (pauseUI != null)
                pauseUI.SetActive(false);

            // 静的イベントを購読
            PlayerInputManager.OnPauseRequested += TogglePause;
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();
            
            // 静的イベントの購読解除
            PlayerInputManager.OnPauseRequested -= TogglePause;
        }

        private void Start()
        {
            // GameManagerにゲーム開始を委譲
            if (GameManager.I != null)
                GameManager.I.StartGame();
        }

        /// <summary>
        /// ポーズ状態を切り替える
        /// </summary>
        private void TogglePause()
        {
            if (IsPaused)
                ResumeGame();
            else
                PauseGame();
        }

        /// <summary>
        /// ゲームをポーズする
        /// </summary>
        public void PauseGame()
        {
            if (IsPaused) return;
            
            IsPaused = true;
            Time.timeScale = 0f;
            
            if (pauseUI != null)
                pauseUI.SetActive(true);
        }

        /// <summary>
        /// ゲームを再開する
        /// </summary>
        public void ResumeGame()
        {
            if (!IsPaused) return;
            
            IsPaused = false;
            Time.timeScale = 1f;
            
            if (pauseUI != null)
                pauseUI.SetActive(false);
        }

        /// <summary>
        /// ゲームをリスタートする
        /// </summary>
        public void RestartGame()
        {
            Time.timeScale = 1f;
            IsPaused = false;
            
            if (GGJ.Scene.SceneController.I != null)
                GGJ.Scene.SceneController.I.LoadScene(GGJ.Scene.SceneName.InGame);
        }

        /// <summary>
        /// タイトルに戻る
        /// </summary>
        public void ReturnToTitle()
        {
            Time.timeScale = 1f;
            IsPaused = false;
            
            if (GGJ.Scene.SceneController.I != null)
                GGJ.Scene.SceneController.I.LoadScene(GGJ.Scene.SceneName.Title);
        }
    }
}
