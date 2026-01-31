using UnityEngine;
using GGJ.Core;
using GGJ.InGame.Audio;
using GGJ.InGame.Events;

namespace GGJ.Manager
{
    /// <summary>
    /// ゲーム全体を統括するマネージャー
    /// アプリケーション制御、設定保存、制限時間管理などを管理
    /// シーンを跨いで保持される
    /// </summary>
    public class GameManager : Singleton<GameManager>
    {
        private const float TIME_ZERO = 0f;
        
        [Header("時間制限設定")]
        [SerializeField] private float gameDuration = 60f;
        
        /// <summary>
        /// 残り時間
        /// </summary>
        public float RemainingTime { get; private set; }
        
        /// <summary>
        /// ゲームが実行中かどうか
        /// </summary>
        public bool IsGameRunning { get; private set; }
        
        /// <summary>
        /// ゲームが終了したかどうか
        /// </summary>
        public bool IsGameEnded { get; private set; }

        protected override void Init()
        {
            Application.targetFrameRate = 60;
            Screen.sleepTimeout = SleepTimeout.NeverSleep;
            
            // イベント購読
            GameEvents.OnNpcInteractionStarted += OnNpcInteractionStarted;
            GameEvents.OnNpcInteractionEnded += OnNpcInteractionEnded;
        }

        #region アプリケーション制御

        /// <summary>
        /// アプリケーションを終了する
        /// </summary>
        public void QuitApplication()
        {
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
            SaveUserSettings();
            
            if (AudioManager.I != null)
                AudioManager.I.StopBGM();
        }

        #endregion

        #region 設定管理

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
        }

        #endregion

        #region Unityイベント

        private void Start()
        {
            LoadUserSettings();
        }

        private void OnApplicationPause(bool pauseStatus)
        {
            if (!pauseStatus) return;
            
            SaveUserSettings();
        }

        private void OnApplicationFocus(bool hasFocus)
        {
            if (hasFocus) return;
            
            SaveUserSettings();
        }

        #endregion
        
        #region ゲーム時間管理
        
        private void Update()
        {
            if (!IsGameRunning || IsGameEnded) return;

            RemainingTime -= Time.deltaTime;
            GameEvents.RaiseTimeUpdate(RemainingTime);
            
            if (RemainingTime <= TIME_ZERO)
                EndGame();
        }
        
        /// <summary>
        /// ゲームを開始する
        /// </summary>
        public void StartGame()
        {
            RemainingTime = gameDuration;
            IsGameRunning = true;
            IsGameEnded = false;

            // InGameBGMを再生
            if (AudioManager.I != null)
                AudioManager.I.PlayBGM(BGMType.InGame, true);
                
            GameEvents.RaiseGameStart();
        }

        /// <summary>
        /// ゲームを終了する
        /// </summary>
        public void EndGame()
        {
            IsGameRunning = false;
            IsGameEnded = true;
            RemainingTime = TIME_ZERO;
            
            GameEvents.RaiseGameEnd();
        }
        
        /// <summary>
        /// ゲームを一時停止する
        /// </summary>
        public void PauseGame()
        {
            IsGameRunning = false;
        }
        
        /// <summary>
        /// ゲームを再開する
        /// </summary>
        public void ResumeGame()
        {
            if (!IsGameEnded)
            {
                IsGameRunning = true;
            }
        }
        
        private void OnNpcInteractionStarted(GameObject npc)
        {
            PauseGame();
        }
        
        private void OnNpcInteractionEnded()
        {
            ResumeGame();
        }
        
        /// <summary>
        /// ボタンクリックSEを再生
        /// </summary>
        public void PlayButtonClickSE()
        {
            if (AudioManager.I != null)
                AudioManager.I.PlaySE(SEType.ButtonClick);
        }
        
        #endregion
        
        protected override void OnDestroy()
        {
            base.OnDestroy();
            
            // イベント購読解除
            GameEvents.OnNpcInteractionStarted -= OnNpcInteractionStarted;
            GameEvents.OnNpcInteractionEnded -= OnNpcInteractionEnded;
        }
    }
}