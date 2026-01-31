using GGJ.Core;
using GGJ.Manager;

namespace GGJ.InGame.Manager
{
    /// <summary>
    /// インゲームシーン固有の処理を管理するマネージャー
    /// 時間制限管理はGameManagerに移植済み
    /// </summary>
    public class InGameManager : Singleton<InGameManager>
    {
        
        protected override bool UseDontDestroyOnLoad => false;

        protected override void Init()
        {
            // インゲームシーン固有の初期化処理
            // 制限時間管理はGameManagerが担当
        }

        private void Start()
        {
            // GameManagerにゲーム開始を委譲
            if (GameManager.I != null)
                GameManager.I.StartGame();
        }
                
        /// <summary>
        /// ゲームを終了する（GameManagerに委譲）
        /// </summary>
        public void EndGame()
        {
            if (GameManager.I != null)
                GameManager.I.EndGame();
        }
    }
}
