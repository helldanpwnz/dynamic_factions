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
	public bool requireCommsForNews = false;
    public float entropyPerUpdate = 2f; 
    public int triggerIntervalHours = 24;
    public int splitMaxDistanceTiles = 25;    
    public float randomSettlementChance = 0f;
    public float attackMultiplier = 1f;
    public float defenseMultiplier = 1f;
    public float leaderPointsMultiplier = 1f;
    public float influenceMultiplier = 1f;
    public float stabilityMultiplier = 1f;
    public float globalCrisisChance = 0.1f;
    public float plagueChance = 0.1f;
	public float nuclearWarChance = 5f;
    
    public override void ExposeData()
    {
        // ТОЧНО ТАК КАК ТЫ УКАЗАЛ:
        Scribe_Values.Look(ref randomSettlementChance, "randomSettlementChance", 1f);
        Scribe_Values.Look(ref enableSplitFaction, "enableSplitFaction", true);
        Scribe_Values.Look(ref limitTechLevel, "limitTechLevel", false);
        Scribe_Values.Look(ref triggerIntervalHours, "triggerIntervalHours", 6);
        Scribe_Values.Look(ref entropyPerUpdate, "entropyPerUpdate", 3f);
        Scribe_Values.Look(ref splitMaxDistanceTiles, "splitMaxDistanceTiles", 100);
        Scribe_Values.Look(ref attackMultiplier, "attackMultiplier", 1f);
        Scribe_Values.Look(ref defenseMultiplier, "defenseMultiplier", 1f);
        Scribe_Values.Look(ref leaderPointsMultiplier, "leaderPointsMultiplier", 1f);
        Scribe_Values.Look(ref influenceMultiplier, "influenceMultiplier", 1f);
        Scribe_Values.Look(ref stabilityMultiplier, "stabilityMultiplier", 1f);
        Scribe_Values.Look(ref globalCrisisChance, "globalCrisisChance", 0.5f);
        Scribe_Values.Look(ref plagueChance, "plagueChance", 0.5f);
		Scribe_Values.Look(ref nuclearWarChance, "nuclearWarChance", 5f);
		Scribe_Values.Look(ref requireCommsForNews, "requireCommsForNews", false);

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
		
		listing.CheckboxLabeled("DF_RequireCommsForNews".Translate(), ref settings.requireCommsForNews, "DF_RequireCommsForNews_Tip".Translate());
        
        listing.CheckboxLabeled("DF_Setting_ShowDebugLogs".Translate(), 
            ref settings.showDebugLogs, 
            "DF_Setting_ShowDebugLogs_Tip".Translate());
        listing.CheckboxLabeled("DF_Setting_EnableDynamicFactions".Translate(), ref settings.enableSplitFaction);
        listing.CheckboxLabeled("DF_Setting_LimitTechLevel".Translate(), 
            ref settings.limitTechLevel, 
            "DF_Setting_LimitTechLevel_Tip".Translate());
        listing.GapLine();
        
        listing.Label("DF_Setting_TriggerIntervalHours".Translate());
        listing.Label("DF_Setting_TriggerIntervalHours_Value".Translate(settings.triggerIntervalHours));
        settings.triggerIntervalHours = (int)listing.Slider(settings.triggerIntervalHours, 1f, 168f);
        listing.GapLine();
        
        listing.Label("DF_Setting_SplitMaxDistanceTiles".Translate());
        listing.Label("DF_Setting_SplitMaxDistanceTiles_Value".Translate(settings.splitMaxDistanceTiles));
        settings.splitMaxDistanceTiles = (int)listing.Slider(settings.splitMaxDistanceTiles, 5f, 300f);
        listing.GapLine();
        
        listing.Label("DF_Setting_AttackMultiplier".Translate());
        listing.Label("DF_Setting_AttackMultiplier_Value".Translate(settings.attackMultiplier.ToString("F1")));
        settings.attackMultiplier = listing.Slider(settings.attackMultiplier, 0f, 5f);
        listing.Gap();
        
        listing.Label("DF_Setting_DefenseMultiplier".Translate());
        listing.Label("DF_Setting_DefenseMultiplier_Value".Translate(settings.defenseMultiplier.ToString("F1")));
        settings.defenseMultiplier = listing.Slider(settings.defenseMultiplier, 0f, 5f);
        listing.GapLine();
        
        listing.Label("DF_Setting_LeaderPointsMultiplier".Translate());
        listing.Label("DF_Setting_LeaderPointsMultiplier_Value".Translate(settings.leaderPointsMultiplier.ToString("F1")));
        settings.leaderPointsMultiplier = listing.Slider(settings.leaderPointsMultiplier, 0f, 5f);
        listing.GapLine();
        
        listing.Label("DF_Setting_StabilityMultiplier".Translate());
        listing.Label("DF_Setting_StabilityMultiplier_Value".Translate(settings.stabilityMultiplier.ToString("F1")));
        settings.stabilityMultiplier = listing.Slider(settings.stabilityMultiplier, 0f, 5f);
        listing.GapLine();
        
        listing.Label("DF_Setting_InfluenceMultiplier".Translate());
        listing.Label("DF_Setting_InfluenceMultiplier_Value".Translate(settings.influenceMultiplier.ToString("F1")));
        settings.influenceMultiplier = listing.Slider(settings.influenceMultiplier, 0f, 5f);
        listing.GapLine();        
    
        listing.Label("DF_Setting_RandomSettlementChance".Translate());
        listing.Label("DF_Setting_RandomSettlementChance_Value".Translate(settings.randomSettlementChance.ToString("F1")));
        settings.randomSettlementChance = listing.Slider(settings.randomSettlementChance, 0f, 10f);
        listing.GapLine();
        
        listing.Label("DF_Setting_GlobalCrisisChance".Translate());
        listing.Label("DF_Setting_GlobalCrisisChance_Value".Translate(settings.globalCrisisChance.ToString("F1")));
        settings.globalCrisisChance = listing.Slider(settings.globalCrisisChance, 0f, 10f);
        listing.GapLine();
        
        listing.Label("DF_Setting_PlagueChance".Translate());
        listing.Label("DF_Setting_PlagueChance_Value".Translate(settings.plagueChance.ToString("F1")));
        settings.plagueChance = listing.Slider(settings.plagueChance, 0f, 10f);
        listing.GapLine();
		
		listing.Label("DF_Setting_NuclearWarChance".Translate());
		listing.Label("DF_Setting_NuclearWarChance_Value".Translate(settings.nuclearWarChance.ToString("F1")));
		settings.nuclearWarChance = listing.Slider(settings.nuclearWarChance, 0f, 100f);
		listing.GapLine();
        
        listing.Label("DF_Setting_EntropyPerUpdate".Translate());
        listing.Label("DF_Setting_EntropyPerUpdate_Value".Translate(settings.entropyPerUpdate.ToString("F1")));
        settings.entropyPerUpdate = listing.Slider(settings.entropyPerUpdate, 0f, 10f);
        listing.GapLine();
        
        // ИСПРАВЛЕННАЯ КНОПКА СБРОСА - ВСЕ ПОЛЯ:
        if (listing.ButtonText("DF_Setting_ResetButton".Translate()))
        {
            // СБРАСЫВАЕМ ВСЕ ПОЛЯ К ЗНАЧЕНИЯМ ИЗ ExposeData:
            settings.randomSettlementChance = 1f;
            settings.enableSplitFaction = true;
            settings.showDebugLogs = false;
            settings.limitTechLevel = false;
            settings.triggerIntervalHours = 6;
            settings.splitMaxDistanceTiles = 100;
            settings.entropyPerUpdate = 2f;
            settings.plagueChance = 0.5f;
			settings.nuclearWarChance = 5f;
            settings.globalCrisisChance = 0.5f;
            settings.attackMultiplier = 1f;    // ← ДОБАВЛЕНО
            settings.defenseMultiplier = 1f;   // ← ДОБАВЛЕНО
            settings.leaderPointsMultiplier = 1f; // ← ДОБАВЛЕНО
            settings.influenceMultiplier = 1f;    // ← ДОБАВЛЕНО (в коде был, но проверь)
            settings.stabilityMultiplier = 1f;    // ← ДОБАВЛЕНО
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
    [TechLevel.Animal] = "DF_SpawnLetterTitle_Animal".Translate(),
    [TechLevel.Neolithic] = "DF_SpawnLetterTitle_Neolithic".Translate(),
    [TechLevel.Medieval] = "DF_SpawnLetterTitle_Medieval".Translate(),
    [TechLevel.Industrial] = "DF_SpawnLetterTitle_Industrial".Translate(),
    [TechLevel.Spacer] = "DF_SpawnLetterTitle_Spacer".Translate(),
    [TechLevel.Ultra] = "DF_SpawnLetterTitle_Ultra".Translate(),
    [TechLevel.Archotech] = "DF_SpawnLetterTitle_Archotech".Translate()
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
    [TechLevel.Neolithic] = "DF_SpawnDescription_Neolithic".Translate(),
    [TechLevel.Medieval] = "DF_SpawnDescription_Medieval".Translate(),
    [TechLevel.Industrial] = "DF_SpawnDescription_Industrial".Translate(),
    [TechLevel.Spacer] = "DF_SpawnDescription_Spacer".Translate(),
    [TechLevel.Ultra] = "DF_SpawnDescription_Ultra".Translate()
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
// ФРАКЦИЯ УНИЧТОЖЕНА
DF_EventManager.Fire(
    "DF_FactionDestroyed",                                    // DefName события
    "DF_FactionDestroyed_Label".Translate(),                  // Заголовок
    "DF_FactionDestroyed_Text".Translate(f.Name),             // Текст (f.Name вставится вместо {0})
    LetterDefOf.NeutralEvent,                                 // Тип письма
    null                                                      // LookTargets: тут null, так как смотреть не на что (городов нет)
    // Если у тебя есть переменная lastTile (тайл последнего города), 
    // можешь заменить null на: new GlobalTargetInfo(lastTile)
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
//отрицательные события влияния
private void TriggerInfluenceEvent(Faction faction, FactionStabilityData data, bool isPositive)
{
    bool ExecuteDefenseInvestment(string reason = null)
    {
        reason ??= "DF_ExecuteDefenseInvestment_Reason".Translate();
        data.attackPoints += 20f;
        data.defensePoints += 20f;
        data.accumulatedEntropy -= 2f;
		data.influencePoints += 90f;
        data.attackPoints = Mathf.Clamp(data.attackPoints, 0f, 200f);
        data.defensePoints = Mathf.Clamp(data.defensePoints, 0f, 200f);
// ✅ ОТПРАВЛЯЕМ ПИСЬМО: Инвестиции в армию (через менеджер событий)
DF_EventManager.Fire(
    "DF_DefenseInvestment",
    "DF_DefenseInvestment_Label".Translate(),
    "DF_DefenseInvestment_Text".Translate(
        faction.Name, 
        data.attackPoints, 
        data.defensePoints
    ),
    LetterDefOf.NeutralEvent,
    null  // Нет конкретной цели для камеры
);
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
                    data.accumulatedEntropy -= 2f;
					data.influencePoints += 90f;
                      // ✅ ОТПРАВЛЯЕМ ПИСЬМО: Дар земель (через менеджер событий)
        DF_EventManager.Fire(
            "DF_LandGift",
            "DF_LandGift_Label".Translate(),
            "DF_LandGift_Text".Translate(
                topFaction.Name,
                topFactionData.influencePoints,
                faction.Name,
                newSett.Name
            ),
            LetterDefOf.NeutralEvent,
            newSett  // Камера прыгнет к новой базе!
        );
        return;
                        }
						}
        else
        {
            // Если не нашли тайл для поселения
            ExecuteDefenseInvestment("DF_LandGift_NoTile".Translate());
            return; // ← Выходим
            }
        }
else if (rollNegative < 0.25f) // Марионеточное правительство 15%
{
    // 1. Ищем кукловода
    var puppetMasterData = factionDataList
        .Where(d => d.faction != faction && !d.faction.defeated && !d.faction.IsPlayer && 
                   !IgnoredFactionNames.Contains(d.faction.def.defName))
        .OrderByDescending(d => d.influencePoints)
        .FirstOrDefault();

    if (puppetMasterData == null)
    {
        // ИСПОЛЬЗУЕМ СУЩЕСТВУЮЩУЮ ФУНКЦИЮ
        ExecuteDefenseInvestment("DF_PuppetGov_NoMaster".Translate());
        return;
    }

    Faction master = puppetMasterData.faction;

    // 2. Восстанавливаем статы
    data.stabilityPoints += 50f; 
    data.stabilityPoints = Mathf.Clamp(data.stabilityPoints, -200f, 200f);
    data.accumulatedEntropy -= 1f;
    data.influencePoints += 90f; 

    // 3. Смена лидера
    if (faction.leader != null)
    {
        Pawn oldLeader = faction.leader;
        Find.WorldPawns.RemovePawn(oldLeader);
        oldLeader.Destroy();
    }
#pragma warning disable CS0618
    faction.GenerateNewLeader();
#pragma warning restore CS0618

    // 4. Дипломатия
    faction.TryAffectGoodwillWith(master, 100);
    master.TryAffectGoodwillWith(faction, 100);

    foreach (Faction other in Find.FactionManager.AllFactions)
    {
        if (other != faction && other != master && !other.IsPlayer && !other.defeated)
        {
            if (master.HostileTo(other))
            {
                faction.TryAffectGoodwillWith(other, -100); 
            }
        }
    }

    // 5. Идеология
    if (ModsConfig.IdeologyActive && master.ideos?.PrimaryIdeo != null)
    {
        faction.ideos.SetPrimary(master.ideos.PrimaryIdeo);
    }

    // 6. Письмо
    string newLeaderName = faction.leader != null ? faction.leader.Name.ToStringFull : "Unknown";

    DF_EventManager.Fire(
        "DF_PuppetGovernment",
        "DF_PuppetGovernment_Label".Translate(faction.Name), // Заголовок: "Марионеточное правительство: [Фракция]"
        "DF_PuppetGovernment_Text".Translate(
            faction.Name,    // {0} - Жертва
            master.Name,     // {1} - Хозяин
            newLeaderName    // {2} - Имя нового лидера
        ),
        LetterDefOf.NeutralEvent,
        null 
    );
    
    return;
}



        else if (rollNegative < 0.40f && ModsConfig.IdeologyActive) // Ересь 15%
        {
var topInfluential = factionDataList
    .Where(d => d.faction != faction 
                && !d.faction.defeated 
                && d.faction.ideos?.PrimaryIdeo != null 
                && !IgnoredFactionNames.Contains(d.faction.def.defName)
                // ✅ ДОБАВЛЕНО: Идеология должна отличаться!
                && (faction.ideos?.PrimaryIdeo == null || d.faction.ideos.PrimaryIdeo != faction.ideos.PrimaryIdeo))
    .OrderByDescending(d => d.influencePoints)
    .FirstOrDefault();
            
if (topInfluential != null)
{
    faction.ideos.SetPrimary(topInfluential.faction.ideos.PrimaryIdeo);
    data.defensePoints += 30f;
    data.defensePoints = Mathf.Clamp(data.defensePoints, 0f, 200f);
    data.accumulatedEntropy -= 2f;
    
    // ✅ ОТПРАВЛЯЕМ ПИСЬМО: Ересь (через менеджер событий)
	data.influencePoints += 90f;
    DF_EventManager.Fire(
        "DF_ReligiousConversion",
        "DF_ReligiousConversion_Label".Translate(),
        "DF_ReligiousConversion_Text".Translate(
            faction.Name,
            topInfluential.faction.Name
        ),
        LetterDefOf.NeutralEvent,
        null  // Камера не нужна (идеологическое событие)
    );
    return;
}

    else
    {
        ExecuteDefenseInvestment("DF_ReligiousConversion_NoTarget".Translate());
        return;
        }
        }
	
else if (rollNegative < 0.50f) // Экономический коллапс 50%
{
    data.stabilityPoints -= 20f;
    if (faction.leader != null)
    {
        data.leaderLegitimacy -= 5f;
        data.leaderLegitimacy = Mathf.Clamp(data.leaderLegitimacy, -20f, 20f);
    }
    data.stabilityPoints = Mathf.Clamp(data.stabilityPoints, -100f, 100f);
    data.accumulatedEntropy -= 2f;
	data.influencePoints += 90f;
    
    // ✅ ОТПРАВЛЯЕМ ПИСЬМО: Экономический коллапс (через менеджер событий)
    DF_EventManager.Fire(
        "DF_EconomicCollapse",
        "DF_EconomicCollapse_Label".Translate(),
        "DF_EconomicCollapse_Text".Translate(faction.Name),
        LetterDefOf.NeutralEvent,
        null  // Камера не нужна (экономическое событие)
    );
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
data.accumulatedEntropy -= 2f;
data.influencePoints += 90f;

// ✅ ОТПРАВЛЯЕМ ПИСЬМО: Экономический коллапс (фоллбек) (через менеджер событий)
DF_EventManager.Fire(
    "DF_EconomicFallback",
    "DF_EconomicFallback_Label".Translate(),
    "DF_EconomicFallback_Text".Translate(faction.Name),
    LetterDefOf.NeutralEvent,
    null  // Камера не нужна
);
return;
            }
            
Faction lostAlly = allies.RandomElement();
faction.TryAffectGoodwillWith(lostAlly, -100);
lostAlly.TryAffectGoodwillWith(faction, -100);
data.accumulatedEntropy -= 2f;
data.influencePoints += 90f;

// ✅ ОТПРАВЛЯЕМ ПИСЬМО: Разрыв отношений (через менеджер событий)
DF_EventManager.Fire(
    "DF_AllianceBreak",
    "DF_AllianceBreak_Label".Translate(),
    "DF_AllianceBreak_Text".Translate(
        faction.Name,
        lostAlly.Name
    ),
    LetterDefOf.NeutralEvent,
    null  // Камера не нужна (дипломатическое событие)
);
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
newBase.Name = oldName;
Find.WorldObjects.Add(newBase);

data.attackPoints += 30f;
data.defensePoints += 30f;
data.attackPoints = Mathf.Clamp(data.attackPoints, 0f, 200f);
data.defensePoints = Mathf.Clamp(data.defensePoints, 0f, 200f);
data.accumulatedEntropy -= 2f;
data.influencePoints += 90f;

// ✅ ОТПРАВЛЯЕМ ПИСЬМО: Продажа земель (через менеджер событий)
DF_EventManager.Fire(
    "DF_LandSale",
    "DF_LandSale_Label".Translate(),
    "DF_LandSale_Text".Translate(
        faction.Name,
        oldName,
        topInfluential.faction.Name
    ),
    LetterDefOf.NeutralEvent,
    newBase  // Камера прыгнет к проданному поселению!
);
return;

    }
    else
    {
        ExecuteDefenseInvestment("DF_LandSale_NoBuyer".Translate());
        return;
    }
        }
else if (rollNegative < 0.90f) // Диктатура 40%
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
    data.accumulatedEntropy -= 2f;
	data.influencePoints += 90f;
    
    // ✅ НОВОЕ: потеря 1-2 союзников
    var allies = Find.FactionManager.AllFactions.Where(f => 
        f != faction && !f.IsPlayer && !f.defeated && f.def.humanlikeFaction && 
        !IgnoredFactionNames.Contains(f.def.defName) && !faction.HostileTo(f)).ToList();

    int alliesToLose = Rand.RangeInclusive(1, Mathf.Min(2, allies.Count));
    for (int i = 0; i < alliesToLose; i++)
    {
        if (allies.Count == 0) break;
        Faction ally = allies.RandomElement();
        faction.TryAffectGoodwillWith(ally, -100);
        ally.TryAffectGoodwillWith(faction, -100);
        allies.Remove(ally);
    }
    
    // ✅ ОТПРАВЛЯЕМ ПИСЬМО: Диктатура (через менеджер событий)
    DF_EventManager.Fire(
        "DF_Dictatorship",
        "DF_Dictatorship_Label".Translate(),
        "DF_Dictatorship_Text".Translate(
            faction.Name,
            alliesToLose
        ),
        LetterDefOf.NeutralEvent,
        null  // Камера не нужна (политическое событие)
    );
    return;
        }
        else // Помощь союзников 10%
        {
            // 1. Ищем хотя бы одного реального союзника
            var allies = Find.FactionManager.AllFactions.Where(f => 
                !f.defeated && f != faction && !f.IsPlayer && 
                !IgnoredFactionNames.Contains(f.def.defName) && 
                faction.RelationKindWith(f) == FactionRelationKind.Ally).ToList();

            if (allies.Count > 0)
            {
                // Есть союзники — всё честно
                data.stabilityPoints += 10f;
                data.attackPoints += 10f;
                data.defensePoints += 10f;
                data.stabilityPoints = Mathf.Clamp(data.stabilityPoints, -100f, 100f);
                data.attackPoints = Mathf.Clamp(data.attackPoints, 0f, 200f);
                data.defensePoints = Mathf.Clamp(data.defensePoints, 0f, 200f);
                data.accumulatedEntropy -= 2f;
                data.influencePoints += 90f; // Восстанавливаем влияние
                
                // Берем случайного союзника для красоты текста
                Faction ally = allies.RandomElement(); 

                DF_EventManager.Fire(
                    "DF_AlliesAid",
                    "DF_AlliesAid_Label".Translate(),
                    "DF_AlliesAid_Text".Translate(faction.Name, ally.Name),
                    LetterDefOf.NeutralEvent,
                    null
                );
                return;
            }
            else
            {
                // Союзников нет — запускаем обычное укрепление (фоллбек)
                ExecuteDefenseInvestment("DF_AlliesAid_NoAllies".Translate());
                return;
            }
        } // Закрываем блок else
    } // <--- ВАЖНО: Закрываем метод TriggerInfluenceEvent


// Положительные события +100 влияния
float roll = Rand.Value;
if (roll < 0.10f) // Аннексия 10%
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
                newBase.Name = oldName;
                Find.WorldObjects.Add(newBase);
                
                FactionStabilityData targetData = GetOrCreateData(targetFaction);
                targetData.influencePoints -= 30f;
                data.accumulatedEntropy += 2f;
				data.influencePoints -= 90f;
                
                // ✅ ОТПРАВЛЯЕМ ПИСЬМО: Аннексия (через менеджер событий)
                DF_EventManager.Fire(
                    "DF_Annexation",
                    "DF_Annexation_Label".Translate(),
                    "DF_Annexation_Text".Translate(
                        faction.Name,
                        oldName
                    ),
                    LetterDefOf.NeutralEvent,
                    newBase  // Камера прыгнет к аннексированному поселению!
                );
                return;
            }
        }
    }
    // Фоллбек: Инвестиции в армию
    ExecuteDefenseInvestment("DF_Annexation_NoTarget".Translate());
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
        friendFaction.TryAffectGoodwillWith(faction, 100);
        data.accumulatedEntropy += 2f;
		data.influencePoints -= 90f;
        
        // ✅ ОТПРАВЛЯЕМ ПИСЬМО: Новая дружба (через менеджер событий)
        DF_EventManager.Fire(
            "DF_NewFriendship",
            "DF_NewFriendship_Label".Translate(),
            "DF_NewFriendship_Text".Translate(
                faction.Name,
                friendFaction.Name
            ),
            LetterDefOf.NeutralEvent,
            null  // Камера не нужна (дипломатическое событие)
        );
        return;
        }
        ExecuteDefenseInvestment("DF_NewFriendship_NoEnemies".Translate());
        return;
    }
else if (roll < 0.30f) // Смерть на переговорах 10%
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
    data.accumulatedEntropy += 2f;
	data.influencePoints -= 90f;
    
    // ✅ ОТПРАВЛЯЕМ ПИСЬМО: Смерть лидера (через менеджер событий)
    DF_EventManager.Fire(
        "DF_LeaderDeath",
        "DF_LeaderDeath_Label".Translate(),
        "DF_LeaderDeath_Text".Translate(
            faction.Name,
            oldLeaderName,
            transferAmount
        ),
        LetterDefOf.NeutralEvent,
        null  // Камера не нужна (лидер уничтожен)
    );
    return;
    }
    else if (roll < 0.40f) // Инвестирование в оборону 10% (было 20%)
    {
        ExecuteDefenseInvestment();
        return;
    }
else if (roll < 0.60f) // Благоустройство 30%
{
    data.stabilityPoints += 20f;
    data.leaderLegitimacy += 5f;
    data.stabilityPoints = Mathf.Clamp(data.stabilityPoints, -100f, 100f);
    data.leaderLegitimacy = Mathf.Clamp(data.leaderLegitimacy, -20f, 20f);
    data.accumulatedEntropy += 2f;
	data.influencePoints -= 90f;
    
    // ✅ ОТПРАВЛЯЕМ ПИСЬМО: Эпоха процветания (через менеджер событий)
    DF_EventManager.Fire(
        "DF_ProsperityEra",
        "DF_ProsperityEra_Label".Translate(),
        "DF_ProsperityEra_Text".Translate(faction.Name),
        LetterDefOf.NeutralEvent,
        null  // Камера не нужна (общее развитие)
    );
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
        // ✅ Фоллбек: нет цели
        DF_EventManager.Fire("DF_DefenseInvestment",
            "DF_DefenseInvestment_Label".Translate(),
            "DF_ColorRevolution_NoTarget_Text".Translate(faction.Name),
            LetterDefOf.NeutralEvent, null);
        ExecuteDefenseInvestment();
        return;
    }
    
    Faction targetFaction = targets[0].faction;
    var targetData = targets[0];
    
    // Получаем поселения цели
    var targetSettlements = Find.WorldObjects.Settlements
        .Where(s => s.Faction == targetFaction).ToList();
    
    if (targetSettlements.Count == 0)
    {
        // ✅ Фоллбек: нет поселений
        DF_EventManager.Fire("DF_DefenseInvestment",
            "DF_DefenseInvestment_Label".Translate(),
            "DF_ColorRevolution_NoSettlements_Text".Translate(faction.Name),
            LetterDefOf.NeutralEvent, null);
        ExecuteDefenseInvestment();
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
        // ✅ Фоллбек: нет шаблонов
        DF_EventManager.Fire("DF_DefenseInvestment",
            "DF_DefenseInvestment_Label".Translate(),
            "DF_ColorRevolution_NoDefs_Text".Translate(faction.Name),
            LetterDefOf.NeutralEvent, null);
        ExecuteDefenseInvestment();
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
            // ✅ Фоллбек: нет места
            DF_EventManager.Fire("DF_DefenseInvestment",
                "DF_DefenseInvestment_Label".Translate(),
                "DF_ColorRevolution_NoTile_Text".Translate(faction.Name),
                LetterDefOf.NeutralEvent, null);
            ExecuteDefenseInvestment();
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
    
    // 7. ПОНИЖАЕМ ВЛИЯНИЕ СПОНСОРА
    data.influencePoints -= 50f;
    data.accumulatedEntropy += 2f;
    
    // ✅ ГЛАВНОЕ ПИСЬМО О СОБЫТИИ
    string sponsorName = faction.Name;
    string targetName = targetFaction.Name;
    string rebelName = rebelFaction.Name;
    
    DF_EventManager.Fire(
        "DF_ColorRevolution",
        "DF_ColorRevolution_Label".Translate(),
        "DF_ColorRevolution_Text".Translate(sponsorName, targetName, rebelName, rebelBase.Name),
        LetterDefOf.NeutralEvent,
        rebelBase
    );
    
    OnFactionUsed(newFactionDef);
    return;
}
else // Пропаганда (теперь занимает оставшиеся 25%)
{
    if (!ModsConfig.IdeologyActive)
    {
        // Фоллбек: нет DLC Ideology
        DF_EventManager.Fire("DF_DefenseInvestment",
            "DF_DefenseInvestment_Label".Translate(),
            "DF_CulturalPropaganda_NoDLC_Text".Translate(faction.Name),
            LetterDefOf.NeutralEvent, null);
        ExecuteDefenseInvestment();
        return;
    }
    
    // Выбираем цель с самой низкой стабильностью
var targetData = factionDataList
    .Where(d => d.faction != faction 
                && !d.faction.defeated 
                && !IgnoredFactionNames.Contains(d.faction.def.defName)
                // ✅ ДОБАВЛЕНО: Цель должна иметь ДРУГУЮ идеологию (или не иметь её, если это возможно)
                && (d.faction.ideos?.PrimaryIdeo == null || d.faction.ideos.PrimaryIdeo != faction.ideos.PrimaryIdeo))
    .OrderBy(d => d.stabilityPoints) 
    .FirstOrDefault();
    
    if (targetData != null && faction.ideos?.PrimaryIdeo != null)
    {
        Ideo myIdeo = faction.ideos.PrimaryIdeo;
        targetData.faction.ideos.SetPrimary(myIdeo);
        
        // ✅ НОВОЕ: Наносим "ущерб" и бонусы цели
        targetData.influencePoints -= 50f; // Теряют влияние из-за смены идеологии
        targetData.defensePoints += 50f;   // Но армия усиливается (репрессии/сплочение)
        
        // Ограничиваем значения, чтобы не вылетело за рамки
        targetData.influencePoints = Mathf.Clamp(targetData.influencePoints, -200f, 200f);
        targetData.defensePoints = Mathf.Clamp(targetData.defensePoints, 0f, 200f);

        // Расходы атакующего
        data.accumulatedEntropy += 2f;
        data.influencePoints -= 90f;
        
        // Отправляем письмо
        DF_EventManager.Fire(
            "DF_CulturalPropaganda",
            "DF_CulturalPropaganda_Label".Translate(),
            "DF_CulturalPropaganda_Text".Translate(
                faction.Name,            
                targetData.faction.Name, 
                myIdeo.name              
            ),
            LetterDefOf.NeutralEvent,
            null 
        );
        return;
    }
    
    // Фоллбек: нет цели
    DF_EventManager.Fire("DF_DefenseInvestment",
        "DF_DefenseInvestment_Label".Translate(),
        "DF_CulturalPropaganda_NoTarget_Text".Translate(faction.Name),
        LetterDefOf.NeutralEvent, null);
    ExecuteDefenseInvestment();
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
                DF_EventManager.Fire(
                    "DF_IdeologicalSchism",
                    "DF_IdeologicalSchism_Label".Translate(),
                    "DF_IdeologicalSchism_Text".Translate(
                        schismFaction.Name,  // {0} - фракция
                        oldIdeo.name,        // {1} - старая идеология
                        newIdeo.name         // {2} - новая идеология
                    ),
                    LetterDefOf.NeutralEvent,
                    null
                );
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

// 2. Лидер (смена + пассивка) ← ОСТАВЛЕНО КАК ЕСТЬ
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

// 🔥 ПАССИВКА (ОБНОВЛЕННАЯ)
// Используем префикс p_, чтобы избежать ошибок дублирования переменных (CS0128)
float p_leaderStableBonus = (faction.leader != null ? data.leaderLegitimacy / 10f : 0f);

// 1. Считаем врагов (Штраф -1 за каждого)
int p_enemyCount = Find.FactionManager.AllFactions.Count(f => 
    !f.defeated && 
    f != faction && 
    f.def.humanlikeFaction && 
    !IgnoredFactionNames.Contains(f.def.defName) && 
    faction.HostileTo(f));

// 2. Считаем союзников (Бонус +1 за каждого)
int p_allyCount = Find.FactionManager.AllFactions.Count(f => 
    !f.defeated && 
    f != faction && 
    f.def.humanlikeFaction && 
    !IgnoredFactionNames.Contains(f.def.defName) && 
    faction.RelationKindWith(f) == FactionRelationKind.Ally);

float p_warPenalty = p_enemyCount * 0.5f;
float p_allyBonus = p_allyCount * 0.5f;

// ИТОГОВАЯ ФОРМУЛА: Лидер + Союзники - Враги (без поселений)
data.stabilityPoints += p_leaderStableBonus + p_allyBonus - p_warPenalty;

if (DynamicFactionsMod.settings.showDebugLogs)
{
    Log.Message($"[DF] {faction.Name}: Leader({p_leaderStableBonus:F1}) + Allies({p_allyBonus:F1}) - War({p_warPenalty:F1}) = TotalChange({p_leaderStableBonus + p_allyBonus - p_warPenalty:F1})");
}

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

int enemyCount = Find.FactionManager.AllFactions.Count(f => 
    f != faction && !f.IsPlayer && !f.defeated && !f.Hidden && 
    !f.def.defName.Contains("Trade") && 
    faction.HostileTo(f));

int settlementCount = Find.WorldObjects.Settlements.Count(s => s.Faction == faction);

// Штрафы
float sizePenalty = settlementCount * 0.5f; // 0.5 за каждое поселение
float enemyLoss   = enemyCount * -1f;    // -0.5 за каждого врага

float leaderInfPassive = (faction.leader != null ? data.leaderLegitimacy / 10f : 0f);

// ✅ ИСПРАВЛЕННАЯ ФОРМУЛА: убрал baseDecay
float influenceChange = (allyCount * 0.5f) + (sameIdeoCount * 1f) + leaderInfPassive + sizePenalty + enemyLoss;

influenceChange *= DynamicFactionsMod.settings.influenceMultiplier;
data.influencePoints += influenceChange;
data.influencePoints = Mathf.Clamp(data.influencePoints, -100f, 100f);

// ЛОГИКА ШАНСА ВЛИЯНИЯ
float infChance = 0f;
if (data.influencePoints > 50f)
{
    infChance = (data.influencePoints - 50f) / 100f; 
}
else if (data.influencePoints < -50f)
{
    infChance = (Mathf.Abs(data.influencePoints) - 50f) / 100f;
}

// Проверяем событие влияния
if (selectedEvent == "Influence" && allowEvents && !eventOccurred && infChance > 0 && Rand.Value < infChance)
{
    if (data.influencePoints > 50f)
    {
		TriggerInfluenceEvent(faction, data, true); // Очки спишутся внутри
    }
    else
    {
    TriggerInfluenceEvent(faction, data, false); // Очки добавятся внутри
    }
    eventOccurred = true; 
}

// Проверка изменения поселений
if (data.lastSettlementCount != -1) 
{
    // ✅ ИСПРАВЛЕНО: settlementDiff вместо diff (чтобы не было конфликта имен)
    int settlementDiff = settlementCount - data.lastSettlementCount;
    
    if (settlementDiff > 0)
    {
        // Захватили или основали (+15 за каждое)
        float bonus = settlementDiff * 30f;
        data.influencePoints += bonus;
    }
    else if (settlementDiff < 0)
    {
        // Потеряли или уничтожили (-30 за каждое)
        float penalty = settlementDiff * 15f; 
        data.influencePoints += penalty;
    }
}
// Запоминаем текущее кол-во для следующего раза
data.lastSettlementCount = settlementCount;


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
        string reason = "DF_AttackReason_Aggression".Translate();
	
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
                    conflictReason = "DF_ConflictReason_Diplomatic".Translate();
                else if (conflictChance < 0.6f)
                    conflictReason = "DF_ConflictReason_Border".Translate();
                else if (conflictChance < 0.8f && ModsConfig.IdeologyActive)
                    conflictReason = "DF_ConflictReason_Ideological".Translate();
                else
                    conflictReason = "DF_ConflictReason_Economic".Translate();

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
                    reason = "DF_AttackReason_IdeologyThreat".Translate(threatIdeo.name);
                }
            }
            else if (moderateThreat && threatIdeo != null && Rand.Value < 0.50f)
            {
                var worthyCultEnemies = validEnemies.Where(x => x.fac.ideos?.PrimaryIdeo == threatIdeo && x.tech >= faction.def.techLevel).ToList();
                if (worthyCultEnemies.Count > 0)
                {
                    enemyTarget = worthyCultEnemies.OrderBy(x => x.dist).First().fac;
                    reason = "DF_AttackReason_IdeologyContainment".Translate(threatIdeo.name);
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
                        reason = "DF_AttackReason_TechnologicalRivalry".Translate();
                    }
                    else if (validEnemies.Count > 0)
                    {
                        enemyTarget = validEnemies[0].fac; // ближайший
                        reason = "DF_AttackReason_TerritorialDispute".Translate();
                    }
                }
                else // 20% ближайший
                {
                    if (validEnemies.Count > 0)
                    {
                        enemyTarget = validEnemies[0].fac;
                        reason = "DF_AttackReason_TerritorialDispute".Translate();
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
            // ✅ ИСПРАВЛЕННАЯ ПРОВЕРКА: Мир действует, пока текущее время меньше времени окончания
            if (data.lastPeaceFaction == enemyTarget && 
                data.lastPeaceTick > Find.TickManager.TicksGame)
            {
                if (DynamicFactionsMod.settings.showDebugLogs)
                {
                    float daysLeft = (data.lastPeaceTick - Find.TickManager.TicksGame) / 60000f;
                    Log.Message($"[DF] ⏳ {faction.Name} пропускает атаку на {enemyTarget.Name} (до конца мира {daysLeft:F1} дн.)");
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
			
            // Полный расчет переговоров (как в старом кода)
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
    
DF_EventManager.Fire(
    "DF_PeaceMade",
    "DF_PeaceMade_Label".Translate(),
    "DF_PeaceMade_Text".Translate(faction.Name, enemyTarget.Name, negotiationChance.ToString("P0")),
    LetterDefOf.NeutralEvent,
    null
);
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
float battleAtkAttack = (data.attackPoints + data.defensePoints * 0.5f) * techAtkMult;

float battleDefLeader = targetData.leaderLegitimacy;
float battleDefDefense = (targetData.defensePoints + targetData.attackPoints * 0.5f) * techDefMult;

float successChance = (battleAtkLeader + battleAtkAttack - battleDefLeader - battleDefDefense) / 200f;
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
 //Пиррова победа
CaptureBases(
    faction, 
    targetBases, 
    1, 
    "DF_PyrrhicVictory_Label".Translate(),
    "DF_PyrrhicVictory_Text".Translate(faction.Name, targetBases.RandomElement().Name),
    "DF_PyrrhicVictory" // ← ДОБАВИЛИ DefName события
);

}
// 2) 40% Захват 1-3 ближайших + 15% смерть лидера защитника
else if (roll < 0.10f + 0.40f)
{
int count = Mathf.Min(targetBases.Count, Rand.RangeInclusive(1, 3));
CaptureBases(faction, targetBases, count, 
    "DF_Invasion_Label".Translate(),
    "DF_Invasion_Text".Translate(faction.Name, count, enemyTarget.Name),
    "DF_Invasion");
}

// 3) 20% Обращение в идеологию без потерь, защитник -50 влияния
else if (roll < 0.10f + 0.40f + 0.20f)
{
    if (ModsConfig.IdeologyActive && faction.ideos?.PrimaryIdeo != null && enemyTarget.ideos != null)
    {
        enemyTarget.ideos.SetPrimary(faction.ideos.PrimaryIdeo);
    }
    targetData.influencePoints -= 50f;
//Идеологическая капитуляция
DF_EventManager.Fire(
    "DF_IdeologicalCapitulation",
    "DF_IdeologicalCapitulation_Label".Translate(),
    "DF_IdeologicalCapitulation_Text".Translate(enemyTarget.Name, faction.Name),
    LetterDefOf.NeutralEvent,
    null
);

                            data.attackPoints *= 0.85f;
                        }
// 4) 20% Уничтожение 1 базы -> руины
else if (roll < 0.10f + 0.40f + 0.20f + 0.20f)
{
    targetData.stabilityPoints -= 30f;
//Варварская расправа
DestroyBases(targetBases, 1, 
    "DF_BarbarianSlaughter_Label".Translate(),
    "DF_BarbarianSlaughter_Text".Translate(faction.Name, enemyTarget.Name),
    "DF_BarbarianSlaughter");
                        }
                        // 5) 10% "Война пришла в дом": обмен ближайшими базами
                        else
                        {
                            if (attackerBases.Count == 0 || targetBases.Count == 0)
                            {
								//налёт
CaptureBases(faction, targetBases, 1, 
    "DF_Raid_Label".Translate(),
    "DF_Raid_Text".Translate(faction.Name, enemyTarget.Name),
    "DF_Raid"
);
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
                                aNew.Name = aName;
                                Find.WorldObjects.Add(aNew);

                                // захват атакующим базы защитника
                                int dTile = defendersClosestToAttacker.Tile;
                                string dName = defendersClosestToAttacker.Name;
                                Find.WorldObjects.Remove(defendersClosestToAttacker);
                                Settlement dNew = (Settlement)WorldObjectMaker.MakeWorldObject(WorldObjectDefOf.Settlement);
                                dNew.SetFaction(faction);
                                dNew.Tile = dTile;
                                dNew.Name = dName;
                                Find.WorldObjects.Add(dNew);
// война пришла в дом
DF_EventManager.Fire(
    "DF_WarAtHome",
    "DF_WarAtHome_Label".Translate(),
    "DF_WarAtHome_Text".Translate(faction.Name, dName, enemyTarget.Name, aName),
    LetterDefOf.NeutralEvent,
    new GlobalTargetInfo(dTile)
);
                            }
                        }
                    }
                    else // Industrial+
                    {
                        // 1. ПРОВЕРКА НА ЯДЕРНУЮ ВОЙНУ
                        if (Rand.Value < DynamicFactionsMod.settings.nuclearWarChance / 100f)
                        {
                            var factionsToNuke = Find.FactionManager.AllFactions.Where(f => !f.IsPlayer && !f.defeated).ToList();
                            
                            foreach (Faction f in factionsToNuke)
                            {
                                var fSettlements = Find.WorldObjects.Settlements.Where(s => s.Faction == f).ToList();
                                if (fSettlements.Count == 0) continue;
                                
                                int minDestroy = Verse.Rand.RangeInclusive(2, 6);
                                int maxDestroy = Verse.Rand.RangeInclusive(4, 8);
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
                            
                            DF_EventManager.Fire(
                                "DF_NuclearWar",
                                "DF_NuclearWar_Label".Translate(),
                                "DF_NuclearWar_Text".Translate(),
                                LetterDefOf.ThreatBig,
                                null
                            );
                            
                            Map playerMap = Find.AnyPlayerHomeMap;
                            if (playerMap != null)
                            {
                                IncidentParms parms = StorytellerUtility.DefaultParmsNow(IncidentCategoryDefOf.ThreatBig, playerMap);
                                parms.target = playerMap;
                                IncidentDef falloutDef = DefDatabase<IncidentDef>.GetNamed("ToxicFallout");
                                if (falloutDef != null) falloutDef.Worker.TryExecute(parms);
                            }
                            return;
                        }

                        // 2. ОБЫЧНАЯ РУЛЕТКА
                        float normalRoll = Rand.Value;

                        // Пиррова победа
                        if (normalRoll < 0.10f)
                        {
                            KillLeaderNegotiationsStyle(faction, data, -8f, -2f);
                            data.influencePoints -= 30f;
                            CaptureBases(faction, targetBases, 1, 
                                "DF_PyrrhicVictory_Industrial_Label".Translate(),
                                "DF_PyrrhicVictory_Industrial_Text".Translate(faction.Name),
                                "DF_PyrrhicVictory_Industrial");
                        }
                        // Блицкриг
                        else if (normalRoll < 0.45f)
                        {
                            int count = Mathf.Min(targetBases.Count, Rand.RangeInclusive(1, 3));
                            MaybeKillDefenderLeader15();
                            CaptureBases(faction, targetBases, count, 
                                "DF_Blitzkrieg_Label".Translate(),
                                "DF_Blitzkrieg_Text".Translate(faction.Name, count, enemyTarget.Name),
                                "DF_Blitzkrieg");
                        }
                        // Инфо победа
                        else if (normalRoll < 0.60f)
                        {
                            if (ModsConfig.IdeologyActive && faction.ideos?.PrimaryIdeo != null && enemyTarget.ideos != null)
                            {
                                enemyTarget.ideos.SetPrimary(faction.ideos.PrimaryIdeo);
                            }
                            targetData.influencePoints -= 50f;
                            DF_EventManager.Fire(
                                "DF_InformationalVictory",
                                "DF_InformationalVictory_Label".Translate(),
                                "DF_InformationalVictory_Text".Translate(faction.Name, enemyTarget.Name),
                                LetterDefOf.NeutralEvent,
                                null
                            );
                            data.attackPoints *= 0.85f;
                        }
                        // Точечный удар
                        else if (normalRoll < 0.80f)
                        {
                            MaybeKillDefenderLeader15();
                            DestroyBases(targetBases, 1, 
                                "DF_PrecisionStrike_Label".Translate(),
                                "DF_PrecisionStrike_Text".Translate(faction.Name, enemyTarget.Name),
                                "DF_PrecisionStrike"
                            );
                        }
                        // Ракетный удар
                        else
                        {
                            int count = Mathf.Min(targetBases.Count, Rand.RangeInclusive(1, 3));
                            MaybeKillDefenderLeader15();
                            targetData.stabilityPoints -= 30f;
                            DestroyBases(targetBases, count, 
                                "DF_RocketStrike_Label".Translate(),
                                "DF_RocketStrike_Text".Translate(faction.Name, count, enemyTarget.Name),
                                "DF_RocketStrike"
                            );
                        }
                    } // <-- ЗАКРЫВАЕМ else (Industrial+)
                } // <-- ЗАКРЫВАЕМ if (Rand.Value < successChance) (ПОБЕДА)

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
        
DF_EventManager.Fire(
    "DF_Defense",
    "DF_Defense_Label".Translate(),
    "DF_Defense_Text".Translate(enemyTarget.Name, faction.Name, data.attackPoints, targetData.defensePoints),
    LetterDefOf.NeutralEvent,
    null
);

    }
    else if (outcomeRoll < 0.70f) // 20% - Тяжелая оборона
    {
        data.attackPoints = Mathf.Max(0, data.attackPoints - 20f);
        targetData.defensePoints = Mathf.Max(0, targetData.defensePoints - 40f);
        
        data.influencePoints -= 20f;
        data.stabilityPoints -= 10f;
        targetData.influencePoints += 15f;
        targetData.stabilityPoints += 10f;
        
DF_EventManager.Fire(
    "DF_HeavyDefense",
    "DF_HeavyDefense_Label".Translate(),
    "DF_HeavyDefense_Text".Translate(enemyTarget.Name, faction.Name, data.attackPoints, targetData.defensePoints),
    LetterDefOf.NeutralEvent,
    null
);
    }
    else if (outcomeRoll < 0.90f) // 20% - Отражено
    {
        data.attackPoints = Mathf.Max(0, data.attackPoints - 20f);
        
        data.influencePoints -= 25f;
        data.stabilityPoints -= 15f;
        targetData.influencePoints += 20f;
        targetData.stabilityPoints += 15f;
        
DF_EventManager.Fire(
    "DF_Repelled",
    "DF_Repelled_Label".Translate(),
    "DF_Repelled_Text".Translate(enemyTarget.Name, faction.Name, data.attackPoints, targetData.defensePoints),
    LetterDefOf.NeutralEvent,
    null
);
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
                newBase.Name = oldName;
                Find.WorldObjects.Add(newBase);
                
DF_EventManager.Fire(
    "DF_Counteroffensive",
    "DF_Counteroffensive_Label".Translate(),
    "DF_Counteroffensive_Text".Translate(enemyTarget.Name, faction.Name, oldName),
    LetterDefOf.NeutralEvent,
    newBase
);
            }
            else
            {
DF_EventManager.Fire(
    "DF_Counteroffensive",
    "DF_Counteroffensive_Label".Translate(),
    "DF_Counteroffensive_Text".Translate(enemyTarget.Name, faction.Name),
    LetterDefOf.NeutralEvent,
    null
);
            }
        }
        else
        {
DF_EventManager.Fire(
    "DF_Counteroffensive",
    "DF_Counteroffensive_Label".Translate(),
    "DF_Counteroffensive_NoBases_Text".Translate(enemyTarget.Name, faction.Name),
    LetterDefOf.NeutralEvent,
    null
);
        }
    }
    
    // Общие изменения для всех исходов поражения
    data.attackPoints = Mathf.Clamp(data.attackPoints, 0f, 200f);
    data.defensePoints = Mathf.Clamp(data.defensePoints, 0f, 200f);
    targetData.attackPoints = Mathf.Clamp(targetData.attackPoints, 0f, 200f);
    targetData.defensePoints = Mathf.Clamp(targetData.defensePoints, 0f, 200f);
    
    data.influencePoints = Mathf.Clamp(data.influencePoints, -100f, 100f);
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
float leaderMilitaryBonus = faction.leader != null ? data.leaderLegitimacy / 10f : 0f;  // твой вариант, или *0.1f

int enemyFactionCount = Find.FactionManager.AllFactions.Count(f => 
    f != faction && !f.IsPlayer && !f.defeated && f.def.humanlikeFaction && 
    faction.HostileTo(f));

int allyFactionCount = Find.FactionManager.AllFactions.Count(f => 
    f != faction && !f.IsPlayer && !f.defeated && f.def.humanlikeFaction && 
    !faction.HostileTo(f));

if (militaryFocusAttack)
{
    float newAttackGain = (enemyFactionCount + 0.5f) + leaderMilitaryBonus;  // +1f базовый
    newAttackGain *= DynamicFactionsMod.settings.attackMultiplier;
    data.attackPoints += newAttackGain;
}
else
{
    float newDefenseGain = (allyFactionCount + 0.5f) + leaderMilitaryBonus;  // +1f базовый
    newDefenseGain *= DynamicFactionsMod.settings.defenseMultiplier;
    data.defensePoints += newDefenseGain;
}

// Пассивные бонусы от influence/stability
data.attackPoints += data.influencePoints / 100f;
data.defensePoints += data.stabilityPoints / 100f;

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
        Log.Warning($" 🔥 ТРИГГЕР +: {faction.Name}");
        TrySpawnSplitFaction(faction, true); // Просто вызываем, очки спишутся внутри
    }
    else
    {
        Log.Warning($" 🔥 ТРИГГЕР -: {faction.Name}");
        TrySpawnSplitFaction(faction, false); // Просто вызываем, очки добавятся 
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
    
    string focusLabel = militaryFocusAttack ? "DF_Log_Focus_Attack".Translate() : "DF_Log_Focus_Defense".Translate();
    
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
    data.influencePoints = Mathf.Clamp(data.influencePoints, -100f, 100f);
    
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
    string t = "DF_Strengthening_Label".Translate();
    string b = "DF_Strengthening_Text".Translate(reasonPrefix, parentFaction.Name, data.attackPoints, data.defensePoints, data.leaderLegitimacy);
    data.accumulatedEntropy += 2f;
	data.stabilityPoints -= 90f; 
    DF_EventManager.Fire(
    "DF_Strengthening",
    t,
    b,
    LetterDefOf.NeutralEvent,
    originSettlement
);
    return false;
}
        
        // Интриги: 0.5% за базу
float intriguesChance = parentSettlements.Count * 0.005f;
if (Rand.Value < intriguesChance)
{
    spawnNewSettlement = false;
    settlementsToSeize = Rand.Range(1, 4);
    targetTech = parentFaction.def.techLevel;
    
    // ✅ ИСПРАВЛЕНО: Передаем ИМЯ + ЧИСЛО (2 аргумента)
    label = "DF_Intrigues_Label".Translate(parentFaction.Name);
    text = "DF_Intrigues_Text".Translate(parentFaction.Name, settlementsToSeize);
    letterDef = LetterDefOf.NeutralEvent;
    goodwillChange = -200;
    data.accumulatedEntropy += 2f;
	data.stabilityPoints -= 90f; 
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
if (newTile == -1) return ExecuteStrengthening("DF_Colonization_NoLand".Translate());
Settlement newColony = (Settlement)WorldObjectMaker.MakeWorldObject(WorldObjectDefOf.Settlement);
newColony.SetFaction(parentFaction);
newColony.Tile = newTile;
newColony.Name = SettlementNameGenerator.GenerateSettlementName(newColony, null);
Find.WorldObjects.Add(newColony);
data.accumulatedEntropy += 2f;
data.stabilityPoints -= 90f; 
DF_EventManager.Fire(
    "DF_Colonization",
    "DF_Colonization_Label".Translate(parentFaction.Name),
    "DF_Colonization_Text".Translate(newColony.Name),
    LetterDefOf.NeutralEvent,
    newColony
);
return true;
            }
else if (roll < 0.70f) // 20% Крупная сделка
{
    data.influencePoints += 20f;
    data.leaderLegitimacy += 5f;
    data.leaderLegitimacy = Mathf.Clamp(data.leaderLegitimacy, -20f, 20f);
    data.accumulatedEntropy += 2f;
	data.stabilityPoints -= 90f; 
DF_EventManager.Fire(
    "DF_MajorDeal",
    "DF_MajorDeal_Label".Translate(parentFaction.Name),
    "DF_MajorDeal_Text".Translate(data.influencePoints, data.leaderLegitimacy),
    LetterDefOf.NeutralEvent,
    originSettlement
);
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
                if (checkTile == -1) return ExecuteStrengthening("DF_ScientificBreakthrough_NoAcademy".Translate());
                
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
                
label = "DF_ScientificBreakthrough_Label".Translate(parentFaction.Name.Named("FACTION"));
text = "DF_ScientificBreakthrough_Text".Translate(
    parentFaction.Name.Named("FACTION"), 
    parentTech.ToString().Named("OLDTECH"), 
    targetTech.ToString().Named("NEWTECH")
);
goodwillChange = 100;
spawnNewSettlement = true;
data.accumulatedEntropy += 2f;
data.stabilityPoints -= 90f; 
            }
            else // 10% Золотой век
            {
    data.leaderLegitimacy += 10f;
    data.leaderLegitimacy = Mathf.Clamp(data.leaderLegitimacy, -20f, 20f);
    data.accumulatedEntropy += 2f;
	data.stabilityPoints -= 90f; 
DF_EventManager.Fire(
    "DF_GoldenAge",
    "DF_GoldenAge_Label".Translate(parentFaction.Name),
    "DF_GoldenAge_Text".Translate(data.leaderLegitimacy),
    LetterDefOf.NeutralEvent,
    originSettlement
);
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
label = "DF_LeaderHero_Label".Translate(parentFaction.Name);
text = "DF_LeaderHero_Text".Translate(parentFaction.Name, parentFaction.leader.Name.ToStringFull, data.leaderLegitimacy);
data.accumulatedEntropy -= 2f;
data.stabilityPoints += 90f; 
DF_EventManager.Fire(
    "DF_LeaderHero",
    label,
    text,
    LetterDefOf.NeutralEvent,
    originForLetter
);
return true;

        }
Settlement newColony = (Settlement)WorldObjectMaker.MakeWorldObject(WorldObjectDefOf.Settlement);
newColony.SetFaction(parentFaction);
newColony.Tile = newTile;
newColony.Name = SettlementNameGenerator.GenerateSettlementName(newColony, null);
Find.WorldObjects.Add(newColony);
label = "DF_LeaderUnites_Label".Translate(parentFaction.Name);
text = "DF_LeaderUnites_Text".Translate(parentFaction.Name, parentFaction.leader.Name.ToStringFull, newColony.Name, origin.Name, data.leaderLegitimacy);
data.accumulatedEntropy -= 2f;
data.stabilityPoints += 90f; 
DF_EventManager.Fire(
    "DF_LeaderUnites",
    label,
    text,
    LetterDefOf.NeutralEvent,
    newColony
);
return true;

    }
    roll = (roll - leaderSaveChance) / (1f - leaderSaveChance);
if (roll < 0.20f) // Гражданская война 20%
{
    FactionStabilityData data = GetOrCreateData(parentFaction);
    targetTech = parentTech;
    spawnNewSettlement = true;
    settlementsToSeize = 0;
    label = "DF_CivilWar_Label".Translate(parentFaction.Name);
    text = "DF_CivilWar_Text".Translate(parentFaction.Name);
    letterDef = LetterDefOf.NeutralEvent;
    goodwillChange = -100;
	data.stabilityPoints += 90f; 
    data.accumulatedEntropy -= 2f;
}

else if (roll < 0.40f) // Военный переворот 20%
{
    FactionStabilityData data = GetOrCreateData(parentFaction);
    targetTech = parentTech;
    spawnNewSettlement = false;
    settlementsToSeize = Rand.Range(1, 3);
    label = "DF_MilitaryCoup_Label".Translate(parentFaction.Name);
    text = "DF_MilitaryCoup_Text".Translate(parentFaction.Name);
    letterDef = LetterDefOf.NeutralEvent;
    goodwillChange = -100;
	data.stabilityPoints += 90f; 
    data.accumulatedEntropy -= 2f;
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
        
label = "DF_SocietyCollapse_Label".Translate(parentFaction.Name);
text = "DF_SocietyCollapse_Text".Translate(parentFaction.Name, targetTech.ToString());
letterDef = LetterDefOf.NeutralEvent;
goodwillChange = -50;
data.stabilityPoints += 90f; 
data.accumulatedEntropy -= 2f;
    }
    else if (roll < 0.65f) // Эпидемия 10%
    {
    FactionStabilityData data = GetOrCreateData(parentFaction);
    data.attackPoints += 15f;
    data.defensePoints -= 15f;
    data.influencePoints -= 15f;
    data.attackPoints = Mathf.Clamp(data.attackPoints, 0f, 200f);
    data.defensePoints = Mathf.Clamp(data.defensePoints, 0f, 200f);
    data.influencePoints = Mathf.Clamp(data.influencePoints, -100f, 100f);
    label = "DF_Epidemic_Label".Translate(parentFaction.Name);
    text = "DF_Epidemic_Text".Translate(parentFaction.Name, data.attackPoints, data.defensePoints, data.influencePoints);
	data.stabilityPoints += 90f; 
    data.accumulatedEntropy -= 2f;
DF_EventManager.Fire(
    "DF_Epidemic",
    label,
    text,
    LetterDefOf.NeutralEvent,
    originSettlement
);
    return false;
    }
    else // Изоляция 15%
    {
    FactionStabilityData data = GetOrCreateData(parentFaction);
    data.attackPoints += 20f;
    data.defensePoints += 20f;
    data.influencePoints -= 20f;
	data.stabilityPoints += 90f; 
    data.attackPoints = Mathf.Clamp(data.attackPoints, 0f, 200f);
    data.defensePoints = Mathf.Clamp(data.defensePoints, 0f, 200f);
    data.influencePoints = Mathf.Clamp(data.influencePoints, -100f, 100f);
    label = "DF_Isolation_Label".Translate(parentFaction.Name);
    text = "DF_Isolation_Text".Translate(parentFaction.Name, data.attackPoints, data.defensePoints, data.influencePoints);
    data.accumulatedEntropy -= 2f;
DF_EventManager.Fire(
    "DF_Isolation",
    label,
    text,
    LetterDefOf.NeutralEvent,
    originSettlement
);
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
        newSettlement.Name = capturedName;
        Find.WorldObjects.Add(newSettlement);
        
        firstTile = capturedTile;
    }
    letterTarget = Find.WorldObjects.SettlementBaseAt(firstTile) ?? originSettlement;
    text += "DF_SettlementCapture_Additional".Translate(targets.Count);
    }
    
    newFaction.TryAffectGoodwillWith(parentFaction, goodwillChange);
    newFaction.TryAffectGoodwillWith(Faction.OfPlayer, 0);
DF_EventManager.Fire(
    "DF_SplitFaction",
    label,
    text,
    letterDef,
    letterTarget
);
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
    : "DF_SpawnLetterTitle_Default".Translate();

string description = SpawnDescriptions.TryGetValue(randomDef.techLevel, out string desc) 
    ? "DF_SpawnDescription_WithName".Translate(desc, newSettlement.Name)
    : "DF_SpawnDescription_Default".Translate(newSettlement.Name);

// Добавим немного разнообразия в конец описания
string[] flavorTexts = {
    "DF_SpawnFlavor_1".Translate(),
    "DF_SpawnFlavor_2".Translate(),
    "DF_SpawnFlavor_3".Translate(),
    "DF_SpawnFlavor_4".Translate(),
    "DF_SpawnFlavor_5".Translate(),
    "DF_SpawnFlavor_6".Translate(),
    "DF_SpawnFlavor_7".Translate()
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

DF_EventManager.Fire(
    "DF_RandomSettlement",
    title,
    description,
    letterType,
    newSettlement
);
    
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
        
private void CaptureBases(Faction winner, List<Settlement> targets, int count, string title, string text, string eventDefName = "DF_BaseCapturedGeneric")
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
        newBase.Name = name;
        Find.WorldObjects.Add(newBase);
    }
        DF_EventManager.Fire(
        eventDefName, // Используем переданный DefName
        title, 
        text, 
        LetterDefOf.NeutralEvent, 
        new GlobalTargetInfo(seized[0].Tile) // Цель камеры на захваченное
    );
}

        
private void DestroyBases(List<Settlement> targets, int count, string title, string text, string eventDefName = "DF_DestroyBases")
{
    count = Mathf.Min(count, targets.Count);
    var destroyed = targets.Take(count).ToList();
    LookTargets lookTarget = null;
    
    foreach (var s in destroyed)
    {
        int tile = s.Tile;
        string name = s.Name;
        Find.WorldObjects.Remove(s);
        
        DestroyedSettlement ruins = (DestroyedSettlement)WorldObjectMaker.MakeWorldObject(WorldObjectDefOf.DestroyedSettlement);
        ruins.Tile = tile;
        Find.WorldObjects.Add(ruins);
        
        if (lookTarget == null)
        {
            lookTarget = ruins;
        }
    }
    
    // Через менеджер
    DF_EventManager.Fire(
        eventDefName,
        title,
        text,
        LetterDefOf.NeutralEvent,
        lookTarget
    );
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
DF_EventManager.Fire(
    "DF_EndOfPandemic",
    "DF_EndOfPandemic_Label".Translate(),
    "DF_EndOfPandemic_Text".Translate(),
    LetterDefOf.PositiveEvent,
    null
);
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
DF_EventManager.Fire(
    "DF_CityDied",
    "DF_CityDied_Label".Translate(),
    "DF_CityDied_Text".Translate(victimName, faction.Name),
    LetterDefOf.NeutralEvent,
    new GlobalTargetInfo(victim.Tile)
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

        LookTargets look = null;
        if (parms is IncidentParms_DF dfParms)
        {
            look = dfParms.specificLookTargets;
        }

        // ====================================================
        // НОВАЯ ЛОГИКА: ФИЛЬТР ПИСЕМ
        // ====================================================
        
        bool shouldSendLetter = true;

        // Проверяем настройку. Если включено "Требовать консоль":
        if (DynamicFactionsMod.settings.requireCommsForNews)
        {
            // Угрозы (красные письма) показываем ВСЕГДА (ядерный гриб виден и без радио)
            // Если ты хочешь скрывать и угрозы, убери проверку "type != LetterDefOf.ThreatBig"
            if (type != LetterDefOf.ThreatBig && type != LetterDefOf.ThreatSmall)
            {
                // Если связи НЕТ -> письмо НЕ отправляем
                if (!DF_CommsUtility.PlayerHasCommunications())
                {
                    shouldSendLetter = false;
                }
            }
        }

        // 3. Отправляем письмо ТОЛЬКО если прошли проверку
        if (shouldSendLetter)
        {
            Find.LetterStack.ReceiveLetter(label, text, type, look);
        }
        else
        {
            // (Опционально) Можно писать в дебаг лог, что событие случилось скрытно
            if (DynamicFactionsMod.settings.showDebugLogs)
            {
                Log.Message($"[DF] Событие '{label}' произошло, но письмо скрыто (нет связи).");
            }
        }
        
        return true; // Событие всегда считается успешным, механики сработали
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

public static class DF_CommsUtility
{
    // Кешируем результат, чтобы не проверять каждый миллисекунду (опционально, но полезно)
    private static bool cachedResult = false;
    private static int lastCheckTick = -1;

    public static bool PlayerHasCommunications()
    {
        // Проверяем раз в 60 тиков (1 сек), чтобы не грузить процессор
        if (Find.TickManager.TicksGame - lastCheckTick < 60 && lastCheckTick != -1)
        {
            return cachedResult;
        }

        lastCheckTick = Find.TickManager.TicksGame;
        cachedResult = CheckMapsForComms();
        return cachedResult;
    }

    private static bool CheckMapsForComms()
    {
        List<Map> maps = Find.Maps;
        for (int i = 0; i < maps.Count; i++)
        {
            if (!maps[i].IsPlayerHome) continue;

            // 1. Ванильная консоль связи (CommsConsole)
            foreach (Building building in maps[i].listerBuildings.AllBuildingsColonistOfDef(ThingDef.Named("CommsConsole")))
            {
                var power = building.GetComp<CompPowerTrader>();
                if (power == null || power.PowerOn) return true;
            }

            // 2. Поддержка модов (по именам дефов, как в No Quests Without Comms)
            // Племенной костер (Tribal Signal Fire)
            if (DefDatabase<ThingDef>.GetNamed("SignalFire", false) != null)
            {
                foreach (Building b in maps[i].listerBuildings.AllBuildingsColonistOfDef(ThingDef.Named("SignalFire")))
                {
                    var glower = b.TryGetComp<CompGlower>(); // Костер работает, если светится
                    if (glower != null && glower.Glows) return true;
                }
            }

            // Стол птичьей почты (Medieval Overhaul / Nopower Comms)
            string[] medievalTables = { "BirdPostMessageTable", "DankPyon_ScribeTable", "Estate_Radio" };
            foreach (string defName in medievalTables)
            {
                ThingDef def = DefDatabase<ThingDef>.GetNamed(defName, false);
                if (def != null)
                {
                    foreach (Building b in maps[i].listerBuildings.AllBuildingsColonistOfDef(def))
                    {
                        var power = b.GetComp<CompPowerTrader>();
                        if (power == null || power.PowerOn) return true;
                    }
                }
            }
        }
        return false;
    }
}




}