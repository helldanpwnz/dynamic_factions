using System;
using System.Collections.Generic;
using System.Linq;
using RimWorld;
using RimWorld.Planet;
using Verse;
using UnityEngine;
using System.Text;

namespace DynamicFaction
{
	
    // ============================================ БЛОК 1: НАСТРОЙКИ ============================================
    public class DynamicFactionsSettings : ModSettings
    {
        public bool enableSplitFaction = true;
		public bool showDebugLogs = false;
		public bool limitTechLevel = false; 
		public float entropyPerUpdate = 3f; 
        public int triggerIntervalHours = 24;
        public int splitMaxDistanceTiles = 25;	
        public float randomSettlementChance = 0f;
        public float attackMultiplier = 1f;
        public float defenseMultiplier = 1f;
		public float leaderPointsMultiplier = 1f;
        public float influenceMultiplier = 1f;
		public float stabilityMultiplier = 1f;
		public float globalCrisisChance = 0.1f; // 0.1% за обновление
		public float plagueChance = 0.1f;
        
        public override void ExposeData()
        {
            Scribe_Values.Look(ref randomSettlementChance, "randomSettlementChance", 1f);
            Scribe_Values.Look(ref enableSplitFaction, "enableSplitFaction", true);
			Scribe_Values.Look(ref limitTechLevel, "limitTechLevel", false);
            Scribe_Values.Look(ref triggerIntervalHours, "triggerIntervalHours", 6);
			Scribe_Values.Look(ref entropyPerUpdate, "entropyPerUpdate", 3f);
            Scribe_Values.Look(ref splitMaxDistanceTiles, "splitMaxDistanceTiles", 25);
            Scribe_Values.Look(ref attackMultiplier, "attackMultiplier", 1f);
            Scribe_Values.Look(ref defenseMultiplier, "defenseMultiplier", 1f);
			Scribe_Values.Look(ref leaderPointsMultiplier, "leaderPointsMultiplier", 1f);
            Scribe_Values.Look(ref influenceMultiplier, "influenceMultiplier", 1f);
			Scribe_Values.Look(ref stabilityMultiplier, "stabilityMultiplier", 1f);
			Scribe_Values.Look(ref globalCrisisChance, "globalCrisisChance", 0.01f);
			Scribe_Values.Look(ref plagueChance, "plagueChance", 0.01f);
        }
    }

    // ============================================ БЛОК 2: МОД И НАСТРОЙКИ UI ============================================
    public class DynamicFactionsMod : Mod
    {
        public static DynamicFactionsSettings settings;
        private static Vector2 scrollPosition = Vector2.zero;
        
        public DynamicFactionsMod(ModContentPack content) : base(content)
        {
            settings = GetSettings<DynamicFactionsSettings>();
        }
        
        public override void DoSettingsWindowContents(Rect inRect)
        {
            const float ScrollBarWidthMargin = 18f;
            Rect outerRect = inRect.ContractedBy(10f);
            float totalHeight = 70f * 21;
            Rect scrollViewRect = new Rect(0f, 0f, outerRect.width - ScrollBarWidthMargin, totalHeight);
            
            Widgets.BeginScrollView(outerRect, ref scrollPosition, scrollViewRect);
            Listing_Standard listing = new Listing_Standard();
            listing.Begin(scrollViewRect);
			listing.CheckboxLabeled("Показывать логи отладки", 
			ref settings.showDebugLogs, 
			"Включает подробные логи");
            listing.CheckboxLabeled("✅ Включить систему Динамических Фракций", ref settings.enableSplitFaction);
			listing.CheckboxLabeled("🛡️ Ограничить технологии (макс. Средневековье)", 
			ref settings.limitTechLevel, 
			"Если включено, новые фракции никогда не будут выше уровня Medieval (средневековье).");
			listing.GapLine();
            listing.Label("⏰ Интервал пересчета (часов):");
            listing.Label($"Каждые {settings.triggerIntervalHours} ч. (6-24)");
            settings.triggerIntervalHours = (int)listing.Slider(settings.triggerIntervalHours, 1f, 168f);
            listing.GapLine();
            listing.Label("⚔️ Радиус новой фракции (тайлов):");
            listing.Label($"{settings.splitMaxDistanceTiles} тайлов");
            settings.splitMaxDistanceTiles = (int)listing.Slider(settings.splitMaxDistanceTiles, 5f, 300f);
            listing.GapLine();
			listing.Label("⚔️ Множитель очков АТАКИ:");
			listing.Label($"x{settings.attackMultiplier:F1} (0 = отключить систему)");
			settings.attackMultiplier = listing.Slider(settings.attackMultiplier, 0f, 5f);
            listing.Gap();
			listing.Label("🛡️ Множитель очков БРОНИ:");
			listing.Label($"x{settings.defenseMultiplier:F1} (0 = отключить систему)");
			settings.defenseMultiplier = listing.Slider(settings.defenseMultiplier, 0f, 5f);
            listing.GapLine();
			listing.Label("👑 Множитель очков ЛИДЕРА:");
			listing.Label($"x{settings.leaderPointsMultiplier:F1} (0 = отключить)");
			settings.leaderPointsMultiplier = listing.Slider(settings.leaderPointsMultiplier, 0f, 5f);
			listing.GapLine();
			listing.Label("🏛️ Множитель изменений СТАБИЛЬНОСТИ:");
			listing.Label($"x{settings.stabilityMultiplier:F1} (0 = отключить)");
			settings.stabilityMultiplier = listing.Slider(settings.stabilityMultiplier, 0f, 5f);
			listing.GapLine();
			listing.Label("🌟 Множитель очков ВЛИЯНИЯ:");
			listing.Label($"x{settings.influenceMultiplier:F1} (0 = отключить систему)");
			settings.influenceMultiplier = listing.Slider(settings.influenceMultiplier, 0f, 5f);
            listing.GapLine();		
			listing.Label("🎲 Шанс спавна случайной базы (%):");
			listing.Label($"{settings.randomSettlementChance:F1}% (0 = отключить спавн)");
			settings.randomSettlementChance = listing.Slider(settings.randomSettlementChance, 0f, 10f);
			listing.GapLine();
			listing.Label("💥 Шанс Мирового Кризиса (% за обновление):");
			listing.Label($"{settings.globalCrisisChance:F1}% (0 = отключить кризисы)");
			settings.globalCrisisChance = listing.Slider(settings.globalCrisisChance, 0f, 10f);
			listing.GapLine();
			listing.Label("🦠 Шанс Пандемии Чумы (% за обновление):");
			listing.Label($"{settings.plagueChance:F1}% (0 = отключить чуму)");
			settings.plagueChance = listing.Slider(settings.plagueChance, 0f, 10f);  // ← ПРОСТОЙ слайдер
			listing.GapLine();
			listing.Label("💀 Энтропия за обновление:");
			listing.Label($"{settings.entropyPerUpdate:F1} (0 = отключить энтропию)");
			settings.entropyPerUpdate = listing.Slider(settings.entropyPerUpdate, 0f, 10f);
			listing.GapLine();
            
            if (listing.ButtonText("🔄 Сбросить"))
            {
                settings.randomSettlementChance = 0f;
                settings.enableSplitFaction = true;
                settings.triggerIntervalHours = 6;
                settings.splitMaxDistanceTiles = 50;
                settings.influenceMultiplier = 1f;
				settings.entropyPerUpdate = 3f;
				settings.limitTechLevel = false;
				settings.plagueChance = 0.1f; 
				settings.globalCrisisChance = 0.1f; // ← 0.1% кризис
				 
            }
            
            listing.End();
            Widgets.EndScrollView();
            settings.Write();
        }
        
        public override string SettingsCategory() => "Dynamic Factions";
    }

    // ============================================ БЛОК 3: ДАННЫЕ СТАБИЛЬНОСТИ ============================================
    public class FactionStabilityData : IExposable
    {
        public Faction faction;
        public float stabilityPoints;
        public int lastSettlementCount;
        public Pawn lastLeader;
        public float leaderLegitimacy = 0f;
        public int leaderStartTick = -1;
        public float attackPoints = 0f;
        public float defensePoints = 0f;
        public float influencePoints = 0f;
		public float accumulatedEntropy = 0f; 
        public Faction currentTargetFaction;
        public int lastSettlementCountForInfluence = 0;
	    public Faction lastPeaceFaction;
		public int lastPeaceTick = -1;	
        
        public void ExposeData()
        {
            Scribe_References.Look(ref faction, "faction");
            Scribe_Values.Look(ref stabilityPoints, "stabilityPoints", 0f);
            Scribe_Values.Look(ref lastSettlementCount, "lastSettlementCount", 0);
            Scribe_References.Look(ref lastLeader, "lastLeader");
            Scribe_Values.Look(ref leaderLegitimacy, "leaderLegitimacy", 0f);
            Scribe_Values.Look(ref leaderStartTick, "leaderStartTick", -1);
            Scribe_Values.Look(ref attackPoints, "attackPoints", 0f, forceSave: true);
            Scribe_Values.Look(ref defensePoints, "defensePoints", 0f, forceSave: true);
            Scribe_Values.Look(ref influencePoints, "influencePoints", 0f, forceSave: true);
            Scribe_Values.Look(ref lastSettlementCountForInfluence, "lastSettlementCountForInfluence", 0);
			Scribe_Values.Look(ref accumulatedEntropy, "accumulatedEntropy", 0f);
            Scribe_References.Look(ref currentTargetFaction, "currentTargetFaction");
			Scribe_References.Look(ref lastPeaceFaction, "lastPeaceFaction");
            Scribe_Values.Look(ref lastPeaceTick, "lastPeaceTick", -1);
        }
    }

    // ============================================ БЛОК 4: МЕНЕДЖЕР МИРА ============================================
    public class DynamicFactionsManager : WorldComponent
    {
        private List<FactionDef> allHiddenFactionDefs = new List<FactionDef>();
        private List<FactionDef> activePool = new List<FactionDef>();
        private List<FactionStabilityData> factionDataList = new List<FactionStabilityData>();
        private const int POOL_SIZE = 100;
        private bool initialized = false;
		private int nextTriggerTick = -1;
private List<FactionDef> usedDefs = new List<FactionDef>();
private static readonly HashSet<string> IgnoredFactionNames = new HashSet<string>
{
    "Ancient", "Horus", "DarkPhenomena", "TradersGuild", "Salvagers"
};
  
        
        // Множители технологий
        private static readonly Dictionary<TechLevel, float> TechMultipliers = new()
        {
            [TechLevel.Animal] = 0.7f, [TechLevel.Neolithic] = 0.8f, [TechLevel.Medieval] = 0.9f,
            [TechLevel.Industrial] = 1.0f, [TechLevel.Spacer] = 1.1f, [TechLevel.Ultra] = 1.2f, [TechLevel.Archotech] = 1.3f
        };
        
        private static readonly Dictionary<TechLevel, float> BattleTechMultipliers = new()
        {
            [TechLevel.Animal] = 0.4f, [TechLevel.Neolithic] = 0.6f, [TechLevel.Medieval] = 0.8f,
            [TechLevel.Industrial] = 1.0f, [TechLevel.Spacer] = 1.2f, [TechLevel.Ultra] = 1.4f, [TechLevel.Archotech] = 1.6f
        };
        
        public DynamicFactionsManager(World world) : base(world) { }
		
// В начало класса (рядом с другими словарями)
private static readonly Dictionary<TechLevel, string> SpawnLetterTitles = new()
{
    [TechLevel.Animal] = "Появление диких племён",
    [TechLevel.Neolithic] = "Кочующие охотники",
    [TechLevel.Medieval] = "Новые феодальные владения",
    [TechLevel.Industrial] = "Индустриальная экспансия",
    [TechLevel.Spacer] = "Космические колонисты",
    [TechLevel.Ultra] = "Ультратехнологическая угроза",
    [TechLevel.Archotech] = "Аркотех-явления"
};		
        
// ВЕСА ТЕХУРОВНЕЙ ДЛЯ СЛУЧАЙНОГО СПАВНА
private static readonly Dictionary<TechLevel, float> TechSpawnWeights = new()
{
    [TechLevel.Neolithic] = 0.35f,    // 35% - больше всего
    [TechLevel.Medieval] = 0.30f,     // 30%
    [TechLevel.Industrial] = 0.20f,   // 20%
    [TechLevel.Spacer] = 0.10f,       // 10%
    [TechLevel.Ultra] = 0.05f         // 5% - редкость
};

// ОПИСАНИЯ ФРАКЦИЙ ПО ТЕХУРОВНЯМ
private static readonly Dictionary<TechLevel, string> SpawnDescriptions = new()
{
    [TechLevel.Neolithic] = "дикие охотники-кочевники, поклоняющиеся древним тотемам",
    [TechLevel.Medieval] = "феодальные рыцари, верные своему сюзерену",
    [TechLevel.Industrial] = "индустриальная корпорация с конвейерным производством",
    [TechLevel.Spacer] = "космическая колония с орбитальными станциями",
    [TechLevel.Ultra] = "ультра-технологический синдикат с ИИ-управлением"
};

// ============================================ БЛОК 4.1: ГЛАВНЫЙ ЦИКЛ ============================================
public override void WorldComponentTick()
{
    if (!DynamicFactionsMod.settings.enableSplitFaction) return;
    
    int currentTick = Find.TickManager.TicksGame;
	
 
// 🧹 Умная очистка (каждые 10k тиков)
if (currentTick % 10000 == 0)
{
	
// 1. Находим "зомби" фракции (нет баз, не скрыты, не игрок)
var zombieFactions = Find.FactionManager.AllFactions
    .Where(f => !f.IsPlayer 
             && !f.Hidden 
             && !f.def.hidden 
             && f.def.humanlikeFaction
             && !IgnoredFactionNames.Contains(f.def.defName)
             && !new HashSet<string> { "TradersGuild", "Salvagers" }.Contains(f.def.defName))
    .ToList();

    int removedCount = 0;
    foreach (var f in zombieFactions)
    {
        // Если нет поселений
        if (!Find.WorldObjects.Settlements.Any(s => s.Faction == f))
        {
			            // ✅ ОТПРАВЛЯЕМ ПИСЬМО (до того, как пометим фракцию скрытой)
            Find.LetterStack.ReceiveLetter(
                "Фракция уничтожена", 
                $"Мир облетела весть: фракция {f.Name} больше не существует. Последние их города пали, а народ рассеялся по пустошам.", 
                LetterDefOf.NeutralEvent
            );
			
            f.defeated = true;
            f.hidden = true; // ← ГЛАВНОЕ: скрывает из списка
            removedCount++;
        }
    }
	
	
	
    // 2. Чистим твои списки
    factionDataList.RemoveAll(d => d.faction == null || d.faction.defeated || d.faction.Hidden);
    usedDefs.RemoveAll(def => def == null || Find.FactionManager.AllFactionsVisible.Any(f => f.def == def && !f.defeated));
    
    if (removedCount > 0)
{
    if (DynamicFactionsMod.settings.showDebugLogs)
    {
        Log.Message($"[DF] 🧹 Скрыто {removedCount} уничтоженных фракций из UI");
    }
}
}
    
    // Инициализация
    if (!initialized && currentTick > 100)
    {
        Initialize();
        initialized = true;
    }
    
    // 🎯 НОВЫЙ ТРИГГЕР С РАНДОМИЗАЦИЕЙ
    if (nextTriggerTick < 0)  // Первый запуск
{
    int interval = DynamicFactionsMod.settings.triggerIntervalHours * 2500;
    nextTriggerTick = currentTick + interval + Rand.Range(-interval / 4, interval / 4);
    
    if (DynamicFactionsMod.settings.showDebugLogs)
    {
        Log.Message($"[DF] ⏰ Первый расчет: тика {nextTriggerTick} (~{DynamicFactionsMod.settings.triggerIntervalHours}ч)");
    }
}
// Удаление руин (30-100 дней)
List<DestroyedSettlement> ruinsToRemove = new List<DestroyedSettlement>();
foreach (DestroyedSettlement ruin in Find.WorldObjects.DestroyedSettlements)
{
    int ticksSinceDestroyed = Find.TickManager.TicksGame - ruin.creationGameTicks;
    int daysPassed = ticksSinceDestroyed / 60000;
    if (daysPassed >= Verse.Rand.RangeInclusive(30, 100))
    {
        ruinsToRemove.Add(ruin);
    }
}
foreach (DestroyedSettlement ruin in ruinsToRemove)
{
    int ticksSinceDestroyed = Find.TickManager.TicksGame - ruin.creationGameTicks;
    int daysPassed = ticksSinceDestroyed / 60000;
    Find.WorldObjects.Remove(ruin);
    
    if (DynamicFactionsMod.settings.showDebugLogs)
    {
        Log.Message($"[DF] 🧹 Руины тайла {ruin.Tile} исчезли ({daysPassed} дней)");
    }
}


 
if (currentTick >= nextTriggerTick)
{
    ProcessFactionsStability();
		//ЧУМА
	ProcessPlague();	
    int interval = DynamicFactionsMod.settings.triggerIntervalHours * 2500;
    nextTriggerTick = currentTick + interval; 
    
    // Мировой кризис
    if (Verse.Rand.Value < DynamicFactionsMod.settings.globalCrisisChance / 100f)
    {
        TriggerGlobalCrisis();
    }
    
if (DynamicFactionsMod.settings.showDebugLogs)
{
    Log.Message($"[DF] ✅ Расчет завершен...");
}




}
}
 

 
// ============================================ БЛОК 4.2: СОБЫТИЯ ВЛИЯНИЯ ============================================
private void TriggerInfluenceEvent(Faction faction, FactionStabilityData data, bool isPositive)
{
    bool ExecuteDefenseInvestment(string reason = "Влияние потрачено на укрепление обороны.")
    {
        data.attackPoints += 20f;
        data.defensePoints += 20f;
        data.accumulatedEntropy -= 1f;
        data.attackPoints = Mathf.Clamp(data.attackPoints, 0f, 200f);
        data.defensePoints = Mathf.Clamp(data.defensePoints, 0f, 200f);
        Find.LetterStack.ReceiveLetter("🌟 Инвестиции в армию",
            $"{faction.Name} {reason}\n\n• Атака: +20 (всего: {data.attackPoints:F1})\n• Броня: +20 (всего: {data.defensePoints:F1})",
            LetterDefOf.PositiveEvent);
        return false;
    }
    
    if (!isPositive)
    {
        float rollNegative = Rand.Value;
        if (rollNegative < 0.10f) // Дар земель 10%
        {
            var topFactionData = factionDataList
                .Where(d => d.faction != faction && !d.faction.defeated && !IgnoredFactionNames.Contains(d.faction.def.defName))
                .OrderByDescending(d => d.influencePoints)
                .FirstOrDefault();
            
            if (topFactionData != null)
            {
                Faction topFaction = topFactionData.faction;
                topFactionData.influencePoints -= 50f;
                int newTile = TileFinder.RandomSettlementTileFor(faction);
                if (newTile >= 0)
                {
                    Settlement newSett = (Settlement)WorldObjectMaker.MakeWorldObject(WorldObjectDefOf.Settlement);
                    newSett.SetFaction(faction);
                    newSett.Tile = newTile;
                    newSett.Name = SettlementNameGenerator.GenerateSettlementName(newSett, null);
                    Find.WorldObjects.Add(newSett);
                    data.accumulatedEntropy -= 1f;
                    Find.LetterStack.ReceiveLetter("🌟 Дар земель",
                        $"{topFaction.Name} (Влияние: {topFactionData.influencePoints:F0}) передала земли фракции {faction.Name} для стабилизации региона.\nНовая база: {newSett.Name}", 
                        LetterDefOf.NeutralEvent, newSett);
                    return;
                        }
						}
        else
        {
            // Если не нашли тайл для поселения
            ExecuteDefenseInvestment("не удалось передать земли (нет подходящей территории).");
            return; // ← Выходим
            }
        }
        else if (rollNegative < 0.30f) // Свержение власти 20%
        {
            Find.LetterStack.ReceiveLetter("🔥 Свержение власти", 
                $"В {faction.Name} начался вооруженный мятеж! Часть гарнизонов перешла на сторону восставших.", 
                LetterDefOf.NegativeEvent);
            data.accumulatedEntropy -= 1f;
            bool success = TrySpawnSplitFaction(faction, false);
            if (success) { data.influencePoints += 100f; return; }
        }
        else if (rollNegative < 0.40f && ModsConfig.IdeologyActive) // Ересь 10%
        {
            var topInfluential = factionDataList
                .Where(d => d.faction != faction && !d.faction.defeated && d.faction.ideos?.PrimaryIdeo != null && !IgnoredFactionNames.Contains(d.faction.def.defName))
                .OrderByDescending(d => d.influencePoints)
                .FirstOrDefault();
            
            if (topInfluential != null)
            {
                faction.ideos.SetPrimary(topInfluential.faction.ideos.PrimaryIdeo);
                data.defensePoints += 20f;
                data.defensePoints = Mathf.Clamp(data.defensePoints, 0f, 200f);
                data.accumulatedEntropy -= 1f;
                Find.LetterStack.ReceiveLetter("🔥 Ересь",
                    $"{faction.Name} отреклась от старых богов!\nПод давлением {topInfluential.faction.Name}, они приняли новую веру.\n\n• Идеология изменена\n• Броня: +20 (Фанатизм)", 
                    LetterDefOf.NeutralEvent);
                return;
            }
    else
    {
        ExecuteDefenseInvestment("не нашла цель для ереси.");
        return;
        }
        }
	
        else if (rollNegative < 0.50f) // Экономический коллапс 10% (было 20%)
        {
            data.stabilityPoints -= 20f;
            if (faction.leader != null)
            {
                data.leaderLegitimacy -= 5f;
                data.leaderLegitimacy = Mathf.Clamp(data.leaderLegitimacy, -20f, 20f);
            }
            data.stabilityPoints = Mathf.Clamp(data.stabilityPoints, -100f, 100f);
            data.accumulatedEntropy -= 1f;
            Find.LetterStack.ReceiveLetter("📉 Экономический коллапс",
                $"Экономика {faction.Name} рухнула под гнетом внешнего давления.\n\n• Стабильность: -20\n• Лидер: -5", 
                LetterDefOf.NegativeEvent);
            return;
        }
        else if (rollNegative < 0.60f) // ✅ НОВОЕ: Разрыв отношений 10%
        {
var allies = Find.FactionManager.AllFactions
    .Where(f => !f.IsPlayer 
                && !f.defeated 
                && f != faction 
                && f.AllyOrNeutralTo(faction)
                && !IgnoredFactionNames.Contains(f.def.defName))
    .ToList();
            
            if (allies.Count == 0)
            {
                // Фоллбек: экономический коллапс если нет союзников
                data.stabilityPoints -= 20f;
                if (faction.leader != null)
                {
                    data.leaderLegitimacy -= 5f;
                    data.leaderLegitimacy = Mathf.Clamp(data.leaderLegitimacy, -20f, 20f);
                }
                data.stabilityPoints = Mathf.Clamp(data.stabilityPoints, -100f, 100f);
                data.accumulatedEntropy -= 1f;
                Find.LetterStack.ReceiveLetter("📉 Экономический коллапс (фоллбек)",
                    $"Нет союзников для предательства → Экономика {faction.Name} рухнула.\n\n• Стабильность: -20\n• Лидер: -5", 
                    LetterDefOf.NegativeEvent);
                return;
            }
            
Faction lostAlly = allies.RandomElement();
faction.TryAffectGoodwillWith(lostAlly, -100);
lostAlly.TryAffectGoodwillWith(faction, -100); // Добавляем обратное изменение
data.accumulatedEntropy -= 1f;
            Find.LetterStack.ReceiveLetter("💔 Разрыв отношений",
                $"{faction.Name} предала старого союзника {lostAlly.Name}!\n\n" +
                "Дипломатический скандал: теперь это враг.\n• Союзник → Враг", 
                LetterDefOf.NegativeEvent);
            return;
        }
        else if (rollNegative < 0.70f) // Продажа земель 10% (сдвинуто)
        {
            var topInfluential = factionDataList
                .Where(d => d.faction != faction && !d.faction.defeated && !IgnoredFactionNames.Contains(d.faction.def.defName))
                .OrderByDescending(d => d.influencePoints)
                .FirstOrDefault();
            
            var mySettlements = Find.WorldObjects.Settlements.Where(s => s.Faction == faction).ToList();
            
if (topInfluential != null && mySettlements.Count > 0)
{
    Settlement soldSettlement = mySettlements.RandomElement();
    int tile = soldSettlement.Tile;
    string oldName = soldSettlement.Name;
    
    // ✅ ФИКС: Удаляем старое и создаем новое
    Find.WorldObjects.Remove(soldSettlement);
    Settlement newBase = (Settlement)WorldObjectMaker.MakeWorldObject(WorldObjectDefOf.Settlement);
    newBase.SetFaction(topInfluential.faction);
    newBase.Tile = tile;
    newBase.Name = oldName + " (продано)";
    Find.WorldObjects.Add(newBase);
    
    data.attackPoints += 30f;
    data.defensePoints += 30f;
    data.attackPoints = Mathf.Clamp(data.attackPoints, 0f, 200f);
    data.defensePoints = Mathf.Clamp(data.defensePoints, 0f, 200f);
    data.accumulatedEntropy -= 1f;
    Find.LetterStack.ReceiveLetter("💰 Продажа земель",
        $"{faction.Name} продала поселение {oldName} фракции {topInfluential.faction.Name} за военные поставки.\n\n• Атака: +30\n• Броня: +30", 
        LetterDefOf.NeutralEvent, newBase);
    return;
    }
    else
    {
        ExecuteDefenseInvestment("нечего продавать или нет покупателя.");
        return;
    }
        }
        else if (rollNegative < 0.90f) // Диктатура 20% (сдвинуто)
        {
            data.stabilityPoints -= 30f;
            if (faction.leader != null) 
            {
                data.leaderLegitimacy -= 5f;
                data.leaderLegitimacy = Mathf.Clamp(data.leaderLegitimacy, -20f, 20f);
            }
            data.attackPoints += 30f;
            data.defensePoints += 30f;
            data.stabilityPoints = Mathf.Clamp(data.stabilityPoints, -100f, 100f);
            data.attackPoints = Mathf.Clamp(data.attackPoints, 0f, 200f);
            data.defensePoints = Mathf.Clamp(data.defensePoints, 0f, 200f);
            data.accumulatedEntropy -= 1f;
            
            // ✅ НОВОЕ: потеря 1-2 союзников
            var allies = Find.FactionManager.AllFactions.Where(f => 
                f != faction && !f.IsPlayer && !f.defeated && f.def.humanlikeFaction && 
                !IgnoredFactionNames.Contains(f.def.defName) && !faction.HostileTo(f)).ToList();

            int alliesToLose = Rand.RangeInclusive(1, Mathf.Min(2, allies.Count));
for (int i = 0; i < alliesToLose; i++)
{
    if (allies.Count == 0) break;
    Faction ally = allies.RandomElement();
    faction.TryAffectGoodwillWith(ally, -100);  // -100 → hostile
    ally.TryAffectGoodwillWith(faction, -100);  // Добавляем обратное изменение
    allies.Remove(ally);
}
            
            Find.LetterStack.ReceiveLetter("🛡️ Диктатура",
                $"В {faction.Name} установлен жесткий военный режим для удержания власти.\n\n" +
                $"• Стабильность: -30\n• Лидер: -5\n• Атака: +30\n• Броня: +30\n" +
                $"• Потеряно союзников: {alliesToLose}", 
                LetterDefOf.NegativeEvent);
            return;
        }
        else // Помощь союзников 10% (сдвинуто)
        {
            data.stabilityPoints += 10f;
            data.attackPoints += 10f;
            data.defensePoints += 10f;
            data.stabilityPoints = Mathf.Clamp(data.stabilityPoints, -100f, 100f);
            data.attackPoints = Mathf.Clamp(data.attackPoints, 0f, 200f);
            data.defensePoints = Mathf.Clamp(data.defensePoints, 0f, 200f);
            data.accumulatedEntropy -= 1f;
            Find.LetterStack.ReceiveLetter("🤝 Помощь союзников",
                $"{faction.Name} получила гуманитарную помощь от соседей.\n\n• Стабильность: +10\n• Атака: +10\n• Броня: +10", 
                LetterDefOf.PositiveEvent);
            return;
        }
    }

    // Положительные события +100 влияния
float roll = Rand.Value;
if (roll < 0.10f) // Аннексия 10% (было 20%)
{
    var allFactionsData = factionDataList.Where(d => d.faction != null && !d.faction.defeated && d.faction != faction && !IgnoredFactionNames.Contains(d.faction.def.defName)).ToList();
    if (allFactionsData.Count > 0)
    {
        allFactionsData.Sort((a, b) => a.influencePoints.CompareTo(b.influencePoints));
        float annexRoll = Rand.Value;
        Faction targetFaction = null;
        if (annexRoll < 0.50f && allFactionsData.Count >= 1) targetFaction = allFactionsData[0].faction;
        else if (annexRoll < 0.80f && allFactionsData.Count >= 2) targetFaction = allFactionsData[1].faction;
        else if (allFactionsData.Count >= 3) targetFaction = allFactionsData[2].faction;
        
        if (targetFaction != null)
        {
            var targetBases = Find.WorldObjects.Settlements.Where(s => s.Faction == targetFaction).ToList();
            if (targetBases.Count > 0)
            {
                Settlement annexed = targetBases.RandomElement();
                int capturedTile = annexed.Tile;
                string oldName = annexed.Name;
                
                // ✅ ФИКС: Удаляем старое и создаем новое с правильным цветом
                Find.WorldObjects.Remove(annexed);
                Settlement newBase = (Settlement)WorldObjectMaker.MakeWorldObject(WorldObjectDefOf.Settlement);
                newBase.SetFaction(faction);
                newBase.Tile = capturedTile;
                newBase.Name = oldName + " (аннексирована)";
                Find.WorldObjects.Add(newBase);
                
                FactionStabilityData targetData = GetOrCreateData(targetFaction);
                targetData.influencePoints -= 30f;
                data.accumulatedEntropy += 1f;
                Find.LetterStack.ReceiveLetter("🌟 Аннексия", $"{faction.Name} мирно поглотила {oldName}!", LetterDefOf.PositiveEvent, newBase);
                return;
            }
			
        }
    }
    ExecuteDefenseInvestment("не смогла аннексировать цель.");
    return;
}
    else if (roll < 0.20f) // ✅ НОВОЕ: Установить дружбу 10%
    {
var potentialFriends = Find.FactionManager.AllFactions
    .Where(f => !f.IsPlayer 
                && !f.defeated 
                && f != faction
                && !IgnoredFactionNames.Contains(f.def.defName))
    .ToList();
        
        if (potentialFriends.Count > 0)
        {
Faction friendFaction = potentialFriends.RandomElement();
faction.TryAffectGoodwillWith(friendFaction, 100);
friendFaction.TryAffectGoodwillWith(faction, 100); // Добавляем обратное изменение
data.accumulatedEntropy += 1f;
            Find.LetterStack.ReceiveLetter("🤝 Новая дружба",
                $"{faction.Name} протянула руку дружбы {friendFaction.Name}!\n\n" +
                "Дипломатический прорыв: бывшие враги стали союзниками.\n• Враг → Друг", 
                LetterDefOf.PositiveEvent);
            return;
        }
        ExecuteDefenseInvestment("не нашла врагов для дружбы.");
        return;
    }
    else if (roll < 0.30f) // Смерть на переговорах 10% (осталось)
    {
        if (faction.leader == null) { ExecuteDefenseInvestment(); return; }
        Pawn oldLeader = faction.leader;
        string oldLeaderName = oldLeader.Name.ToStringFull;
        Find.WorldPawns.RemovePawn(oldLeader);
        oldLeader.Destroy();
#pragma warning disable CS0618
        faction.GenerateNewLeader();
#pragma warning restore CS0618
        data.lastLeader = faction.leader;
        data.leaderLegitimacy = Rand.Range(-8f, -2f);
        float transferAmount = Mathf.Min(50f, data.defensePoints);
        data.defensePoints -= transferAmount;
        data.attackPoints += transferAmount;
        data.stabilityPoints -= 15f;
        data.accumulatedEntropy += 1f;
        Find.LetterStack.ReceiveLetter("☠️ Смерть лидера",
            $"Лидер {faction.Name}, {oldLeaderName}, убит на переговорах!\n\nАрмия в ярости: оборона ослаблена, атака усилена.\n• Атака: +{transferAmount:F0}\n• Броня: -{transferAmount:F0}",
            LetterDefOf.NeutralEvent);
        return;
    }
    else if (roll < 0.40f) // Инвестирование в оборону 10% (было 20%)
    {
        ExecuteDefenseInvestment();
        return;
    }
    else if (roll < 0.60f) // Благоустройство 20% (осталось)
    {
        data.stabilityPoints += 20f;
        data.leaderLegitimacy += 5f;
        data.stabilityPoints = Mathf.Clamp(data.stabilityPoints, -100f, 100f);
        data.leaderLegitimacy = Mathf.Clamp(data.leaderLegitimacy, -20f, 20f);
        data.accumulatedEntropy += 1f;
        Find.LetterStack.ReceiveLetter("🌟 Эпоха процветания",
            $"{faction.Name} улучшила инфраструктуру.\n\n• Стабильность: +20\n• Лидер: +5",
            LetterDefOf.PositiveEvent);
        return;
    }
else if (roll < 0.75f) // Цветная революция 15%
{
    // Выбираем цель - самую слабую по влиянию фракцию
    var targets = factionDataList
        .Where(d => d.faction != faction && !d.faction.defeated && !d.faction.Hidden && 
                   d.faction.def.humanlikeFaction && !IgnoredFactionNames.Contains(d.faction.def.defName))
        .OrderBy(d => d.influencePoints)
        .ToList();
    
    if (targets.Count == 0)
    {
        ExecuteDefenseInvestment("не нашла цель для революции.");
        return;
    }
    
    Faction targetFaction = targets[0].faction;
    var targetData = targets[0];
    
    // Получаем поселения цели
    var targetSettlements = Find.WorldObjects.Settlements
        .Where(s => s.Faction == targetFaction).ToList();
    
    if (targetSettlements.Count == 0)
    {
        ExecuteDefenseInvestment("цель не имеет поселений.");
        return;
    }
    
    // Выбираем деф для новой фракции (тот же techLevel)
    var availableDefs = allHiddenFactionDefs
        .Where(d => d.techLevel == targetFaction.def.techLevel && !usedDefs.Contains(d))
        .ToList();
    
    if (availableDefs.Count == 0)
    {
        usedDefs.RemoveAll(d => d.techLevel == targetFaction.def.techLevel);
        availableDefs = allHiddenFactionDefs
            .Where(d => d.techLevel == targetFaction.def.techLevel)
            .ToList();
    }
    
    if (availableDefs.Count == 0)
    {
        ExecuteDefenseInvestment("нет доступных шаблонов фракций.");
        return;
    }
    
    // 1. СОЗДАЕМ НОВУЮ ФРАКЦИЮ-ПОВСТАНЦЕВ
    FactionDef newFactionDef = availableDefs.RandomElement();
    usedDefs.Add(newFactionDef);
    
    FactionGeneratorParms parms = new FactionGeneratorParms(newFactionDef);
    Faction rebelFaction = FactionGenerator.NewGeneratedFaction(parms);
    Find.FactionManager.Add(rebelFaction);
    
    // 2. УДАЛЯЕМ СГЕНЕРИРОВАННЫЕ ПОСЕЛЕНИЯ (если есть)
    var generatedSettlements = Find.WorldObjects.Settlements
        .Where(s => s.Faction == rebelFaction).ToList();
    foreach (var s in generatedSettlements) 
        Find.WorldObjects.Remove(s);
    
    // 3. СОЗДАЕМ НОВОЕ ПОСЕЛЕНИЕ РЯДОМ С ЦЕЛЬЮ
    Settlement targetBase = targetSettlements.RandomElement();
    int maxDist = DynamicFactionsMod.settings.splitMaxDistanceTiles;
    int newTile = -1;
    
    // Ищем тайл для нового поселения
    for (int i = 0; i < 100; i++)
    {
        int t = TileFinder.RandomSettlementTileFor(rebelFaction);
        if (t >= 0 && TileFinder.IsValidTileForNewSettlement(t) &&
            Find.WorldGrid.ApproxDistanceInTiles(targetBase.Tile, t) <= maxDist)
        {
            newTile = t;
            break;
        }
    }
    
    if (newTile == -1)
    {
        // Если не нашли рядом, ищем любой валидный тайл
        newTile = TileFinder.RandomSettlementTileFor(rebelFaction);
        if (newTile == -1)
        {
            // Уничтожаем созданную фракцию
            rebelFaction.defeated = true;
            ExecuteDefenseInvestment("не удалось найти место для поселения повстанцев.");
            return;
        }
    }
    
    // Создаем поселение повстанцев
    Settlement rebelBase = (Settlement)WorldObjectMaker.MakeWorldObject(WorldObjectDefOf.Settlement);
    rebelBase.SetFaction(rebelFaction);
    rebelBase.Tile = newTile;
    rebelBase.Name = SettlementNameGenerator.GenerateSettlementName(rebelBase, null);
    Find.WorldObjects.Add(rebelBase);
    
    // 4. НАСТРАИВАЕМ ИДЕОЛОГИЮ (если есть DLC)
    if (ModsConfig.IdeologyActive && rebelFaction.ideos != null)
    {
        // 60% шанс принять идеологию спонсора, 40% - создать новую
        if (faction.ideos?.PrimaryIdeo != null && Rand.Value < 0.6f)
        {
            rebelFaction.ideos.SetPrimary(faction.ideos.PrimaryIdeo);
        }
        else
        {
            try
            {
                Ideo newIdeo = IdeoGenerator.GenerateIdeo(new IdeoGenerationParms(rebelFaction.def));
                Find.IdeoManager.Add(newIdeo);
                rebelFaction.ideos.SetPrimary(newIdeo);
            }
            catch
            {
                // Фоллбек: если не удалось создать, берем рандомную существующую
                var existingIdeos = Find.FactionManager.AllFactions
                    .Where(f => f.ideos?.PrimaryIdeo != null)
                    .Select(f => f.ideos.PrimaryIdeo)
                    .Distinct()
                    .ToList();
                
                if (existingIdeos.Count > 0)
                    rebelFaction.ideos.SetPrimary(existingIdeos.RandomElement());
            }
        }
    }
    
    // 5. УСТАНАВЛИВАЕМ ДИПЛОМАТИЧЕСКИЕ ОТНОШЕНИЯ
    
    // Спонсор ↔ Повстанцы: дружба (+100)
    faction.TryAffectGoodwillWith(rebelFaction, 100);
    rebelFaction.TryAffectGoodwillWith(faction, 100);
    
    // Цель ↔ Повстанцы: война (-100)
    targetFaction.TryAffectGoodwillWith(rebelFaction, -100);
    rebelFaction.TryAffectGoodwillWith(targetFaction, -100);
    
    // Спонсор ↔ Цель: ухудшение отношений (-50)
    faction.TryAffectGoodwillWith(targetFaction, -50);
    targetFaction.TryAffectGoodwillWith(faction, -50);
    
    // Повстанцы ↔ Игрок: нейтрально (0) или немного положительно
    rebelFaction.TryAffectGoodwillWith(Faction.OfPlayer, Rand.Range(-20, 40));
    
    // Повстанцы ↔ Остальные фракции: случайные отношения
    foreach (Faction other in Find.FactionManager.AllFactions)
    {
        if (other == rebelFaction || other == faction || other == targetFaction || other.IsPlayer) 
            continue;
        
        if (other.HostileTo(targetFaction))
            // Если фракция враждебна цели, то повстанцы дружелюбны к ней
            rebelFaction.TryAffectGoodwillWith(other, Rand.Range(20, 60));
        else if (other.GoodwillWith(targetFaction) > 50)
            // Если фракция дружит с целью, то повстанцы враждебны
            rebelFaction.TryAffectGoodwillWith(other, Rand.Range(-80, -40));
        else
            // Остальные - случайно
            rebelFaction.TryAffectGoodwillWith(other, Rand.Range(-50, 50));
    }
    
    // 6. СНИЖАЕМ СТАБИЛЬНОСТЬ ЦЕЛИ
    targetData.stabilityPoints -= 30f;
    targetData.stabilityPoints = Mathf.Clamp(targetData.stabilityPoints, -100f, 100f);
    
    // 7. ПОВЫШАЕМ ВЛИЯНИЕ СПОНСОРА
    data.influencePoints += 50f;
    data.accumulatedEntropy += 1f;
    
    // 8. ОТПРАВЛЯЕМ СООБЩЕНИЕ
    string sponsorName = faction.Name;
    string targetName = targetFaction.Name;
    string rebelName = rebelFaction.Name;
    
    string letterText = $"{sponsorName} организовала цветную революцию в {targetName}!\n\n" +
                       $"• Создана новая фракция: {rebelName}\n" +
                       $"• Основана база: {rebelBase.Name}\n" +
                       $"• {targetName}: стабильность -30\n" +
                       $"• {sponsorName}: влияние +50\n\n" +
                       $"Повстанцы враждуют с {targetName}, но дружественны к {sponsorName}.";
    
    Find.LetterStack.ReceiveLetter("🌟 Цветная революция", 
        letterText, 
        LetterDefOf.NeutralEvent, 
        rebelBase);
    
    OnFactionUsed(newFactionDef);
    return;
}
    else if (roll < 0.90f) // Пропаганда 15% (осталось)
    {
        if (!ModsConfig.IdeologyActive)
        {
            ExecuteDefenseInvestment("использовала влияние без DLC Ideology.");
            return;
        }
        
        var targetData = factionDataList
            .Where(d => d.faction != faction && !d.faction.defeated && !IgnoredFactionNames.Contains(d.faction.def.defName))
            .OrderBy(d => d.stabilityPoints)
            .FirstOrDefault();
        
        if (targetData != null && faction.ideos?.PrimaryIdeo != null)
        {
            Ideo myIdeo = faction.ideos.PrimaryIdeo;
            targetData.faction.ideos.SetPrimary(myIdeo);
            data.accumulatedEntropy += 1f;
            Find.LetterStack.ReceiveLetter("🌟 Культурная победа",
                $"Пропаганда {faction.Name} захватила умы {targetData.faction.Name}!\nПриняли идеологию: {myIdeo.name}.",
                LetterDefOf.PositiveEvent);
            return;
        }
        ExecuteDefenseInvestment("пропаганда не нашла цели.");
        return;
    }
    else // Фоллбек: Инвестирование в оборону 10%
    {
        ExecuteDefenseInvestment("использовала влияние на стандартное укрепление.");
        return;
    }
}

        // ============================================ БЛОК 4.3: ОСНОВНАЯ ЛОГИКА СТАБИЛЬНОСТИ ============================================
 private void ProcessFactionsStability()
{
	HashSet<string> factionsAttackedThisTick = new HashSet<string>();
    float influenceMult = DynamicFactionsMod.settings.influenceMultiplier;
    HashSet<string> ignoredDefNames = new HashSet<string> { "TradersGuild", "Salvagers" };
    
    // Идеологический раскол (глобально)
    if (ModsConfig.IdeologyActive)
    {
        var allFactionsWithIdeo = Find.FactionManager.AllFactions
            .Where(f => !f.IsPlayer && !f.defeated && f.ideos?.PrimaryIdeo != null)
            .GroupBy(f => f.ideos.PrimaryIdeo)
            .Where(g => g.Count() > 5)
            .ToList();
        
        if (allFactionsWithIdeo.Count > 0)
        {
            var schismGroup = allFactionsWithIdeo.RandomElement();
            Ideo oldIdeo = schismGroup.Key;
            var ideoFactions = schismGroup.ToList();
            int factionCount = ideoFactions.Count;
            float schismChance = 0.01f + (factionCount - 5) * 0.05f;
            
            if (Rand.Value < schismChance)
            {
                Log.Warning($"[DF] ☯️ РАСКОЛ {oldIdeo.name} ({factionCount} фракций)");
                var schismFactions = ideoFactions.OrderBy(x => Guid.NewGuid()).Take(factionCount / 3).ToList();
                foreach (Faction schismFaction in schismFactions)
                {
                    Ideo newIdeo;
                    float newIdeoRoll = Rand.Value;
                    if (newIdeoRoll < 0.5f)
                    {
                        try
                        {
                            newIdeo = IdeoGenerator.GenerateIdeo(new IdeoGenerationParms(schismFaction.def));
                            Find.IdeoManager.Add(newIdeo);
                        }
                        catch
                        {
                            newIdeo = Find.FactionManager.AllFactions
                                .Where(f => f.ideos?.PrimaryIdeo != null && f.ideos.PrimaryIdeo != oldIdeo)
                                .Select(f => f.ideos.PrimaryIdeo).Distinct().RandomElementWithFallback(oldIdeo);
                        }
                    }
                    else
                    {
                        newIdeo = Find.FactionManager.AllFactions
                            .Where(f => f.ideos?.PrimaryIdeo != null && f.ideos.PrimaryIdeo != oldIdeo)
                            .Select(f => f.ideos.PrimaryIdeo).Distinct()
                            .RandomElementWithFallback(oldIdeo);
                    }
                    schismFaction.ideos.SetPrimary(newIdeo);
                    Find.LetterStack.ReceiveLetter("Идеологический раскол", 
                        $"{schismFaction.Name} покинула {oldIdeo.name}!\nНовая вера: {newIdeo.name}", 
                        LetterDefOf.NeutralEvent);
                }
            }
        }
    }
            
    List<Settlement> allSettlements = Find.WorldObjects.Settlements;
var relevantFactions = Find.FactionManager.AllFactions
    .Where(f => !f.IsPlayer 
             && !f.defeated 
             && f.def.humanlikeFaction 
             && !IgnoredFactionNames.Contains(f.def.defName)
             && !ignoredDefNames.Contains(f.def.defName)
             && Find.WorldObjects.Settlements.Any(s => s.Faction == f && s.Faction != null && !s.Destroyed)
             && f != Faction.OfPlayer)
    .ToList();
    
if (relevantFactions.Count == 0)
{
    // В ПУСТОМ МИРЕ используем НАСТРОЙКУ шанса спавна
    if (Rand.Value < DynamicFactionsMod.settings.randomSettlementChance / 100f)
    {
        TrySpawnRandomSettlement();
    }
    return;
}

    
// Обрабатываем ВСЕ фракции
List<Faction> randomFactions = relevantFactions.ToList();

// Выбираем одну случайную фракцию для событий
Faction eventFaction = null;
if (randomFactions.Count > 0)
{
    eventFaction = randomFactions.RandomElement();
    
    if (DynamicFactionsMod.settings.showDebugLogs)
    {
        Log.Message($"[DF] 🎲 ФРАКЦИЯ ДЛЯ СОБЫТИЙ: {eventFaction.Name}");
    }
}

// Вариант 2 (более информативный):
if (DynamicFactionsMod.settings.showDebugLogs)
{
    Log.Message($"[DF] 📊 РАСЧЕТ ВСЕХ {randomFactions.Count} ФРАКЦИЙ (из {relevantFactions.Count} доступных)");
}
            
            foreach (var faction in randomFactions)
            {
				if (factionsAttackedThisTick.Contains(faction.def.defName)) continue;
                FactionStabilityData data = GetOrCreateData(faction);
				bool allowEvents = (faction == eventFaction);
                string factionHeader = $"🔹 {faction.Name} | Было: {data.stabilityPoints:F1} | Лидер: {data.leaderLegitimacy:F1}";
				float changeSettlements = 0f;
				bool eventOccurred = false;

// 1. Поселения (+15/-30) ← ОСТАВИТЬ КАК ЕСТЬ
int currentSettlements = allSettlements.Count(s => s.Faction == faction);
int diff = currentSettlements - data.lastSettlementCount;
if (currentSettlements == 0) continue;
if (data.lastSettlementCount > 0 && diff != 0)
{
    changeSettlements = diff > 0 ? diff * 15f : -(Mathf.Abs(diff) * 30f);
    if (faction.leader != null)
    {
        if (diff > 0) data.leaderLegitimacy += 5.0f * diff;
        else data.leaderLegitimacy -= 5.0f * Mathf.Abs(diff);
        data.leaderLegitimacy = Mathf.Clamp(data.leaderLegitimacy, -20f, 20f);
    }
    data.stabilityPoints += changeSettlements;
}
data.lastSettlementCount = currentSettlements;

// 2. Лидер (смена + пассивка) ← ОСТАВИТЬ КАК ЕСТЬ
if (faction.leader != data.lastLeader)
{
    if (data.lastLeader != null)
    {
        float deathImpact = -10f * data.leaderLegitimacy;  // ← +50/-50 при смерти!
        data.stabilityPoints += deathImpact;
		data.influencePoints += deathImpact;
    }
    data.lastLeader = faction.leader;
    data.leaderStartTick = Find.TickManager.TicksGame;
    data.leaderLegitimacy = Rand.Range(-5.0f, 5.0f);
}
else if (faction.leader != null)
{
    data.leaderLegitimacy += 0.1f;
    data.leaderLegitimacy = Mathf.Clamp(data.leaderLegitimacy, -20f, 20f);
}

// 🔥 ПАССИВКА (НОВОЕ)
float leaderStableBonus = (faction.leader != null ? data.leaderLegitimacy / 5f : 0f);
float baseBonus = currentSettlements * 1f;
data.stabilityPoints += leaderStableBonus + baseBonus;

data.stabilityPoints = Mathf.Clamp(data.stabilityPoints, -100f, 100f);

// ============================================
// 🎲 РУЛЕТКА ВСТАВИТЬ СЮДА (СРАЗУ ПОСЛЕ CLAMP)
// ============================================
string selectedEvent = "None";
if (allowEvents && !eventOccurred)
{
    List<string> roulette = new List<string>();
    if (Mathf.Abs(data.influencePoints) > 50f) roulette.Add("Influence");
    if (data.attackPoints > 50f) roulette.Add("Attack");
    if (Mathf.Abs(data.stabilityPoints) > 50f) roulette.Add("Stability");
    if (roulette.Count > 0) selectedEvent = roulette.RandomElement();
}

// Вспомогательная функция для смерти лидера
void KillLeaderNegotiationsStyle(Faction who, FactionStabilityData whoData, float legitimacyMin, float legitimacyMax)
{
    if (who == null || who.leader == null) return;

    Pawn oldLeader = who.leader;
    string oldLeaderName = oldLeader.Name.ToStringFull;

    Find.WorldPawns.RemovePawn(oldLeader);
    oldLeader.Destroy();

#pragma warning disable CS0618
    who.GenerateNewLeader();
#pragma warning restore CS0618

    whoData.lastLeader = who.leader;
    whoData.leaderLegitimacy = Rand.Range(legitimacyMin, legitimacyMax);

    if (DynamicFactionsMod.settings.showDebugLogs)
    {
        Log.Message($"[DF] ☠️ Лидер погиб: {who.Name} ({oldLeaderName}) → новый лидер назначен");
    }
}            

// ---------------- ВЛИЯНИЕ (ШАНСОВАЯ СИСТЕМА) ----------------
int allyCount = Find.FactionManager.AllFactions.Count(f => 
    f != faction && !f.IsPlayer && !f.defeated && !f.Hidden && 
    f.def.defName != "TradersGuild" && f.def.defName != "Salvagers" && 
    !faction.HostileTo(f));

int sameIdeoCount = 0;
if (ModsConfig.IdeologyActive && faction.ideos?.PrimaryIdeo != null)
{
    sameIdeoCount = Find.FactionManager.AllFactions.Count(other => 
        other != faction && !other.defeated && 
        other.ideos?.PrimaryIdeo == faction.ideos.PrimaryIdeo);
}

float leaderInfPassive = (faction.leader != null ? data.leaderLegitimacy / 5f : 0f);

float influenceChange = (allyCount * 1f) + (sameIdeoCount * 2f) + leaderInfPassive;
influenceChange *= DynamicFactionsMod.settings.influenceMultiplier;
data.influencePoints += influenceChange;

// ЛОГИКА ШАНСА ВЛИЯНИЯ
float infChance = 0f;
if (data.influencePoints > 50f)
{
    infChance = (data.influencePoints - 50f) / 100f; // 70 -> 0.2 (20%)
}
else if (data.influencePoints < -50f)
{
    infChance = (Mathf.Abs(data.influencePoints) - 50f) / 100f;
}

// Проверяем событие влияния только если не было других событий
if (selectedEvent == "Influence" && allowEvents && !eventOccurred && infChance > 0 && Rand.Value < infChance)
{
    if (data.influencePoints > 50f)
    {
        TriggerInfluenceEvent(faction, data, true);
        data.influencePoints -= 100f;
    }
    else
    {
        TriggerInfluenceEvent(faction, data, false);
        data.influencePoints += 100f;
    }
    eventOccurred = true; // Отмечаем, что событие произошло
}


// ============================================
// ЕДИНЫЙ БЛОК: МГНОВЕННАЯ АТАКА (сохраняет всю старую логику)
// ============================================



// 1. Проверка готовности к войне (Порог 50) - сразу атака
if (selectedEvent == "Attack" && allowEvents && !eventOccurred && data.attackPoints > 50f)
{
    float excessPoints = data.attackPoints - 50f;
    float attackChance = excessPoints / 100f;
    
    if (Rand.Value < attackChance)
    {
        eventOccurred = true; 
        Faction enemyTarget = null;
        string reason = "агрессия";
	
        // ------------------------------------------------------------
        // ПОИСК ЦЕЛИ (полная логика из старого кода)
        // ------------------------------------------------------------
        var mySettlements = Find.WorldObjects.Settlements.Where(s => s.Faction == faction).ToList();
        
        if (mySettlements.Count > 0)
        {
            var validEnemies = new List<(Faction fac, float dist, TechLevel tech)>();
            Settlement myBase = mySettlements.RandomElement();

            // А) Сбор врагов
            foreach (var f in Find.FactionManager.AllFactions)
            {   
                if (IgnoredFactionNames.Contains(f.def.defName)) continue;
                if (f.IsPlayer || f == Faction.OfPlayer || f == faction || f.defeated || f.Hidden 
                    || f.def.defName == "TradersGuild" || f.def.defName == "Salvagers") continue;

                if (ModsConfig.IdeologyActive && faction.ideos?.PrimaryIdeo != null 
                    && f.ideos?.PrimaryIdeo == faction.ideos.PrimaryIdeo) continue;

                var enemyBases = Find.WorldObjects.Settlements.Where(s => s.Faction == f).ToList();
                if (enemyBases.Count == 0) continue;

                if (!faction.HostileTo(f)) continue; // Только враги

                float minDist = 9999f;
                foreach (var eb in enemyBases)
                {
                    float d = Find.WorldGrid.ApproxDistanceInTiles(myBase.Tile, eb.Tile);
                    if (d < minDist) minDist = d;
                }
                validEnemies.Add((f, minDist, f.def.techLevel));
            }

            // Б) Потенциальные "не-враги" для конфликта (10%)
            var potentialConflicts = new List<(Faction fac, float dist, string reason)>();
            
            foreach (var f in Find.FactionManager.AllFactions)
            {
                if (IgnoredFactionNames.Contains(f.def.defName)) continue;
                if (f.IsPlayer || f == Faction.OfPlayer || f == faction || f.defeated || f.Hidden 
                    || f.def.defName == "TradersGuild" || f.def.defName == "Salvagers") continue;

                var otherBases = Find.WorldObjects.Settlements.Where(s => s.Faction == f).ToList();
                if (otherBases.Count == 0) continue;

                // НЕ враги, но близко + есть конфликт
                if (faction.HostileTo(f)) continue; // враги уже в validEnemies

                float minDist = otherBases.Min(os => Find.WorldGrid.ApproxDistanceInTiles(myBase.Tile, os.Tile));
                if (minDist > 50f) continue; // слишком далеко для конфликта

                string conflictReason = "";
                float conflictChance = Rand.Value;

                if (conflictChance < 0.3f)
                    conflictReason = "дипломатический инцидент";
                else if (conflictChance < 0.6f)
                    conflictReason = "пограничный спор";
                else if (conflictChance < 0.8f && ModsConfig.IdeologyActive)
                    conflictReason = "идеологический спор";
                else
                    conflictReason = "экономическая конкуренция";

                potentialConflicts.Add((f, minDist, conflictReason));
            }

            // В) Логика Идеологии (полная из старого кода)
            Ideo threatIdeo = null;
            bool massiveThreat = false;
            bool moderateThreat = false;
            if (ModsConfig.IdeologyActive && faction.ideos?.PrimaryIdeo != null)
            {
                var ideoCounts = new Dictionary<Ideo, int>();
                foreach (var f in Find.FactionManager.AllFactions)
                {
                    if (IgnoredFactionNames.Contains(f.def.defName)) continue;
                    if (f.defeated || f.Hidden || f.IsPlayer || f.ideos?.PrimaryIdeo == null) continue;
                    Ideo i = f.ideos.PrimaryIdeo;
                    if (!ideoCounts.ContainsKey(i)) ideoCounts[i] = 0;
                    ideoCounts[i]++;
                }
                foreach (var kvp in ideoCounts)
                {
                    if (kvp.Key == faction.ideos.PrimaryIdeo) continue;
                    if (kvp.Value > 5) { threatIdeo = kvp.Key; massiveThreat = true; break; }
                    else if (kvp.Value > 3) { threatIdeo = kvp.Key; moderateThreat = true; }
                }
            }

            // Г) Выбор цели (полная логика приоритетов)
            if (massiveThreat && threatIdeo != null)
            {
                var cultEnemies = validEnemies.Where(x => x.fac.ideos?.PrimaryIdeo == threatIdeo).ToList();
                if (cultEnemies.Count > 0)
                {
                    enemyTarget = cultEnemies.OrderBy(x => x.dist).First().fac;
                    reason = $"угроза доминирования идеологии {threatIdeo.name}";
                }
            }
            else if (moderateThreat && threatIdeo != null && Rand.Value < 0.50f)
            {
                var worthyCultEnemies = validEnemies.Where(x => x.fac.ideos?.PrimaryIdeo == threatIdeo && x.tech >= faction.def.techLevel).ToList();
                if (worthyCultEnemies.Count > 0)
                {
                    enemyTarget = worthyCultEnemies.OrderBy(x => x.dist).First().fac;
                    reason = $"сдерживание идеологии {threatIdeo.name}";
                }
            }

            // Д) Стратегия (если нет идео-угрозы)
            if (enemyTarget == null)
            {
                float strategyRoll = Rand.Value;
                if (strategyRoll < 0.80f) // 80% техно-соперничество
                {
                    var worthyEnemies = validEnemies.Where(x => x.tech >= faction.def.techLevel).ToList();
                    if (worthyEnemies.Count > 0)
                    {
                        enemyTarget = worthyEnemies.OrderBy(x => x.dist).First().fac;
                        reason = "технологическое соперничество";
                    }
                    else if (validEnemies.Count > 0)
                    {
                        enemyTarget = validEnemies[0].fac; // ближайший
                        reason = "территориальный спор";
                    }
                }
                else // 20% ближайший
                {
                    if (validEnemies.Count > 0)
                    {
                        enemyTarget = validEnemies[0].fac;
                        reason = "территориальный спор";
                    }
                }
            }

            // Е) 10% случайный конфликт с НЕ-врагом
            if (enemyTarget == null && Rand.Value < 0.10f && potentialConflicts.Count > 0)
            {
                var conflict = potentialConflicts.OrderBy(x => x.dist).First();
                enemyTarget = conflict.fac;
                reason = conflict.reason;
            }
        }

        // ------------------------------------------------------------
        // ЕСЛИ ЦЕЛЬ НАЙДЕНА - ПОЛНАЯ ЛОГИКА БОЯ (как в старом коде)
        // ------------------------------------------------------------
        // Если цель найдена, проверяем кулдаун:
        if (enemyTarget != null)
        {
            // ✅ ПРОВЕРКА КУЛДАУНА 30 ДНЕЙ
            if (data.lastPeaceFaction == enemyTarget && 
                data.lastPeaceTick > 0 && 
                Find.TickManager.TicksGame - data.lastPeaceTick < 30 * 60000)
            {
                if (DynamicFactionsMod.settings.showDebugLogs)
                {
                    Log.Message($"[DF] ⏳ {faction.Name} пропускает атаку на {enemyTarget.Name} (30 дней мира)");
                }
                data.attackPoints *= 0.8f; // Штраф за нерешительность
                // Выходим из блока атаки
                enemyTarget = null;
            }
        }

        // Если enemyTarget стал null, пропускаем атаку
        if (enemyTarget == null)
        {
            data.attackPoints *= 0.8f; // Штраф за нерешительность
            // Выходим из блока атаки
        }
        else
        {
            // Если enemyTarget не null, продолжаем обычную логику
            FactionStabilityData targetData = GetOrCreateData(enemyTarget);
			
            // Полный расчет переговоров (как в старом коде)
            float defLeader = targetData.leaderLegitimacy;
            float defInfluence = targetData.influencePoints;
            float defStability = targetData.stabilityPoints;
            float defDefense = targetData.defensePoints;
            
            float atkLeader = data.leaderLegitimacy;
            float atkInfluence = data.influencePoints;
            float atkStability = data.stabilityPoints;
            float atkAttack = data.attackPoints;

            float negotiationChance = (defLeader + defInfluence + defStability + defDefense 
                                     - atkLeader - atkInfluence - atkStability - atkAttack) / 100f;
            negotiationChance = Mathf.Clamp01(negotiationChance);
            
            if (DynamicFactionsMod.settings.showDebugLogs)
            {
                Log.Message($"[DF] 🗣️ Переговоры {faction.Name}→{enemyTarget.Name}: {negotiationChance:P1}");
            }
            
            // МИР (полная логика из старого кода)
if (Rand.Value < negotiationChance)
{
    data.attackPoints *= 0.5f;
    data.stabilityPoints += 30f;
    targetData.influencePoints += 10f;
    targetData.stabilityPoints += 10f;
    
    // ✅ СЛУЧАЙНАЯ ПРОДОЛЖИТЕЛЬНОСТЬ МИРА: 10-100 дней
    int peaceDays = Rand.RangeInclusive(10, 100);
    data.lastPeaceFaction = enemyTarget;
    data.lastPeaceTick = Find.TickManager.TicksGame + peaceDays * 60000;
    
    Find.LetterStack.ReceiveLetter("Мир заключён", 
        $"{faction.Name} и {enemyTarget.Name} заключили мир после переговоров!\nШанс: {negotiationChance:P0}%", 
        LetterDefOf.PositiveEvent);
}
            // БОЙ (полная логика из старого кода)
            else
            {
if (!faction.HostileTo(enemyTarget))
{
    // Используем TryAffectGoodwillWith для фракций с goodwill
    if (faction.def.humanlikeFaction && enemyTarget.def.humanlikeFaction)
    {
        faction.TryAffectGoodwillWith(enemyTarget, -100);
        enemyTarget.TryAffectGoodwillWith(faction, -100);
    }
    else
    {
        // Для фракций без goodwill оставляем SetRelationDirect
        faction.SetRelationDirect(enemyTarget, FactionRelationKind.Hostile);
    }
}

                // Полный расчет боя с технологиями
                float techAtkMult = BattleTechMultipliers.TryGetValue(faction.def.techLevel, out float ta) ? ta : 1f;
                float techDefMult = BattleTechMultipliers.TryGetValue(enemyTarget.def.techLevel, out float td) ? td : 1f;

                float battleAtkLeader = data.leaderLegitimacy;
                float battleAtkAttack = data.attackPoints * techAtkMult;
                float battleDefLeader = targetData.leaderLegitimacy;
                float battleDefDefense = targetData.defensePoints * techDefMult;

                float successChance = (battleAtkLeader + battleAtkAttack - battleDefLeader - battleDefDefense) / 100f;
                successChance = Mathf.Clamp01(successChance);

                if (DynamicFactionsMod.settings.showDebugLogs)
                {
                    Log.Message($"[DF] ⚔️ {faction.Name} T{techAtkMult:F1} vs {enemyTarget.Name} T{techDefMult:F1}: " +
                                $"({battleAtkLeader:F0}+{battleAtkAttack:F0}-{battleDefLeader:F0}-{battleDefDefense:F0}) = {successChance:P1}");
                }

                // Вспомогательная функция: шанс смерти лидера защитника 15%
                Action MaybeKillDefenderLeader15 = () => {
                    if (Rand.Value < 0.15f)
                    {
                        KillLeaderNegotiationsStyle(enemyTarget, targetData, -8f, -2f);
                    }
                };

                // ПОБЕДА
                if (Rand.Value < successChance)
                {
                    data.attackPoints *= 0.70f;
                    targetData.defensePoints *= 0.40f;

                    if (enemyTarget.defeated) return;

                    List<Settlement> targetBases = Find.WorldObjects.Settlements.Where(s => s.Faction == enemyTarget).ToList();
                    if (targetBases.Count == 0) return;

                    List<Settlement> attackerBases = Find.WorldObjects.Settlements.Where(s => s.Faction == faction).ToList();

                    TechLevel atkTech = faction.def.techLevel;
                    bool isBarbarian = (atkTech <= TechLevel.Medieval);
                    float roll = Rand.Value;

                    if (isBarbarian)
                    {
                        // 1) 10% Пиррова победа
                        if (roll < 0.10f)
                        {
                            KillLeaderNegotiationsStyle(faction, data, -8f, -2f);
                            data.influencePoints -= 30f;

                            CaptureBases(faction, targetBases, 1, "Пиррова победа",
                                $"Победа далась страшной ценой: лидер {faction.Name} погиб в бою.\n" +
                                $"Влияние ослабло (-30), но армия захватила ближайшую базу врага.");
                        }
                        // 2) 40% Захват 1-3 ближайших + 15% смерть лидера защитника
                        else if (roll < 0.10f + 0.40f)
                        {
                            int count = Mathf.Min(targetBases.Count, Rand.RangeInclusive(1, 3));
                            MaybeKillDefenderLeader15();

                            CaptureBases(faction, targetBases, count, "Вторжение",
                                $"{faction.Name} прорвала оборону и захватила {count} ближайших поселений {enemyTarget.Name}.");
                        }
                        // 3) 20% Обращение в идеологию без потерь, защитник -50 влияния
                        else if (roll < 0.10f + 0.40f + 0.20f)
                        {
                            if (ModsConfig.IdeologyActive && faction.ideos?.PrimaryIdeo != null && enemyTarget.ideos != null)
                            {
                                enemyTarget.ideos.SetPrimary(faction.ideos.PrimaryIdeo);
                            }
                            targetData.influencePoints -= 50f;

                            Find.LetterStack.ReceiveLetter("Идеологическая капитуляция",
                                $"{enemyTarget.Name} признала культурное превосходство {faction.Name} и приняла их идеологию.\n\n" +
                                $"• {enemyTarget.Name}: -50 влияния",
                                LetterDefOf.NeutralEvent);

                            data.attackPoints *= 0.85f;
                        }
                        // 4) 20% Уничтожение 1 базы -> руины
                        else if (roll < 0.10f + 0.40f + 0.20f + 0.20f)
                        {
                            targetData.stabilityPoints -= 30f;

                            DestroyBases(targetBases, 1, "Варварская расправа",
                                $"{faction.Name} уничтожила поселение {enemyTarget.Name}.\n" +
                                $"Выжившие рассказывают о чудовищных зверствах: убиты дети, женщины подверглись насилию.\n\n" +
                                $"• {enemyTarget.Name}: -30 стабильности (паника)");
                        }
                        // 5) 10% "Война пришла в дом": обмен ближайшими базами
                        else
                        {
                            if (attackerBases.Count == 0 || targetBases.Count == 0)
                            {
                                CaptureBases(faction, targetBases, 1, "Налёт",
                                    $"{faction.Name} воспользовалась хаосом и захватила ближайшую базу {enemyTarget.Name}.");
                            }
                            else
                            {
                                Settlement attackerRef = attackerBases.RandomElement();
                                Settlement defenderRef = targetBases.RandomElement();

                                Settlement defendersClosestToAttacker = targetBases
                                    .OrderBy(s => Find.WorldGrid.ApproxDistanceInTiles(attackerRef.Tile, s.Tile)).First();
                                Settlement attackersClosestToDefender = attackerBases
                                    .OrderBy(s => Find.WorldGrid.ApproxDistanceInTiles(defenderRef.Tile, s.Tile)).First();

                                // захват защитником базы атакующего
                                int aTile = attackersClosestToDefender.Tile;
                                string aName = attackersClosestToDefender.Name;
                                Find.WorldObjects.Remove(attackersClosestToDefender);
                                Settlement aNew = (Settlement)WorldObjectMaker.MakeWorldObject(WorldObjectDefOf.Settlement);
                                aNew.SetFaction(enemyTarget);
                                aNew.Tile = aTile;
                                aNew.Name = aName + " (захвачена)";
                                Find.WorldObjects.Add(aNew);

                                // захват атакующим базы защитника
                                int dTile = defendersClosestToAttacker.Tile;
                                string dName = defendersClosestToAttacker.Name;
                                Find.WorldObjects.Remove(defendersClosestToAttacker);
                                Settlement dNew = (Settlement)WorldObjectMaker.MakeWorldObject(WorldObjectDefOf.Settlement);
                                dNew.SetFaction(faction);
                                dNew.Tile = dTile;
                                dNew.Name = dName + " (захвачена)";
                                Find.WorldObjects.Add(dNew);

                                Find.LetterStack.ReceiveLetter("Война пришла в дом",
                                    $"Фронт рухнул, и война пришла в тыл.\n" +
                                    $"{faction.Name} захватила {dName}, а {enemyTarget.Name} в ответ захватила {aName}.",
                                    LetterDefOf.NeutralEvent, new GlobalTargetInfo(dTile));
                            }
                        }
                    }
                    else // Industrial+
                    {
                        // 1) 10% Пиррова победа
                        if (roll < 0.10f)
                        {
                            KillLeaderNegotiationsStyle(faction, data, -8f, -2f);
                            data.influencePoints -= 30f;

                            CaptureBases(faction, targetBases, 1, "Пиррова победа",
                                $"Победа далась ценой жизни командующего {faction.Name}.\n" +
                                $"Влияние ослабло (-30), но операция завершилась захватом ближайшей базы.");
                        }
                        // 2) 35% Захват 1-3 ближайших + 15% смерть лидера защитника
                        else if (roll < 0.10f + 0.35f)
                        {
                            int count = Mathf.Min(targetBases.Count, Rand.RangeInclusive(1, 3));
                            MaybeKillDefenderLeader15();

                            CaptureBases(faction, targetBases, count, "Блицкриг",
                                $"{faction.Name} провела успешную операцию: захвачено {count} ближайших баз {enemyTarget.Name}.");
                        }
                        // 3) 15% Обращение в идеологию
                        else if (roll < 0.10f + 0.35f + 0.15f)
                        {
                            if (ModsConfig.IdeologyActive && faction.ideos?.PrimaryIdeo != null && enemyTarget.ideos != null)
                            {
                                enemyTarget.ideos.SetPrimary(faction.ideos.PrimaryIdeo);
                            }
                            targetData.influencePoints -= 50f;

                            Find.LetterStack.ReceiveLetter("Информационная победа",
                                $"{faction.Name} сломила сопротивление {enemyTarget.Name} через пропаганду и дипломатию.\n\n" +
                                $"• {enemyTarget.Name}: -50 влияния",
                                LetterDefOf.NeutralEvent);

                            data.attackPoints *= 0.85f;
                        }
                        // 4) 20% Разрушить 1 базу -> руины + 15% смерть лидера защитника
                        else if (roll < 0.10f + 0.35f + 0.15f + 0.20f)
                        {
                            MaybeKillDefenderLeader15();

                            DestroyBases(targetBases, 1, "Точечный удар",
                                $"{faction.Name} нанесла точечный удар по инфраструктуре {enemyTarget.Name}.\n" +
                                $"Одна база уничтожена и превратилась в руины.");
                        }
                        // 5) 15% Ракетный удар
                        else if (roll < 0.10f + 0.35f + 0.15f + 0.20f + 0.15f)
                        {
                            int count = Mathf.Min(targetBases.Count, Rand.RangeInclusive(1, 3));

                            MaybeKillDefenderLeader15();
                            targetData.stabilityPoints -= 30f;

                            DestroyBases(targetBases, count, "Ракетный удар",
                                $"{faction.Name} нанесла ракетный удар.\n" +
                                $"Уничтожено {count} баз {enemyTarget.Name}; территория превратилась в руины.\n\n" +
                                $"• {enemyTarget.Name}: -30 стабильности (шок)");
                        }
                        // 6) 5% Ядерная война
                        else
                        {
                            // Ядерный удар по ВСЕМ фракциям
                            var factionsToNuke = Find.FactionManager.AllFactions.Where(f => !f.IsPlayer && !f.defeated).ToList();
                            
                          foreach (Faction f in factionsToNuke)
{
    var fSettlements = Find.WorldObjects.Settlements.Where(s => s.Faction == f).ToList();
    if (fSettlements.Count == 0) continue;
    
    // ✅ СЛУЧАЙНЫЙ диапазон ДЛЯ КАЖДОЙ фракции
    int minDestroy = Verse.Rand.RangeInclusive(2, 6);
    int maxDestroy = Verse.Rand.RangeInclusive(5, 10);
    int toDestroy = Verse.Rand.RangeInclusive(minDestroy, Mathf.Min(maxDestroy, fSettlements.Count));
    
    var nukedBases = fSettlements.InRandomOrder().Take(toDestroy).ToList();
    
    foreach (Settlement s in nukedBases)

                                {
                                    SpawnRuins(s.Tile, s.Name);
                                    Find.WorldObjects.Remove(s);
                                }
                                
                                FactionStabilityData fData = GetOrCreateData(f);
                                fData.attackPoints *= 0.2f;
                                fData.defensePoints *= 0.2f;
                                fData.stabilityPoints -= 50f;
                            }
                            
                            Find.LetterStack.ReceiveLetter("ЯДЕРНАЯ ВОЙНА", 
                                "Война. Война никогда не меняется...", 
                                LetterDefOf.ThreatBig);
								
// ✅ Toxic Fallout для ИГРОКА (30-60 дней)
Map playerMap = Find.AnyPlayerHomeMap;
if (playerMap != null)
{
    IncidentParms parms = StorytellerUtility.DefaultParmsNow(IncidentCategoryDefOf.ThreatBig, playerMap);
    parms.target = playerMap;
    
    IncidentDef falloutDef = DefDatabase<IncidentDef>.GetNamed("ToxicFallout");
    if (falloutDef != null)
    {
        falloutDef.Worker.TryExecute(parms);  // ← Без durationDays!
    }
}
								
                            return;
                        }
                    }
                }
// ПОРАЖЕНИЕ
else
{
    // Удаляем старый фиксированный код и заменяем на систему событий
    float outcomeRoll = Rand.Value;
    
    if (outcomeRoll < 0.50f) // 50% - Оборона
    {
        data.attackPoints = Mathf.Max(0, data.attackPoints - 20f);
        targetData.defensePoints = Mathf.Max(0, targetData.defensePoints - 20f);
        
        data.influencePoints -= 15f;
        data.stabilityPoints -= 5f;
        targetData.influencePoints += 10f;
        targetData.stabilityPoints += 5f;
        
        Find.LetterStack.ReceiveLetter("🛡️ Оборона", 
            $"{enemyTarget.Name} успешно оборонялся от {faction.Name}!\n" +
            $"• {faction.Name}: -20 атаки (теперь {data.attackPoints:F0})\n" +
            $"• {enemyTarget.Name}: -20 защиты (теперь {targetData.defensePoints:F0})", 
            LetterDefOf.NeutralEvent);
    }
    else if (outcomeRoll < 0.70f) // 20% - Тяжелая оборона
    {
        data.attackPoints = Mathf.Max(0, data.attackPoints - 20f);
        targetData.defensePoints = Mathf.Max(0, targetData.defensePoints - 40f);
        
        data.influencePoints -= 20f;
        data.stabilityPoints -= 10f;
        targetData.influencePoints += 15f;
        targetData.stabilityPoints += 10f;
        
        Find.LetterStack.ReceiveLetter("🛡️ Тяжелая оборона", 
            $"{enemyTarget.Name} отразил атаку {faction.Name}, но с большими потерями в обороне!\n" +
            $"• {faction.Name}: -20 атаки (теперь {data.attackPoints:F0})\n" +
            $"• {enemyTarget.Name}: -40 защиты (теперь {targetData.defensePoints:F0})", 
            LetterDefOf.NeutralEvent);
    }
    else if (outcomeRoll < 0.90f) // 20% - Отражено
    {
        data.attackPoints = Mathf.Max(0, data.attackPoints - 20f);
        
        data.influencePoints -= 25f;
        data.stabilityPoints -= 15f;
        targetData.influencePoints += 20f;
        targetData.stabilityPoints += 15f;
        
        Find.LetterStack.ReceiveLetter("🛡️ Отражено", 
            $"{enemyTarget.Name} полностью отразил атаку {faction.Name} без потерь в обороне!\n" +
            $"• {faction.Name}: -20 атаки (теперь {data.attackPoints:F0})\n" +
            $"• {enemyTarget.Name}: оборона не повреждена ({targetData.defensePoints:F0})", 
            LetterDefOf.PositiveEvent);
    }
    else // 10% - Контрнаступление
    {
        // Основные потери очков
        data.attackPoints = Mathf.Max(0, data.attackPoints - 20f);
        data.defensePoints = Mathf.Max(0, data.defensePoints - 20f);
        targetData.attackPoints = Mathf.Max(0, targetData.attackPoints - 20f);
        
        data.influencePoints -= 30f;
        data.stabilityPoints -= 20f;
        targetData.influencePoints += 25f;
        targetData.stabilityPoints += 20f;
        
        // Захват базы атакующего
        var attackerBases = Find.WorldObjects.Settlements.Where(s => s.Faction == faction).ToList();
        Settlement capturedBase = null;
        
        if (attackerBases.Count > 0)
        {
            // Находим ближайшую к линии фронта базу для захвата
            var enemyBases = Find.WorldObjects.Settlements.Where(s => s.Faction == enemyTarget).ToList();
            if (enemyBases.Count > 0)
            {
                Settlement referenceEnemyBase = enemyBases[0];
                capturedBase = attackerBases
                    .OrderBy(s => Find.WorldGrid.ApproxDistanceInTiles(referenceEnemyBase.Tile, s.Tile))
                    .FirstOrDefault();
            }
            else
            {
                capturedBase = attackerBases.RandomElement();
            }
            
            if (capturedBase != null)
            {
                int tile = capturedBase.Tile;
                string oldName = capturedBase.Name;
                
                // Удаляем старое и создаем новое поселение
                Find.WorldObjects.Remove(capturedBase);
                Settlement newBase = (Settlement)WorldObjectMaker.MakeWorldObject(WorldObjectDefOf.Settlement);
                newBase.SetFaction(enemyTarget);
                newBase.Tile = tile;
                newBase.Name = oldName + " (захвачено)";
                Find.WorldObjects.Add(newBase);
                
                Find.LetterStack.ReceiveLetter("🛡️ Контрнаступление", 
                    $"{enemyTarget.Name} не только отразил атаку, но и перешел в контрнаступление!\n\n" +
                    $"• {faction.Name}: -20 атаки, -20 защиты\n" +
                    $"• {enemyTarget.Name}: -20 атаки\n" +
                    $"• Захвачена база: {oldName} → теперь принадлежит {enemyTarget.Name}", 
                    LetterDefOf.PositiveEvent, newBase);
            }
            else
            {
                Find.LetterStack.ReceiveLetter("🛡️ Контрнаступление", 
                    $"{enemyTarget.Name} не только отразил атаку, но и перешел в контрнаступление!\n" +
                    $"• {faction.Name}: -20 атаки, -20 защиты\n" +
                    $"• {enemyTarget.Name}: -20 атаки\n" +
                    $"• Захватить базу не удалось (ошибка)", 
                    LetterDefOf.PositiveEvent);
            }
        }
        else
        {
            Find.LetterStack.ReceiveLetter("🛡️ Контрнаступление", 
                $"{enemyTarget.Name} не только отразил атаку, но и перешел в контрнаступление!\n" +
                $"• {faction.Name}: -20 атаки, -20 защиты\n" +
                $"• {enemyTarget.Name}: -20 атаки\n" +
                $"• {faction.Name} не имеет баз для захвата", 
                LetterDefOf.PositiveEvent);
        }
    }
    
    // Общие изменения для всех исходов поражения
    data.attackPoints = Mathf.Clamp(data.attackPoints, 0f, 200f);
    data.defensePoints = Mathf.Clamp(data.defensePoints, 0f, 200f);
    targetData.attackPoints = Mathf.Clamp(targetData.attackPoints, 0f, 200f);
    targetData.defensePoints = Mathf.Clamp(targetData.defensePoints, 0f, 200f);
    
    data.influencePoints = Mathf.Clamp(data.influencePoints, -200f, 200f);
    data.stabilityPoints = Mathf.Clamp(data.stabilityPoints, -100f, 100f);
    targetData.influencePoints = Mathf.Clamp(targetData.influencePoints, -200f, 200f);
    targetData.stabilityPoints = Mathf.Clamp(targetData.stabilityPoints, -100f, 100f);
}



            }
        }
     
    }
}
             
// ---------------- АТАКА И БРОНЯ (С ФОКУСОМ) ----------------
bool militaryFocusAttack = Rand.Value < 0.5f;
float leaderMilitaryBonus = faction.leader != null ? data.leaderLegitimacy / 5f : 0f;  // твой вариант, или *0.1f

int enemyFactionCount = Find.FactionManager.AllFactions.Count(f => 
    f != faction && !f.IsPlayer && !f.defeated && f.def.humanlikeFaction && 
    faction.HostileTo(f));

int allyFactionCount = Find.FactionManager.AllFactions.Count(f => 
    f != faction && !f.IsPlayer && !f.defeated && f.def.humanlikeFaction && 
    !faction.HostileTo(f));

if (militaryFocusAttack)
{
    float newAttackGain = (enemyFactionCount + 1f) + leaderMilitaryBonus;  // +1f базовый
    newAttackGain *= DynamicFactionsMod.settings.attackMultiplier;
    data.attackPoints += newAttackGain;
}
else
{
    float newDefenseGain = (allyFactionCount + 1f) + leaderMilitaryBonus;  // +1f базовый
    newDefenseGain *= DynamicFactionsMod.settings.defenseMultiplier;
    data.defensePoints += newDefenseGain;
}

// Пассивные бонусы от influence/stability
data.attackPoints += data.influencePoints / 50f;
data.defensePoints += data.stabilityPoints / 50f;

// Финальный Clamp
data.attackPoints = Mathf.Clamp(data.attackPoints, 0f, 200f);
data.defensePoints = Mathf.Clamp(data.defensePoints, 0f, 200f);


// ---------------- ЭНТРОПИЯ ----------------
float entropyAmount = DynamicFactionsMod.settings.entropyPerUpdate + data.accumulatedEntropy;

data.stabilityPoints -= entropyAmount;
data.influencePoints -= entropyAmount;

// ПОСТОЯННАЯ: медленно затухает (90% от текущего значения)
data.accumulatedEntropy *= 1f;

// ЛОГИКА ШАНСА СТАБИЛЬНОСТИ
float stabChance = 0f;
if (data.stabilityPoints > 50f)
{
    stabChance = (data.stabilityPoints - 50f) / 100f;
}
else if (data.stabilityPoints < -50f)
{
    stabChance = (Mathf.Abs(data.stabilityPoints) - 50f) / 100f;
}

// Проверяем событие стабильности только если не было других событий
if (selectedEvent == "Stability" && allowEvents && !eventOccurred && stabChance > 0 && Rand.Value < stabChance)
{
    if (data.stabilityPoints > 50f)
    {
        Log.Warning($" 🔥 ТРИГГЕР +: {faction.Name} (стаб: {data.stabilityPoints:F1}, шанс: {stabChance:P0})");
        if (TrySpawnSplitFaction(faction, true))
            data.stabilityPoints -= 100f;
    }
    else
    {
        Log.Warning($" 🔥 ТРИГГЕР -: {faction.Name} (стаб: {data.stabilityPoints:F1}, шанс: {stabChance:P0})");
        if (TrySpawnSplitFaction(faction, false))
            data.stabilityPoints += 100f;
    }
    eventOccurred = true; // Отмечаем, что событие произошло
}

// ---------------- ЛОГИ ----------------
if (DynamicFactionsMod.settings.showDebugLogs)
{
    float deathStabLog = data.lastLeader != faction.leader ? -10f * data.leaderLegitimacy : 0f;
    float deathInfLog = data.lastLeader != faction.leader ? -10f * data.leaderLegitimacy : 0f;
	float infBonusLog = data.influencePoints / 50f;
    float stabBonusLog = data.stabilityPoints / 50f;
    
    // ✅ НОВОЕ: общая энтропия + накопленная
    float totalEntropyLog = DynamicFactionsMod.settings.entropyPerUpdate + data.accumulatedEntropy;
    float baseEntropyLog = DynamicFactionsMod.settings.entropyPerUpdate;
    
    string focusLabel = militaryFocusAttack ? "⚔️ АТАКА" : "🛡️ БРОНЯ";
    
    // События лидера
    float leaderEventsLog = 0f;
    if (diff > 0) leaderEventsLog = 5f * diff;
    else if (diff < 0) leaderEventsLog = -5f * Mathf.Abs(diff);
    
    // Шансы
    string stabChanceStr = stabChance > 0 ? $" [🎲{stabChance:P0}]" : "";
    string infChanceStr = infChance > 0 ? $" [🎲{infChance:P0}]" : "";

    Log.Message($"-------{faction.Name} ({currentSettlements} баз) [{focusLabel}]------");
    Log.Message($"Лидер: Пассивно+0.1 События{leaderEventsLog:+0.0;-0.0} → {data.leaderLegitimacy:F1}");
    Log.Message($"Стабильность: Базы{changeSettlements:+0;-0} СмертьЛид{deathStabLog:F0} ОтЛидера{data.leaderLegitimacy/5f:F1} Базы{currentSettlements:F0} Энтропия-{totalEntropyLog:F1}(+{data.accumulatedEntropy:F1}) → {data.stabilityPoints:F1}{stabChanceStr}");
    Log.Message($"Влияние: Союзники({allyCount}x1) Идеология({sameIdeoCount}x2) ОтЛидера({data.leaderLegitimacy/5f:F1}) Энтропия-{totalEntropyLog:F1}(+{data.accumulatedEntropy:F1}) → {data.influencePoints:F1}{infChanceStr}");
    Log.Message($"Атака: Враги({enemyFactionCount}+1)+ОтЛидера({leaderMilitaryBonus:F1})×{DynamicFactionsMod.settings.attackMultiplier:F1} +Влияние({infBonusLog:F1}) → {data.attackPoints:F1}");
    Log.Message($"Броня: Союзники({allyFactionCount}+1)+ОтЛидера({leaderMilitaryBonus:F1})×{DynamicFactionsMod.settings.defenseMultiplier:F1} +Стаб({stabBonusLog:F1}) → {data.defensePoints:F1}");
}

} // конец цикла foreach

// Рандомный спавн базы
if (Rand.Value < DynamicFactionsMod.settings.randomSettlementChance / 100f)
{
    if (DynamicFactionsMod.settings.showDebugLogs) Log.Message($"[DF] 🎲 Рандом-чек: УДАЧА!");
    TrySpawnRandomSettlement();
}

foreach (var data in factionDataList)
{
    if (data?.faction == null || data.faction.defeated) continue;
    
    // Множитель стабильности
    data.stabilityPoints *= DynamicFactionsMod.settings.stabilityMultiplier;
    data.stabilityPoints = Mathf.Clamp(data.stabilityPoints, -100f, 100f);
    
    // Множитель атаки
    data.attackPoints *= DynamicFactionsMod.settings.attackMultiplier;
    data.attackPoints = Mathf.Clamp(data.attackPoints, 0f, 200f);
    
    // Множитель защиты
    data.defensePoints *= DynamicFactionsMod.settings.defenseMultiplier;
    data.defensePoints = Mathf.Clamp(data.defensePoints, 0f, 200f);
    
    // Множитель влияния
    data.influencePoints *= DynamicFactionsMod.settings.influenceMultiplier;
    data.influencePoints = Mathf.Clamp(data.influencePoints, -200f, 200f);
    
    // ▼▼▼ ДОБАВЬ ЭТО - МНОЖИТЕЛЬ ОЧКОВ ЛИДЕРА ▼▼▼
    data.leaderLegitimacy *= DynamicFactionsMod.settings.stabilityMultiplier;
    data.leaderLegitimacy = Mathf.Clamp(data.leaderLegitimacy, -20f, 20f);
    // ▲▲▲ ДОБАВЬ ЭТО ▲▲▲
}


}

        
// ============================================ БЛОК 4.4: СОБЫТИЯ СТАБИЛЬНОСТИ ============================================
// ПОЛОЖИТЕЛЬНАЯ СТАБИЛЬНОСТЬ
public bool TrySpawnSplitFaction(Faction parentFaction, bool isPositiveEvent)
{
    if (parentFaction.IsPlayer || parentFaction == Faction.OfPlayer)
    {
        Log.Warning($"[DF] Игнорируем событие для PlayerColony");
        return false;
    }

    var parentSettlements = Find.WorldObjects.Settlements.Where(s => s.Faction == parentFaction).ToList();
    if (parentSettlements.Count == 0) return false;

    Settlement originSettlement = parentSettlements.RandomElementWithFallback(null);
    if (originSettlement == null) return false;

    TechLevel parentTech = parentFaction.def.techLevel;
    TechLevel targetTech = parentTech;
    string label = "";
    string text = "";
    LetterDef letterDef = LetterDefOf.NeutralEvent;
    int goodwillChange = isPositiveEvent ? 100 : -100;
    bool spawnNewSettlement = true;
    int settlementsToSeize = 0;

    if (isPositiveEvent)
    {
        FactionStabilityData data = GetOrCreateData(parentFaction);

        bool ExecuteStrengthening(string reasonPrefix = "")
        {
            data.attackPoints += 15f;
            data.defensePoints += 15f;
            data.leaderLegitimacy += 5f;
            data.attackPoints = Mathf.Clamp(data.attackPoints, 0f, 200f);
            data.defensePoints = Mathf.Clamp(data.defensePoints, 0f, 200f);
            data.leaderLegitimacy = Mathf.Clamp(data.leaderLegitimacy, -20f, 20f);
            Log.Warning($"[DF] BOOST {parentFaction.Name}: Атака={data.attackPoints:F1}, Броня={data.defensePoints:F1}, Лидер={data.leaderLegitimacy:F1}");
            string t = "Укрепление боевой мощи";
            string b = $"{reasonPrefix}Фракция {parentFaction.Name} направила ресурсы на армию.\n\n• Атака: +15 (Всего: {data.attackPoints:F1})\n• Броня: +15 (Всего: {data.defensePoints:F1})\n• Лидер: +5 (Всего: {data.leaderLegitimacy:F1})";
            data.accumulatedEntropy += 1f;
            Find.LetterStack.ReceiveLetter(t, b, LetterDefOf.NeutralEvent, originSettlement);
            return false;
        }
        
        // Интриги: 0.5% за базу
        float intriguesChance = parentSettlements.Count * 0.005f;
        if (Rand.Value < intriguesChance)
        {
            spawnNewSettlement = false;
            settlementsToSeize = Rand.Range(1, 4);
            targetTech = parentFaction.def.techLevel;
            label = $"Интриги: {parentFaction.Name}";
            text = $"Внутренние интриги привели к разделению власти. {settlementsToSeize} поселений отошли новой группировке.";
            letterDef = LetterDefOf.NeutralEvent;
            goodwillChange = -200;
            data.accumulatedEntropy += 1f;
        }
        else
        {
            float roll = Rand.Value;
            if (roll < 0.50f) // 50% Колонизация
            {
                int newTile = -1;
                int parentTile = originSettlement.Tile;
                for (int i = 0; i < 50; i++)
                {
                    int t = TileFinder.RandomSettlementTileFor(parentFaction);
                    if (Find.WorldGrid.ApproxDistanceInTiles(parentTile, t) <= DynamicFactionsMod.settings.splitMaxDistanceTiles)
                    {
                        newTile = t; break;
                    }
                }
                if (newTile == -1) return ExecuteStrengthening("Экспедиция не нашла земель для колонизации. ");
                Settlement newColony = (Settlement)WorldObjectMaker.MakeWorldObject(WorldObjectDefOf.Settlement);
                newColony.SetFaction(parentFaction);
                newColony.Tile = newTile;
                newColony.Name = SettlementNameGenerator.GenerateSettlementName(newColony, null);
                Find.WorldObjects.Add(newColony);
                data.accumulatedEntropy += 1f;
                Find.LetterStack.ReceiveLetter($"Колонизация: {parentFaction.Name}", 
                    $"Основана новая база {newColony.Name}.", LetterDefOf.NeutralEvent, newColony);
                return true;
            }
            else if (roll < 0.70f) // 20% Крупная сделка
            {
                data.influencePoints += 20f;
                data.leaderLegitimacy += 5f;
                data.leaderLegitimacy = Mathf.Clamp(data.leaderLegitimacy, -20f, 20f);
                data.accumulatedEntropy += 1f;
                Find.LetterStack.ReceiveLetter($"Крупная сделка: {parentFaction.Name}",
                    $"Торговая гильдия заключила выгодный контракт.\n\n• Влияние: +20 (Всего: {data.influencePoints:F1})\n• Лидер: +5 (Всего: {data.leaderLegitimacy:F1})", 
                    LetterDefOf.NeutralEvent, originSettlement);
                return false;
            }
            else if (roll < 0.85f) // 15% Укрепление
            {
                return ExecuteStrengthening();
            }
            else if (roll < 0.90f) // 5% Техн прорыв
            {
				
				    if (DynamicFactionsMod.settings.limitTechLevel)
					{
					return ExecuteStrengthening("");
					}
                int checkTile = TileFinder.RandomSettlementTileFor(parentFaction);
                if (checkTile == -1) return ExecuteStrengthening("Ученые не нашли места для академии. ");
                
                // НАУЧНЫЙ ПРОРЫВ: +1 уровень, но не выше Archotech
                if (parentTech < TechLevel.Archotech)
                {
                    // Определяем следующий уровень технологий
                    var techLevels = new List<TechLevel> {
                        TechLevel.Animal,
                        TechLevel.Neolithic,
                        TechLevel.Medieval,
                        TechLevel.Industrial,
                        TechLevel.Spacer,
                        TechLevel.Ultra,
                        TechLevel.Archotech
                    };
                    
                    int currentIndex = techLevels.IndexOf(parentTech);
                    if (currentIndex >= 0 && currentIndex < techLevels.Count - 1)
                    {
                        targetTech = techLevels[currentIndex + 1];
                    }
                    else
                    {
                        targetTech = parentTech; // Остаемся на текущем
                    }
                }
                else
                {
                    targetTech = TechLevel.Archotech;
                }
                
                label = $"Научный прорыв: {parentFaction.Name}";
                text = $"Технологический скачок привел к созданию новой прогрессивной фракции (Уровень {parentTech} -> {targetTech}).";
                goodwillChange = 100;
                spawnNewSettlement = true;
                data.accumulatedEntropy += 1f;
            }
            else // 10% Золотой век
            {
                data.leaderLegitimacy += 10f;
                data.leaderLegitimacy = Mathf.Clamp(data.leaderLegitimacy, -20f, 20f);
                data.accumulatedEntropy += 1f;
                Find.LetterStack.ReceiveLetter($"Золотой век: {parentFaction.Name}",
                    $"Народ боготворит лидера.\n(Легитимность +10, Всего: {data.leaderLegitimacy:F1})", 
                    LetterDefOf.NeutralEvent, originSettlement);
                return false;
            }
        }
    }
else // НЕГАТИВНЫЕ СОБЫТИЯ СТАБИЛЬНОСТИ
{
    float roll = Rand.Value;
    float leaderSaveChance = Mathf.Max(0.05f, 0.20f - 0.01f * parentSettlements.Count);
    if (roll < leaderSaveChance)
    {
        FactionStabilityData data = GetOrCreateData(parentFaction);
        data.leaderLegitimacy += 5f;
        data.leaderLegitimacy = Mathf.Clamp(data.leaderLegitimacy, -20f, 20f);
        Settlement origin = parentSettlements.RandomElement();
        int parentTile = origin.Tile;
        int maxDist = DynamicFactionsMod.settings.splitMaxDistanceTiles;
        int newTile = -1;
        for (int i = 0; i < 50; i++)
        {
            int t = TileFinder.RandomSettlementTileFor(parentFaction);
            if (Find.WorldGrid.ApproxDistanceInTiles(parentTile, t) <= maxDist * 0.7f)
            {
                newTile = t; break;
            }
        }
        if (newTile == -1) 
        {
            Settlement originForLetter = parentSettlements.RandomElement();
            label = $"Лидер-герой: {parentFaction.Name}";
            text = $"В момент кризиса {parentFaction.leader.Name.ToStringFull} стабилизировал ситуацию!\n\nЕго авторитет вырос.\n(Легитимность +5, теперь: {data.leaderLegitimacy:F1})";
            data.accumulatedEntropy -= 1f;
            Find.LetterStack.ReceiveLetter(label, text, LetterDefOf.NeutralEvent, originForLetter);
            return true;
        }
        Settlement newColony = (Settlement)WorldObjectMaker.MakeWorldObject(WorldObjectDefOf.Settlement);
        newColony.SetFaction(parentFaction);
        newColony.Tile = newTile;
        newColony.Name = SettlementNameGenerator.GenerateSettlementName(newColony, null);
        Find.WorldObjects.Add(newColony);
        label = $"Лидер объединяет: {parentFaction.Name}";
        text = $"{parentFaction.leader.Name.ToStringFull} предотвратил раскол и убедил революционеров!\n\nБунтовщики стали колонистами и основали новую базу {newColony.Name} рядом с {origin.Name}.\nФракция вышла из кризиса сильнее!\n(Легитимность +5, теперь: {data.leaderLegitimacy:F1})";
        data.accumulatedEntropy -= 1f;
        Find.LetterStack.ReceiveLetter(label, text, LetterDefOf.NeutralEvent, newColony);
        return true;
    }
    roll = (roll - leaderSaveChance) / (1f - leaderSaveChance);
if (roll < 0.20f) // Гражданская война 20%
{
    FactionStabilityData data = GetOrCreateData(parentFaction);
    targetTech = parentTech;
    spawnNewSettlement = true;  // ДОБАВЛЕНО
    settlementsToSeize = 0;      // ДОБАВЛЕНО
    label = $"Гражданская война: {parentFaction.Name}";
    text = $"Непримиримые политические разногласия раскололи {parentFaction.Name}.\nОппозиция покинула города и основала независимое государство.\nОни сохранили тот же уровень технологий, но теперь являются врагами.";
    letterDef = LetterDefOf.NeutralEvent;
    goodwillChange = -100;
    data.accumulatedEntropy -= 1f;
}
    else if (roll < 0.40f) // Военный переворот 20%
    {
        FactionStabilityData data = GetOrCreateData(parentFaction);
		targetTech = parentTech;
        spawnNewSettlement = false;
        settlementsToSeize = Rand.Range(1, 6);
        label = $"Военный переворот: {parentFaction.Name}";
        text = $"Генералы и силовики {parentFaction.Name} подняли мятеж против центральной власти.\nГарнизоны нескольких поселений перешли на сторону хунты, началась война за контроль над регионом.";
        letterDef = LetterDefOf.NeutralEvent;
        goodwillChange = -100;
        data.accumulatedEntropy -= 1f;
    }
    else if (roll < 0.55f) // Крах общества 15%
    {
        FactionStabilityData data = GetOrCreateData(parentFaction);
		
        // КРАХ ОБЩЕСТВА: -1 уровень, но не ниже Neolithic
        if (parentTech > TechLevel.Neolithic)
        {
            // Определяем предыдущий уровень технологий
            var techLevels = new List<TechLevel> {
                TechLevel.Animal,
                TechLevel.Neolithic,
                TechLevel.Medieval,
                TechLevel.Industrial,
                TechLevel.Spacer,
                TechLevel.Ultra,
                TechLevel.Archotech
            };
            
            int currentIndex = techLevels.IndexOf(parentTech);
            if (currentIndex > 0)
            {
                targetTech = techLevels[currentIndex - 1];
            }
            else
            {
                targetTech = TechLevel.Neolithic;
            }
        }
        else
        {
            targetTech = TechLevel.Neolithic;
        }
        
        label = $"Крах общества: {parentFaction.Name}";
        text = $"Экономический коллапс, эпидемии и внутренние конфликты разрушили институты {parentFaction.Name}.\nОтколовшиеся выжившие утратили значительную часть знаний и деградировали до уровня {targetTech}.\nИх идеология теперь проста: выжить любой ценой.";
        letterDef = LetterDefOf.NeutralEvent;
        goodwillChange = -50;
        data.accumulatedEntropy -= 1f;
    }
    else if (roll < 0.65f) // Эпидемия 10%
    {
        FactionStabilityData data = GetOrCreateData(parentFaction);
        data.attackPoints += 15f;
        data.defensePoints -= 15f;
        data.influencePoints -= 15f;
        data.attackPoints = Mathf.Clamp(data.attackPoints, 0f, 200f);
        data.defensePoints = Mathf.Clamp(data.defensePoints, 0f, 200f);
        data.influencePoints = Mathf.Clamp(data.influencePoints, -200f, 200f);
        label = $"Эпидемия в {parentFaction.Name}";
        text = $"Смертельная болезнь охватила {parentFaction.Name}.\n\n• Армия мобилизована (+15 Атаки)\n• Гражданские потери (-15 Брони)\n• Паника (-15 Влияния)\n\nВсего: Атака {data.attackPoints:F1}, Броня {data.defensePoints:F1}, Влияние {data.influencePoints:F1}";
        data.accumulatedEntropy -= 1f;
        Find.LetterStack.ReceiveLetter(label, text, LetterDefOf.ThreatSmall, originSettlement);
        return false;
    }
    else // Изоляция 15%
    {
        FactionStabilityData data = GetOrCreateData(parentFaction);
        data.attackPoints += 20f;
        data.defensePoints += 20f;
        data.influencePoints -= 20f;
        data.attackPoints = Mathf.Clamp(data.attackPoints, 0f, 200f);
        data.defensePoints = Mathf.Clamp(data.defensePoints, 0f, 200f);
        data.influencePoints = Mathf.Clamp(data.influencePoints, -200f, 200f);
        label = $"Изоляция: {parentFaction.Name}";
        text = $"{parentFaction.Name} закрыла границы и объявила всеобщую мобилизацию.\n\n• Армия усилена (+20 Атаки)\n• Крепости построены (+20 Брони)\n• Дипломатия разорвана (-20 Влияния)\n\nВсего: Атака {data.attackPoints:F1}, Броня {data.defensePoints:F1}, Влияние {data.influencePoints:F1}";
        data.accumulatedEntropy -= 1f;
        Find.LetterStack.ReceiveLetter(label, text, LetterDefOf.NeutralEvent, originSettlement);
        return false;
    }
}
            
// ================= НАЧАЛО БЛОКА ВЫБОРА ФРАКЦИИ =================

FactionDef newFactionDef = null;

// 1. Получаем список дефов, которые УЖЕ используются живыми фракциями в мире
HashSet<FactionDef> activeDefsInWorld = Find.FactionManager.AllFactions
    .Where(f => !f.defeated && f.def != null) 
    .Select(f => f.def)
    .ToHashSet();

// 2. Ищем в нашем пуле деф, которого НЕТ среди активных (абсолютная уникальность)
var availableDefs = allHiddenFactionDefs
    .Where(d => d.techLevel == targetTech && !activeDefsInWorld.Contains(d))
    .ToList();

// 3. Если уникальные закончились (все заняты), разрешаем повторы
if (availableDefs.Count == 0)
{
    // Опционально: можно добавить лог
    // Log.Warning($"[DF] Уникальные иконки для {targetTech} кончились. Используем повтор.");
    
    availableDefs = allHiddenFactionDefs
        .Where(d => d.techLevel == targetTech)
        .ToList();
}

// 4. Финальная проверка: если вообще ничего нет (даже для повтора)
if (availableDefs.Count == 0)
{
    // Специальный фикс для Революций (повышения уровня), если нет новых дефов
    if (targetTech > parentTech) 
    {
         // Откатываемся на старый уровень, если для нового нет картинок
         targetTech = parentTech;
         availableDefs = allHiddenFactionDefs
            .Where(d => d.techLevel == targetTech)
            .ToList();
    }
    
    if (availableDefs.Count == 0)
    {
        Log.Error($"[DF] CRITICAL: Не найдены шаблоны фракций для уровня {targetTech}! Проверьте XML.");
        return false;
    }
}

// 5. Выбираем случайную
newFactionDef = availableDefs.RandomElement();

// ================= КОНЕЦ БЛОКА ВЫБОРА ФРАКЦИИ =================

    
    FactionGeneratorParms parms = new FactionGeneratorParms(newFactionDef);
    Faction newFaction = FactionGenerator.NewGeneratedFaction(parms);
    Find.FactionManager.Add(newFaction);
    
    int parentGoodwill = isPositiveEvent ? 60 : Rand.Range(-100, -40);
    if (newFaction.def.techLevel > parentFaction.def.techLevel)
    {
        parentGoodwill = 100;
        if (DynamicFactionsMod.settings.showDebugLogs)
        {
            Log.Message($"[DF] 🔬 {newFaction.Name} +100 НАУЧНЫЙ СОЮЗНИК {parentFaction.Name}");
        }
    }
    
    newFaction.TryAffectGoodwillWith(parentFaction, parentGoodwill);
    parentFaction.TryAffectGoodwillWith(newFaction, parentGoodwill);
    
    float goodwillRoll = Rand.Value;
    if (goodwillRoll < 0.50f) newFaction.TryAffectGoodwillWith(Faction.OfPlayer, Rand.Range(-100, 0));
    else newFaction.TryAffectGoodwillWith(Faction.OfPlayer, parentFaction.GoodwillWith(Faction.OfPlayer));
    
// Автоматическая вражда к другим фракциям
foreach (Faction other in Find.FactionManager.AllFactions.Where(f => !f.IsPlayer && !f.defeated && f != newFaction && f != parentFaction))
{
    if (Verse.Rand.Value < 0.8f) // 80% шанс вражды
    {
        newFaction.TryAffectGoodwillWith(other, -90);
        other.TryAffectGoodwillWith(newFaction, -90); // Добавляем обратное изменение
    }
}
    
    if (ModsConfig.IdeologyActive && newFaction.ideos != null)
    {
        if (parentFaction.ideos?.PrimaryIdeo != null && Rand.Value < 0.50f)
        {
            newFaction.ideos.SetPrimary(parentFaction.ideos.PrimaryIdeo);
        }
        else
        {
            try
            {
                Ideo randomIdeo = IdeoGenerator.GenerateIdeo(new IdeoGenerationParms(newFaction.def));
                Find.IdeoManager.Add(randomIdeo);
                newFaction.ideos.SetPrimary(randomIdeo);
                if (DynamicFactionsMod.settings.showDebugLogs)
                {
                    Log.Message($"[DF] ➕ Идеология '{randomIdeo.name}' создана для {newFaction.Name}");
                }
            }
            catch
            {
                if (parentFaction.ideos?.PrimaryIdeo != null)
                    newFaction.ideos.SetPrimary(parentFaction.ideos.PrimaryIdeo);
                Log.Warning($"[DF] ⚠️ Не удалось создать уникальную идеологию (закончились имена богов). Взята родительская.");
            }
        }
    }
    
    // Удаляем сгенерированные поселения (если есть)
    List<Settlement> generatedSettlements = Find.WorldObjects.Settlements.Where(s => s.Faction == newFaction).ToList();
    foreach (var s in generatedSettlements) Find.WorldObjects.Remove(s);
    Settlement letterTarget = null;
    
    if (spawnNewSettlement)
    {
        int parentTile = originSettlement.Tile;
        int maxDist = DynamicFactionsMod.settings.splitMaxDistanceTiles;
        int newTile = -1;
        for (int i = 0; i < 50; i++)
        {
            int t = TileFinder.RandomSettlementTileFor(newFaction);
            if (Find.WorldGrid.ApproxDistanceInTiles(parentTile, t) <= maxDist && TileFinder.IsValidTileForNewSettlement(t))
            {
                newTile = t; break;
            }
        }
        if (newTile == -1)
        {
            for (int i = 0; i < 50; i++)
            {
                int t = TileFinder.RandomSettlementTileFor(newFaction);
                if (Find.WorldGrid.ApproxDistanceInTiles(parentTile, t) <= maxDist * 2 && TileFinder.IsValidTileForNewSettlement(t))
                {
                    newTile = t; break;
                }
            }
        }
        if (newTile == -1) return false;
        Settlement newSettlement = (Settlement)WorldObjectMaker.MakeWorldObject(WorldObjectDefOf.Settlement);
        newSettlement.SetFaction(newFaction);
        newSettlement.Tile = newTile;
        newSettlement.Name = SettlementNameGenerator.GenerateSettlementName(newSettlement, null);
        Find.WorldObjects.Add(newSettlement);
        letterTarget = newSettlement;
    }
    else
    {
    var targets = parentSettlements.OrderBy(s => Find.WorldGrid.ApproxDistanceInTiles(originSettlement.Tile, s.Tile)).Take(settlementsToSeize).ToList();
    if (targets.Count == 0) return false;
    int firstTile = -1;
    foreach (var oldSettlement in targets)
    {
        int capturedTile = oldSettlement.Tile;
        string capturedName = oldSettlement.Name;
        
        // ✅ ФИКС: Удаляем старое и создаем новое
        Find.WorldObjects.Remove(oldSettlement);
        Settlement newSettlement = (Settlement)WorldObjectMaker.MakeWorldObject(WorldObjectDefOf.Settlement);
        newSettlement.SetFaction(newFaction);
        newSettlement.Tile = capturedTile;
        newSettlement.Name = capturedName + " (мятеж)";
        Find.WorldObjects.Add(newSettlement);
        
        firstTile = capturedTile;
    }
    letterTarget = Find.WorldObjects.SettlementBaseAt(firstTile) ?? originSettlement;
    text += $"\n\nЗахвачено поселений: {targets.Count}. Базы переименованы и перестроены под новую власть.";
    }
    
    newFaction.TryAffectGoodwillWith(parentFaction, goodwillChange);
    newFaction.TryAffectGoodwillWith(Faction.OfPlayer, 0);
    Find.LetterStack.ReceiveLetter(label, text, letterDef, letterTarget);
    OnFactionUsed(newFactionDef);
    return true;
}
        
        // ============================================ БЛОК 4.5: СЛУЧАЙНОЕ ПОСЕЛЕНИЕ ============================================
private bool TrySpawnRandomSettlement()
{
    if (activePool.Count == 0) return false;
    
    var availableDefs = allHiddenFactionDefs.Except(usedDefs).ToList();
    if (availableDefs.Count == 0) 
    { 
        usedDefs.Clear(); 
        availableDefs = allHiddenFactionDefs.ToList(); 
    }
    
    if (DynamicFactionsMod.settings.showDebugLogs)
    {
        Log.Message($"[DF] 📊 Статистика доступных дефов:");
        var techGroups = availableDefs.GroupBy(d => d.techLevel)
                                      .OrderBy(g => g.Key.ToString());
        foreach (var group in techGroups)
        {
            Log.Message($"  {group.Key}: {group.Count()} дефов");
        }
    }
    
     // ВЗВЕШЕННЫЙ ВЫБОР ТЕХУРОВНЯ
    TechLevel selectedTech = TechLevel.Neolithic; // fallback по умолчанию
    
    // Получаем исходный список
    var availableTechs = availableDefs.Select(d => d.techLevel).Distinct().ToList();

    if (DynamicFactionsMod.settings.showDebugLogs)
    {
        Log.Message($"[DF] 🎲 Доступные техуровни (все): {string.Join(", ", availableTechs)}");
    }
	
    // --- ФИЛЬТР ПО НАСТРОЙКЕ ---
    if (DynamicFactionsMod.settings.limitTechLevel)
    {
        // ВАЖНО: Добавлено .ToList(), так как availableTechs это List<TechLevel>
        availableTechs = availableTechs.Where(t => t <= TechLevel.Medieval).ToList();
        
        if (DynamicFactionsMod.settings.showDebugLogs)
        {
             Log.Message($"[DF] 🛡️ Ограничение активно. Остались: {string.Join(", ", availableTechs)}");
        }
    }
    // ----------------------------

    var weightedTechs = availableTechs.Where(t => TechSpawnWeights.ContainsKey(t)).ToList();

    if (weightedTechs.Count > 0)
    {
        float totalWeight = weightedTechs.Sum(t => TechSpawnWeights[t]);
        float roll = Rand.Range(0f, totalWeight);
        float current = 0f;
        
        bool found = false;
        foreach (var tech in weightedTechs)
        {
            current += TechSpawnWeights[tech];
            if (roll <= current)
            {
                selectedTech = tech;
                found = true;
                break;
            }
        }
        
        // Если не нашли (из-за ошибки округления), берем последний
        if (!found)
        {
            selectedTech = weightedTechs.Last();
        }
        
        if (DynamicFactionsMod.settings.showDebugLogs)
        {
            Log.Message($"[DF] 🎲 Выбран техуровень: {selectedTech} (шанс был {roll:F2}/{totalWeight:F2})");
        }
    }
    else
    {
        selectedTech = availableDefs.First().techLevel;
    }

    // Выбираем деф нужного уровня
    var techDefs = availableDefs.Where(d => d.techLevel == selectedTech).ToList();

    // Если нет дефов выбранного уровня, пробуем найти любой подходящий
    if (techDefs.Count == 0)
    {
        if (DynamicFactionsMod.settings.showDebugLogs)
        {
            Log.Warning($"[DF] ⚠️ Нет дефов для {selectedTech}, ищем альтернативы...");
        }
        
        // Сначала пробуем понизить уровень
        var allTechLevels = new List<TechLevel> 
        { 
            TechLevel.Neolithic, 
            TechLevel.Medieval, 
            TechLevel.Industrial, 
            TechLevel.Spacer, 
            TechLevel.Ultra 
        };
        
        int currentIndex = allTechLevels.IndexOf(selectedTech);
        
        // Ищем ближайший доступный уровень (сначала ниже, потом выше)
        for (int offset = 1; offset < allTechLevels.Count; offset++)
        {
            // Проверяем уровень ниже
            if (currentIndex - offset >= 0)
            {
                var lowerTech = allTechLevels[currentIndex - offset];
                var lowerDefs = availableDefs.Where(d => d.techLevel == lowerTech).ToList();
                if (lowerDefs.Count > 0)
                {
                    techDefs = lowerDefs;
                    selectedTech = lowerTech;
                    if (DynamicFactionsMod.settings.showDebugLogs)
                    {
                        Log.Message($"[DF] 🔻 Переход на {lowerTech} (ниже)");
                    }
                    break;
                }
            }
            
            // Проверяем уровень выше
            if (currentIndex + offset < allTechLevels.Count)
            {
                var higherTech = allTechLevels[currentIndex + offset];
                var higherDefs = availableDefs.Where(d => d.techLevel == higherTech).ToList();
                if (higherDefs.Count > 0)
                {
                    techDefs = higherDefs;
                    selectedTech = higherTech;
                    if (DynamicFactionsMod.settings.showDebugLogs)
                    {
                        Log.Message($"[DF] 🔺 Переход на {higherTech} (выше)");
                    }
                    break;
                }
            }
        }
        
        // Если всё равно не нашли, берем все доступные
        if (techDefs.Count == 0)
        {
            techDefs = availableDefs;
            selectedTech = availableDefs.First().techLevel;
        }
    }

    FactionDef randomDef = techDefs.RandomElement();
    usedDefs.Add(randomDef);
    
    FactionGeneratorParms parms = new FactionGeneratorParms(randomDef);
    Faction newFaction = FactionGenerator.NewGeneratedFaction(parms);
    Find.FactionManager.Add(newFaction);
    
    newFaction.TryAffectGoodwillWith(Faction.OfPlayer, Rand.Range(-100, 0));
    
    if (ModsConfig.IdeologyActive && newFaction.ideos != null)
    {
        Ideo randomIdeo = IdeoGenerator.GenerateIdeo(new IdeoGenerationParms(newFaction.def));
        Find.IdeoManager.Add(randomIdeo);
        newFaction.ideos.SetPrimary(randomIdeo);
        
        if (DynamicFactionsMod.settings.showDebugLogs)
        {
            Log.Message($"[DF] ➕ Случайная идеология '{randomIdeo.name}' для {newFaction.Name}");
        }
    }
    
    List<Settlement> generatedSettlements = Find.WorldObjects.Settlements.Where(s => s.Faction == newFaction).ToList();
    foreach (var s in generatedSettlements) Find.WorldObjects.Remove(s);
    
    int newTile = -1;
    const int MAX_ATTEMPTS = 20;
    for (int attempt = 0; attempt < MAX_ATTEMPTS; attempt++)
    {
        int candidate = TileFinder.RandomSettlementTileFor(newFaction);
        if (TileFinder.IsValidTileForNewSettlement(candidate))
        {
            newTile = candidate;
            if (DynamicFactionsMod.settings.showDebugLogs)
            {
                Log.Message($"[DF] 🎲 Valid тайл #{attempt+1}: {newTile}");
            }
            break;
        }
    }
    
    if (newTile == -1)
    {
        Log.Error($"[DF] ❌ {MAX_ATTEMPTS} invalid тайлов! Фракция без базы.");
        newFaction.defeated = true;
        return false;
    }
    
    Settlement newSettlement = (Settlement)WorldObjectMaker.MakeWorldObject(WorldObjectDefOf.Settlement);
    newSettlement.SetFaction(newFaction);
    newSettlement.Tile = newTile;
    newSettlement.Name = SettlementNameGenerator.GenerateSettlementName(newSettlement, null);
    Find.WorldObjects.Add(newSettlement);
    
    foreach (var otherFaction in Find.FactionManager.AllFactions.Where(f => !f.IsPlayer && !f.defeated))
    {
        int goodwill = Rand.Range(-100, 30);
        newFaction.TryAffectGoodwillWith(otherFaction, goodwill);
        otherFaction.TryAffectGoodwillWith(newFaction, goodwill);
    }
    
// УНИКАЛЬНОЕ ОПИСАНИЕ И ЗАГОЛОВОК ПО ТЕХУРОВНЮ
string title = SpawnLetterTitles.TryGetValue(randomDef.techLevel, out string titleText) 
    ? titleText 
    : "Случайное поселение";

string description = SpawnDescriptions.TryGetValue(randomDef.techLevel, out string desc) 
    ? $"{desc}. Они назвали своё поселение '{newSettlement.Name}'." 
    : $"Таинственная группа основала базу {newSettlement.Name}.";

// Добавим немного разнообразия в конец описания
string[] flavorTexts = {
    "\n\nИх намерения пока неясны.",
    "\n\nКак это повлияет на региональный баланс сил?",
    "\n\nРазведка ведёт наблюдение за новой группой.",
    "\n\nТорговцы уже проявляют интерес.",
    "\n\nСоседние фракции настороже.",
    "\n\nИх технологический уровень вызывает вопросы.",
    "\n\nКакие ресурсы они ищут в этом регионе?"
};

description += flavorTexts.RandomElement();

// Выбираем тип письма в зависимости от техуровня
LetterDef letterType = LetterDefOf.NeutralEvent;
if (randomDef.techLevel >= TechLevel.Spacer)
{
    letterType = LetterDefOf.NeutralEvent; // Можно сделать особый для высоких уровней
}
else if (randomDef.techLevel <= TechLevel.Neolithic)
{
    letterType = LetterDefOf.NeutralEvent;
}

Find.LetterStack.ReceiveLetter(title, description, letterType, newSettlement);
    
    OnFactionUsed(randomDef);
    
    if (DynamicFactionsMod.settings.showDebugLogs)
    {
        Log.Message($"[DF] 🎲 СПАВН! {newFaction.Name} ({randomDef.techLevel}) на тайле {newTile}");
    }
    
    return true;
}
        // ============================================ БЛОК 4.6: ВСПОМОГАТЕЛЬНЫЕ МЕТОДЫ ============================================
        private FactionStabilityData GetOrCreateData(Faction f)
        {
            var data = factionDataList.FirstOrDefault(x => x.faction == f);
            if (data == null)
            {
                int currentSettlements = Find.WorldObjects.Settlements.Count(s => s.Faction == f);
                data = new FactionStabilityData
                {
                    faction = f,
                    stabilityPoints = 0,
                    lastSettlementCount = currentSettlements,
                    lastSettlementCountForInfluence = currentSettlements,
                    lastLeader = f.leader
                };
                factionDataList.Add(data);
                if (f.leader != null)
                {
                    data.lastLeader = f.leader;
                    data.leaderStartTick = Find.TickManager.TicksGame;
                    data.leaderLegitimacy = Rand.Range(-5.0f, 5.0f);
                    if (DynamicFactionsMod.settings.showDebugLogs)
{
    Log.Message($"[DF] 🎲 ЛИДЕР {f.Name}: {data.leaderLegitimacy:F1}");
}

                }
            }
            return data;
        }
        
private void CaptureBases(Faction winner, List<Settlement> targets, int count, string title, string text)
{
    count = Mathf.Min(count, targets.Count);
    var myBases = Find.WorldObjects.Settlements.Where(s => s.Faction == winner).ToList();
    Settlement refBase = myBases.Count > 0 ? myBases.RandomElement() : targets[0];
    var seized = targets.OrderBy(s => Find.WorldGrid.ApproxDistanceInTiles(refBase.Tile, s.Tile)).Take(count).ToList();
    foreach (var s in seized)
    {
        int tile = s.Tile;
        string name = s.Name;
        Find.WorldObjects.Remove(s);
        
        // ✅ ФИКС: Создаем новое поселение с правильным цветом фракции
        Settlement newBase = (Settlement)WorldObjectMaker.MakeWorldObject(WorldObjectDefOf.Settlement);
        newBase.SetFaction(winner);
        newBase.Tile = tile;
        newBase.Name = name + " (захвачено)";
        Find.WorldObjects.Add(newBase);
    }
    Find.LetterStack.ReceiveLetter(title, text, LetterDefOf.ThreatBig, new GlobalTargetInfo(seized[0].Tile));
}

        
        private void DestroyBases(List<Settlement> targets, int count, string title, string text)
        {
            count = Mathf.Min(count, targets.Count);
            var destroyed = targets.Take(count).ToList();
            foreach (var s in destroyed)
            {
                int tile = s.Tile;
                string name = s.Name;
                Find.WorldObjects.Remove(s);
                SpawnRuins(tile, name);
            }
            Find.LetterStack.ReceiveLetter(title, text, LetterDefOf.NegativeEvent, new GlobalTargetInfo(destroyed[0].Tile));
        }
        
        private void SpawnRuins(int tile, string oldName)
        {
            DestroyedSettlement ruins = (DestroyedSettlement)WorldObjectMaker.MakeWorldObject(WorldObjectDefOf.DestroyedSettlement);
            ruins.Tile = tile;
            Find.WorldObjects.Add(ruins);
        }
        
        private float CalculateBattleScore(FactionStabilityData d, float basePoints)
        {
            float techMult = BattleTechMultipliers.TryGetValue(d.faction.def.techLevel, out float m) ? m : 0.8f;
            float totalScore = basePoints * techMult;
            totalScore += d.influencePoints * 0.3f;
            totalScore += d.stabilityPoints * 0.3f;
            totalScore += d.lastSettlementCount;
            totalScore += d.leaderLegitimacy;
            int allies = Find.FactionManager.AllFactions.Count(f => f != d.faction && !f.IsPlayer && !f.defeated && !f.Hidden 
                && f.def.defName != "TradersGuild" && f.def.defName != "Salvagers" && !d.faction.HostileTo(f));
            totalScore += allies;
            return totalScore;
        }
        
private void Initialize()
{
	plagueEndTick = -1;
    allHiddenFactionDefs = DefDatabase<FactionDef>.AllDefs
        .Where(d => d.defName.StartsWith("DF_") && d.maxConfigurableAtWorldCreation == 0).ToList();
    RefreshActivePool();
	usedDefs.Clear();
    
    // 🎲 СЛУЧАЙНЫЕ ОЧКИ НА СТАРТЕ (+/-40)
    foreach (Faction f in Find.FactionManager.AllFactions)
    {
        if (f.IsPlayer || f.defeated || !f.def.humanlikeFaction || 
            new HashSet<string> { "TradersGuild", "Salvagers" }.Contains(f.def.defName)) continue;
        
FactionStabilityData data = GetOrCreateData(f);

data.stabilityPoints = Rand.Range(-40f, 40f);
data.attackPoints = Rand.Range(-40f, 40f);    
data.defensePoints = Rand.Range(-40f, 40f);
data.influencePoints = Rand.Range(-40f, 40f);
        
        if (f.leader != null)
        {
            data.leaderLegitimacy = (int)(Rand.Range(-1f, 1.1f) * 10f);
            data.attackPoints += data.leaderLegitimacy;
            data.defensePoints += data.leaderLegitimacy;
        }
        
        data.lastSettlementCount = Find.WorldObjects.Settlements.Count(s => s.Faction == f);
        data.lastSettlementCountForInfluence = data.lastSettlementCount;
        
        if (DynamicFactionsMod.settings.showDebugLogs)
{
    Log.Message($"[DF 🎲] {f.Name}: С:{data.stabilityPoints} А:{data.attackPoints} Б:{data.defensePoints} В:{data.influencePoints} Л:{data.leaderLegitimacy}");
}

    }
    
    if (DynamicFactionsMod.settings.showDebugLogs)
{
    Log.Message($"[DF] ✅ Инициализация завершена. Данных: {factionDataList.Count}");
}

}







 private void TriggerGlobalCrisis()
{
    float severity = Verse.Rand.Range(0.1f, 1.0f);
    List<Faction> factions = Find.FactionManager.AllFactions
        .Where(f => !f.IsPlayer && !f.defeated && !f.Hidden && f.def.humanlikeFaction)
        .ToList();
    
    int count = Mathf.Max(2, (int)(factions.Count * severity));
    List<Faction> victims = factions.InRandomOrder().Take(count).ToList();
    
foreach (Faction f1 in victims)
{
    foreach (Faction f2 in factions)
    {
        if (f1 == f2) continue;
        if (Verse.Rand.Value < 0.5f) continue;
        
        f1.TryAffectGoodwillWith(f2, -200);
        f2.TryAffectGoodwillWith(f1, -200);
        // Убираем SetRelationDirect - goodwill -200 автоматически сделает врагами
    }
}
    
    DF_EventManager.Fire
	("DF_GlobalCrisis","DF_GlobalCrisis_Label".Translate(),
	"DF_GlobalCrisis_Text".Translate(count, factions.Count, severity.ToString("P0")), 
    LetterDefOf.ThreatBig);
    Log.Warning($"[DF] 💥 КРИЗИС: {count} фракций hostile!");
}
       
		
		
        private void RefreshActivePool()
        {
            activePool.Clear();
            var shuffled = allHiddenFactionDefs.InRandomOrder().Take(POOL_SIZE).ToList();
            activePool.AddRange(shuffled);
        }
        
        private void OnFactionUsed(FactionDef usedDef)
        {
            activePool.Remove(usedDef);
            if (activePool.Count < POOL_SIZE / 4) RefreshActivePool();
        }
       
	// --- Переменные для Пандемии ---
	private int plagueEndTick = -1; // -1 значит событие неактивно
	
private void ProcessPlague()
{
    int currentTick = Find.TickManager.TicksGame;

// 1. ПОПЫТКА ЗАПУСКА (если чума не идет)
if (plagueEndTick == -1)
{
    if (DynamicFactionsMod.settings.plagueChance > 0 && 
        Rand.Value < DynamicFactionsMod.settings.plagueChance / 100f)  // ← /100f добавлено!
    {
plagueEndTick = currentTick + Rand.RangeInclusive(30, 60) * 60000;
DF_EventManager.Fire("DF_PlagueOutbreak","DF_PlagueStart_Label".Translate(),
 "DF_PlagueStart_Text".Translate(), 
    LetterDefOf.ThreatBig);
    }
    return;
}

    // 2. ЗАВЕРШЕНИЕ (если время вышло)
    if (currentTick > plagueEndTick)
    {
        plagueEndTick = -1;
        Find.LetterStack.ReceiveLetter("Конец пандемии", 
            "Вспышка Черной Чумы пошла на спад.", 
            LetterDefOf.PositiveEvent);
        return;
    }
	
// ✅ Урон и эффекты только раз в сутки
int intervalTicks = DynamicFactionsMod.settings.triggerIntervalHours * 2500;
int prevTick = currentTick - intervalTicks;
bool isNewDay = (currentTick / 60000) > (prevTick / 60000);
if (!isNewDay) return;

    // 3. ЭФФЕКТ ДЛЯ ИГРОКА (исправил шанс с 0.9 на 0.05)
    if (Rand.Value < 0.05f)  // <-- Было 0.9f (90% шанс!), стало 0.05f (5%)
    {
        Map playerMap = Find.AnyPlayerHomeMap;
        if (playerMap != null)
        {
            IncidentParms parms = StorytellerUtility.DefaultParmsNow(IncidentCategoryDefOf.DiseaseHuman, playerMap);
            parms.target = playerMap;
            
            IncidentDef plagueDef = DefDatabase<IncidentDef>.GetNamed("Disease_Plague");
            if (plagueDef != null)
                plagueDef.Worker.TryExecute(parms);
        }
    }

// 4. ЭФФЕКТ ДЛЯ ФРАКЦИЙ (с руинами!)
foreach (var faction in Find.FactionManager.AllFactions)
{
    if (faction.IsPlayer || faction.defeated || faction.Hidden || 
        IgnoredFactionNames.Contains(faction.def.defName)) continue;

    float killChance;
    switch (faction.def.techLevel)
    {
        case TechLevel.Animal:
        case TechLevel.Neolithic: killChance = 0.10f; break;
        case TechLevel.Medieval:  killChance = 0.08f; break;
        case TechLevel.Industrial:killChance = 0.05f; break;
        case TechLevel.Spacer:    killChance = 0.03f; break; 
		case TechLevel.Ultra:     killChance = 0.01f; break; // 5%
		case TechLevel.Archotech: killChance = 0.01f; break; // 2% 
        default:                  killChance = 0.01f; break;
    }

    if (Rand.Value < killChance)
    {
        var settlements = Find.WorldObjects.Settlements.Where(s => s.Faction == faction).ToList();
        if (settlements.Any())
        {
            var victim = settlements.RandomElement();
            string victimName = victim.Name; // Запоминаем имя

            // ✅ ОПОВЕЩЕНИЕ (вставляем сюда)
            Find.LetterStack.ReceiveLetter(
                "Город вымер", 
                $"Пандемия уничтожила всё население города {victimName}, принадлежавшего фракции {faction.Name}. На его месте остались лишь руины.", 
                LetterDefOf.NeutralEvent, 
                new GlobalTargetInfo(victim.Tile) // Чтобы камера прыгала к руинам при клике
            );
            
            // ✅ СОЗДАЁМ РУИНЫ
            DestroyedSettlement ruin = (DestroyedSettlement)WorldObjectMaker.MakeWorldObject(WorldObjectDefOf.DestroyedSettlement);
            ruin.Tile = victim.Tile;
            ruin.SetFaction(victim.Faction);
            Find.WorldObjects.Add(ruin);
            
            // ✅ УДАЛЯЕМ ПОСЕЛЕНИЕ
            Find.WorldObjects.Remove(victim);
        }
        }
    }
}


	   
        public override void ExposeData()
{
    base.ExposeData();
    // ЭТУ СТРОКУ МЫ УДАЛИЛИ: Scribe_Collections.Look(ref allHiddenFactionDefs...

    Scribe_Collections.Look(ref activePool, "activePool", LookMode.Def);
    Scribe_Values.Look(ref initialized, "initialized", false);
    Scribe_Values.Look(ref nextTriggerTick, "nextTriggerTick", -1);
    Scribe_Collections.Look(ref factionDataList, "factionDataList", LookMode.Deep);
    Scribe_Collections.Look(ref usedDefs, "usedDefs", LookMode.Def);
	Scribe_Values.Look(ref plagueEndTick, "plagueEndTick", -1);

    // ВСТАВИТЬ ЭТО В КОНЕЦ МЕТОДА:
    if (Scribe.mode == LoadSaveMode.PostLoadInit)
    {
        // Принудительно обновляем список из базы данных после загрузки
        allHiddenFactionDefs = DefDatabase<FactionDef>.AllDefs
            .Where(d => d.defName.StartsWith("DF_") && d.maxConfigurableAtWorldCreation == 0)
            .ToList();

        if (activePool == null) activePool = new List<FactionDef>();
    }
}
    }
	
	
	
	public class IncidentParms_DF : IncidentParms
{
    // Наше кастомное поле для передачи целей камеры
    public LookTargets specificLookTargets;
}
	
	
public class IncidentWorker_GenericDF : IncidentWorker
{
    protected override bool TryExecuteWorker(IncidentParms parms)
    {
        // 1. Берем стандартные данные
        string label = parms.customLetterLabel ?? def.label;
        string text = parms.customLetterText ?? def.description;
        LetterDef type = parms.customLetterDef ?? LetterDefOf.NeutralEvent;

        // 2. Пытаемся достать наши цели для камеры
        LookTargets look = null;
        
        // Проверяем, является ли parms нашим кастомным классом
        if (parms is IncidentParms_DF dfParms)
        {
            look = dfParms.specificLookTargets;
        }

        // 3. Отправляем письмо с учетом lookTargets (если look == null, игра это поймет)
        Find.LetterStack.ReceiveLetter(label, text, type, look);
        
        return true; // Событие засчитано
    }
}


public static class DF_EventManager
{
    // Аргумент lookTargets добавлен последним (по умолчанию null)
    public static void Fire(string defName, string title, string message, LetterDef type = null, LookTargets lookTargets = null)
    {
        IncidentDef def = DefDatabase<IncidentDef>.GetNamed(defName, false);
        
        if (def == null)
        {
            Log.Error($"[DF] XML Def '{defName}' не найден!");
            return;
        }

        // ВАЖНО: Создаем НАШ кастомный класс параметров вместо стандартного
        IncidentParms_DF parms = new IncidentParms_DF();
        
        // Заполняем обязательные поля
        parms.target = Find.World; // Обязательно для мировых событий
        parms.customLetterLabel = title;
        parms.customLetterText = message;
        parms.customLetterDef = type ?? LetterDefOf.NeutralEvent;
        
        // Заполняем наше новое поле
        parms.specificLookTargets = lookTargets;
        
        // Запускаем событие. Worker примет наш IncidentParms_DF, так как он наследуется от IncidentParms
        def.Worker.TryExecute(parms);
    }
}


}
	
