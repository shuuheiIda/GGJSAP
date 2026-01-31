using UnityEngine;
using System.Collections.Generic;

namespace GGJ.InGame.NPC
{
    /// <summary>
    /// セリフ・セットを管理するScriptableObject
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
        [Tooltip("聞き込みへの応答（犯人・女性） {COLOR}で色を挿入")]
        [TextArea(2, 5)]
        public List<string> criminalInquiryResponsesFemale = new List<string>();
        
        [Tooltip("聞き込みへの応答（犯人・男性） {COLOR}で色を挿入")]
        [TextArea(2, 5)]
        public List<string> criminalInquiryResponsesMale = new List<string>();
        
        [Tooltip("犯人と言われた時の反応（実際に犯人・女性）")]
        [TextArea(2, 5)]
        public List<string> criminalAccusedResponsesFemale = new List<string>();
        
        [Tooltip("犯人と言われた時の反応（実際に犯人・男性）")]
        [TextArea(2, 5)]
        public List<string> criminalAccusedResponsesMale = new List<string>();
        
        [Tooltip("ヒント（犯人・女性） {COLOR}で色を挿入")]
        [TextArea(2, 5)]
        public List<string> criminalHintsFemale = new List<string>();
        
        [Tooltip("ヒント（犯人・男性） {COLOR}で色を挿入")]
        [TextArea(2, 5)]
        public List<string> criminalHintsMale = new List<string>();
        
        [Header("無実のNPC用のセリフ")]
        [Tooltip("聞き込みへの応答（無実・女性） {COLOR}で色を挿入")]
        [TextArea(2, 5)]
        public List<string> innocentInquiryResponsesFemale = new List<string>();
        
        [Tooltip("聞き込みへの応答（無実・男性） {COLOR}で色を挿入")]
        [TextArea(2, 5)]
        public List<string> innocentInquiryResponsesMale = new List<string>();
        
        [Tooltip("犯人と言われた時の反応（実際は無実・女性）")]
        [TextArea(2, 5)]
        public List<string> innocentAccusedResponsesFemale = new List<string>();
        
        [Tooltip("犯人と言われた時の反応（実際は無実・男性）")]
        [TextArea(2, 5)]
        public List<string> innocentAccusedResponsesMale = new List<string>();
        
        [Tooltip("ヒント（無実・女性） {COLOR}で色を挿入")]
        [TextArea(2, 5)]
        public List<string> innocentHintsFemale = new List<string>();
        
        [Tooltip("ヒント（無実・男性） {COLOR}で色を挿入")]
        [TextArea(2, 5)]
        public List<string> innocentHintsMale = new List<string>();
        
        /// <summary>
        /// セリフを取得（背景情報で挿入）
        /// </summary>
        public string GetDialogue(bool isCriminal, bool hasReceivedHint, bool isAccused, int index, NpcAppearance criminalAppearance, Gender npcGender)
        {
            List<string> targetList;
            
            if (isCriminal)
            {
                if (isAccused)
                    targetList = npcGender == Gender.Woman ? criminalAccusedResponsesFemale : criminalAccusedResponsesMale;
                else if (hasReceivedHint)
                    targetList = npcGender == Gender.Woman ? criminalHintsFemale : criminalHintsMale;
                else
                    targetList = npcGender == Gender.Woman ? criminalInquiryResponsesFemale : criminalInquiryResponsesMale;
            }
            else
            {
                if (isAccused)
                    targetList = npcGender == Gender.Woman ? innocentAccusedResponsesFemale : innocentAccusedResponsesMale;
                else if (hasReceivedHint)
                    targetList = npcGender == Gender.Woman ? innocentHintsFemale : innocentHintsMale;
                else
                    targetList = npcGender == Gender.Woman ? innocentInquiryResponsesFemale : innocentInquiryResponsesMale;
            }
            
            if (targetList == null || targetList.Count == 0)
                return "...";
            
            if (index < 0 || index >= targetList.Count)
                index = Random.Range(0, targetList.Count);
            
            string dialogue = targetList[index];
            
            if (criminalAppearance != null)
                dialogue = ReplacePlaceholders(dialogue, criminalAppearance);
            
            return dialogue;
        }
        
        /// <summary>
        /// プレースホルダーを実際の色情報で挿入
        /// </summary>
        private string ReplacePlaceholders(string text, NpcAppearance appearance)
        {
            text = text.Replace("{CLOTHES_COLOR}", ColorToJapanese(appearance.clothesColor));
            text = text.Replace("{MASK_COLOR}", ColorToJapanese(appearance.maskColor));
            text = text.Replace("{HAIR_COLOR}", ColorToJapanese(appearance.hairColor));
            text = text.Replace("{HAT_COLOR}", ColorToJapanese(appearance.hatColor));
            text = text.Replace("{SHOE_COLOR}", ColorToJapanese(appearance.shoeColor));
            text = text.Replace("{COLOR}", ColorToJapanese(appearance.clothesColor));
            
            return text;
        }
        
        /// <summary>
        /// Colorを日本語の色名に変換（主要6色のみ）
        /// </summary>
        private string ColorToJapanese(NpcColor color)
        {
            return color switch
            {
                NpcColor.Red => "赤色",
                NpcColor.Blue => "青色",
                NpcColor.Green => "緑色",
                NpcColor.Yellow => "黄色",
                NpcColor.White => "白色",
                NpcColor.Black => "黒色",
                _ => "不明な色"
            };
        }
    }
}