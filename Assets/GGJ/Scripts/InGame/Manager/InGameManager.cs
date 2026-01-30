using System;
using UnityEngine;
using GGJ.Core;

namespace GGJ.InGame.Manager
{
    /// <summary>
    /// ゲーム内を管理するクラス
    /// </summary>
    public class InGameManager : Singleton<InGameManager>
    {
        [Header("時間制限設定")]
        [SerializeField] private float gameDuration = 60f; // 制限時間（秒）
        
        public float RemainingTime { get; private set; }
        public bool IsGameRunning { get; private set; }
        
        public event Action OnGameStart;
        public event Action OnGameEnd;
        public event Action<float> OnTimeUpdate;
        
        protected override bool UseDontDestroyOnLoad => false;

        private void Start()
        {
            StartGame();
        }

        private void Update()
        {
            if (IsGameRunning)
            {
                RemainingTime -= Time.deltaTime;
                OnTimeUpdate?.Invoke(RemainingTime);
                
                if (RemainingTime <= 0f)
                {
                    EndGame();
                }
            }
        }

        /// <summary>
        /// ゲームを開始
        /// </summary>
        public void StartGame()
        {
            RemainingTime = gameDuration;
            IsGameRunning = true;
            OnGameStart?.Invoke();
        }

        /// <summary>
        /// ゲームを終了
        /// </summary>
        private void EndGame()
        {
            IsGameRunning = false;
            RemainingTime = 0f;
            OnGameEnd?.Invoke();
        }
    }
}
