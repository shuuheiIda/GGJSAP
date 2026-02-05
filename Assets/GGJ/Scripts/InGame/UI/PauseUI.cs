using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using GGJ.InGame.Audio;
using GGJ.InGame.Manager;

namespace GGJ.InGame.UI
{
    /// <summary>
    /// ポーズ画面のUI制御
    /// </summary>
    public class PauseUI : MonoBehaviour
    {
        [Header("ボタン")]
        [SerializeField] private Button restartButton;
        [SerializeField] private Button titleButton;
        [SerializeField] private Button resumeButton;

        [Header("フォーカス設定")]
        [SerializeField] private GameObject firstSelectedButton;

        private void Start()
        {
            InitializeButtons();
        }

        private void OnEnable()
        {
            SetFirstSelected();
        }

        /// <summary>
        /// ボタンのイベントを初期化
        /// </summary>
        private void InitializeButtons()
        {
            if (resumeButton != null)
            {
                resumeButton.onClick.AddListener(() =>
                {
                    AudioManager.I?.PlaySE(SEType.ButtonClick);
                    OnResumeClicked();
                });
            }

            if (restartButton != null)
            {
                restartButton.onClick.AddListener(() =>
                {
                    AudioManager.I?.PlaySE(SEType.ButtonClick);
                    OnRestartClicked();
                });
            }

            if (titleButton != null)
            {
                titleButton.onClick.AddListener(() =>
                {
                    AudioManager.I?.PlaySE(SEType.ButtonClick);
                    OnTitleClicked();
                });
            }
        }

        /// <summary>
        /// 最初に選択されるボタンを設定
        /// </summary>
        private void SetFirstSelected()
        {
            if (firstSelectedButton != null && EventSystem.current != null)
            {
                EventSystem.current.SetSelectedGameObject(null);
                EventSystem.current.SetSelectedGameObject(firstSelectedButton);
            }
        }

        /// <summary>
        /// ゲームに戻るボタン
        /// </summary>
        private void OnResumeClicked()
        {
            if (InGameManager.I != null)
                InGameManager.I.ResumeGame();
        }

        /// <summary>
        /// リスタートボタン
        /// </summary>
        private void OnRestartClicked()
        {
            if (InGameManager.I != null)
                InGameManager.I.RestartGame();
        }

        /// <summary>
        /// タイトルに戻るボタン
        /// </summary>
        private void OnTitleClicked()
        {
            if (InGameManager.I != null)
                InGameManager.I.ReturnToTitle();
        }
    }
}
