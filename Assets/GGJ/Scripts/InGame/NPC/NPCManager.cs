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
        [Header("Npc管理")]
        [Tooltip("管理するNPCをここにアタッチしてください")]
        [SerializeField] private List<NpcController> npcReferences = new List<NpcController>();
        
        [Tooltip("実行時に管理されている全NPC")]
        [SerializeField] private List<INpc> allNpcs = new List<INpc>();
        
        [Header("セリフデータ")]
        [SerializeField] private DialogueData dialogueData;
        
        protected override bool UseDontDestroyOnLoad => false;
        
        protected override void Init()
        {
            allNpcs.Clear();
            
            // アタッチされたNPCを登録
            foreach (var npc in npcReferences)
            {
                if (npc != null)
                    RegisterNpc(npc);
            }
            
            // ヒント取得イベントを購読
            GameEvents.OnHintReceived += OnHintReceived;
            
            // ゲーム開始イベントを購読
            GameEvents.OnGameStart += OnGameStart;
        }
        
        private void Start()
        {
            // 全NPCが登録されるのを待ってから犯人を決定
            // (NpcControllerのStartはこの後に実行される)
            Invoke(nameof(InitializeCriminalIfNeeded), 0.1f);
        }
        
        private void InitializeCriminalIfNeeded()
        {
            if (GetCriminal() == null && allNpcs.Count > 0)
            {
                Debug.Log($"[NpcManager] 全NPCが登録されました。犯人を決定します (合計: {allNpcs.Count}体)");
                RandomizeCriminal();
            }
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
            {
                Debug.LogWarning("[NpcManager] NPCが登録されていないため、犯人を設定できません");
                return;
            }
            
            // 全Npcの犯人フラグをリセット
            foreach (var npc in allNpcs)
            {
                npc.SetCriminal(false);
            }
            
            // 繝ｩ繝ｳ繝繝縺ｫ1菴薙ｒ驕ｸ繧薙〒迥ｯ莠ｺ縺ｫ縺吶ｋ
            int randomIndex = Random.Range(0, allNpcs.Count);
            allNpcs[randomIndex].SetCriminal(true);
            
            // デバッグログ
            var criminalData = allNpcs[randomIndex].GetNpcData();
            string criminalName = criminalData != null ? criminalData.npcName : "不明";
            Debug.Log($"[NpcManager] 犯人が決定しました: {criminalName} (Index: {randomIndex}, 合計NPC数: {allNpcs.Count})");
        }
        
        /// <summary>
        /// Npcを登録
        /// </summary>
        public void RegisterNpc(INpc npc)
        {
            if (npc == null) return;
            if (allNpcs.Contains(npc)) return;
            
            allNpcs.Add(npc);
            Debug.Log($"[NpcManager] NPCを登録しました: {npc.GetNpcData()?.npcName ?? "不明"} (合計: {allNpcs.Count}体)");
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
            
            // 性別ごとにインデックスを計算（同性のNPC間で一意にする）
            Gender npcGender = npc.GetNpcData()?.appearance.gender ?? Gender.Woman;
            int genderSpecificIndex = 0;
            for (int i = 0; i < npcIndex; i++)
            {
                if (allNpcs[i].GetNpcData()?.appearance.gender == npcGender)
                    genderSpecificIndex++;
            }
            
            var criminal = GetCriminal();
            if (criminal == null)
                Debug.LogError("[NpcManager] 犯人が設定されていません！GameEvents.RaiseGameStart()を呼んでください");
            
            NpcAppearance criminalAppearance = criminal?.GetNpcData()?.appearance;
            
            return dialogueData.GetDialogue(isCriminal, hasHint, isAccused, genderSpecificIndex, criminalAppearance, npcGender);
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
        public List<INpc> GetNpcsByClothesColor(NpcColor color)
        {
            return allNpcs.Where(npc =>
            {
                var data = npc.GetNpcData();
                return data != null && data.appearance.clothesColor == color;
            }).ToList();
        }
        
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
            bool clothesMatch = npcAppearance.clothesColor == targetAppearance.clothesColor;
            bool maskMatch = npcAppearance.maskColor == targetAppearance.maskColor;
            bool hairMatch = npcAppearance.hairColor == targetAppearance.hairColor;
            bool hatMatch = npcAppearance.hatColor == targetAppearance.hatColor;
            bool shoeMatch = npcAppearance.shoeColor == targetAppearance.shoeColor;
            
            return genderMatch && directionMatch && clothesMatch && 
                   maskMatch && hairMatch && hatMatch && shoeMatch;
        }
    }
}
