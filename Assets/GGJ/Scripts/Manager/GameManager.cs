using UnityEngine;
using GGJ.Core;
using GGJ.InGame.Audio;
using GGJ.InGame.Events;
using GGJ.Scene;

namespace GGJ.Manager
{
    /// <summary>
    /// ゲーム全体を統括するマネージャー
    /// アプリケーション制御、設定保存、経過時間管理などを管理
    /// シーンを跨いで保持される
    /// </summary>
    public class GameManager : Singleton<GameManager>
    {
        /// <summary>ゲーム開始からの経過時間</summary>
        public float ElapsedTime { get; private set; }
        
        /// <summary>使用したヒント数</summary>
        public int HintUsedCount { get; private set; }
        
        /// <summary>ゲームが実行中かどうか</summary>
        public bool IsGameRunning { get; private set; }
        
        /// <summary>ゲームが終了したかどうか</summary>
        public bool IsGameEnded { get; private set; }

        protected override void Init()
        {
            Application.targetFrameRate = 60;
            Screen.sleepTimeout = SleepTimeout.NeverSleep;
            
            // マウスカーソルを非表示にする（ゲーム全体で）
            Cursor.visible = false;
            Cursor.lockState = CursorLockMode.Locked;
            
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

            ElapsedTime += Time.deltaTime;
            GameEvents.RaiseTimeUpdate(ElapsedTime);
        }
        
        /// <summary>
        /// ゲームを開始する
        /// </summary>
        public void StartGame()
        {
            ElapsedTime = 0f;
            HintUsedCount = 0;
            IsGameRunning = true;
            IsGameEnded = false;

            // InGameBGMを再生
            if (AudioManager.I != null)
                AudioManager.I.PlayBGM(BGMType.InGame, true);
                
            GameEvents.RaiseGameStart();
        }

        /// <summary>
        /// ヒント使用数を増やす
        /// </summary>
        public void IncrementHintCount() => HintUsedCount++;

        /// <summary>
        /// ゲームを終了する
        /// </summary>
        public void EndGame()
        {
            IsGameRunning = false;
            IsGameEnded = true;
        }
        
        /// <summary>
        /// ゲームを一時停止する
        /// </summary>
        private void PauseGame() => IsGameRunning = false;
        
        /// <summary>
        /// ゲームを再開する
        /// </summary>
        private void ResumeGame()
        {
            if (!IsGameEnded)
                IsGameRunning = true;
        }
        
        private void OnNpcInteractionStarted(GameObject npc) => PauseGame();
        
        private void OnNpcInteractionEnded() => ResumeGame();
        
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