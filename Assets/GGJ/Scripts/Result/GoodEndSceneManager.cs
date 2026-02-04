using UnityEngine;
using TMPro;
using GGJ.Core;
using GGJ.InGame.UI;
using GGJ.Manager;

namespace GGJ.Result
{
    /// <summary>
    /// 勝利シーン（GoodEnd）の管理
    /// ゲーム統計情報（経過時間、ヒント使用数）を表示
    /// </summary>
    public class GoodEndSceneManager : MonoBehaviour
    {
        [Header("UI要素")]
        [SerializeField] private TextMeshProUGUI winStoryText; // 勝利ストーリーテキスト
        [SerializeField] private TextMeshProUGUI clearTimeText; // 経過時間テキスト（Inspector で "クリアタイム: {0}秒" のようなフォーマット文字列を設定）
        [SerializeField] private TextMeshProUGUI hintUsedText; // ヒント使用数テキスト（Inspector で "ヒント使用数: {0}回" のようなフォーマット文字列を設定）
        
        [Header("タイプライター設定")]
        [SerializeField] private float typeSpeed = 0.05f; // 1文字あたりの表示速度（秒）
        
        [Header("コントローラーナビゲーション")]
        [SerializeField] private GameObject firstSelectedObject; // シーン開始時に最初に選択されるUI要素（例：タイトルに戻るボタン）

        private string clearTimeFormat; // クリアタイムのフォーマット文字列
        private string hintUsedFormat; // ヒント使用数のフォーマット文字列

        private void Awake()
        {
            // Inspector で設定されたフォーマット文字列を保存してからテキストをクリア
            if (clearTimeText != null)
            {
                clearTimeFormat = clearTimeText.text;
                clearTimeText.text = string.Empty; // 初期表示を非表示にする
            }
            
            if (hintUsedText != null)
            {
                hintUsedFormat = hintUsedText.text;
                hintUsedText.text = string.Empty; // 初期表示を非表示にする
            }
        }

        private void Start()
        {
            StartCoroutine(PlayStoryTextAnimation());
            SetInitialSelection();
        }
        

        /// <summary>
        /// ゲーム統計情報を表示
        /// </summary>
        private System.Collections.IEnumerator DisplayGameStats()
        {
            if (GameManager.I == null)
            {
                Debug.LogWarning("GameManager が見つかりません");
                yield break;
            }
            
            // GameManagerから経過時間とヒント使用数を取得
            float elapsedTime = GameManager.I.ElapsedTime;
            int hintCount = GameManager.I.HintUsedCount;
            
            // クリアタイムをタイプライター効果で表示
            if (clearTimeText != null && !string.IsNullOrEmpty(clearTimeFormat))
            {
                string formattedText = string.Format(clearTimeFormat, Mathf.FloorToInt(elapsedTime));
                yield return TextTypewriterEffect.TypeText(clearTimeText, formattedText, typeSpeed);
            }
            
            // ヒント使用数をタイプライター効果で表示
            if (hintUsedText != null && !string.IsNullOrEmpty(hintUsedFormat))
            {
                string formattedText = string.Format(hintUsedFormat, hintCount);
                yield return TextTypewriterEffect.TypeText(hintUsedText, formattedText, typeSpeed);
            }
        }

        /// <summary>
        /// ストーリーテキストをタイプライター効果で表示
        /// </summary>
        private System.Collections.IEnumerator PlayStoryTextAnimation()
        {
            if (winStoryText != null)
            {
                // winStoryTextから元のテキストを取得（タイプライター効果で上書きされる前に保存）
                string originalText = winStoryText.text;
                
                yield return TextTypewriterEffect.TypeText(
                    winStoryText,
                    originalText,
                    typeSpeed
                );
            }
            
            // テキストアニメーション終了後にゲーム統計を表示
            yield return DisplayGameStats();
        }

        /// <summary>
        /// コントローラー用の初期選択を設定
        /// </summary>
        private void SetInitialSelection()
        {
            if (firstSelectedObject != null)
            {
                UIHelper.SetFirstSelected(firstSelectedObject);
            }
        }
    }
}
