using UnityEngine;
using UnityEngine.UI;
using GGJ.InGame.Audio;
using GGJ.Manager;

namespace GGJ.UI
{
    /// <summary>
    /// タイトル画面のUI制御
    /// 各種ボタンの処理やパネルの制御を管理
    /// </summary>
    public class TitleUI : MonoBehaviour
    {
        [Header("UI References")]
        [SerializeField] private SettingsUI settingsUI; // 設定UIの参照
        [SerializeField] private Button audioSettingsButton; // Audio設定を開くボタン
        [SerializeField] private Button startGameButton; // ゲーム開始ボタン
        [SerializeField] private Button exitButton; // 終了ボタン

        private void Start()
        {
            InitializeButtons();
        }

        /// <summary>
        /// ボタンのイベントを初期化
        /// </summary>
        private void InitializeButtons()
        {
            // Audio設定ボタンの設定
            if (audioSettingsButton != null && settingsUI != null)
            {
                audioSettingsButton.onClick.AddListener(() =>
                {
                    PlayButtonSound();
                    settingsUI.OpenSettings();
                });
            }

            // ゲーム開始ボタンの設定
            if (startGameButton != null)
            {
                startGameButton.onClick.AddListener(OnStartGameClicked);
            }

            // 終了ボタンの設定
            if (exitButton != null)
            {
                exitButton.onClick.AddListener(OnExitClicked);
            }
        }

        /// <summary>
        /// Audio設定パネルを開く
        /// </summary>
        public void OpenAudioSettings()
        {
            if (settingsUI != null)
            {
                PlayButtonSound();
                settingsUI.OpenSettings();
            }
        }

        /// <summary>
        /// ゲーム開始ボタンが押されたときの処理
        /// </summary>
        private void OnStartGameClicked()
        {
            PlayButtonSound();
            
            // TODO: Scene管理クラスを使用してゲームシーンに遷移
            Debug.Log("ゲーム開始 - Scene管理クラスで実装予定");
        }

        /// <summary>
        /// 終了ボタンが押されたときの処理
        /// </summary>
        private void OnExitClicked()
        {
            PlayButtonSound();
            
            if (GameManager.I != null)
            {
                GameManager.I.QuitApplication();
            }
            else
            {
                Debug.LogError("GameManager not found!");
                
                #if UNITY_EDITOR
                    UnityEditor.EditorApplication.isPlaying = false;
                #else
                    Application.Quit();
                #endif
            }
        }

        /// <summary>
        /// ボタンクリック音を再生
        /// </summary>
        private void PlayButtonSound()
        {
            if (AudioManager.I != null)
            {
                AudioManager.I.PlaySE(SEType.ButtonClick);
            }
        }

        private void OnDestroy()
        {
            // イベントの解除
            if (audioSettingsButton != null)
                audioSettingsButton.onClick.RemoveAllListeners();

            if (startGameButton != null)
                startGameButton.onClick.RemoveAllListeners();

            if (exitButton != null)
                exitButton.onClick.RemoveAllListeners();
        }
    }
}