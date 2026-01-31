using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
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
        [Header("UI参照")]
        [SerializeField] private SettingsUI settingsUI; // 設定UIの参照
        [SerializeField] private Button audioSettingsButton; // Audio設定を開くボタン
        [SerializeField] private Button startGameButton; // ゲーム開始ボタン
        [SerializeField] private Button exitButton; // 終了ボタン

        [Header("コントローラーのフォーカスを最初に当てるオブジェクト")]
        [SerializeField] private GameObject firstSelectedButton; // 最初に選択されるボタン（通常はStartButton）

        private void Start()
        {
            InitializeButtons();
            SetFirstSelected();
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
            if (audioSettingsButton != null && settingsUI != null)
            {
                audioSettingsButton.onClick.AddListener(() =>
                {
                    AudioManager.I?.PlaySE(SEType.ButtonClick);
                    settingsUI.OpenSettings();
                });
            }

            if (startGameButton != null)
                startGameButton.onClick.AddListener(OnStartGameClicked);

            if (exitButton != null)
                exitButton.onClick.AddListener(OnExitClicked);
        }

        /// <summary>
        /// Audio設定パネルを開く
        /// </summary>
        public void OpenAudioSettings()
        {
            if (settingsUI == null) return;
            
            AudioManager.I?.PlaySE(SEType.ButtonClick);
            settingsUI.OpenSettings();
        }

        /// <summary>
        /// ゲーム開始ボタンが押されたときの処理
        /// </summary>
        private void OnStartGameClicked()
        {
            AudioManager.I?.PlaySE(SEType.ButtonClick);
        }

        /// <summary>
        /// 終了ボタンが押されたときの処理
        /// </summary>
        private void OnExitClicked()
        {
            AudioManager.I?.PlaySE(SEType.ButtonClick);
            
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
        /// コントローラー用：最初に選択されるボタンを設定
        /// </summary>
        private void SetFirstSelected()
        {
            GameObject targetButton = firstSelectedButton != null ? firstSelectedButton : startGameButton?.gameObject;
            
            if (targetButton == null || EventSystem.current == null) return;
            
            EventSystem.current.SetSelectedGameObject(null);
            EventSystem.current.SetSelectedGameObject(targetButton);
        }

        private void OnDestroy()
        {
            if (audioSettingsButton != null)
                audioSettingsButton.onClick.RemoveAllListeners();

            if (startGameButton != null)
                startGameButton.onClick.RemoveAllListeners();

            if (exitButton != null)
                exitButton.onClick.RemoveAllListeners();
        }
    }
}