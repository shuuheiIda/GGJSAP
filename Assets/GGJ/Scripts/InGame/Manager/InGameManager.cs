using System;
using UnityEngine;
using GGJ.Core;

namespace GGJ.InGame.Manager
{
    /// <summary>
    /// ゲーム全体の進行と時間制限を管理するマネージャー
    /// </summary>
    public class InGameManager : Singleton<InGameManager>
    {
        private const float TIME_ZERO = 0f;

        [Header("時間制限設定")]
        [SerializeField] private float gameDuration = 60f;
        
        public float RemainingTime { get; private set; }
        public bool IsGameRunning { get; private set; }
        
        public event Action OnGameStart;
        public event Action OnGameEnd;
        public event Action<float> OnTimeUpdate;
        
        protected override bool UseDontDestroyOnLoad => false;

        private void Start() => StartGame();

        private void Update()
        {
            if (!IsGameRunning) return;

            RemainingTime -= Time.deltaTime;
            OnTimeUpdate?.Invoke(RemainingTime);
            
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
            OnGameStart?.Invoke();
        }

        /// <summary>
        /// ゲームを終了する
        /// </summary>
        private void EndGame()
        {
            IsGameRunning = false;
            RemainingTime = TIME_ZERO;
            OnGameEnd?.Invoke();
        }
    }
}
