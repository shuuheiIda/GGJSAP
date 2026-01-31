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
        
        [Header("Npc管理")]
        [SerializeField] private List<INpc> allNpcs = new List<INpc>();
        
        [Header("セリフデータ")]
        [SerializeField] private DialogueData dialogueData;
        
        private Dictionary<INpc, int> npcDialogueIndices = new Dictionary<INpc, int>();
        
        protected override bool UseDontDestroyOnLoad => true;
        
        protected override void Init()
        {
            allNpcs.Clear();
            npcDialogueIndices.Clear();
            GameEvents.OnGameStart += OnGameStart;
        }
        
        protected override void OnDestroy()
        {
            base.OnDestroy();
            GameEvents.OnGameStart -= OnGameStart;
        }
        
        private void OnGameStart() => RandomizeCriminal();
        
        /// <summary>
        /// ランダムにNpcを1体選んで犯人にする
        /// </summary>
        public void RandomizeCriminal()
        {
            if (allNpcs.Count == 0) return;
            
            foreach (var npc in allNpcs)
                npc.SetCriminal(false);
            
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
            
            if (!npcDialogueIndices.ContainsKey(npc))
                npcDialogueIndices[npc] = allNpcs.Count - 1;
            
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
                throw new System.Exception("[NpcManager] DialogueDataがnullです。InspectorでDialogueDataを割り当ててください");
            
            bool isCriminal = npc.IsCriminal();
            bool hasHint = npc.HasReceivedHint();
            bool isAccused = npc.IsAccused();
            
            int npcIndex = npcDialogueIndices.ContainsKey(npc) ? npcDialogueIndices[npc] : 0;
            
            var criminal = GetCriminal();
            if (criminal == null)
                Debug.LogError("[NpcManager] 犯人が設定されていません！GameEvents.RaiseGameStart()を呼んでください");
            
            NpcAppearance criminalAppearance = criminal?.GetNpcData()?.appearance;
            Gender npcGender = npc.GetNpcData()?.appearance.gender ?? Gender.Woman;
            
            return dialogueData.GetDialogue(isCriminal, hasHint, isAccused, npcIndex, criminalAppearance, npcGender);
        }
        
        public INpc GetCriminal() => allNpcs.FirstOrDefault(npc => npc.IsCriminal());
        
        public List<INpc> GetNpcsByAppearance(NpcAppearance targetAppearance) =>
            allNpcs.Where(npc =>
            {
                var data = npc.GetNpcData();
                if (data == null) return false;
                return DoesAppearanceMatch(data.appearance, targetAppearance);
            }).ToList();

        public List<INpc> GetNpcsByGender(Gender gender) =>
            allNpcs.Where(npc =>
            {
                var data = npc.GetNpcData();
                return data != null && data.appearance.gender == gender;
            }).ToList();
        
        public List<INpc> GetNpcsByDirection(Direction direction) =>
            allNpcs.Where(npc =>
            {
                var data = npc.GetNpcData();
                return data != null && data.appearance.positionFromCenter == direction;
            }).ToList();
        
        public List<INpc> GetNpcsByClothesColor(NpcColor color, float tolerance = ColorToleranceThreshold) =>
            allNpcs.Where(npc =>
            {
                var data = npc.GetNpcData();
                if (data == null) return false;
                var npcColorUnity = data.appearance.GetUnityColor(data.appearance.clothesColor);
                var targetColorUnity = data.appearance.GetUnityColor(color);
                return ColorDistance(npcColorUnity, targetColorUnity) < tolerance;
            }).ToList();
        
        public void SetAllHintsReceived(bool received)
        {
            foreach (var npc in allNpcs)
                npc.SetHintReceived(received);
        }
        
        private bool DoesAppearanceMatch(NpcAppearance npcAppearance, NpcAppearance targetAppearance)
        {
            bool genderMatch = npcAppearance.gender == targetAppearance.gender;
            bool directionMatch = npcAppearance.positionFromCenter == targetAppearance.positionFromCenter;
            bool clothesMatch = ColorDistance(npcAppearance.GetUnityColor(npcAppearance.clothesColor), targetAppearance.GetUnityColor(targetAppearance.clothesColor)) < ColorToleranceThreshold;
            bool maskMatch = ColorDistance(npcAppearance.GetUnityColor(npcAppearance.maskColor), targetAppearance.GetUnityColor(targetAppearance.maskColor)) < ColorToleranceThreshold;
            bool hairMatch = ColorDistance(npcAppearance.GetUnityColor(npcAppearance.hairColor), targetAppearance.GetUnityColor(targetAppearance.hairColor)) < ColorToleranceThreshold;
            bool hatMatch = ColorDistance(npcAppearance.GetUnityColor(npcAppearance.hatColor), targetAppearance.GetUnityColor(targetAppearance.hatColor)) < ColorToleranceThreshold;
            bool shoeMatch = ColorDistance(npcAppearance.GetUnityColor(npcAppearance.shoeColor), targetAppearance.GetUnityColor(targetAppearance.shoeColor)) < ColorToleranceThreshold;
            
            return genderMatch && directionMatch && clothesMatch && 
                   maskMatch && hairMatch && hatMatch && shoeMatch;
        }
        
        private float ColorDistance(Color a, Color b)
        {
            float dr = a.r - b.r;
            float dg = a.g - b.g;
            float db = a.b - b.b;
            return Mathf.Sqrt(dr * dr + dg * dg + db * db);
        }
    }
}
