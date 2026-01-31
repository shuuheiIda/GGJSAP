using UnityEngine;
using System.Collections.Generic;

namespace GGJ.InGame.NPC
{
    /// <summary>
    /// セリフのセットを管理するScriptableObject
    /// </summary>
    [CreateAssetMenu(fileName = "DialogueData", menuName = "GGJ/DialogueData", order = 1)]
    public class DialogueData : ScriptableObject
    {
        [Header("ストーリーテキスト")]
        [Tooltip("開始直後のストーリー")]
        [TextArea(3, 10)]
        public string openingStory = "";
        
        [Tooltip("犯人を捕まえた時のストーリー")]
        [TextArea(3, 10)]
        public string victoryStory = "";
        
        [Tooltip("犯人を捕まえられなかった時のストーリー")]
        [TextArea(3, 10)]
        public string defeatStory = "";
        
        [Header("犯人用のセリフ")]
        [Tooltip("聞き込みへの返答（犯人）- {COLOR}で色を置換")]
        [TextArea(2, 5)]
        public List<string> criminalInquiryResponses = new List<string>();
        
        [Tooltip("犯人と言われた時の反応（実際に犯人）")]
        [TextArea(2, 5)]
        public List<string> criminalAccusedResponses = new List<string>();
        
        [Tooltip("ヒント（犯人）- {COLOR}で色を置換")]
        [TextArea(2, 5)]
        public List<string> criminalHints = new List<string>();
        
        [Header("無実のNPC用のセリフ")]
        [Tooltip("聞き込みへの返答（無実）- {COLOR}で色を置換")]
        [TextArea(2, 5)]
        public List<string> innocentInquiryResponses = new List<string>();
        
        [Tooltip("犯人と言われた時の反応（実際は無実）")]
        [TextArea(2, 5)]
        public List<string> innocentAccusedResponses = new List<string>();
        
        [Tooltip("ヒント（無実）- {COLOR}で色を置換")]
        [TextArea(2, 5)]
        public List<string> innocentHints = new List<string>();
        
        /// <summary>
        /// セリフを取得（色情報で置換）
        /// </summary>
        public string GetDialogue(bool isCriminal, bool hasReceivedHint, int index, NPCAppearance criminalAppearance)
        {
            List<string> targetList;
            
            if (isCriminal)
            {
                // 犯人の場合
                if (hasReceivedHint)
                    targetList = criminalAccusedResponses; // ヒント後は犯人として問い詰められる
                else
                    targetList = criminalInquiryResponses; // ヒント前は通常の聞き込み
            }
            else
            {
                // 無実の場合
                if (hasReceivedHint)
                    targetList = innocentHints; // ヒント後はヒントを提供
                else
                    targetList = innocentInquiryResponses; // ヒント前は通常の聞き込み
            }
            
            if (targetList == null || targetList.Count == 0)
                return "...";
            
            // インデックスが範囲外の場合はランダムに選択
            if (index < 0 || index >= targetList.Count)
                index = Random.Range(0, targetList.Count);
            
            string dialogue = targetList[index];
            
            // 色情報で置換
            if (criminalAppearance != null)
            {
                dialogue = ReplacePlaceholders(dialogue, criminalAppearance);
            }
            
            return dialogue;
        }
        
        /// <summary>
        /// プレースホルダーを実際の色情報で置換
        /// </summary>
        private string ReplacePlaceholders(string text, NPCAppearance appearance)
        {
            // {CLOTHES_COLOR} → 服の色
            text = text.Replace("{CLOTHES_COLOR}", ColorToJapanese(appearance.clothesColor));
            // {MASK_COLOR} → マスクの色
            text = text.Replace("{MASK_COLOR}", ColorToJapanese(appearance.maskColor));
            // {HAIR_COLOR} → 髪の色
            text = text.Replace("{HAIR_COLOR}", ColorToJapanese(appearance.hairColor));
            // {HAT_COLOR} → 帽子の色
            text = text.Replace("{HAT_COLOR}", ColorToJapanese(appearance.hatColor));
            // {SHOE_COLOR} → 靴の色
            text = text.Replace("{SHOE_COLOR}", ColorToJapanese(appearance.shoeColor));
            
            // 汎用的な {COLOR} も服の色として扱う
            text = text.Replace("{COLOR}", ColorToJapanese(appearance.clothesColor));
            
            return text;
        }
        
        /// <summary>
        /// Colorを日本語の色名に変換
        /// </summary>
        private string ColorToJapanese(Color color)
        {
            // 色の判定（簡易版）
            if (IsColorClose(color, Color.red)) return "赤色";
            if (IsColorClose(color, Color.blue)) return "青色";
            if (IsColorClose(color, Color.green)) return "緑色";
            if (IsColorClose(color, Color.yellow)) return "黄色";
            if (IsColorClose(color, Color.white)) return "白色";
            if (IsColorClose(color, Color.black)) return "黒色";
            if (IsColorClose(color, new Color(1f, 0.5f, 0f))) return "オレンジ色";
            if (IsColorClose(color, new Color(0.5f, 0f, 0.5f))) return "紫色";
            if (IsColorClose(color, new Color(1f, 0.75f, 0.8f))) return "ピンク色";
            if (IsColorClose(color, new Color(0.5f, 0.5f, 0.5f))) return "グレー";
            if (IsColorClose(color, new Color(0.65f, 0.16f, 0.16f))) return "茶色";
            
            return "不明な色";
        }
        
        /// <summary>
        /// 色が近いかチェック
        /// </summary>
        private bool IsColorClose(Color a, Color b, float threshold = 0.3f)
        {
            float distance = Mathf.Sqrt(
                Mathf.Pow(a.r - b.r, 2) +
                Mathf.Pow(a.g - b.g, 2) +
                Mathf.Pow(a.b - b.b, 2)
            );
            return distance < threshold;
        }
    }
}
