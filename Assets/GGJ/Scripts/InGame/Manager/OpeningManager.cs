using Cysharp.Threading.Tasks;
using GGJ.InGame.NPC;
using System;
using System.Threading;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using System.Linq;

namespace GGJ
{
    public class OpeningManager : MonoBehaviour
    {
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

        /// <summary>
        /// 全文表示後の待機時間
        /// </summary>
        private const float ReadWaitTime = 0.4f;

        private void Start()
        {
            if (InGameObj != null)
                InGameObj.SetActive(false);
            if (openingText != null)
                openingText.gameObject.SetActive(true);

            ShowOpening();
        }

        /// <summary>
        /// オープニングを表示
        /// </summary>
        public async void ShowOpening()
        {
            try
            {
                await oneCharByDisplay(dialogueDataSO.openingStory, WaitTime, cts.Token);
            }
            catch
            {
                throw;
            }
        }

        /// <summary>
        /// 一文字ずつ表示
        /// </summary>
        private async UniTask oneCharByDisplay(string text, float waitTime, CancellationToken ct)
        {
            string[] charAraray = text.Select(_ => _.ToString()).ToArray();
            openingText.text = "";
            bool skipped = false;

            foreach (var chara in charAraray)
            {
                openingText.text += chara;
                
                // 文字表示後、waitTime秒待機（その間にスキップ判定）
                float elapsed = 0f;
                while (elapsed < waitTime)
                {
                    // スキップ判定（キーボードまたはゲームパッドのボタン入力）
                    bool keyPressed = Keyboard.current != null && Keyboard.current.anyKey.wasPressedThisFrame;
                    bool buttonPressed = Gamepad.current != null && 
                        (Gamepad.current.buttonSouth.wasPressedThisFrame ||
                         Gamepad.current.buttonEast.wasPressedThisFrame ||
                         Gamepad.current.buttonWest.wasPressedThisFrame ||
                         Gamepad.current.buttonNorth.wasPressedThisFrame);
                    
                    if (keyPressed || buttonPressed)
                    {
                        skipped = true;
                        break;
                    }
                    
                    await UniTask.Yield(cancellationToken: ct);
                    elapsed += Time.deltaTime;
                }
                
                if (skipped) break;
            }

            // スキップされた場合は全文表示
            if (skipped)
            {
                openingText.text = text;
            }

            // 全文表示後、待機してからInGameを表示
            await UniTask.Delay(TimeSpan.FromSeconds(ReadWaitTime), cancellationToken: ct);

            InGameObj.SetActive(true);
            openingText.gameObject.SetActive(false);
        }
    }
}
