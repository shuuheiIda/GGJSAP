using UnityEngine;
using System.Collections.Generic;

namespace GGJ.InGame.NPC
{
    /// <summary>
    /// セリフ・セットを管理するScriptableObject
    /// レベルデザイン担当者向け：ゲーム内で表示される全てのセリフを管理します
    /// </summary>
    [CreateAssetMenu(fileName = "DialogueData", menuName = "GGJ/DialogueData", order = 1)]
    public class DialogueData : ScriptableObject
    {
        [Header("■ ストーリーテキスト")]
        [Space(5)]
        [Tooltip("ゲーム開始時に表示されるストーリー文章")]
        [TextArea(3, 10)]
        public string openingStory = "";
        
        [Space(5)]
        [Tooltip("犯人を正しく告発した時のエンディング文章")]
        [TextArea(3, 10)]
        public string victoryStory = "";
        
        [Space(5)]
        [Tooltip("犯人を見つけられなかった時のバッドエンド文章")]
        [TextArea(3, 10)]
        public string defeatStory = "";
        
        [Header("■ 犯人用のセリフ（ヒント前：聞き込み時）")]
        [Space(5)]
        [Tooltip("【重要】ヒント前は曖昧な返答にしてください（例：よく覚えていません）\n女性7人分のセリフを用意してください。各NPCは異なるセリフを使用します。")]
        public List<string> criminalInquiryResponsesFemale = new List<string>();
        
        [Space(5)]
        [Tooltip("【重要】ヒント前は曖昧な返答にしてください（例：よく覚えていません）\n男性7人分のセリフを用意してください。各NPCは異なるセリフを使用します。")]
        public List<string> criminalInquiryResponsesMale = new List<string>();
        
        [Header("■ 犯人用のセリフ（告発時：犯人が正解の場合）")]
        [Space(5)]
        [Tooltip("犯人NPCに「あなたが犯人ですね？」と告発した時の反応（女性）\n観念する、白状する、驚くなどのセリフ")]
        public List<string> criminalAccusedResponsesFemale = new List<string>();
        
        [Space(5)]
        [Tooltip("犯人NPCに「あなたが犯人ですね？」と告発した時の反応（男性）\n観念する、白状する、驚くなどのセリフ")]
        public List<string> criminalAccusedResponsesMale = new List<string>();
        
        [Header("■ 犯人用のセリフ（ヒント後：詳細情報）")]
        [Space(5)]
        [Tooltip("【重要】ヒント後は具体的な情報を提供してください\n使用可能なプレースホルダー：\n{CLOTHES_COLOR}=服の色\n{MASK_COLOR}=マスクの色\n{HAIR_COLOR}=髪の色\n{HAT_COLOR}=帽子の色\n{SHOE_COLOR}=靴の色\n例：「犯人は{CLOTHES_COLOR}の服を着ていました」")]
        public List<string> criminalHintsFemale = new List<string>();
        
        [Space(5)]
        [Tooltip("【重要】ヒント後は具体的な情報を提供してください\n使用可能なプレースホルダー：\n{CLOTHES_COLOR}=服の色\n{MASK_COLOR}=マスクの色\n{HAIR_COLOR}=髪の色\n{HAT_COLOR}=帽子の色\n{SHOE_COLOR}=靴の色\n例：「犯人は{MASK_COLOR}のマスクをしていたぜ」")]
        public List<string> criminalHintsMale = new List<string>();
        
        [Header("■ 無実のNPC用のセリフ（ヒント前：聞き込み時）")]
        [Space(5)]
        [Tooltip("【重要】ヒント前は曖昧な返答にしてください（例：何も知りません）\n女性7人分のセリフを用意してください。各NPCは異なるセリフを使用します。")]
        public List<string> innocentInquiryResponsesFemale = new List<string>();
        
        [Space(5)]
        [Tooltip("【重要】ヒント前は曖昧な返答にしてください（例：何も知りません）\n男性6人分のセリフを用意してください。各NPCは異なるセリフを使用します。")]
        public List<string> innocentInquiryResponsesMale = new List<string>();
        
        [Header("■ 無実のNPC用のセリフ（告発時：冤罪の場合）")]
        [Space(5)]
        [Tooltip("無実のNPCに「あなたが犯人ですね？」と間違って告発した時の反応（女性）\n怒る、困惑する、否定するなどのセリフ")]
        public List<string> innocentAccusedResponsesFemale = new List<string>();
        
        [Space(5)]
        [Tooltip("無実のNPCに「あなたが犯人ですね？」と間違って告発した時の反応（男性）\n怒る、困惑する、否定するなどのセリフ")]
        public List<string> innocentAccusedResponsesMale = new List<string>();
        
        [Header("■ 無実のNPC用のセリフ（ヒント後：詳細情報）")]
        [Space(5)]
        [Tooltip("【重要】ヒント後は具体的な情報を提供してください\n無実のNPCは虚偽の情報を言うこともあります\n使用可能なプレースホルダー：\n{CLOTHES_COLOR}=服の色\n{MASK_COLOR}=マスクの色\n{HAIR_COLOR}=髪の色\n{HAT_COLOR}=帽子の色\n{SHOE_COLOR}=靴の色")]
        public List<string> innocentHintsFemale = new List<string>();
        
        [Space(5)]
        [Tooltip("【重要】ヒント後は具体的な情報を提供してください\n無実のNPCは虚偽の情報を言うこともあります\n使用可能なプレースホルダー：\n{CLOTHES_COLOR}=服の色\n{MASK_COLOR}=マスクの色\n{HAIR_COLOR}=髪の色\n{HAT_COLOR}=帽子の色\n{SHOE_COLOR}=靴の色")]
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
            {
                string listName = isCriminal ? 
                    (isAccused ? "AccusedResponses" : hasReceivedHint ? "Hints" : "InquiryResponses") :
                    (isAccused ? "AccusedResponses" : hasReceivedHint ? "Hints" : "InquiryResponses");
                string genderName = npcGender == Gender.Woman ? "Female" : "Male";
                string criminalStatus = isCriminal ? "Criminal" : "Innocent";
                Debug.LogError($"[DialogueData] セリフリストが空です: {criminalStatus}{listName}{genderName} (犯人:{isCriminal}, ヒント:{hasReceivedHint}, 告発:{isAccused}, 性別:{genderName})");
                
                // デフォルトのセリフを返す
                return GetDefaultDialogue(isCriminal, isAccused, hasReceivedHint);
            }
            
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
        
        /// <summary>
        /// デフォルトのセリフを返す（リストが空の場合）
        /// </summary>
        private string GetDefaultDialogue(bool isCriminal, bool isAccused, bool hasReceivedHint)
        {
            if (isCriminal)
            {
                if (isAccused) return "な、何を言ってるんだ...！";
                if (hasReceivedHint) return "そんなこと知りませんよ...";
                return "さあ...どうでしょうね。";
            }
            else
            {
                if (isAccused) return "えっ？私が犯人？そんなわけないでしょう！";
                if (hasReceivedHint) return "特に気になることはありませんでしたね。";
                return "私は何も知りませんよ。";
            }
        }
    }
}