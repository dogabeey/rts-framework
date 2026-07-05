# instructions.md
# RTS Framework Development Guidelines (Unity)

## Objective

Build a reusable, modular, data-driven RTS framework suitable for creating multiple RTS games (similar to Age of Empires, Warcraft III, StarCraft, Supreme Commander, etc.) without rewriting core systems.

The framework should prioritize:

- SOLID principles
- Composition over inheritance
- Data-driven architecture
- High performance
- Multiplayer-ready architecture
- Easy modding
- Easy extension
- Testability
- Minimal coupling
- Deterministic simulation where possible

---

# General Principles

## Never hardcode gameplay

Everything gameplay-related should come from:

- ScriptableObjects
- Config assets
- Databases
- Data tables

Examples:

- Unit stats
- Building stats
- Weapons
- Damage
- Costs
- Build times
- Vision
- Movement speed
- Abilities
- Resources
- Technologies

Code should only execute logic.

---

## Prefer composition

Bad

EnemyUnit
WorkerUnit
SoldierUnit
KnightUnit

Good

Entity

+ Health
+ Movement
+ Builder
+ Gatherer
+ Inventory
+ Attacker
+ Vision
+ Transport
+ Producer
+ Construction
+ AbilityCaster

Every gameplay feature should be a component.

---

## Systems over giant MonoBehaviours

Gameplay logic belongs inside systems.

Example:

MovementSystem

AttackSystem

ResourceSystem

ConstructionSystem

ProjectileSystem

FogOfWarSystem

SelectionSystem

ProductionSystem

Avoid "God Objects."

---

## Keep MonoBehaviours thin

MonoBehaviours should mostly:

- Receive Unity events
- Hold references
- Forward to systems

Business logic belongs elsewhere.

---

## Code Requirements

Use:

- readonly
- interfaces
- dependency injection where appropriate
- events instead of polling
- object pooling
- async loading when appropriate

Avoid:

- FindObjectOfType
- GameObject.Find
- singleton abuse
- Update() everywhere

---

# Core Architecture

Divide the framework into modules.

Example

Framework/

Core/

Simulation/

Rendering/

Entities/

Movement/

Combat/

Economy/

Construction/

Production/

Selection/

Input/

Orders/

AI/

Networking/

UI/

Saving/

Editor/

Debug/

Each module must be independent.

---

# Entity System

Every entity should support:

Unique ID

Faction

Owner

Health

Armor

Vision

Selection

Movement

Orders

Abilities

Inventory

Resource carrying

Animation

Construction

Destruction

Serialization

Status effects

Upgrades

Tags

States

Do not make different unit classes.

Everything should be components.

---

# Selection

Support

Single click

Drag selection

Shift selection

Ctrl selection

Double click selection

Select same type

Selection priorities

Selection groups (Ctrl+1)

Recall groups (1)

Idle worker selection

Military selection

Filtering

Marquee selection

Hidden unit handling

Air vs ground filtering

Selection events

---

# Orders

Implement command pattern.

Orders include:

Move

Attack

Attack Move

Patrol

Stop

Hold Position

Gather

Return Resources

Repair

Construct

Train

Research

Use Ability

Escort

Follow

Load

Unload

Unload All

Self Destruct

Custom orders

Orders should be queueable.

Shift queue.

Priority queue.

Cancelable.

Serializable.

Replayable.

---

# Movement

Support:

Grid movement

NavMesh

Flow fields

A*

Hierarchical pathfinding

Dynamic obstacle avoidance

Formation movement

Group movement

Unit spacing

Collision avoidance

Large unit support

Different terrain costs

Terrain restrictions

Flying units

Water units

Transport units

Movement modifiers

Speed buffs

Path recalculation

Waypoint queues

Formation preservation

---

# Combat

Support

Melee

Ranged

Projectile

Instant

AOE

Splash

Chain attacks

Damage over time

Armor types

Damage types

Critical hits

Friendly fire

Miss chance

Minimum range

Maximum range

Line of sight

Attack windup

Cooldowns

Attack animation timing

Projectile interception

Projectile lifetime

Attack priorities

Target filters

Target acquisition

Auto attack

Focus fire

Retaliation

---

# Weapons

Weapon definitions should include

Damage

Range

Cooldown

Projectile

Effects

Animation

Target types

Accuracy

AOE

Damage falloff

Critical chance

Projectile speed

Weapon upgrades

---

# Resources

Support

Gold

Wood

Food

Stone

Mana

Oil

Energy

Population

Supply

Custom resources

Gathering

Storage

Depositing

Income over time

Resource generators

Finite resources

Infinite resources

Regeneration

Shared team resources

Per-player resources

---

# Workers

Workers should support

Harvest

Repair

Build

Return cargo

Idle

Auto task

Smart task

Multiple resource types

Carry capacity

Gather upgrades

Repair upgrades

---

# Buildings

Support

Placement

Ghost placement

Grid snapping

Terrain validation

Rotation

Construction progress

Builders

Multiple builders

Power requirements

Adjacency bonuses

Production queues

Rally points

Garrison

Turrets

Storage

Destruction

Collapse

---

# Construction

Support

Blueprints

Construction stages

Repair

Cancellation

Refund

Paused construction

Builder assignment

Auto builders

Instant build cheats

---

# Production

Support

Queues

Multiple queues

Parallel production

Unit production

Research

Hero training

Spawn points

Rally points

Queue cancellation

Queue priorities

---

# Technologies

Support

Research trees

Dependencies

Mutually exclusive research

Faction research

Global upgrades

Unit upgrades

Building upgrades

Weapon upgrades

Armor upgrades

Economy upgrades

Movement upgrades

---

# Abilities

Ability system should support

Active abilities

Passive abilities

Targeted

Ground targeted

Self cast

Aura

Channeling

Cooldown

Charges

Mana

Energy

Custom costs

Status effects

Summons

Teleport

Dash

Heal

Buff

Debuff

Area effects

Projectile abilities

Ability interruption

---

# Status Effects

Support

Buffs

Debuffs

Stacking

Refresh duration

Independent stacks

Movement modifiers

Damage modifiers

DOT

HOT

Silence

Stun

Root

Fear

Taunt

Sleep

Shield

Invulnerability

Custom modifiers

---

# Vision

Support

Fog of war

Shroud

Shared vision

Stealth

Detection

Invisible units

Vision blockers

High ground

Night/day modifiers

Dynamic updates

---

# AI

Support

Worker AI

Combat AI

Target selection

Threat evaluation

Expansion

Economy management

Army production

Base building

Scouting

Attack waves

Retreat

Difficulty levels

Behavior trees

Utility AI

GOAP compatibility

---

# Diplomacy

Support

Teams

Neutral

Enemy

Ally

Shared control

Shared vision

Ceasefire

Custom diplomacy rules

---

# Input

Support

Keyboard

Mouse

Gamepad

Touch

Remappable controls

Camera edge scrolling

Middle mouse drag

Hotkeys

Command cards

Double click actions

---

# Camera

Support

Zoom

Rotate

Pan

Edge scrolling

Follow unit

Jump to alerts

Bounds

Smooth movement

Minimap navigation

---

# Minimap

Support

Camera rectangle

Fog

Unit icons

Building icons

Pings

Navigation

Signals

Alerts

---

# UI

Support

Selection panel

Health bars

Production panel

Ability panel

Tooltip system

Resource bar

Population

Alerts

Notifications

Floating combat text

Command panel

Research UI

Drag queue

---

# Audio

Support

Unit acknowledgements

Attack sounds

Ambient sounds

Music

Spatial audio

Voice lines

UI sounds

Dynamic mixing

---

# Saving

Everything should serialize.

Support

Save

Load

Autosave

Replay

Deterministic replay

Versioning

---

# Multiplayer

Architecture must be multiplayer-friendly.

Avoid direct object references.

Use IDs whenever possible.

Support

Lockstep

Rollback compatibility

Prediction where applicable

Authority abstraction

Replay synchronization

Late join support

---

# Performance

Target

Thousands of units.

Use

Object pooling

Spatial partitioning

Quadtrees

Burst where useful

Jobs where useful

GPU instancing

LOD

Avoid allocations

Avoid LINQ in gameplay loops

Avoid boxing

Cache frequently accessed data

Minimize Update()

---

# Editor Tools

Create custom editor tools for:

Unit database

Ability editor

Research editor

Resource editor

Formation editor

AI editor

Map validation

Dependency graph

Live debugging

---

# Events

Use event-driven architecture.

Examples

EntityCreated

EntityDestroyed

HealthChanged

SelectionChanged

OrderIssued

ResearchCompleted

ConstructionFinished

ResourceChanged

Avoid polling.

---

# Extensibility

Every module should be replaceable.

Movement should be swappable.

Combat should be swappable.

Pathfinding should be swappable.

Input should be swappable.

Networking should be swappable.

No module should know implementation details of another.

Depend only on interfaces.

---

# Testing

Every system should be independently testable.

Prefer pure C# classes over MonoBehaviours.

Simulation should run without rendering.

Rendering should reflect simulation state.

---

# Debugging

Provide debug tools for

Path visualization

Vision visualization

Collision

Orders

AI decisions

Combat

Resources

Influence maps

Heat maps

Navigation

Selection

Formation

Performance

---

# Coding Standards

- Keep classes under ~300 lines when practical.
- Prefer one responsibility per class.
- Prefer immutable configuration objects.
- Avoid magic numbers.
- Every public API should be documented.
- Every complex algorithm should include comments explaining WHY, not WHAT.
- Avoid unnecessary allocations.
- Prefer explicit naming over abbreviations.
- Every new gameplay feature should integrate through existing systems instead of introducing hardcoded special cases.
- Favor generic, reusable solutions over game-specific implementations.

---

# Final Goal

The resulting framework should be capable of supporting:

- Classic RTS
- Sci-fi RTS
- Fantasy RTS
- Modern military RTS
- Tower defense hybrids
- Survival RTS
- Colony simulators
- City builders with RTS control
- Multiplayer RTS
- Single-player RTS

without requiring architectural changes.

When implementing features:

1. Prefer reusable systems.
2. Keep systems decoupled.
3. Favor data over inheritance.
4. Optimize for maintainability first, performance second, and micro-optimizations only when profiling proves necessary.
5. Always consider future extensibility before adding new code.
6. Write production-quality code that can serve as a long-term framework rather than a game-specific implementation.