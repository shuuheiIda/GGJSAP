using Cysharp.Threading.Tasks;
using GGJ.InGame.NPC;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using TMPro;
using UnityEngine;
using System.Linq;
using UnityEngine.Scripting;
using System.Threading.Tasks;

namespace GGJ {
    public class OpeningManager : MonoBehaviour {
        /// <summary>
        /// オープニングテキスト
        /// </summary>
        [SerializeField] private TextMeshProUGUI openingText;

 
        [SerializeField] private GameObject InGameObj;

        /// <summary>
        /// ダイアログデータSO
        /// </summary>
        [SerializeField] private DialogueData dialogueDataSO;

        /// <summary>
        /// 文字表示用cts
        /// </summary>
        private CancellationTokenSource cts = new CancellationTokenSource();

        /// <summary>
        /// 文字表示間隔
        /// </summary>
        private const float WaitTime = 0.1f;

        private void Start() {
            ShowOpening();
        }

        /// <summary>
        /// オープニングを表示
        /// </summary>
        public async void ShowOpening() {
            try {
                await oneCharByDisplay(dialogueDataSO.openingStory, WaitTime, cts.Token);
            }
            catch {
                throw;
            }
        }

        /// <summary>
        /// 一文字ずつ表示
        /// </summary>
        private async UniTask oneCharByDisplay(string text, float waitTime, CancellationToken ct) {
            string[] charAraray = text.Select(_=>_.ToString()).ToArray();
            openingText.text = "";

            foreach (var chara in charAraray) {
                openingText.text += chara; 
                await UniTask.Delay(TimeSpan.FromSeconds(waitTime), cancellationToken: ct);
            }   

            InGameObj.SetActive(true);
            openingText.gameObject.SetActive(false);
        }
    }
}
