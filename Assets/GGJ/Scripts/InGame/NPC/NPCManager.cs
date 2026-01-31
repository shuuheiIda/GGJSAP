using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using GGJ.Core;
using GGJ.InGame.Events;

namespace GGJ.InGame.NPC
{
    /// <summary>
    /// シーン上の全Npcを管理するマネージャー
    /// </summary>
    public class NpcManager : Singleton<NpcManager>
    {
        private const float ColorToleranceThreshold = 0.1f;
        private const float ColorDistanceThreshold = 0.3f;
        
        [Header("Npc管理")]
        [SerializeField] private List<INpc> allNpcs = new List<INpc>();
        
        [Header("セリフデータ")]
        [SerializeField] private DialogueData dialogueData;
        
        protected override bool UseDontDestroyOnLoad => false;
        
        protected override void Init()
        {
            allNpcs.Clear();
            
            // ヒント取得イベントを購読
            GameEvents.OnHintReceived += OnHintReceived;
            
            // ゲーム開始イベントを購読
            GameEvents.OnGameStart += OnGameStart;
        }
        
        protected override void OnDestroy()
        {
            base.OnDestroy();
            GameEvents.OnHintReceived -= OnHintReceived;
            GameEvents.OnGameStart -= OnGameStart;
        }
        
        /// <summary>
        /// 繧ｲ繝ｼ繝髢句ｧ区凾縺ｮ蜃ｦ逅・
        /// </summary>
        private void OnGameStart() => RandomizeCriminal();
        
        /// <summary>
        /// 繝偵Φ繝亥叙蠕玲凾縺ｮ蜃ｦ逅・
        /// </summary>
        private void OnHintReceived() => SetAllHintsReceived(true);
        
        /// <summary>
        /// ランダムにNpcを1体選んで犯人にする
        /// </summary>
        public void RandomizeCriminal()
        {
            if (allNpcs.Count == 0)
                return;
            
            // 全Npcの犯人フラグをリセット
            foreach (var npc in allNpcs)
            {
                npc.SetCriminal(false);
            }
            
            // 繝ｩ繝ｳ繝繝縺ｫ1菴薙ｒ驕ｸ繧薙〒迥ｯ莠ｺ縺ｫ縺吶ｋ
            int randomIndex = Random.Range(0, allNpcs.Count);
            allNpcs[randomIndex].SetCriminal(true);
        }
        
        /// <summary>
        /// Npcを登録（Npc側から呼ばれる）
        /// </summary>
        public void RegisterNpc(INpc npc)
        {
            if (npc == null) return;
            if (allNpcs.Contains(npc)) return;
            
            allNpcs.Add(npc);
            
            if (GetCriminal() == null && allNpcs.Count > 0)
                RandomizeCriminal();
        }
        
        /// <summary>
        /// 全Npcを取得
        /// </summary>
        public List<INpc> GetAllNpcs() => new List<INpc>(allNpcs);
        
        /// <summary>
        /// Npcのセリフを取得
        /// </summary>
        public string GetDialogueForNpc(INpc npc)
        {
            if (dialogueData == null)
            {
                Debug.LogError("[NpcManager] DialogueData縺瑚ｨｭ螳壹＆繧後※縺・∪縺帙ｓ・！nspector縺ｧDialogueData繧貞牡繧雁ｽ薙※縺ｦ縺上□縺輔＞");
                return "...";
            }
            
            bool isCriminal = npc.IsCriminal();
            bool hasHint = npc.HasReceivedHint();
            bool isAccused = npc.IsAccused();
            
            // Npcのインデックスを使ってバリエーションを提供
            int npcIndex = allNpcs.IndexOf(npc);
            
            var criminal = GetCriminal();
            if (criminal == null)
                Debug.LogError("[NpcManager] 犯人が設定されていません！GameEvents.RaiseGameStart()を呼んでください");
            
            NpcAppearance criminalAppearance = criminal?.GetNpcData()?.appearance;
            Gender npcGender = npc.GetNpcData()?.appearance.gender ?? Gender.Female;
            
            return dialogueData.GetDialogue(isCriminal, hasHint, isAccused, npcIndex, criminalAppearance, npcGender);
        }
        
        /// <summary>
        /// 犯人のNpcを取得
        /// </summary>
        public INpc GetCriminal() => allNpcs.FirstOrDefault(npc => npc.IsCriminal());
        
        /// <summary>
        /// 外見の特徴に一致するNpcリストを取得
        /// </summary>
        public List<INpc> GetNpcsByAppearance(NpcAppearance targetAppearance)
        {
            return allNpcs.Where(npc =>
            {
                var data = npc.GetNpcData();
                if (data == null) return false;
                return DoesAppearanceMatch(data.appearance, targetAppearance);
            }).ToList();
        }
        
        /// <summary>
        /// 性別で絞り込み
        /// </summary>
        public List<INpc> GetNpcsByGender(Gender gender)
        {
            return allNpcs.Where(npc =>
            {
                var data = npc.GetNpcData();
                return data != null && data.appearance.gender == gender;
            }).ToList();
        }
        
        /// <summary>
        /// 方向で絞り込み
        /// </summary>
        public List<INpc> GetNpcsByDirection(Direction direction)
        {
            return allNpcs.Where(npc =>
            {
                var data = npc.GetNpcData();
                return data != null && data.appearance.positionFromCenter == direction;
            }).ToList();
        }
        
        /// <summary>
        /// 服の色で絞り込み
        /// </summary>
        public List<INpc> GetNpcsByClothesColor(Color color, float tolerance = ColorToleranceThreshold) =>
            allNpcs.Where(npc =>
            {
                var data = npc.GetNpcData();
                if (data == null) return false;
                return ColorDistance(data.appearance.clothesColor, color) < tolerance;
            }).ToList();
        
        /// <summary>
        /// 全Npcにヒント受信フラグを設定
        /// </summary>
        public void SetAllHintsReceived(bool received)
        {
            foreach (var npc in allNpcs)
            {
                npc.SetHintReceived(received);
            }
        }
        
        /// <summary>
        /// 外見が一致するかチェック（ヒントベース）
        /// </summary>
        private bool DoesAppearanceMatch(NpcAppearance npcAppearance, NpcAppearance targetAppearance)
        {
            bool genderMatch = npcAppearance.gender == targetAppearance.gender;
            bool directionMatch = npcAppearance.positionFromCenter == targetAppearance.positionFromCenter;
            bool clothesMatch = ColorDistance(npcAppearance.clothesColor, targetAppearance.clothesColor) < ColorToleranceThreshold;
            bool maskMatch = ColorDistance(npcAppearance.maskColor, targetAppearance.maskColor) < ColorToleranceThreshold;
            bool hairMatch = ColorDistance(npcAppearance.hairColor, targetAppearance.hairColor) < ColorToleranceThreshold;
            bool hatMatch = ColorDistance(npcAppearance.hatColor, targetAppearance.hatColor) < ColorToleranceThreshold;
            bool shoeMatch = ColorDistance(npcAppearance.shoeColor, targetAppearance.shoeColor) < ColorToleranceThreshold;
            
            return genderMatch && directionMatch && clothesMatch && 
                   maskMatch && hairMatch && hatMatch && shoeMatch;
        }
        
        /// <summary>
        /// 色の距離を計算
        /// </summary>
        private float ColorDistance(Color a, Color b)
        {
            float dr = a.r - b.r;
            float dg = a.g - b.g;
            float db = a.b - b.b;
            return Mathf.Sqrt(dr * dr + dg * dg + db * db);
        }
    }
}
