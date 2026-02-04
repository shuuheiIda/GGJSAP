using UnityEngine;
using TMPro;
using GGJ.Core;
using GGJ.InGame.UI;

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
        [SerializeField] private TextMeshProUGUI clearTimeText; // 経過時間テキスト
        [SerializeField] private TextMeshProUGUI hintUsedText; // ヒント使用数テキスト
        
        [Header("ゲーム統計")]
        [SerializeField] private int clearTime = 0; // クリア時の経過時間（秒）
        [SerializeField] private int hintUsedCount = 0; // 使用したヒント数
        
        [Header("タイプライター設定")]
        [SerializeField] private float typeSpeed = 0.05f; // 1文字あたりの表示速度（秒）
        
        [Header("コントローラーナビゲーション")]
        [SerializeField] private GameObject firstSelectedObject; // シーン開始時に最初に選択されるUI要素（例：タイトルに戻るボタン）

        private void Start()
        {
            DisplayGameStats();
            StartCoroutine(PlayStoryTextAnimation());
            SetInitialSelection();
        }
        

        /// <summary>
        /// ゲーム統計情報を表示
        /// </summary>
        private void DisplayGameStats()
        {
            if (clearTimeText != null)
                clearTimeText.text = $"ResultTime: {clearTime}";
            
            if (hintUsedText != null)
                hintUsedText.text = $"UseHint: {hintUsedCount}";
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
