# BLACKSITE 2D - Complete Development Documentation
**Version 3.0 - Updated: January 26, 2026**
**Session 3 - Weapon System Refactor & Mystery Box Implementation**

---

## 🔴 CRITICAL: READ THIS FIRST

**This is Session 3 documentation.** We've completed a major refactor of the weapon system and implemented a COD Zombies-style mystery box. If you're a new AI instance picking up this project, read this entire document to understand the current architecture.

**Project Status:** MVP Phase 1 - ~65% complete
**Latest Major Changes:** Modular weapon system, mystery box, cleaner architecture
**Next Steps:** Currency system, hub scene, meta-progression

---

## TABLE OF CONTENTS

1. [Project Overview](#project-overview)
2. [Session 3 Summary (What Just Happened)](#session-3-summary)
3. [Current Build Status](#current-build-status)
4. [Complete System Documentation](#complete-system-documentation)
5. [Complete Code Reference](#complete-code-reference)
6. [Unity Setup Guide](#unity-setup-guide)
7. [Architecture Decisions](#architecture-decisions)
8. [Known Issues](#known-issues)
9. [Next Steps & Roadmap](#next-steps-roadmap)
10. [Session History](#session-history)

---

## PROJECT OVERVIEW

### Game Concept
**BLACKSITE** is a 2D top-down roguelite extraction shooter inspired by COD Zombies mechanics combined with roguelite progression and extraction gameplay.

### Core Pillars
1. **Wave-Based Survival** - Controlled pacing, escalating difficulty
2. **Extraction Mechanic** - Risk/reward decisions (push deeper or extract early)
3. **Roguelite Progression** - Meta-progression between runs, permadeath with partial retention
4. **Tactical Combat** - Positioning and kiting over raw DPS
5. **PS1 Horror Aesthetic** - Low-fi visuals with high atmosphere

### Technical Specifications
- **Engine:** Unity 2022.3+ LTS (URP 2D)
- **Target Platform:** PC (Windows)
- **Input:** Keyboard + Mouse
- **Target Resolution:** 1920x1080
- **Target FPS:** 60 FPS
- **Development Approach:** Modular, future-proof architecture

---

## SESSION 3 SUMMARY

### What We Accomplished This Session

**Major Refactor: Weapon System (Modular Architecture)**
- Separated shooting logic from PlayerController
- Created base Weapon class (abstract foundation)
- Created PistolBase class (pistol behavior without stats)
- Created 3 weapon variants: Pistol, RevolverX11, MountainHawk
- Weapons use inheritance hierarchy for easy variants
- Each weapon sets its own stats (no confusing override pattern)

**New Feature: Mystery Box**
- COD Zombies-style weapon randomizer
- Configurable spin duration and weapon pool
- Currency cost ready (commented out for now)
- Take/decline choice system
- Cooldown between uses
- Fully reusable prefab (place multiple in scene)

**Architecture Improvements:**
- PlayerController now ONLY handles movement/aiming
- WeaponController handles ALL shooting logic
- Bullet.cs is fully modular (size, damage, speed customizable)
- Clean separation of concerns
- Ready for future upgrades/powerups

**Bug Fixes:**
- Audio system working (gunshot, damage, death sounds)
- Bullet size fixed (was too large after refactor)
- Mystery box trigger detection working

### Why These Changes Matter

**Before:** Adding a new weapon required modifying PlayerController
**After:** Create new weapon class, inherit from base, set stats - done!

**Before:** Bullet stats were hardcoded
**After:** Bullets accept parameters, can be modified by upgrades

**Before:** No way to get different weapons in-game
**After:** Mystery box provides weapon variety and gameplay loop

---

## CURRENT BUILD STATUS

### ✅ COMPLETED SYSTEMS (100% Functional)

#### Core Gameplay
- ✅ Player movement (WASD, 8-directional, normalized)
- ✅ Mouse aiming (player rotates to face cursor)
- ✅ Modular shooting system (WeaponController)
- ✅ Weapon switching system (mystery box)
- ✅ Bullet physics (penetration, modular stats, auto-destroy)
- ✅ Enemy AI (chase player behavior)
- ✅ Wave spawning (progressive difficulty scaling, edge spawning, gradual)
- ✅ Player health system (damage, invincibility frames, death)
- ✅ Enemy health system (3-hit kills with default pistol, death events)
- ✅ Extraction mechanic (hold E, final wave, success/fail)

#### Weapon System (NEW!)
- ✅ Base Weapon class (abstract foundation)
- ✅ PistolBase class (single-shot behavior)
- ✅ 3 Weapon variants (Pistol, RevolverX11, MountainHawk)
- ✅ WeaponController (handles all shooting input)
- ✅ Modular bullet system (customizable properties)
- ✅ Weapon inheritance hierarchy (easy to add variants)

#### Mystery Box (NEW!)
- ✅ Prefab-based (reusable, place anywhere)
- ✅ Player proximity detection
- ✅ Spin animation (cycles weapon names)
- ✅ Random weapon selection from pool
- ✅ Take/decline choice system
- ✅ Cooldown timer
- ✅ UI prompts (world-space canvas)
- ✅ Currency cost ready (commented out)

#### UI Systems
- ✅ Health display (real-time updates)
- ✅ Wave counter
- ✅ Kill counter (tracks total kills)
- ✅ Death screen (shows kills, restart/menu buttons)
- ✅ Mystery box UI (world-space prompts)

#### Game Feel / Juice
- ✅ Screen shake (shooting, taking damage)
- ✅ Invincibility flash (red sprite flash on damage)
- ✅ Particle effects (enemy death blood splatter)
- ✅ Gradual enemy spawning (trickle-in effect)
- ✅ Off-screen spawning (enemies from edges)
- ✅ Audio system (gunshot, damage, death sounds with 2D spatial)

#### Technical Systems
- ✅ Event-driven architecture (enemy death notifications)
- ✅ Dual collider system (physics + trigger on enemies)
- ✅ Object lifecycle management (proper instantiate/destroy)
- ✅ Hit tracking (HashSet prevents multi-hit bugs)
- ✅ Modular weapon architecture (inheritance-based)
- ✅ Separation of concerns (PlayerController vs WeaponController)

### 🚧 IN PROGRESS
- 🚧 Currency system (foundation designed, ready to implement)

### 📋 PLANNED (Phase 1)
- ⬜ Currency system implementation
- ⬜ Between-run hub (safe zone, results screen)
- ⬜ Basic meta-progression (persistent upgrades)
- ⬜ Victory screen on extraction
- ⬜ Upgrade shop (3-5 upgrades)

### 📋 FUTURE (Phase 2+)
- ⬜ Additional weapons (shotgun, SMG, rifle variants)
- ⬜ Additional enemy types (runner, bruiser, ranged)
- ⬜ Procedural room generation
- ⬜ Weapon upgrade system
- ⬜ PS1 post-processing effects
- ⬜ Multiplayer co-op (architecture ready)
- ⬜ Online accounts & cloud saves (architecture ready)
- ⬜ Stats API & website (architecture ready)

---

## COMPLETE SYSTEM DOCUMENTATION

### 1. WEAPON SYSTEM ARCHITECTURE (NEW!)

**Philosophy:** Modular, inheritance-based system where weapon types (pistol, shotgun) define behavior, and variants (RevolverX11, MountainHawk) define stats.

#### Hierarchy:
```
Weapon (abstract base)
├── PistolBase (abstract - defines single-shot behavior)
│   ├── Pistol (concrete - default pistol)
│   ├── RevolverX11 (concrete - high damage, slow fire)
│   └── MountainHawk (concrete - massive damage, bigger bullets)
│
├── ShotgunBase (future - defines spread-shot behavior)
│   ├── PumpShotgun (future)
│   └── AutoShotgun (future)
│
└── SMGBase (future - defines rapid-fire behavior)
    └── variants...
```

#### How It Works:

**Weapon.cs (Abstract Base):**
- Defines common properties (fireRate, damage, speed, bulletPrefab)
- Provides helper methods (SpawnBullet, CanFire, UpdateFireCooldown)
- Forces child classes to implement Fire() method
- Accessible from anywhere via inheritance

**PistolBase.cs (Weapon Category):**
- Inherits from Weapon
- Defines pistol-specific behavior (single shot)
- Provides helper methods (FireSingleShot, FireBurst, TriggerScreenShake)
- Does NOT set default stats (pure parent)
- Child weapons set their own stats

**Pistol.cs / RevolverX11.cs / MountainHawk.cs (Variants):**
- Inherit from PistolBase
- Set their own stats in Awake()
- Use or override Fire() method
- Can add special effects (MountainHawk has bigger bullets)

#### Adding New Weapons:

**New Pistol Variant:**
```csharp
public class NewPistol : PistolBase
{
    void Awake()
    {
        weaponName = "New Pistol";
        fireRate = 0.3f;
        bulletSpeed = 18f;
        baseDamage = 2;
    }
    // Uses default PistolBase.Fire() - single shot
}
```

**New Weapon Type (Shotgun):**
```csharp
public abstract class ShotgunBase : Weapon
{
    public int pelletCount;
    public float spreadAngle;
    
    public override void Fire(Vector2 direction, Transform firePoint)
    {
        for (int i = 0; i < pelletCount; i++)
        {
            float angle = Random.Range(-spreadAngle, spreadAngle);
            Vector2 spreadDir = RotateVector(direction, angle);
            SpawnBullet(spreadDir, firePoint);
        }
        UpdateFireCooldown();
    }
}

public class PumpShotgun : ShotgunBase
{
    void Awake()
    {
        weaponName = "Pump Shotgun";
        fireRate = 1.0f;
        baseDamage = 3;
        pelletCount = 7;
        spreadAngle = 20f;
    }
}
```

---

### 2. WEAPON CONTROLLER SYSTEM (NEW!)

**File:** `WeaponController.cs`

**Purpose:** Handles ALL shooting input and weapon firing logic (removed from PlayerController).

**How It Works:**
1. Player holds left mouse button
2. WeaponController checks if weapon can fire (`currentWeapon.CanFire()`)
3. Gets mouse position and calculates direction
4. Calls weapon's `Fire()` method with direction
5. Triggers screen shake
6. Weapon handles its own firing logic

**Key Features:**
- Centralized shooting logic (one place to modify)
- Works with any weapon type (polymorphism)
- Easy weapon switching via `EquipWeapon(newWeapon)` method
- Decoupled from player movement

**Integration:**
- Attached to Player GameObject
- References current equipped weapon (child GameObject)
- References FirePoint for bullet spawn position
- References CameraShake for screen shake

---

### 3. MYSTERY BOX SYSTEM (NEW!)

**File:** `MysteryBox.cs`

**Purpose:** COD Zombies-style weapon randomizer. Player activates box, it spins through weapons, lands on random one, player chooses to take or decline.

**How It Works:**
1. Player enters trigger range → UI appears
2. Press E → Spins for `spinDuration` seconds
3. During spin: weapon names cycle rapidly every `spinSpeed` seconds
4. After spin: lands on random weapon from `weaponPool`
5. Player choice: E to take, Q to decline
6. If taken: destroys current weapon, equips new one
7. Box goes on cooldown for `cooldownTime` seconds

**Features:**
- Fully configurable (spin time, cost, cooldown, weapon pool)
- Currency cost ready (commented out, easy to enable)
- World-space UI (canvas follows box)
- Reusable prefab (place multiple in scene)
- Trigger-based detection
- State machine (ready → spinning → weapon offered → cooldown)

**States:**
1. **Ready** - Can be activated (canUse = true, E to use)
2. **Spinning** - Cycling weapon names (isSpinning = true)
3. **Weapon Ready** - Player can choose (weaponReady = true, E/Q to take/decline)
4. **Cooldown** - Can't use yet (canUse = false, countdown display)

**Unity Setup:**
```
MysteryBox (prefab)
├── Sprite (visual placeholder)
├── Circle Collider 2D (trigger, radius ~2-3)
├── Canvas (World Space, above box)
│   └── PromptText (TextMeshPro)
└── MysteryBox (script)
```

**Configuration:**
- Weapon Pool: List of weapon prefabs
- Spin Duration: 3 seconds (time to spin)
- Spin Speed: 0.1 seconds (time between name changes)
- Cost: 950 (commented out)
- Cooldown Time: 5 seconds

**Future Currency Integration:**
Uncomment lines ~45 and ~130 in MysteryBox.cs:
```csharp
if (CurrencyManager.Instance.GetCurrency() >= cost)
{
    CurrencyManager.Instance.SpendCurrency(cost, "mystery_box");
    StartCoroutine(SpinForWeapon());
}
```

---

### 4. PLAYER CONTROLLER SYSTEM (UPDATED - SIMPLIFIED)

**File:** `PlayerController.cs`

**What Changed:** Removed ALL shooting logic - now ONLY handles movement and aiming.

**Current Responsibilities:**
- WASD movement input
- Mouse aiming (rotate to face cursor)
- Physics-based movement (Rigidbody2D)
- That's it!

**What Was Removed:**
- ❌ Shooting logic (moved to WeaponController)
- ❌ Bullet prefab reference
- ❌ Fire rate tracking
- ❌ Camera shake reference (moved to WeaponController)

**Benefits:**
- Cleaner, focused responsibility
- Easier to read and maintain
- Weapon changes don't affect movement code
- Follows Single Responsibility Principle

---

### 5. MODULAR BULLET SYSTEM (UPDATED)

**File:** `Bullet1.cs` (class name: `Bullet`)

**What Changed:** Bullets now accept parameters and can be dynamically modified.

**New Features:**
- `Initialize(direction, damage, speed)` - Set bullet properties on spawn
- `ApplyModifiers(sizeMultiplier, speedMultiplier, damageMultiplier)` - Modify bullet after spawn
- `size` property - Scales bullet visually
- All properties customizable per-bullet

**Use Cases:**
```csharp
// Normal bullet
bullet.Initialize(direction, 1, 20f);

// Powerup: Double size bullets for 30 seconds
bullet.ApplyModifiers(sizeMultiplier: 2f);

// Weapon-specific: MountainHawk fires bigger bullets
bullet.ApplyModifiers(sizeMultiplier: 1.5f);

// Future: Explosive bullets
bullet.ApplyModifiers(sizeMultiplier: 3f, damageMultiplier: 5f);
```

**How Damage Works Now:**
1. Weapon defines `baseDamage` (e.g., RevolverX11 = 3)
2. Weapon calls `SpawnBullet()` which calls `bullet.Initialize(direction, baseDamage, speed)`
3. Bullet stores damage value
4. Bullet hits enemy → calls `enemy.TakeDamage(damage)`

**Hit Tracking:**
- Uses HashSet to prevent multi-hit bugs
- Each bullet can only damage each enemy once
- Bullets penetrate (don't destroy on hit)

---

### 6. EXISTING SYSTEMS (Unchanged from Session 2)

These systems still work as documented in v2.0:

- **Enemy AI System** - Chase player, dual colliders, death events
- **Wave Spawning System** - Progressive difficulty, edge spawning, gradual trickle
- **Player Health System** - Damage, i-frames, visual feedback, death
- **Extraction System** - Hold E, final wave, survival timer
- **Screen Shake System** - Triggered by shooting/damage
- **Particle Effects System** - Enemy death blood splatter
- **Audio System** - 2D spatial audio for gunshots/damage/death
- **UI Systems** - Health, wave, kills, death screen

See v2.0 documentation for details on these systems.

---

## COMPLETE CODE REFERENCE

### NEW SCRIPTS (Session 3)

#### Weapon.cs (Abstract Base)

```csharp
using UnityEngine;

public abstract class Weapon : MonoBehaviour
{
    [Header("Weapon Stats")]
    public string weaponName = "Weapon";
    public float fireRate = 0.2f;      // Time between shots
    public float bulletSpeed = 20f;
    public int baseDamage = 1;
    
    [Header("Prefabs")]
    public GameObject bulletPrefab;
    
    [Header("Audio")]
    public AudioClip fireSound;
    
    protected float nextFireTime = 0f;
    
    // Check if weapon can fire (cooldown ready)
    public bool CanFire()
    {
        return Time.time >= nextFireTime;
    }
    
    // Each weapon implements its own fire behavior
    public abstract void Fire(Vector2 direction, Transform firePoint);
    
    // Helper method for subclasses to spawn bullets
    protected GameObject SpawnBullet(Vector2 direction, Transform firePoint)
    {
        Vector3 spawnPos = firePoint != null ? firePoint.position : transform.position;
        GameObject bullet = Instantiate(bulletPrefab, spawnPos, Quaternion.identity);
        
        Bullet bulletScript = bullet.GetComponent<Bullet>();
        if (bulletScript != null)
        {
            bulletScript.Initialize(direction, baseDamage, bulletSpeed);
        }
        
        return bullet;
    }
    
    // Update cooldown after firing
    protected void UpdateFireCooldown()
    {
        nextFireTime = Time.time + fireRate;
    }
}
```

---

#### PistolBase.cs (Weapon Category)

```csharp
using UnityEngine;

public abstract class PistolBase : Weapon
{
    [Header("Pistol Settings")]
    public float screenShakePower = 0.1f;
    public float screenShakeDuration = 0.1f;
    
    // No Awake() - child classes set their own stats
    // No SetWeaponStats() - stats set directly in child Awake()
    
    // Default pistol firing behavior (single shot)
    public override void Fire(Vector2 direction, Transform firePoint)
    {
        FireSingleShot(direction, firePoint);
        PlayFireSound();
        TriggerScreenShake();
        UpdateFireCooldown();
    }
    
    // Helper method for single shot (reusable)
    protected void FireSingleShot(Vector2 direction, Transform firePoint)
    {
        SpawnBullet(direction, firePoint);
    }
    
    // Helper method for burst fire (for burst pistols)
    protected void FireBurst(Vector2 direction, Transform firePoint, int burstCount, float burstDelay)
    {
        StartCoroutine(BurstFireCoroutine(direction, firePoint, burstCount, burstDelay));
    }
    
    System.Collections.IEnumerator BurstFireCoroutine(Vector2 direction, Transform firePoint, int count, float delay)
    {
        for (int i = 0; i < count; i++)
        {
            SpawnBullet(direction, firePoint);
            if (i < count - 1)
            {
                yield return new WaitForSeconds(delay);
            }
        }
    }
    
    protected void PlayFireSound()
    {
        if (fireSound != null)
        {
            AudioSource.PlayClipAtPoint(fireSound, transform.position);
        }
    }
    
    protected virtual void TriggerScreenShake()
    {
        CameraShake shake = Camera.main.GetComponent<CameraShake>();
        if (shake != null)
        {
            shake.TriggerShake(screenShakePower, screenShakeDuration);
        }
    }
}
```

---

#### Pistol.cs (Default Pistol)

```csharp
using UnityEngine;

public class Pistol : PistolBase
{
    void Awake()
    {
        // Set stats directly
        weaponName = "Pistol";
        fireRate = 0.2f;
        bulletSpeed = 20f;
        baseDamage = 1;
        
        screenShakePower = 0.1f;
        screenShakeDuration = 0.1f;
    }
    
    // Uses default PistolBase.Fire() - single shot
}
```

---

#### RevolverX11.cs (High Damage Pistol)

```csharp
using UnityEngine;

public class RevolverX11 : PistolBase
{
    void Awake()
    {
        // Set stats directly
        weaponName = "Revolver-X11";
        fireRate = 0.5f;           // Slower fire rate
        bulletSpeed = 25f;         // Faster bullets
        baseDamage = 3;            // Higher damage (one-shots 3HP enemies)
        
        screenShakePower = 0.15f;  // Bigger shake
        screenShakeDuration = 0.15f;
    }
    
    // Uses default PistolBase.Fire() - single shot
    // Can override Fire() here if you want special behavior
}
```

---

#### MountainHawk.cs (Massive Damage, Big Bullets)

```csharp
using UnityEngine;

public class MountainHawk : PistolBase
{
    [Header("Mountain Hawk Special")]
    public float bulletSizeMultiplier = 1.5f;
    
    void Awake()
    {
        // Set stats directly
        weaponName = "Mountain Hawk";
        fireRate = 0.8f;           // Very slow fire rate
        bulletSpeed = 30f;         // Very fast bullets
        baseDamage = 5;            // Massive damage
        
        screenShakePower = 0.3f;   // Huge shake
        screenShakeDuration = 0.2f;
    }
    
    public override void Fire(Vector2 direction, Transform firePoint)
    {
        // Fire bullet
        GameObject bulletObj = SpawnBullet(direction, firePoint);
        
        // Special: Make bullet bigger
        Bullet bullet = bulletObj.GetComponent<Bullet>();
        if (bullet != null)
        {
            bullet.ApplyModifiers(sizeMultiplier: bulletSizeMultiplier);
        }
        
        // Play effects
        PlayFireSound();
        TriggerScreenShake();
        UpdateFireCooldown();
    }
}
```

---

#### WeaponController.cs

```csharp
using UnityEngine;

public class WeaponController : MonoBehaviour
{
    [Header("References")]
    public Weapon currentWeapon;     // Current equipped weapon
    public Transform firePoint;       // Where bullets spawn
    
    [Header("Effects")]
    public CameraShake cameraShake;   // Screen shake reference
    
    private Camera mainCamera;
    
    void Start()
    {
        mainCamera = Camera.main;
    }
    
    void Update()
    {
        HandleShooting();
    }
    
    void HandleShooting()
    {
        // Check if shooting and weapon is ready
        if (Input.GetMouseButton(0) && currentWeapon != null && currentWeapon.CanFire())
        {
            // Get mouse position and calculate direction
            Vector3 mousePos = mainCamera.ScreenToWorldPoint(Input.mousePosition);
            Vector2 direction = (mousePos - transform.position).normalized;
            
            // Fire the weapon
            currentWeapon.Fire(direction, firePoint);
            
            // Trigger screen shake (optional - weapons can handle their own)
            if (cameraShake != null)
            {
                cameraShake.TriggerShake(0.1f, 0.1f);
            }
        }
    }
    
    // Method to switch weapons (used by mystery box)
    public void EquipWeapon(Weapon newWeapon)
    {
        currentWeapon = newWeapon;
    }
}
```

---

#### MysteryBox.cs

```csharp
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using TMPro;

public class MysteryBox : MonoBehaviour
{
    [Header("Weapon Pool")]
    [Tooltip("Add weapon prefabs that this box can give")]
    public List<GameObject> weaponPool = new List<GameObject>();
    
    [Header("Box Settings")]
    public float spinDuration = 3f;           // How long to spin
    public float spinSpeed = 0.1f;            // How fast names cycle
    public int cost = 950;                    // Currency cost (for later)
    
    [Header("Cooldown")]
    public float cooldownTime = 5f;           // Time before box ready again
    public bool canUse = true;                // Is box ready?
    
    [Header("UI References")]
    public TextMeshProUGUI promptText;        // UI text
    public Canvas boxCanvas;                  // Canvas for UI
    
    [Header("Visual Feedback")]
    public GameObject spinEffect;             // Optional particles
    
    private bool playerInRange = false;
    private bool isSpinning = false;
    private bool weaponReady = false;
    private GameObject selectedWeapon;
    
    void Start()
    {
        if (boxCanvas != null)
        {
            boxCanvas.enabled = false;
        }
        UpdatePromptText("");
    }
    
    void Update()
    {
        if (!playerInRange) return;
        
        // State 1: Box ready, player can activate
        if (canUse && !isSpinning && !weaponReady && Input.GetKeyDown(KeyCode.E))
        {
            // CURRENCY CHECK (commented out for now)
            // if (CurrencyManager.Instance.GetCurrency() >= cost)
            // {
            //     CurrencyManager.Instance.SpendCurrency(cost, "mystery_box");
                StartCoroutine(SpinForWeapon());
            // }
            // else
            // {
            //     UpdatePromptText("Not enough currency!");
            // }
        }
        
        // State 2: Weapon ready, player chooses
        if (weaponReady)
        {
            if (Input.GetKeyDown(KeyCode.E))
            {
                TakeWeapon();
            }
            else if (Input.GetKeyDown(KeyCode.Q))
            {
                DeclineWeapon();
            }
        }
    }
    
    IEnumerator SpinForWeapon()
    {
        isSpinning = true;
        UpdatePromptText("Mystery Box spinning...");
        
        if (spinEffect != null)
        {
            spinEffect.SetActive(true);
        }
        
        // Cycle through weapon names rapidly
        float elapsed = 0f;
        while (elapsed < spinDuration)
        {
            GameObject randomWeaponPrefab = weaponPool[Random.Range(0, weaponPool.Count)];
            
            // Instantiate temporarily to get initialized name
            GameObject tempWeapon = Instantiate(randomWeaponPrefab);
            Weapon weaponScript = tempWeapon.GetComponent<Weapon>();
            
            if (weaponScript != null)
            {
                UpdatePromptText(weaponScript.weaponName);
            }
            
            Destroy(tempWeapon);
            
            yield return new WaitForSeconds(spinSpeed);
            elapsed += spinSpeed;
        }
        
        if (spinEffect != null)
        {
            spinEffect.SetActive(false);
        }
        
        // Select final weapon
        selectedWeapon = weaponPool[Random.Range(0, weaponPool.Count)];
        
        GameObject tempFinalWeapon = Instantiate(selectedWeapon);
        Weapon finalWeapon = tempFinalWeapon.GetComponent<Weapon>();
        
        if (finalWeapon != null)
        {
            UpdatePromptText($"{finalWeapon.weaponName}\nPress E to take | Press Q to decline");
        }
        
        Destroy(tempFinalWeapon);
        
        isSpinning = false;
        weaponReady = true;
    }
    
    void TakeWeapon()
    {
        Debug.Log("Player took weapon: " + selectedWeapon.name);
        
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player != null)
        {
            WeaponController weaponController = player.GetComponent<WeaponController>();
            if (weaponController != null)
            {
                // Destroy old weapon
                if (weaponController.currentWeapon != null)
                {
                    Destroy(weaponController.currentWeapon.gameObject);
                }
                
                // Instantiate new weapon as child of player
                GameObject newWeapon = Instantiate(selectedWeapon, player.transform);
                Weapon weaponScript = newWeapon.GetComponent<Weapon>();
                
                // Equip new weapon
                weaponController.EquipWeapon(weaponScript);
                
                UpdatePromptText($"Equipped {weaponScript.weaponName}!");
            }
        }
        
        weaponReady = false;
        StartCoroutine(CooldownTimer());
    }
    
    void DeclineWeapon()
    {
        Debug.Log("Player declined weapon: " + selectedWeapon.name);
        UpdatePromptText("Weapon declined");
        weaponReady = false;
        StartCoroutine(CooldownTimer());
    }
    
    IEnumerator CooldownTimer()
    {
        canUse = false;
        
        float remaining = cooldownTime;
        while (remaining > 0)
        {
            UpdatePromptText($"Cooldown: {remaining:F1}s");
            yield return new WaitForSeconds(0.1f);
            remaining -= 0.1f;
        }
        
        canUse = true;
        UpdatePromptText("Press E to use Mystery Box");
    }
    
    void UpdatePromptText(string message)
    {
        if (promptText != null)
        {
            promptText.text = message;
        }
    }
    
    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            playerInRange = true;
            
            if (boxCanvas != null)
            {
                boxCanvas.enabled = true;
            }
            
            if (canUse && !weaponReady)
            {
                // UNCOMMENT when currency system ready:
                // UpdatePromptText($"Press E to use Mystery Box (Cost: {cost})");
                UpdatePromptText("Press E to use Mystery Box");
            }
        }
    }
    
    void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            playerInRange = false;
            
            if (boxCanvas != null)
            {
                boxCanvas.enabled = false;
            }
        }
    }
}
```

---

### UPDATED SCRIPTS (Session 3)

#### PlayerController.cs (SIMPLIFIED)

```csharp
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [Header("Movement Settings")]
    public float moveSpeed = 5f;
    
    private Vector2 moveInput;
    private Rigidbody2D rb;
    private Camera mainCamera;
    
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        mainCamera = Camera.main;
    }
    
    void Update()
    {
        HandleMovementInput();
    }
    
    void FixedUpdate()
    {
        ApplyMovement();
    }
    
    void LateUpdate()
    {
        HandleMouseAim();
    }
    
    void HandleMovementInput()
    {
        float moveX = Input.GetAxisRaw("Horizontal");
        float moveY = Input.GetAxisRaw("Vertical");
        moveInput = new Vector2(moveX, moveY).normalized;
    }
    
    void ApplyMovement()
    {
        rb.linearVelocity = moveInput * moveSpeed;
    }
    
    void HandleMouseAim()
    {
        Vector3 mousePos = mainCamera.ScreenToWorldPoint(Input.mousePosition);
        Vector2 direction = mousePos - transform.position;
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.Euler(0f, 0f, angle - 90f);
    }
}
```

---

#### Bullet1.cs (UPDATED - Modular)

```csharp
using UnityEngine;
using System.Collections.Generic;

public class Bullet : MonoBehaviour
{
    [Header("Bullet Properties")]
    public float speed = 20f;
    public float size = 1f;
    public int damage = 1;
    public float lifetime = 3f;
    
    private Rigidbody2D rb;
    private HashSet<GameObject> hitEnemies = new HashSet<GameObject>();
    
    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }
    
    void Start()
    {
        // Apply visual properties (multiply existing scale, don't replace)
        transform.localScale *= size;
        
        // Destroy after lifetime
        Destroy(gameObject, lifetime);
    }
    
    // Initialize bullet with direction, damage, and speed
    public void Initialize(Vector2 direction, int dmg, float spd)
    {
        damage = dmg;
        speed = spd;
        
        if (rb == null)
        {
            rb = GetComponent<Rigidbody2D>();
        }
        
        rb.linearVelocity = direction * speed;
    }
    
    // Apply upgrade modifiers (for future powerups)
    public void ApplyModifiers(float sizeMultiplier = 1f, float speedMultiplier = 1f, float damageMultiplier = 1f)
    {
        size *= sizeMultiplier;
        speed *= speedMultiplier;
        damage = Mathf.RoundToInt(damage * damageMultiplier);
        
        // Apply new values (multiply, don't replace)
        transform.localScale *= sizeMultiplier;
        if (rb != null)
        {
            rb.linearVelocity = rb.linearVelocity.normalized * speed;
        }
    }
    
    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Enemy"))
        {
            // Check if we already hit this enemy
            if (hitEnemies.Contains(other.gameObject))
            {
                return;
            }
            
            // Mark as hit
            hitEnemies.Add(other.gameObject);
            
            // Damage the enemy
            Enemy enemy = other.GetComponent<Enemy>();
            if (enemy != null)
            {
                enemy.TakeDamage(damage);
            }
        }
    }
}
```

---

### UNCHANGED SCRIPTS (Session 2)

These scripts remain as documented in v2.0:

- **Enemy.cs** - AI, health, death, audio (unchanged)
- **WaveManager.cs** - Spawning, scaling, kill tracking (unchanged)
- **PlayerHealth.cs** - Damage, i-frames, death (unchanged)
- **ExtractionPoint.cs** - Activation, final wave (unchanged)
- **CameraShake.cs** - Screen shake effects (unchanged)
- **AutoDestroy.cs** - Particle cleanup (unchanged)

Refer to v2.0 documentation for full code.

---

## UNITY SETUP GUIDE

### File Structure

```
Assets/
├── Scenes/
│   └── SampleScene.unity
│
├── Scripts/
│   ├── Core/
│   │   ├── PlayerController.cs (UPDATED - simplified)
│   │   ├── Enemy.cs
│   │   ├── WaveManager.cs
│   │   ├── PlayerHealth.cs
│   │   └── ExtractionPoint.cs
│   │
│   ├── Weapons/ (NEW FOLDER)
│   │   ├── Weapon.cs (NEW - abstract base)
│   │   ├── PistolBase.cs (NEW - pistol category)
│   │   ├── Pistol.cs (NEW - default pistol)
│   │   ├── RevolverX11.cs (NEW - variant)
│   │   └── MountainHawk.cs (NEW - variant)
│   │
│   ├── Controllers/
│   │   └── WeaponController.cs (NEW)
│   │
│   ├── Items/
│   │   └── MysteryBox.cs (NEW)
│   │
│   ├── Effects/
│   │   ├── CameraShake.cs
│   │   └── AutoDestroy.cs
│   │
│   └── Projectiles/
│       └── Bullet1.cs (UPDATED - modular)
│
└── Prefabs/
    ├── Bullet.prefab
    ├── Zombie.prefab (Enemy)
    ├── EnemyDeathEffect.prefab
    ├── Pistol.prefab (NEW)
    ├── RevolverX11.prefab (NEW)
    ├── MountainHawk.prefab (NEW)
    └── MysteryBox.prefab (NEW)
```

---

### Player GameObject Setup (UPDATED)

**Hierarchy:**
```
Player
├── FirePoint
├── Pistol (has Pistol script) (NEW)
└── (other weapons added dynamically by mystery box)
```

**Components:**
- Transform (Position: 0,0,0 | Scale: 0.5,0.5,1)
- Sprite Renderer (temporary colored square)
- Rigidbody 2D
  - Body Type: Dynamic
  - Gravity Scale: 0
  - Constraints: Freeze Rotation Z ✓
- Box Collider 2D (Is Trigger: ✗)
- **Player Controller (Script)** - UPDATED, simplified
  - Move Speed: 5
- **Weapon Controller (Script)** - NEW
  - Current Weapon: [Drag Pistol child here]
  - Fire Point: [Drag FirePoint child here]
  - Camera Shake: [Drag Main Camera here]
- Player Health (Script) (unchanged)
  - Max Health: 10
  - Invincibility Time: 1
  - Health Text: [Drag UI text here]
  - Death Screen UI: [Drag death panel here]
  - Camera Shake: [Drag Main Camera here]

---

### Weapon Setup (NEW)

#### FirePoint (Child of Player)
- Empty GameObject
- Position: X: 0, Y: 0.3, Z: 0 (slightly forward)

#### Pistol (Child of Player)
- Empty GameObject
- **Pistol (Script)** component
  - Weapon Name: "Pistol" (set in Inspector to match code)
  - Fire Rate: 0.2
  - Bullet Speed: 20
  - Base Damage: 1
  - Bullet Prefab: [Drag Bullet prefab]
  - Fire Sound: [Drag gunshot sound]

**Note:** Inspector values should match Awake() values. Unity shows serialized values, but Awake() sets runtime values.

---

### Weapon Prefabs (NEW)

To create weapon prefabs for mystery box:

#### Create Pistol Prefab:
1. Duplicate Player → Pistol child
2. Drag duplicate out of Player (to root of Hierarchy)
3. Rename to "Pistol"
4. Drag into Prefabs folder
5. Delete from Hierarchy

#### Create RevolverX11 Prefab:
1. Create empty GameObject in Hierarchy
2. Name it "RevolverX11"
3. Add Component → RevolverX11 script
4. Configure:
   - Weapon Name: "Revolver-X11"
   - Fire Rate: 0.5
   - Bullet Speed: 25
   - Base Damage: 3
   - Bullet Prefab: [Bullet prefab]
   - Fire Sound: [gunshot]
5. Drag into Prefabs folder
6. Delete from Hierarchy

#### Create MountainHawk Prefab:
Same process as RevolverX11, but:
- Use MountainHawk script
- Set stats: Name: "Mountain Hawk", Fire Rate: 0.8, Speed: 30, Damage: 5
- Bullet Size Multiplier: 1.5

---

### Mystery Box Setup (NEW)

#### Create MysteryBox Prefab:

**Step 1: Create Base Object**
1. Hierarchy → Create Empty → Name: "MysteryBox"
2. Position: Anywhere in scene (X: 5, Y: 0 example)

**Step 2: Add Visual**
1. Right-click MysteryBox → 2D Object → Sprite → Square
2. Change color (purple/gold recommended)
3. Scale: 1x1 or 1.5x1.5

**Step 3: Add Collider**
1. Select MysteryBox (parent)
2. Add Component → Circle Collider 2D
3. **Is Trigger:** ✓ CHECKED
4. Radius: 2-3 (proximity detection range)

**Step 4: Add World-Space Canvas**
1. Right-click MysteryBox → UI → Canvas
2. Configure Canvas:
   - Render Mode: **World Space**
   - Width: 200
   - Height: 100
   - Position: X: 0, **Y: 1.5**, Z: 0 (floats above box)
   - Scale: **X: 0.01, Y: 0.01, Z: 0.01** (makes it readable size)

**Step 5: Add UI Text**
1. Right-click Canvas → UI → Text - TextMeshPro
2. Import TMP Essentials if prompted
3. Name: "PromptText"
4. Configure:
   - Text: "Press E to use Mystery Box"
   - Font Size: 24
   - Alignment: Center (horizontal + vertical)
   - Color: White
   - Rect Transform: Stretch to fill canvas

**Step 6: Add Script**
1. Select MysteryBox (parent)
2. Add Component → MysteryBox script
3. Configure:
   - **Weapon Pool:**
     - Size: 3
     - Element 0: [Drag Pistol prefab]
     - Element 1: [Drag RevolverX11 prefab]
     - Element 2: [Drag MountainHawk prefab]
   - **Box Settings:**
     - Spin Duration: 3
     - Spin Speed: 0.1
     - Cost: 950
     - Cooldown Time: 5
     - Can Use: ✓
   - **UI References:**
     - Prompt Text: [Drag PromptText child]
     - Box Canvas: [Drag Canvas child]
   - **Visual Feedback:**
     - Spin Effect: (leave empty for now)

**Step 7: Make Prefab**
1. Drag MysteryBox from Hierarchy → Prefabs folder
2. Should turn blue (prefab instance)
3. Can now duplicate or drag from Prefabs to place multiple

**Final Hierarchy:**
```
MysteryBox (prefab)
├── Square (visual sprite)
└── Canvas (World Space)
    └── PromptText (TextMeshPro)
```

---

### Tags Setup

Required tags (same as v2.0):
1. **Player** - Applied to Player GameObject
2. **Enemy** - Applied to enemy prefabs
3. **Bullet** - Applied to Bullet prefab

---

## ARCHITECTURE DECISIONS

### Why We Refactored the Weapon System

**Problem:** Original system had shooting logic in PlayerController. Adding new weapons meant modifying PlayerController each time.

**Solution:** Inheritance-based modular weapon system.

**Benefits:**
1. **Separation of Concerns** - PlayerController = movement, WeaponController = shooting
2. **Extensibility** - New weapon = new class, no existing code changes
3. **Polymorphism** - WeaponController works with any weapon type
4. **Reusability** - Weapon behaviors (burst fire, spread shot) reusable across variants
5. **Future-Proof** - Ready for upgrades, powerups, weapon mods

---

### Inspector vs Runtime Values

**Important Understanding:**

**Inspector shows:** Serialized values (saved in prefab/file)
- Set when prefab created
- Come from public variable declarations
- **Do not update during Play mode**

**Runtime uses:** Values after Awake() runs
- `Awake()` sets weapon stats
- Game uses these values
- Inspector doesn't reflect changes

**Example:**
- Inspector shows: Base Damage = 1 (default from declaration)
- Awake() sets: baseDamage = 3
- Game uses: 3 damage (correct!)
- Inspector still shows: 1 (misleading but harmless)

**Fix:** Set Inspector values to match Awake() values for clarity. Both should be same.

**Why This Happens:** Unity separates design-time (Editor) from runtime (Play mode). Inspector is design-time view.

---

### Why PistolBase is Abstract

**Decision:** Make PistolBase abstract (can't instantiate directly).

**Reason:** PistolBase defines behavior (how pistols fire), not a specific weapon. You use Pistol, RevolverX11, etc. - not PistolBase itself.

**Pattern:**
- `Weapon` - Abstract (all weapons)
- `PistolBase` - Abstract (pistol behavior)
- `Pistol` - Concrete (usable weapon)
- `RevolverX11` - Concrete (usable weapon)

This enforces clean architecture and prevents accidentally using the base class.

---

### Mystery Box Instantiation Pattern

**Challenge:** How to get weapon stats (name) from prefab before instantiating?

**Problem:** Prefabs don't run Awake() until instantiated.

**Solutions Tried:**
1. ❌ Check prefab directly - shows "Weapon" (default, not set yet)
2. ✅ Instantiate temporarily, get name, destroy - works!

**Current Approach:**
```csharp
GameObject temp = Instantiate(weaponPrefab);
Weapon weapon = temp.GetComponent<Weapon>();
string name = weapon.weaponName; // Awake() has run
Destroy(temp);
```

Creates temporary instance to trigger Awake(), reads name, destroys instance. Clean and works.

---

### Future Currency Integration

Mystery box is **currency-ready**. When currency system implemented:

**Step 1:** Uncomment lines in MysteryBox.cs (~45, ~130)
**Step 2:** Implement CurrencyManager singleton
**Step 3:** Mystery box automatically uses currency

**Design:** Currency check commented out, not removed. Easy to enable later with minimal changes.

---

## KNOWN ISSUES

### Minor Issues

1. **Inspector Shows Wrong Stats**
   - **Status:** Cosmetic only, runtime is correct
   - **Cause:** Inspector shows serialized values, Awake() sets runtime
   - **Workaround:** Set Inspector values to match Awake() for clarity
   - **Impact:** None - game uses correct runtime values

2. **Placeholder Art**
   - **Status:** Temporary, planned for replacement
   - **Current:** Colored squares for all sprites
   - **Planned:** PS1-style pixelated sprites

3. **No Meta-Progression Yet**
   - **Status:** Planned for next phase
   - **Current:** Each run is independent
   - **Planned:** Persistent currency and upgrades

4. **Single Enemy Type**
   - **Status:** Planned for Phase 2
   - **Current:** Only basic chaser zombie
   - **Planned:** Runner, Bruiser, Ranged enemies

5. **Mystery Box UI Not Rotating with Player**
   - **Status:** World-space canvas always faces same direction
   - **Workaround:** Position box where readable from all angles
   - **Planned Fix:** Billboard shader or face-camera script

### Technical Debt

1. **No Object Pooling**
   - **Impact:** May cause lag at high bullet/enemy counts (Wave 10+)
   - **Solution:** Implement object pooling for bullets and enemies

2. **Debug.Log in Production**
   - **Impact:** Minor performance overhead
   - **Solution:** Remove or wrap in `#if UNITY_EDITOR` before build

3. **FindGameObjectWithTag in Update-Like Functions**
   - **Impact:** Minor performance hit (mystery box uses this)
   - **Solution:** Cache player reference in Start()

4. **Temporary Instantiation for Names**
   - **Impact:** Minor overhead (mystery box weapon name cycling)
   - **Solution:** Acceptable for MVP, could optimize with ScriptableObjects

---

## NEXT STEPS & ROADMAP

### Immediate Next Steps (This Week)

**Priority 1: Currency System** ⭐ HIGHEST PRIORITY
- Create CurrencyManager singleton
- Track currency from kills (10 per bullet hit, 100 per kill)
- Display currency in UI
- Save/load with PlayerPrefs
- Enable currency in mystery box (uncomment)
- **Time:** ~2-3 hours
- **Complexity:** Medium
- **Blocks:** Shop, meta-progression, mystery box cost

**Priority 2: Hub Scene**
- Create between-run safe zone
- Results screen (kills, wave, currency earned)
- "Start Run" button
- **Time:** ~1-2 hours
- **Complexity:** Low-Medium

**Priority 3: Simple Upgrade Shop**
- 3-5 permanent upgrades (health, damage, speed)
- Spend currency to purchase
- Persistent between runs
- **Time:** ~2-3 hours
- **Complexity:** Medium

---

### Phase 1 Completion (Next 2-3 Weeks)

**Core Loop Polish:**
- ✅ Sound effects - COMPLETED
- ✅ Screen shake - COMPLETED
- ✅ Particle effects - COMPLETED
- ⬜ Victory screen on extraction (show stats, earned currency)
- ⬜ Enhanced visual feedback (hit sparks, muzzle flash)

**Currency & Progression:**
- ⬜ Currency system (earn, display, save)
- ⬜ Between-run hub
- ⬜ Upgrade shop (3-5 upgrades)
- ⬜ Persistent data system

**Content:**
- ⬜ 2-3 more weapon variants (using existing system)
- ⬜ Test balance (difficulty curve, weapon stats)

---

### Phase 2: Content Expansion (Weeks 4-8)

**Weapons:**
- ⬜ Shotgun base class + 2 variants
- ⬜ SMG base class + 2 variants
- ⬜ Weapon unlock system (purchase from shop)
- ⬜ Weapon switching UI (number keys)

**Enemies:**
- ⬜ Runner enemy (fast, low health, aggressive)
- ⬜ Bruiser enemy (slow, high health, blocks paths)
- ⬜ Ranged enemy (shoots projectiles, stays at distance)
- ⬜ Enemy variety in waves (mixed spawns)

**Map:**
- ⬜ Procedural room generation (basic)
- ⬜ Multiple room types (corridors, arenas, tight spaces)
- ⬜ Door system (opens between waves)
- ⬜ Environmental hazards (optional - spike traps)

**Meta-Progression:**
- ⬜ 5-10 permanent upgrades total
- ⬜ Unlock tiers (weapon slots, starting currency bonus)
- ⬜ Balance progression pacing (~10 runs to unlock core content)

---

### Phase 3: Polish & Release (Weeks 9-12)

**Visual Polish:**
- ⬜ PS1 post-processing (CRT, pixelation, film grain, color grading)
- ⬜ Replace placeholder sprites with pixel art
- ⬜ Lighting system (darkness + flashlight optional)
- ⬜ More particle effects (muzzle flash, casings, explosions)

**Audio:**
- ⬜ Background music (ambient, tense, escalating)
- ⬜ Full SFX suite
- ⬜ Audio mixing and balance

**Game Feel:**
- ⬜ Enhanced screenshake varieties
- ⬜ Camera trauma effects
- ⬜ Hit-stop frames (brief freeze on hit)
- ⬜ Animation (sprite sheets for walk/shoot)

**UI/UX:**
- ⬜ Main menu
- ⬜ Options menu (volume, controls, graphics)
- ⬜ Pause menu
- ⬜ Better HUD design
- ⬜ Tutorial/onboarding

**Balance & Testing:**
- ⬜ Difficulty curve tuning
- ⬜ Weapon balance
- ⬜ Enemy spawn rates
- ⬜ Meta-progression pacing
- ⬜ Extensive playtesting

---

### Stretch Goals (If Time Permits)

- ⬜ Multiple biomes/zones
- ⬜ Boss enemy or mini-bosses
- ⬜ Co-op multiplayer (2 players local)
- ⬜ Leaderboards (wave survival mode)
- ⬜ Daily challenge runs
- ⬜ Mod support (weapon/enemy modding)

---

## SESSION HISTORY

### Session 1 (January 10, 2026)
**Focus:** Core systems implementation
- Player movement (WASD + mouse aim)
- Basic shooting (hold to fire)
- Enemy AI (chase player)
- Wave spawning (basic scaling)
- Player health (damage, i-frames, death)
- Basic UI (health, wave)

**Status:** MVP foundations established (~40% complete)

---

### Session 2 (January 24, 2026)
**Focus:** Bug fixes and game feel

**Bugs Fixed:**
- Kill counter double-counting (event subscription bug)
- Enemies dying in one bullet (multi-hit bug, HashSet solution)
- Bullet multi-hit per frame (same enemy)
- Deprecation warnings (FindObjectOfType)

**Features Added:**
- Screen shake system (CameraShake.cs)
- Particle effects (enemy death blood splatter)
- Audio system (gunshot, damage, death sounds with 2D spatial)
- Gradual enemy spawning (coroutine-based trickle)
- Off-screen edge spawning (calculated from camera bounds)

**Improvements:**
- Enemy spawn pattern (circle → screen edges)
- Spawn timing (instant → gradual)
- Bullet damage handling (centralized in Bullet.cs)
- Visual feedback (shake, particles, sound)

**Documentation:**
- Created comprehensive v2.0 documentation
- Future-proofing architecture guide

**Status:** Core gameplay polished (~55% complete)

---

### Session 3 (January 26, 2026) - CURRENT
**Focus:** Weapon system refactor and mystery box

**Major Refactor:**
- Separated shooting from PlayerController
- Created modular weapon architecture
- Implemented inheritance hierarchy (Weapon → PistolBase → variants)
- Created WeaponController for shooting logic
- Updated Bullet to be fully modular

**New Scripts:**
- Weapon.cs (abstract base)
- PistolBase.cs (pistol behavior)
- Pistol.cs (default pistol)
- RevolverX11.cs (high damage variant)
- MountainHawk.cs (massive damage, big bullets)
- WeaponController.cs (shooting controller)
- MysteryBox.cs (weapon randomizer)

**Updated Scripts:**
- PlayerController.cs (simplified - movement only)
- Bullet1.cs (modular properties, ApplyModifiers)

**Features Added:**
- Mystery box (COD Zombies-style)
- Weapon switching system
- 3 weapon variants with different stats
- World-space UI for mystery box

**Architecture Decisions:**
- Clean inheritance structure
- PistolBase as pure parent (no preset stats)
- Weapons set stats in Awake()
- Inspector vs runtime values clarified
- Currency integration ready (commented out)

**Status:** Modular foundation complete (~65% complete)

**Next Session Focus:** Currency system implementation

---

## QUICK START GUIDE

### For Resuming Development in New Chat

**Current Status:** MVP Phase 1, ~65% complete, Session 3 complete

**What's Working:**
- Core gameplay loop (move, shoot, survive waves, extract)
- Modular weapon system (easy to add variants)
- Mystery box (weapon randomizer)
- Player and enemy systems
- Wave spawning and scaling
- Complete audio/visual feedback
- UI systems

**Recent Changes (Session 3):**
- ✅ Weapon system refactored (modular, inheritance-based)
- ✅ Mystery box implemented (COD Zombies-style)
- ✅ PlayerController simplified (movement only)
- ✅ WeaponController created (shooting logic)
- ✅ 3 weapon variants created (Pistol, RevolverX11, MountainHawk)

**Critical Files:**
- PlayerController.cs (UPDATED - simplified)
- WeaponController.cs (NEW)
- Weapon.cs, PistolBase.cs (NEW - base classes)
- Pistol.cs, RevolverX11.cs, MountainHawk.cs (NEW - variants)
- MysteryBox.cs (NEW)
- Bullet1.cs (UPDATED - modular)

**To Continue:**
1. Open Unity project: BLACKSITE_2D
2. Scene: SampleScene
3. Press Play to test current build
4. Check mystery box functionality
5. Next task: Implement currency system
6. Reference this document for all technical details
7. See Future-Proofing Architecture doc for currency system design

---

### Common Questions

**Q: Why are weapon stats in Inspector different from what they should be?**
A: Inspector shows serialized values (design-time). Awake() sets runtime values. Game uses runtime values (correct). Set Inspector to match for clarity, but not required.

**Q: How do I add a new weapon variant?**
A: Inherit from appropriate base (PistolBase, ShotgunBase, etc.), set stats in Awake(), optionally override Fire(). Create prefab, add to mystery box pool.

**Q: How do I add the currency system?**
A: See Future-Proofing Architecture doc for complete design. Create CurrencyManager singleton, track kills, save with PlayerPrefs, uncomment mystery box currency code.

**Q: Why did we separate PlayerController and WeaponController?**
A: Separation of concerns. PlayerController = movement/aiming. WeaponController = shooting. Easier to modify, test, extend. Follows Single Responsibility Principle.

**Q: Can I add powerups that modify bullets?**
A: Yes! Use `bullet.ApplyModifiers(sizeMultiplier, speedMultiplier, damageMultiplier)`. Example: Double size powerup = `bullet.ApplyModifiers(sizeMultiplier: 2f)`.

**Q: Why is PistolBase abstract?**
A: It defines behavior (how pistols work), not a specific weapon. Use Pistol, RevolverX11, etc. - not PistolBase directly. Enforces clean architecture.

---

## APPENDIX

### Learning Resources

**Unity Concepts Mastered (Session 1-3):**
1. Component System - GetComponent<>(), AddComponent()
2. Prefabs - Reusable GameObjects, Instantiate()
3. Physics 2D - Rigidbody2D, Collider2D, triggers vs collisions
4. Input System - Input.GetAxisRaw(), Input.GetMouseButton(), GetKey()
5. Coroutines - IEnumerator, yield return, StartCoroutine()
6. Events - C# System.Action, Invoke() pattern
7. Scene Management - LoadScene(), scene names
8. UI System - Canvas, TextMeshPro, Rect Transform, World Space canvas
9. Update Loops - Update(), FixedUpdate(), LateUpdate(), Awake() vs Start()
10. Vectors & Math - Vector2, normalized, Atan2, Quaternion
11. Collections - HashSet for fast lookups, List for ordered collections
12. Particle Systems - Emission, lifetime, gravity
13. **Inheritance** - abstract classes, virtual methods, override (NEW)
14. **Polymorphism** - base class references to child instances (NEW)
15. **Separation of Concerns** - Single Responsibility Principle (NEW)

---

### Key Unity APIs Used

**Session 1-2 (Previous):**
- Rigidbody2D.linearVelocity
- Transform.position, Transform.rotation, Transform.localScale
- Vector2.normalized
- Input.GetAxisRaw(), Input.GetMouseButton(), Input.GetKey()
- Camera.ScreenToWorldPoint()
- Instantiate(), Destroy()
- Time.deltaTime, Time.time
- GetComponent<T>()
- CompareTag()
- OnCollisionStay2D(), OnTriggerEnter2D(), OnTriggerExit2D()
- SceneManager.LoadScene()
- TextMeshProUGUI.text
- SpriteRenderer.color
- Mathf.Atan2(), Mathf.Max(), Mathf.RoundToInt()
- Quaternion.Euler()
- Random.Range(), Random.insideUnitCircle
- StartCoroutine(), yield return WaitForSeconds
- System.Action (C# events)

**Session 3 (New):**
- abstract classes (Weapon, PistolBase)
- virtual methods (Fire, TriggerScreenShake)
- override keyword (child classes override parent methods)
- protected access modifier (helper methods for children)
- System.Collections.Generic.List<T>
- Canvas render modes (World Space)
- TextMeshProUGUI (world-space UI)

---

### Project Statistics

**Lines of Code (Approximate):**
- Weapon.cs: 50 lines
- PistolBase.cs: 80 lines
- Pistol.cs: 15 lines
- RevolverX11.cs: 20 lines
- MountainHawk.cs: 35 lines
- WeaponController.cs: 45 lines
- MysteryBox.cs: 180 lines
- PlayerController.cs: 50 lines (reduced from ~100)
- Bullet1.cs: 80 lines (updated)

**Total New/Updated Code This Session:** ~550 lines

**Project Files:**
- Scripts: 15 files
- Prefabs: 7 prefabs
- Scenes: 1 scene

---

## CHANGELOG

### Version 3.0 (January 26, 2026) - Session 3

**Added:**
- Weapon.cs (abstract base class for all weapons)
- PistolBase.cs (abstract pistol behavior class)
- Pistol.cs (default pistol implementation)
- RevolverX11.cs (high damage pistol variant)
- MountainHawk.cs (massive damage pistol with big bullets)
- WeaponController.cs (centralized shooting controller)
- MysteryBox.cs (COD Zombies-style weapon randomizer)
- Pistol prefab, RevolverX11 prefab, MountainHawk prefab
- MysteryBox prefab (reusable, world-space UI)

**Changed:**
- PlayerController.cs - Simplified, removed all shooting logic
- Bullet1.cs - Added Initialize() and ApplyModifiers() methods
- Bullet1.cs - Fixed scale bug (multiply instead of replace)
- Player GameObject - Added WeaponController component
- Player GameObject - Added Pistol child weapon

**Fixed:**
- Bullet size bug (was 1x1 instead of 0.1x0.1)
- Mystery box trigger detection (required Player tag)
- Weapon name display (instantiate temp to get Awake() name)

**Architectural:**
- Implemented inheritance-based weapon system
- Separated shooting from movement (SRP)
- Made bullets fully modular (upgrades ready)
- Prepared currency integration (commented code)

---

### Version 2.0 (January 24, 2026) - Session 2

**Added:**
- CameraShake.cs (screen shake system)
- AutoDestroy.cs (particle cleanup)
- EnemyDeathEffect prefab (blood particles)
- Audio system (2D spatial sound)
- Gradual spawning (coroutine)
- Edge spawning (off-screen)

**Fixed:**
- Kill counter double-counting
- Enemies dying in one bullet
- Bullet multi-hit per frame

**Changed:**
- Enemy spawn pattern (circle → edges)
- Enemy spawn timing (instant → gradual)
- Bullet damage handling (centralized)

---

### Version 1.0 (January 10, 2026) - Session 1

**Initial MVP Development:**
- Complete player movement
- Basic shooting mechanics
- Enemy AI with chase behavior
- Wave spawning with difficulty scaling
- Player health with i-frames
- Basic UI (health, wave)
- Dual collider system (physics + trigger)
- Event-driven architecture
- Extraction mechanic
- Death screen

---

**END OF DOCUMENTATION**

*This document should be updated after each major milestone or session.*

*Last updated: January 26, 2026 - After weapon system refactor and mystery box implementation (Session 3)*

---

## FOR NEW AI INSTANCES

If you're a new Claude instance (or Opus 4.6!) picking up this project:

1. **Read the Session 3 Summary** first for recent context
2. **Check Current Build Status** to see what's done
3. **Review Complete System Documentation** for how systems work
4. **Reference Complete Code Reference** for implementation details
5. **Check Next Steps** for what to build next
6. **Read Future-Proofing Architecture doc** for currency system design

**Current Priority:** Implement currency system (see Future-Proofing doc for complete architecture).

**User's Development Style:**
- Prefers step-by-step instructions with full code examples
- Likes to understand WHY (engineering mindset)
- Breaks big problems into small chunks
- Values modularity and future-proofing
- Tests frequently, iterates carefully
- Documents thoroughly

**Communication Preferences:**
- Explain concepts clearly with examples
- Provide complete code (not snippets)
- Explain trade-offs and design decisions
- Be direct about issues
- Suggest best practices

Good luck! The project is in great shape. 🚀
