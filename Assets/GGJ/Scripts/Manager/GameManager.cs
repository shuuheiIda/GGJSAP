using UnityEngine;
using GGJ.Core;
using GGJ.InGame.Audio;

namespace GGJ.Manager
{
    /// <summary>
    /// ゲーム全体を統括するマネージャー
    /// アプリケーション制御、設定保存などを管理
    /// シーンを跨いで保持される
    /// </summary>
    public class GameManager : Singleton<GameManager>
    {
        [Header("Game Settings")]
        [SerializeField] private bool enableDebugMode = false;

        /// <summary>
        /// デバッグモードの有効/無効
        /// </summary>
        public bool IsDebugMode => enableDebugMode;

        protected override void Init()
        {
            // ターゲットフレームレートの設定
            Application.targetFrameRate = 60;
            
            // スクリーンスリープの無効化
            Screen.sleepTimeout = SleepTimeout.NeverSleep;
            
            LogInfo("GameManager initialized successfully.");
        }

        #region Application Control

        /// <summary>
        /// アプリケーションを終了する
        /// </summary>
        public void QuitApplication()
        {
            LogInfo("Application quit requested.");

            // ゲーム終了前の処理
            OnApplicationQuit();

            #if UNITY_EDITOR
                UnityEditor.EditorApplication.isPlaying = false;
            #else
                Application.Quit();
            #endif
        }

        /// <summary>
        /// アプリケーション終了前の処理
        /// </summary>
        private void OnApplicationQuit()
        {
            // 設定の保存
            SaveUserSettings();
            
            // BGM停止
            if (AudioManager.I != null)
            {
                AudioManager.I.StopBGM();
            }
            
            LogInfo("Application quit preparations completed.");
        }

        #endregion

        #region Settings Management

        /// <summary>
        /// ユーザー設定を保存
        /// </summary>
        public void SaveUserSettings()
        {
            if (AudioManager.I != null)
            {
                PlayerPrefs.SetFloat("MasterVolume", AudioManager.I.masterVolume);
                PlayerPrefs.SetFloat("BGMVolume", AudioManager.I.bgmVolume);
                PlayerPrefs.SetFloat("SEVolume", AudioManager.I.seVolume);
            }

            PlayerPrefs.Save();
            LogInfo("User settings saved.");
        }

        /// <summary>
        /// ユーザー設定を読み込み
        /// </summary>
        public void LoadUserSettings()
        {
            if (AudioManager.I != null)
            {
                float masterVolume = PlayerPrefs.GetFloat("MasterVolume", 1f);
                float bgmVolume = PlayerPrefs.GetFloat("BGMVolume", 0.7f);
                float seVolume = PlayerPrefs.GetFloat("SEVolume", 1f);

                AudioManager.I.SetMasterVolume(masterVolume);
                AudioManager.I.SetBGMVolume(bgmVolume);
                AudioManager.I.SetSEVolume(seVolume);
            }

            LogInfo("User settings loaded.");
        }

        #endregion

        #region Unity Events

        private void Start()
        {
            // 設定を読み込み
            LoadUserSettings();
        }

        private void OnApplicationPause(bool pauseStatus)
        {
            // アプリケーションがバックグラウンドに移行した際の処理
            if (pauseStatus)
            {
                SaveUserSettings();
                LogInfo("Application paused - settings saved.");
            }
        }

        private void OnApplicationFocus(bool hasFocus)
        {
            // アプリケーションがフォーカスを失った際の処理
            if (!hasFocus)
            {
                SaveUserSettings();
                LogInfo("Application focus lost - settings saved.");
            }
        }

        #endregion

        #region Debug and Logging

        /// <summary>
        /// 情報ログを出力
        /// </summary>
        private void LogInfo(string message)
        {
            if (enableDebugMode)
            {
                Debug.Log($"[GameManager] {message}");
            }
        }

        /// <summary>
        /// エラーログを出力
        /// </summary>
        private void LogError(string message)
        {
            Debug.LogError($"[GameManager] {message}");
        }

        #endregion
    }
}