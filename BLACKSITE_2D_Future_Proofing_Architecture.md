# BLACKSITE 2D - Future-Proofing & Architecture Guide
**Version 1.0 - Created: January 26, 2026**

---

## CRITICAL: READ THIS FIRST

This document outlines architectural decisions and design patterns that must be followed NOW during MVP development to avoid costly rewrites later when implementing:
- Multiplayer co-op
- Online accounts & cloud saves
- Stats tracking & API
- Anti-cheat measures
- Disconnect/crash protection

**Every system we build should be designed with these future features in mind.**

---

## TABLE OF CONTENTS

1. [Currency System Architecture](#currency-system-architecture)
2. [Multiplayer Considerations](#multiplayer-considerations)
3. [Online Accounts & Cloud Saves](#online-accounts-cloud-saves)
4. [Stats Tracking & API](#stats-tracking-api)
5. [Anti-Cheat Design](#anti-cheat-design)
6. [Data Persistence Strategy](#data-persistence-strategy)
7. [Implementation Checklist](#implementation-checklist)

---

## CURRENCY SYSTEM ARCHITECTURE

### Confirmed Requirements

**Currency Earning Rules:**
- **10 points** per bullet hit (any weapon)
- **100 points** for kill completion (enemy death)
- **115 points** for melee kill (future feature)
- Total per normal kill: **10 × 3 hits + 100 = 130 points**
- Total per melee kill: **115 points**

### Architecture Pattern: Centralized Currency Manager

**DO THIS (Future-Proof):**
```csharp
public class CurrencyManager : MonoBehaviour
{
    // Singleton pattern for easy access
    public static CurrencyManager Instance { get; private set; }
    
    // Events for currency changes (critical for multiplayer sync)
    public event System.Action<int, int> OnCurrencyChanged; // (oldValue, newValue)
    public event System.Action<int, string> OnCurrencyEarned; // (amount, reason)
    
    private int currentCurrency = 0;
    private int sessionEarnings = 0; // Track per-run earnings
    
    // Currency transaction log (for anti-cheat and stats)
    private List<CurrencyTransaction> transactionLog = new List<CurrencyTransaction>();
    
    public void AddCurrency(int amount, string reason)
    {
        // Validate transaction (server-side in multiplayer)
        LogTransaction(amount, reason);
        
        int oldValue = currentCurrency;
        currentCurrency += amount;
        sessionEarnings += amount;
        
        OnCurrencyChanged?.Invoke(oldValue, currentCurrency);
        OnCurrencyEarned?.Invoke(amount, reason);
    }
    
    // More methods...
}

[System.Serializable]
public class CurrencyTransaction
{
    public int amount;
    public string reason;
    public float timestamp;
    public string sessionId; // For multiplayer tracking
}
```

**WHY:**
- ✅ Centralized = easy to add server validation later
- ✅ Event system = UI updates automatically (works in multiplayer)
- ✅ Transaction log = anti-cheat detection (unusual earning patterns)
- ✅ Session tracking = statistics for API
- ✅ Singleton = accessible from anywhere (player, enemies, weapons)

**DON'T DO THIS (Not Future-Proof):**
```csharp
// ❌ BAD: Direct variable manipulation in WaveManager
private int totalCurrency = 0;
totalCurrency += 10; // No validation, no events, no logging
```

---

### Currency Earning Implementation

**Pattern to Follow:**

```csharp
// In Bullet.cs when hitting enemy
CurrencyManager.Instance.AddCurrency(10, "bullet_hit");

// In Enemy.cs when dying
CurrencyManager.Instance.AddCurrency(100, "kill_basic");

// Future: In MeleeWeapon.cs when killing
CurrencyManager.Instance.AddCurrency(115, "kill_melee");
```

**Benefits:**
- Reason string = analytics ("what earns most currency?")
- Centralized = server can validate in multiplayer
- Event-driven = stats tracking, UI updates, achievements all hooked in
- Easy to adjust values (change 10 → 15 in one place)

---

## MULTIPLAYER CONSIDERATIONS

### Core Principle: Authoritative Server Model

**For Anti-Cheat in Multiplayer:**
- Client predicts actions (shooting, moving) for responsiveness
- Server validates and confirms (currency earning, damage, kills)
- Client-side cheating becomes impossible

### Data Architecture for Multiplayer

**DO THIS NOW (Even in Single-Player):**

```csharp
public class PlayerData
{
    public string playerId;        // Unique ID (GUID or account ID)
    public string playerName;      // Display name
    public int totalCurrency;      // Persistent currency
    public PlayerStats stats;      // Lifetime stats
    public PlayerProgression progression; // Unlocks, upgrades
    
    // Session data (resets per run)
    public SessionData currentSession;
}

public class SessionData
{
    public string sessionId;       // Unique per run (for tracking)
    public int sessionCurrency;    // Earned this run
    public int sessionKills;
    public int waveReached;
    public float sessionTime;
    public bool extracted;         // Success or death
    
    // Multiplayer
    public List<string> partyMembers; // Player IDs in co-op
    public bool isHost;
}
```

**WHY:**
- PlayerData = account-level (saved to cloud)
- SessionData = run-level (sent to server at end for validation)
- Separation = easy to implement cloud saves later
- SessionID = track disconnects, prevent duplication exploits

---

### Disconnect/Crash Protection Strategy

**The Problem:**
Player disconnects → loses all session progress → frustrating

**The Solution: Periodic Checkpointing**

```csharp
public class SessionManager : MonoBehaviour
{
    private float checkpointInterval = 30f; // Save every 30 seconds
    private float lastCheckpoint = 0f;
    
    void Update()
    {
        if (Time.time - lastCheckpoint >= checkpointInterval)
        {
            CreateCheckpoint();
            lastCheckpoint = Time.time;
        }
    }
    
    void CreateCheckpoint()
    {
        SessionCheckpoint checkpoint = new SessionCheckpoint
        {
            sessionId = currentSession.sessionId,
            currency = CurrencyManager.Instance.GetSessionEarnings(),
            wave = WaveManager.Instance.currentWave,
            health = PlayerHealth.Instance.GetCurrentHealth(),
            timestamp = Time.time
        };
        
        // Save locally (MVP)
        SaveCheckpoint(checkpoint);
        
        // Future: Upload to server (multiplayer)
        // UploadCheckpointToServer(checkpoint);
    }
    
    void OnApplicationQuit()
    {
        // Detect clean exit vs crash
        PlayerPrefs.SetInt("CleanExit", 1);
    }
    
    void Start()
    {
        int cleanExit = PlayerPrefs.GetInt("CleanExit", 1);
        if (cleanExit == 0)
        {
            // Game crashed last time
            OfferRestoreCheckpoint();
        }
        PlayerPrefs.SetInt("CleanExit", 0); // Mark as running
    }
}
```

**Benefits:**
- Saves progress every 30 seconds
- Detects crashes vs intentional quits
- Offers restore on restart
- Works offline (MVP) and online (multiplayer)
- No exploitation (server validates checkpoints in multiplayer)

---

### Multiplayer Script Organization

**Folder Structure (Plan Now):**
```
Scripts/
├── Core/              (Single-player logic)
│   ├── PlayerController.cs
│   ├── Enemy.cs
│   ├── WaveManager.cs
│   └── CurrencyManager.cs
│
├── Managers/          (Shared systems)
│   ├── SessionManager.cs
│   ├── GameStateManager.cs
│   └── DataManager.cs
│
├── Multiplayer/       (Future - keep separate)
│   ├── NetworkManager.cs
│   ├── PlayerSync.cs
│   ├── ServerValidator.cs
│   └── LobbyManager.cs
│
└── Data/              (Data structures)
    ├── PlayerData.cs
    ├── SessionData.cs
    └── GameConfig.cs
```

**Benefits:**
- Core scripts never reference Multiplayer scripts (can ship without it)
- Multiplayer scripts CAN reference Core (wraps functionality)
- Easy to add/remove multiplayer package
- No spaghetti dependencies

---

## ONLINE ACCOUNTS & CLOUD SAVES

### Recommended Backend: PlayFab or Firebase

**Why PlayFab (Recommended):**
- ✅ Free tier (100k players)
- ✅ Built for games (leaderboards, stats, cloud saves, authentication)
- ✅ Unity SDK available
- ✅ Server-side validation built-in
- ✅ Analytics dashboard
- ✅ Cross-platform (PC, mobile, console)

**Why Firebase (Alternative):**
- ✅ Google-backed (reliable)
- ✅ Free tier generous
- ✅ Real-time database
- ✅ Good for web integration
- ❌ Less game-specific features

### Account System Architecture

**Data Flow:**
```
Player → Unity Client → Backend (PlayFab/Firebase) → Database
                 ↓
              Website API → Stats Display
```

**Implementation Pattern (Future):**

```csharp
public class AccountManager : MonoBehaviour
{
    public async Task<bool> Login(string email, string password)
    {
        // Call PlayFab/Firebase login
        var result = await PlayFabClientAPI.LoginWithEmailAddressAsync(
            new LoginWithEmailAddressRequest
            {
                Email = email,
                Password = password
            }
        );
        
        if (result.Error == null)
        {
            // Load player data from cloud
            LoadPlayerDataFromCloud(result.Result.PlayFabId);
            return true;
        }
        return false;
    }
    
    public async Task SavePlayerData()
    {
        // Upload PlayerData to cloud
        await PlayFabClientAPI.UpdateUserDataAsync(
            new UpdateUserDataRequest
            {
                Data = new Dictionary<string, string>
                {
                    { "currency", PlayerData.totalCurrency.ToString() },
                    { "stats", JsonUtility.ToJson(PlayerData.stats) },
                    { "progression", JsonUtility.ToJson(PlayerData.progression) }
                }
            }
        );
    }
}
```

### Migration Path: PlayerPrefs → Cloud Saves

**Phase 1 (MVP - Now):**
- Use PlayerPrefs for local saves
- Store: currency, unlocks, upgrades
- Works offline

**Phase 2 (Post-MVP):**
- Add PlayFab SDK
- Implement login/accounts
- On first login: migrate PlayerPrefs data to cloud
- Keep PlayerPrefs as backup/offline mode

**Code Pattern:**
```csharp
public class DataManager : MonoBehaviour
{
    public void SaveData()
    {
        // Always save locally (instant, works offline)
        SaveToPlayerPrefs();
        
        // If online, also save to cloud (async)
        if (AccountManager.IsLoggedIn)
        {
            SaveToCloud();
        }
    }
    
    public void LoadData()
    {
        if (AccountManager.IsLoggedIn)
        {
            // Load from cloud (authoritative)
            LoadFromCloud();
        }
        else
        {
            // Load from PlayerPrefs (offline mode)
            LoadFromPlayerPrefs();
        }
    }
}
```

**Benefits:**
- Works offline for MVP
- Easy migration path
- No data loss
- Can add accounts later without rewrite

---

## STATS TRACKING & API

### Stats to Track (Lifetime)

**Combat Stats:**
- Total kills
- Total deaths
- Total damage dealt
- Total damage taken
- Accuracy (shots fired / shots hit)
- Favorite weapon (most kills)

**Progression Stats:**
- Total currency earned (lifetime)
- Total currency spent
- Highest wave reached
- Successful extractions
- Failed extractions
- Total playtime

**Session Stats (Per Run):**
- Kills this run
- Wave reached this run
- Currency earned this run
- Accuracy this run
- Weapon used
- Extract success/failure

### Stats Architecture

```csharp
[System.Serializable]
public class PlayerStats
{
    // Combat
    public int totalKills;
    public int totalDeaths;
    public long totalDamageDealt;
    public long totalDamageTaken;
    public int totalShotsFired;
    public int totalShotsHit;
    
    // Progression
    public long lifetimeCurrencyEarned;
    public long lifetimeCurrencySpent;
    public int highestWave;
    public int successfulExtractions;
    public int failedExtractions;
    public float totalPlaytime;
    
    // Derived (calculated)
    public float Accuracy => totalShotsFired > 0 ? (float)totalShotsHit / totalShotsFired : 0f;
    public float KillDeathRatio => totalDeaths > 0 ? (float)totalKills / totalDeaths : totalKills;
    
    // Update methods
    public void RecordKill() { totalKills++; }
    public void RecordDeath() { totalDeaths++; }
    public void RecordShot(bool hit) 
    { 
        totalShotsFired++; 
        if (hit) totalShotsHit++; 
    }
}
```

### Website API Integration (Future)

**REST API Endpoints (on backend):**
```
GET /api/player/{playerId}/stats
    → Returns PlayerStats JSON

GET /api/leaderboards/highest-wave
    → Returns top 100 players by wave

GET /api/player/{playerId}/sessions
    → Returns last 10 sessions with details

GET /api/player/{playerId}/progression
    → Returns unlocks, upgrades, current build
```

**Website Implementation:**
- Simple React/Vue frontend
- Calls backend API
- Displays stats in nice UI
- Think: Destiny Tracker, Halo Waypoint, OP.GG

**Unity → Backend Data Flow:**
```csharp
public class StatsUploader : MonoBehaviour
{
    public async Task UploadSessionStats(SessionData session)
    {
        // Create JSON payload
        string json = JsonUtility.ToJson(session);
        
        // POST to backend
        using (UnityWebRequest request = UnityWebRequest.Post(
            "https://yourbackend.com/api/sessions",
            json,
            "application/json"
        ))
        {
            await request.SendWebRequest();
            
            if (request.result == UnityWebRequest.Result.Success)
            {
                Debug.Log("Session uploaded!");
            }
        }
    }
}
```

**Benefits:**
- All stats stored server-side (can't be cheated)
- Website can access via API
- Leaderboards automatically updated
- Players can compare stats

---

## ANTI-CHEAT DESIGN

### Threat Model

**What Players Could Cheat (Without Anti-Cheat):**
1. **Currency editing** - Modify PlayerPrefs to give infinite currency
2. **Stat manipulation** - Change kill count, wave reached
3. **Unlock hacking** - Unlock weapons/upgrades without earning
4. **Progress injection** - Fake session data

### Anti-Cheat Strategy: Server Authority

**Single-Player (MVP):**
- Basic obfuscation (encrypt PlayerPrefs)
- Checksum validation
- Unusual pattern detection

**Multiplayer (Future):**
- Server validates ALL transactions
- Client sends actions, server confirms results
- Server stores authoritative data
- Anti-cheat middleware (PlayFab has built-in)

### Implementation Pattern

```csharp
public class SecureDataManager : MonoBehaviour
{
    private const string ENCRYPTION_KEY = "YourSecretKey123"; // Change this!
    
    public void SaveSecure(string key, string value)
    {
        string encrypted = Encrypt(value, ENCRYPTION_KEY);
        string checksum = GenerateChecksum(encrypted);
        
        PlayerPrefs.SetString(key, encrypted);
        PlayerPrefs.SetString(key + "_checksum", checksum);
    }
    
    public string LoadSecure(string key)
    {
        string encrypted = PlayerPrefs.GetString(key);
        string storedChecksum = PlayerPrefs.GetString(key + "_checksum");
        string calculatedChecksum = GenerateChecksum(encrypted);
        
        if (storedChecksum != calculatedChecksum)
        {
            Debug.LogWarning("Data tampering detected!");
            return null; // Data was modified
        }
        
        return Decrypt(encrypted, ENCRYPTION_KEY);
    }
    
    private string GenerateChecksum(string data)
    {
        // Simple hash (use SHA256 in production)
        return data.GetHashCode().ToString();
    }
}
```

**For Multiplayer:**
```csharp
public class ServerValidator : MonoBehaviour
{
    public async Task<bool> ValidateKill(string sessionId, int enemyId)
    {
        // Client says they killed enemy
        // Server checks: did player deal enough damage?
        
        var response = await PostToServer("/validate/kill", new
        {
            sessionId = sessionId,
            enemyId = enemyId,
            timestamp = Time.time
        });
        
        return response.isValid;
    }
}
```

### Multiplayer-Specific Anti-Cheat

**Host-Client Model (Don't Use):**
- ❌ Host has authority → host can cheat
- ❌ Peer-to-peer → sync issues, cheating

**Dedicated Server Model (Use):**
- ✅ Server is authoritative (player can't modify)
- ✅ Server validates actions before confirming
- ✅ Cheating becomes difficult/impossible

**Use Unity Netcode for GameObjects or Mirror:**
- Both support dedicated server model
- Server-side validation built-in
- Good anti-cheat foundation

---

## DATA PERSISTENCE STRATEGY

### Three-Tier Persistence Model

**Tier 1: PlayerPrefs (MVP - Local)**
- **What:** Simple key-value storage
- **When:** MVP, offline mode
- **Pros:** Easy, instant, works offline
- **Cons:** Not secure, can't sync across devices, local only

**Tier 2: Cloud Save (Post-MVP - PlayFab/Firebase)**
- **What:** JSON data uploaded to backend
- **When:** After adding accounts
- **Pros:** Cross-device, secure, backed up
- **Cons:** Requires internet, slight delay

**Tier 3: Database (Future - MySQL/PostgreSQL)**
- **What:** Full relational database for stats/leaderboards
- **When:** When you need complex queries (leaderboards, matchmaking)
- **Pros:** Powerful querying, analytics, scalable
- **Cons:** More complex, needs dedicated server

### Migration Path

**Phase 1 (Now - MVP):**
```csharp
public class SaveSystem
{
    public void Save()
    {
        PlayerPrefs.SetInt("currency", currency);
        PlayerPrefs.SetString("unlocks", JsonUtility.ToJson(unlocks));
        PlayerPrefs.Save();
    }
    
    public void Load()
    {
        currency = PlayerPrefs.GetInt("currency", 0);
        string json = PlayerPrefs.GetString("unlocks", "{}");
        unlocks = JsonUtility.FromJson<Unlocks>(json);
    }
}
```

**Phase 2 (Post-MVP - Cloud):**
```csharp
public class SaveSystem
{
    public async Task Save()
    {
        // Save locally first (instant)
        SaveToPlayerPrefs();
        
        // Then upload to cloud (async)
        if (IsOnline)
        {
            await SaveToCloud();
        }
    }
    
    public async Task Load()
    {
        if (IsOnline)
        {
            // Load from cloud (authoritative)
            await LoadFromCloud();
        }
        else
        {
            // Fallback to local
            LoadFromPlayerPrefs();
        }
    }
}
```

**Phase 3 (Future - Full Backend):**
```csharp
// All saves go through backend API
// Backend writes to database
// Website reads from same database
```

---

## IMPLEMENTATION CHECKLIST

Use this checklist when implementing each system:

### Currency System
- [ ] Create CurrencyManager singleton
- [ ] Use events for currency changes (OnCurrencyChanged)
- [ ] Log transactions with reasons
- [ ] Track session earnings separately
- [ ] Store currency in PlayerData class (not loose variable)
- [ ] Save/load through DataManager (not directly)

### Stats Tracking
- [ ] Create PlayerStats class
- [ ] Create SessionStats class
- [ ] Track all required stats (see list above)
- [ ] Update stats through StatsManager (centralized)
- [ ] Save stats with currency (atomic operation)

### Session Management
- [ ] Create SessionData class with unique sessionId
- [ ] Track start time, end time, outcome
- [ ] Implement checkpoint system (every 30s)
- [ ] Detect clean exit vs crash
- [ ] Offer restore on crash recovery

### Save System
- [ ] Create DataManager singleton
- [ ] Separate PlayerData (persistent) vs SessionData (temporary)
- [ ] Use JSON serialization (easy to migrate)
- [ ] Implement Save() and Load() methods
- [ ] Plan for cloud save migration (structure ready)

### Anti-Cheat (MVP Level)
- [ ] Encrypt PlayerPrefs data
- [ ] Add checksum validation
- [ ] Log unusual patterns (10000 currency in 1 second = suspicious)
- [ ] Don't trust client calculations (validate server-side later)

### Multiplayer Preparation
- [ ] Keep Core scripts independent of multiplayer
- [ ] Use events/delegates for cross-script communication
- [ ] Plan folder structure (Core, Managers, Multiplayer, Data)
- [ ] Design for server authority (server validates, client predicts)

---

## CRITICAL ARCHITECTURE RULES

Follow these rules religiously:

### 1. Separation of Concerns
✅ **DO:** Currency logic in CurrencyManager
❌ **DON'T:** Currency logic scattered across Enemy, Player, WaveManager

### 2. Event-Driven Architecture
✅ **DO:** Use events for communication (OnCurrencyChanged)
❌ **DON'T:** Direct method calls between systems

### 3. Data Encapsulation
✅ **DO:** Store data in classes (PlayerData, SessionData)
❌ **DON'T:** Loose variables everywhere

### 4. Single Source of Truth
✅ **DO:** CurrencyManager.currency is authoritative
❌ **DON'T:** Multiple copies of currency value

### 5. Validation at Edges
✅ **DO:** Validate when adding currency (CurrencyManager.AddCurrency)
❌ **DON'T:** Trust client values blindly

### 6. Logging Everything
✅ **DO:** Log all transactions, actions, errors
❌ **DON'T:** Silent failures

### 7. Plan for Async
✅ **DO:** Design systems to work async (cloud saves)
❌ **DON'T:** Assume instant operations

---

## WHEN TO IMPLEMENT EACH FEATURE

### MVP (Now - Next 4 Weeks)
- Currency system (centralized, event-driven)
- Basic stats tracking (kills, deaths, waves)
- PlayerPrefs save/load
- Session management (crash protection)
- Upgrade shop (3-5 upgrades)
- Weapon system (3 weapons)

### Post-MVP v1.1 (Month 2)
- PlayFab/Firebase integration
- Cloud saves
- Account system (login/register)
- Basic leaderboards

### v1.2 (Month 3)
- Multiplayer foundation
- Co-op lobbies (2 players)
- Server validation
- Sync systems

### v2.0 (Month 4+)
- Full multiplayer (4 players)
- Website with stats API
- Advanced leaderboards
- Matchmaking

---

## RECOMMENDED TECH STACK

### MVP
- Unity 2022.3 LTS
- PlayerPrefs (local saves)
- JSON serialization

### Post-MVP
- PlayFab or Firebase (backend)
- Unity Netcode for GameObjects (multiplayer)
- React (website frontend)
- Node.js + Express (API server - optional)

### Future
- PostgreSQL or MongoDB (database)
- AWS/Azure (hosting)
- Docker (deployment)

---

## FINAL NOTES

**Remember:**
1. **Build for multiplayer from day one** - even if you don't ship it
2. **Centralize everything** - managers are your friend
3. **Use events** - direct calls are the enemy
4. **Log everything** - you'll thank yourself later
5. **Plan for the server** - even in single-player

**If you follow these patterns:**
- ✅ Adding multiplayer = adding scripts, not rewriting
- ✅ Adding cloud saves = swapping save method, not restructuring
- ✅ Adding stats API = reading existing data, not collecting new
- ✅ Adding anti-cheat = enabling validation, not redesigning

**This document is your north star. Reference it before implementing ANY new system.**

---

**Last Updated:** January 26, 2026
**Next Review:** After implementing currency system (validate patterns work)

