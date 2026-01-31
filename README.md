I had an initial idea that I was missing some activity outside my own colony. I wanted factions to actively change and trigger events based on some logic. At the same time, whether you play for 1 year or 10 years, the set of factions doesn't change, and the number of religions in the world doesn't really grow. Of course, mods like Dynamic Diplomacy or RimWar add some activity, but the first is too random, and the second is too straightforward and labor-intensive, without any interesting political events. So, I came up with this system.

# 100 New Factions

At the moment, my mod adds 100 factions to a hidden pool: they are not available for selection at the start of the game, but they can appear during events. Let me say right away: these are fully-fledged factions. They have their own icons, faction and settlement names (besides the icon, each faction can randomly choose one of 25 colors, so that's 2500 faction variants, lol), their own leader, separate religion, relations, and so on. The only thing is that they are made on the principle of inheritance, i.e., they inherit the traits and characteristics of vanilla factions of the corresponding TechLevel.

If patched, inheritance can be made from anyone. Currently, I added a patch for medieval factions, so far only for **Vanilla Factions Expanded - Medieval 2**. It is not mandatory, but with it, medieval factions will appear in the image and likeness of VFE factions. Without it, you will get strange medieval factions because the vanilla game doesn't have pure medieval mechanics; there are tribals and "others." The difference there is only in the technology level, so without *Vanilla Factions Expanded - Medieval 2*, you will simply have a medieval era half-mixed with firearms, but everything will work fine technically.

# Brief Description (if you are too lazy to read)

*   **New Factions:** Can appear as a result of civil wars, intrigues, overthrow of power, revolutions, scientific breakthroughs (the faction will advance to a higher TechLevel), or social degradation (the new faction's TechLevel will drop). Also, in the settings, you can enable random spawning on the map.
*   **Deep Mechanics:** Each faction tracks Stability, Influence, Leader Legitimacy, Attack, Defense, and Entropy. These parameters determine its actions and events in the world.
*   **Active Diplomacy & Wars:** Factions negotiate, form alliances, attack each other, capture settlements, convert others to their religion, intervene in politics, assassinate leaders, and lose settlements based on complex logic, not randomly (well, almost).
*   **Global Events:** Nuclear war can destroy up to 8 settlements of every faction and cause fallout on the player's map (by the way, factions below the Industrial level cannot start a nuclear war). The Black Plague pandemic will shake Neolithic and Medieval factions harder, while Industrial and higher factions will cope much better; the player can also get infected.
*   **Settings:** I tried to put important things in the settings, such as point multipliers, limiting the TechLevel to Medieval (so Industrial and higher factions don't appear), disabling messages until you have a Comms Console (events will still happen), mod update frequency, chance of World Crisis, Nuclear War, Black Plague, and more. You can disable Attack and Defense systems if you play with other combat mods.
*   **Compatibility:** Potential compatibility with anything, as I didn't change game files, but only built a layer on top. Conversely, many mods and DLCs can nicely complement this: for example, Ideology mods will add more diverse religions for new factions.

# Performance
The mod is completely passive in the background; activity occurs only at the moment of the mod update/tick when calculations happen. But the calculations are quite simple: I compared it with the Dynamic Diplomacy mod, which many consider lightweight, and my mod works noticeably faster. Of course, if you max out the settings and spawn 100 factions in the game, it might lag, but that doesn't apply to the mod itself. Additionally, the mod regularly clears the cache, removing dead factions and other junk. The factions themselves stored in the hidden pool can increase the save size by a couple of megabytes at most, so performance is excellent.

# Save Game Compatibility
You can install it on an existing save, but note an unpleasant moment: if you have VE Framework installed, it will offer to place all hidden factions in the world, and you will have to click "deny" 100 times. But after that, everything should work fine. I might be able to fix this in the future.

**Can I remove it from a save?**
No, you cannot. This will break your save, so make a backup or do not remove mods in the middle of a game.

# Compatibility with Other Mods
As I wrote above, the mod adds its own logic without interfering with vanilla mechanics, so in theory, it should be compatible with everything, even with mods like Dynamic Diplomacy, RimWar, and other political or faction war mods. However, my mod will simply act within its own system, so functions might duplicate. For this, I made it possible to disable certain aspects: if you wish, you can disable all complex mechanics and leave only, for example, random settlement spawning. Or, if playing with RimWar, remove the attack and armor multipliers in my mod settings — stability and influence events will still work.

# Current Issues
*   **Balance is raw.** I realized that polishing the balance alone will be extremely difficult and long, so I would be grateful for feedback and ideas from your side; I plan to improve this aspect in the future. Moreover, many systems had to be scrapped (initially, radius/proximity to enemies and allies influenced points), but that was tough, and I cut that system out, especially since people play on maps of different sizes.
*   **Unpleasant moment with Xenotypes.** The faction inherits xenotypes not from the parent faction, but from the one specified in the mod file or the patched faction. Therefore, illogical situations arise where, for example, 100% Baseliners separate from 100% Impids. For now, you can use Xenotype Spawn Control, as I don't quite understand how to fix this yet. But overall, it doesn't bother me much.
*   **Faction Icons.** Obviously, it's a matter of taste. Most of them were generated via neural networks. I hope this doesn't offend anyone, but without AI, this mod would never have seen the light of day, considering there are 100 of them. They were made with love but quickly, so not everything turned out beautiful. If you have the desire, you can send me your icon variants, and I could add them to the mod.

***
I realized that I set myself an overly ambitious task, especially for a person who is not a programmer. For almost a month, I tried to make what was intended without a break, but this mod started driving me crazy because events and mechanics grew like a snowball, and all this needs to be optimized and balanced somehow. If you like my idea, and I'm not just a madman who decided to mess up some mechanics, then I will continue to update the mod — after a break, perhaps.

Here you can contact me or support me, or not contact and not support:
Discord: [https://discord.gg/zDhKjAz8](https://discord.gg/zDhKjAz8)
Boosty: [https://boosty.to/helldan](https://boosty.to/helldan)

***
***

### BLOCK 2: FOR DISCUSSIONS

# Detailed Mechanics Description

How do Stability and Influence events work? They accumulate and are lost both passively and actively for every faction in the game. Active actions are performed by a random faction every mod update (cycle), while passive points are awarded to all factions every mod update.

With Stability at -50 and below, each subsequent negative stability point will give a chance for a negative event, and each positive point above +50 will give a chance for a positive event. The chance equals the percentage depending on the points above the limit (for example, at Stability 70, the chance for a positive event per update will be 20%; at Stability -83, the chance for a negative event will be 33%). To avoid repetition: the logic for Influence events is similar, only calculated from Influence respectively.

Also, for every positive event, the faction receives +2 Entropy points (we'll talk about this later) and -90 points to the target characteristic. For negative events, the faction loses 2 Entropy points and gains +90 points to the target characteristic.

### Positive Influence Events

1.  **Annexation:** 10% chance. Seizure of one random settlement from the faction weakest in Influence. Target Influence: -30.
2.  **New Friendship:** 10% chance. Establishing an alliance with a random faction. +100 relations with the selected faction.
3.  **Defense Investment:** 10% chance. The faction strengthens its military potential. Attack +20, Defense +20.
4.  **Era of Prosperity:** 20% chance. Internal development and economic growth. Stability +20, Leader Legitimacy +5.
5.  **Propaganda:** 25% chance. Imposing one's ideology on another faction with the lowest Stability. The target faction accepts your ideology. Damage to target: Influence -50, Target Defense +50.
6.  **Color Revolution:** 15% chance. Supporting rebels in the territory of the weakest faction, causing a new faction to spawn near the target settlement or replacing the target settlement.
7.  **Leader Death at Negotiations:** 10% chance. The leader was set up and killed during negotiations. The army is furious: Attack +50, Defense -50. (Yes, this is a negative event with a positive outcome, again for balance).

### Negative Influence Events

1.  **Land Sale:** 10% chance. Transfer of one settlement to the most influential faction, in exchange for Attack +30 and Defense +30.
2.  **Puppet Government:** 15% chance. Your faction is subjugated by a more influential one. Stability +50, Leader replaced with a puppet, +100 relations with the host faction, adopt their ideology, break relations with all the host's enemies.
3.  **Heresy:** 15% chance. Religious schism, adoption of the ideology of the most influential faction. Effect: Fanaticism (Defense +30).
4.  **Economic Collapse:** 10% chance. Economic crisis. Stability -20, Leader Legitimacy -5.
5.  **Severing Relations:** 10% chance. Loss of an ally, -100 relations with a random ally.
6.  **Dictatorship:** 20% chance. Establishment of an authoritarian regime. Stability -30, Legitimacy -5, Attack +30, Defense +30, Loss of 1-2 allies (-100 relations).
7.  **Allied Aid:** 10% chance. Allies help exit the crisis. Stability +10, Attack +10, Defense +10 (positive event).
8.  **Gift of Lands:** 10% chance. The most influential faction, at the cost of losing 50 Influence, gifts land (positive event).

### Positive Stability Events

1.  **Colonization:** 50% chance. Founding a new settlement on a free tile within range, expanding faction territory.
2.  **Major Deal:** 20% chance. Conclusion of a profitable trade contract on the faction's terms. Influence +20, Leader Legitimacy +5.
3.  **Fortification:** 15% chance. Mobilization of internal resources to strengthen the army. Attack +15, Defense +15, Leader Legitimacy +5.
4.  **Golden Age:** 10% chance. Period of cultural flourishing and popular support. Leader Legitimacy +10.
5.  **Scientific Breakthrough:** 5% chance. Technological leap increasing TechLevel (TechLevel +1). Spawn of an advanced settlement, Relations +100.
6.  **Intrigues:** Chance 0.5% per settlement. Internal schism leads to the separation of 1-4 settlements into a new group. Relations with the new faction -200 (negative outcome on a positive check).

### Negative Stability Events

1.  **Civil War:** 20% chance. Faction split, separatists found their own state and declare war. Relations -100.
2.  **Military Coup:** 20% chance. Seizure of power by a junta, loss of 1-3 settlements to a new aggressive faction. Relations -100.
3.  **Societal Collapse:** 15% chance. Social degradation and loss of technology (TechLevel -1). Relations with everyone -50.
4.  **Isolation:** 15% chance. Closing borders to stabilize the situation inside the country. Attack +20, Defense +20, Influence -20.
5.  **Epidemic:** 10% chance. Disease outbreak mobilizes forces but weakens defense and economy. Attack +15, Defense -15, Influence -15.
6.  **Hero Leader:** Chance 5-20%. The leader prevents the crisis through personal intervention. Leader Legitimacy +5, Crisis cancelled.
7.  **Leader Negotiated:** Chance 5-20%. The leader steers the conflict into a peaceful channel. Peaceful separation of a new settlement (ally), Leader Legitimacy +5 (the fewer settlements a faction has, the higher the chance of this event).

# Attack and Defense

Regarding attack and defense: I will write the specific formulas and point scoring below. Here I will only indicate that the logic is similar to Influence/Stability. When Attack rises above 50, the attack percentage grows proportionally with each subsequent point. The target is selected based on the following criteria:

**Top Priority:** If DLC Ideology is active, the algorithm analyzes world politics.
*   *Condition:* If there are **more than 5** factions in the world with the same foreign ideology.
*   *Action:* The nearest hostile faction of this faith is attacked.
*   *Condition:* If there are more than 3 but less than 6 factions with a foreign faith. Triggers with a 50% chance.
*   *Action:* The nearest of such enemies is attacked.

**Second Priority: "Strategy and Technology" (Main)**
If there are no ideological reasons (or the DLC is disabled), normal war logic activates.
*   *Technological Rivalry (80% Chance):* Algorithm searches for enemies whose TechLevel >= yours. That is, equal or stronger.
*   *Choice:* Attacks the nearest of the strong enemies.
If there are no strong enemies, it switches to "Territorial Dispute" logic (attack any nearest enemy).
*   *Territorial Dispute (20% Chance):* Algorithm ignores technology. Simply attacks the very nearest enemy.

**Third Priority: "New Conflict" (If no enemies)**
If no target from points 1 and 2 was selected (e.g., no enemies nearby), there is a chance to start a new war.
*   *Chance:* 10%.
*   *Target:* Factions that are **NOT** enemies but have settlements within a 50-tile radius of your bases.
*   *Choice:* Attacks the nearest neighbor.
*   *Reason:* Chosen randomly (Diplomatic scandal / Border conflict / Religious friction / Economy).

**Final Check: "Peace Treaty"**
After a target is selected, a safety mechanism triggers.
*   *Check:* If peace was concluded with this target.
*   *Result:* Attack is cancelled. Attack points are penalized by 20% (for indecision), event ends without action.

### Diplomatic Outcome (Pre-battle)

1.  **Peace Agreement:** Chance depends on the difference in power and leadership. Negotiations successful, war cancelled for 10-100 days.
    *   *Effects:* Attack (aggressor) reduced by 50%, Stability +30; Target gains Influence +10 and Stability +10.

### Attack Success Formula

`Attack Power = (Accumulated Attack Points + Defense Points divided by 2) * Attacker TechLevel Multiplier + Leader Legitimacy`

`Defense Power = (Accumulated Defense Points + Attack Points divided by 2) * Defender TechLevel Multiplier + Enemy Leader Legitimacy`

**TechLevel Multipliers:**
*   Neolithic: x0.6 (40% penalty)
*   Medieval: x0.8 (20% penalty)
*   Industrial: x1.0 (base)
*   Spacer: x1.2 (20% bonus)
*   Ultra: x1.4 (40% bonus)

`Win Chance = (Attack Power - Defense Power)`
The resulting number will be the percentage of success.
*For example:* Let's take a Medieval faction with Attack 150 and Defense 50 with Leader Legitimacy 10, against a Spacer faction with Attack 60, Defense 100, and Leader Legitimacy -5.
*   Medieval: `((150 + (50/2)) * 0.8) + 10 = 150`
*   Spacer: `((60 + (100/2)) * 1.2) - 5 = 127`
*   150 - 127 = 23.
*   Total is a 23% chance of a successful attack by the Medieval faction.

### Low-Tech Conflicts (Medieval and below)

1.  **Invasion:** 40% chance. Successful capture of 1-3 enemy settlements. 15% chance to kill the enemy leader.
2.  **Ideological Capitulation:** 20% chance. Imposing one's ideology on the enemy without destruction. Target adopts ideology, Target Influence -50.
3.  **Barbaric Slaughter:** 20% chance. Complete destruction of one settlement (turning into ruins). Target Stability -30.
4.  **War Comes Home:** 10% chance. Base exchange: defender captures enemy settlement, but enemy captures the nearest settlement (or just a raid if no bases exist).
5.  **Disputed Victory:** 10% chance. Hard victory with capture of 1 settlement, but Attacking Leader dies. Influence -30.

### High-Tech Conflicts (Industrial and above)

1.  **Nuclear War:** Chance configurable in options (default 5%). Global catastrophe. Destruction of 2-8 bases of every faction in the world, Toxic Fallout on player map, Attack and Defense of all factions reduced by 80%, Stability -50.
2.  **Surprise Attack:** 35% chance. Lightning war, capture of 1-3 settlements. 15% chance to kill the enemy leader.
3.  **Missile Barrage:** 20% chance. Massive bombardment, destruction of 1-3 settlements (ruins). 15% chance to kill the enemy leader, Target Stability -30.
4.  **Surgical Strike:** 20% chance. Surgical destruction of 1 key settlement. 15% chance to kill the enemy leader.
5.  **Information Victory:** 15% chance. Propaganda and network hacking. Target accepts your ideology, Target Influence -50.
6.  **Disputed Victory:** 10% chance. Leader dies during assault, 1 settlement captured. Influence -30.

### Defense Events (Aggressor Attack Failure)

1.  **Defense Successful:** 50% chance. Standard repulsion of attack with moderate fortification losses.
    *   Aggressor: Attack -20, Influence -15, Stability -5.
    *   Defender: Defense -20, Influence +10, Stability +5.
2.  **Heavy Defense:** 20% chance. Enemy stopped, but defender's infrastructure suffered heavily.
    *   Aggressor: Attack -20, Influence -20, Stability -10.
    *   Defender: Defense -40 (significant damage), Influence +15, Stability +10.
3.  **Attack Repelled:** 20% chance. Brilliant tactics, enemy thrown back with no losses to defender's fortifications.
    *   Aggressor: Attack -20, Influence -25, Stability -15.
    *   Defender: Defense not spent, Influence +20, Stability +15.
4.  **Counter-offensive:** 10% chance. Defender not only beats back the attack but captures a base on the heels of the retreating enemy.
    *   Aggressor: Loss of 1 settlement (nearest to front), Attack -20, Defense -20, Influence -30, Stability -20.
    *   Defender: Gains new settlement, Influence +25, Stability +20.

# Global Events

1.  **Black Plague Pandemic:**
    *   Duration: 30-60 game days (random).
    *   Trigger chance: configurable in settings (default 0.5%).
    *   Effects applied once per day (at the start of each new day).
    *   *Chance of settlement destruction depends on TechLevel:*
        *   Neolithic: 10%
        *   Medieval: 8%
        *   Industrial: 5%
        *   Spacer: 3%
        *   Ultra: 1%
    *   *Impact on player:* 5% chance of colony infection per day during the Black Plague. After 30-60 days, message "Pandemic has ended" appears.

2.  **World Crisis:**
    *   Randomly starts with a 0.5% chance each cycle (changeable in mod settings).
    *   Selects several random factions (10-100% of all).
    *   Forces them to become enemies with each other.
    *   Chaos and redistribution of spheres of influence begins in the world.

3.  **Ideological Schism:**
    *   Large religious/ideological group disintegrates.
    *   When 6+ factions follow one ideology. Chance grows with each additional follower.
    *   One third of these factions change faith.
    *   Some create a new ideology, others switch to existing competitors.

# Passive Point Accumulation

Passive points are awarded to all factions every mod update. These include:

**Entropy:**
Default 2 (configurable in options). For every positive event, 2 entropy is given; for every negative event, 2 entropy is taken away. Entropy is subtracted from Stability and Influence every update. For example, at 5 Entropy, after a mod update, 5 will be subtracted from 40 Stability and 30 Influence, resulting in 35 Stability and 25 Influence. This is done to restrain super-successful factions and add a natural variable of chaos (complicating systems if they are doing too well).

**Leader Legitimacy:**
*   *Constant:* At the start, each faction randomly selects a leader with legitimacy from -5 to +5. The leader gains +0.1 legitimacy every mod update.
*   *One-time:* +5 legitimacy for every captured settlement, -5 legitimacy for every lost one.

**Stability:**
*   *Constant:*
    *   +0.5 for every faction ally.
    *   -0.5 for every faction enemy.
    *   Leader Legitimacy points divided by 10 (e.g., at 7 legitimacy, passive stability bonus is 7/5=0.7).
    *   -Entropy.
*   *One-time:*
    *   On settlement capture: +15 Stability.
    *   On settlement loss: -30 Stability.
    *   On leader death: if they had, say, +7 legitimacy, faction loses 70 Stability, but if leader had -10, faction gains 100 Stability.

**Influence:**
*   *Constant:*
    *   +0.5 for every settlement.
    *   -1 for every hostile faction.
    *   +0.5 for every ally.
    *   +1 for every similar ideology in the world.
    *   Leader Legitimacy points divided by 10.
    *   -Entropy.
*   *One-time:*
    *   On settlement capture: +30 Influence.
    *   On settlement loss: -15 Influence.

**Attack and Defense:**
In the case of attack and defense, passive points are awarded every turn, but randomly: either focus on Attack or focus on Defense. I.e., if Attack focus rolls, the faction gets only Attack points this turn; if Defense, then only Defense points.

*   **Focus on Attack:**
    *   Leader Legitimacy divided by 10.
    *   Influence divided by 100.
    *   +0.5 for every faction enemy.

*   **Focus on Defense:**
    *   Leader Legitimacy divided by 10.
    *   Stability divided by 100.
    *   +0.5 for every allied faction.

### Point Limits
*   Influence: (-100...+100) — diplomatic weight.
*   Stability: (-100...+100) — general stability.
*   Leader Legitimacy: (-20...+20).
*   Attack: (0..200) — military aggression.
*   Defense: (0..200) — defensive capability.
