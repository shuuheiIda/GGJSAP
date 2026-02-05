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
        [Tooltip("【重要】ヒント後は具体的な情報を提供してください\n使用可能なプレースホルダー：\n{CLOTHES_COLOR}=服の色\n{MASK_COLOR}=マスクの色\n{HAIR_COLOR}=髪の色\n{HAT_COLOR}=帽子の色\n{SHOE_COLOR}=靴の色\n{GENDER}=性別（男性/女性）\n{LOCATION}=場所（上/下/左/右）\n例：「犯人は{LOCATION}にいて{CLOTHES_COLOR}の服を着た{GENDER}でした」")]
        public List<string> criminalHintsFemale = new List<string>();
        
        [Space(5)]
        [Tooltip("【重要】ヒント後は具体的な情報を提供してください\n使用可能なプレースホルダー：\n{CLOTHES_COLOR}=服の色\n{MASK_COLOR}=マスクの色\n{HAIR_COLOR}=髪の色\n{HAT_COLOR}=帽子の色\n{SHOE_COLOR}=靴の色\n{GENDER}=性別（男性/女性）\n{LOCATION}=場所（上/下/左/右）\n例：「犯人は{LOCATION}にいて{MASK_COLOR}のマスクをしていたぜ」")]
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
        [Tooltip("【重要】ヒント後は具体的な情報を提供してください\n無実のNPCは虚偽の情報を言うこともあります\n使用可能なプレースホルダー：\n{CLOTHES_COLOR}=服の色\n{MASK_COLOR}=マスクの色\n{HAIR_COLOR}=髪の色\n{HAT_COLOR}=帽子の色\n{SHOE_COLOR}=靴の色\n{GENDER}=性別（男性/女性）\n{LOCATION}=場所（上/下/左/右）")]
        public List<string> innocentHintsFemale = new List<string>();
        
        [Space(5)]
        [Tooltip("【重要】ヒント後は具体的な情報を提供してください\n無実のNPCは虚偽の情報を言うこともあります\n使用可能なプレースホルダー：\n{CLOTHES_COLOR}=服の色\n{MASK_COLOR}=マスクの色\n{HAIR_COLOR}=髪の色\n{HAT_COLOR}=帽子の色\n{SHOE_COLOR}=靴の色\n{GENDER}=性別（男性/女性）\n{LOCATION}=場所（上/下/左/右）")]
        public List<string> innocentHintsMale = new List<string>();
        
        /// <summary>
        /// セリフを取得（背景情報で挿入）
        /// </summary>
        /// <param name="isCriminal">犯人かどうか</param>
        /// <param name="hasReceivedHint">ヒントを受け取ったかどうか</param>
        /// <param name="isAccused">告発されたかどうか</param>
        /// <param name="dialogueIndex">セリフ前のindex（話しかける前のセリフ用）</param>
        /// <param name="hintIndex">ヒント用のindex（-1の場合は新しく生成）</param>
        /// <param name="criminalAppearance">犯人の外見</param>
        /// <param name="npcGender">NPCの性別</param>
        /// <param name="actualHintIndex">実際に使用されたヒントのindex（out）</param>
        public string GetDialogue(bool isCriminal, bool hasReceivedHint, bool isAccused, int dialogueIndex, int hintIndex, NpcAppearance criminalAppearance, Gender npcGender, out int actualHintIndex)
        {
            List<string> targetList;
            bool isHintDialogue = false; // ヒント用のセリフかどうか
            
            if (isCriminal)
            {
                if (isAccused)
                    targetList = npcGender == Gender.Woman ? criminalAccusedResponsesFemale : criminalAccusedResponsesMale;
                else if (hasReceivedHint)
                {
                    targetList = npcGender == Gender.Woman ? criminalHintsFemale : criminalHintsMale;
                    isHintDialogue = true;
                }
                else
                    targetList = npcGender == Gender.Woman ? criminalInquiryResponsesFemale : criminalInquiryResponsesMale;
            }
            else
            {
                if (isAccused)
                    targetList = npcGender == Gender.Woman ? innocentAccusedResponsesFemale : innocentAccusedResponsesMale;
                else if (hasReceivedHint)
                {
                    targetList = npcGender == Gender.Woman ? innocentHintsFemale : innocentHintsMale;
                    isHintDialogue = true;
                }
                else
                    targetList = npcGender == Gender.Woman ? innocentInquiryResponsesFemale : innocentInquiryResponsesMale;
            }
            
            if (targetList == null || targetList.Count == 0)
            {
                actualHintIndex = -1;
                return GetDefaultDialogue(isCriminal, isAccused, hasReceivedHint);
            }
            
            int selectedIndex;
            if (isHintDialogue)
            {
                // ヒント用のセリフの場合
                if (hintIndex >= 0 && hintIndex < targetList.Count)
                {
                    // すでにヒントindexが決まっている場合はそれを使用
                    selectedIndex = hintIndex;
                }
                else
                {
                    // 新しくランダムで決定
                    selectedIndex = Random.Range(0, targetList.Count);
                }
                actualHintIndex = selectedIndex;
            }
            else
            {
                // ヒント前のセリフの場合は従来通りdialogueIndexを使用
                selectedIndex = dialogueIndex;
                if (selectedIndex < 0 || selectedIndex >= targetList.Count)
                    selectedIndex = Random.Range(0, targetList.Count);
                actualHintIndex = -1; // ヒント用ではないので-1
            }
            
            string dialogue = targetList[selectedIndex];
            
            if (criminalAppearance != null)
                dialogue = ReplacePlaceholders(dialogue, criminalAppearance);
            
            return dialogue;
        }
        
        private string ReplacePlaceholders(string text, NpcAppearance appearance)
        {
            text = text.Replace("{CLOTHES_COLOR}", ColorToJapanese(appearance.clothesColor));
            text = text.Replace("{MASK_COLOR}", ColorToJapanese(appearance.maskColor));
            text = text.Replace("{HAIR_COLOR}", ColorToJapanese(appearance.hairColor));
            text = text.Replace("{HAT_COLOR}", ColorToJapanese(appearance.hatColor));
            text = text.Replace("{SHOE_COLOR}", ColorToJapanese(appearance.shoeColor));
            text = text.Replace("{COLOR}", ColorToJapanese(appearance.clothesColor));
            text = text.Replace("{GENDER}", GenderToJapanese(appearance.gender));
            text = text.Replace("{LOCATION}", LocationToJapanese(appearance.positionFromCenter));
            
            return text;
        }
        
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
        
        private string GenderToJapanese(Gender gender)
        {
            return gender switch
            {
                Gender.Man => "男性",
                Gender.Woman => "女性",
                _ => "不明な性別"
            };
        }
        
        private string LocationToJapanese(Direction direction)
        {
            return direction switch
            {
                Direction.Up => "上",
                Direction.Down => "下",
                Direction.Left => "左",
                Direction.Right => "右",
                _ => "不明な場所"
            };
        }
        
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