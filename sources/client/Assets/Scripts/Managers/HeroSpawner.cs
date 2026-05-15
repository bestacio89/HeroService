using MobaPrototype.AI;
using MobaPrototype.Systems.Heroes;
using System;
using System.Collections.Generic;
using UI;
using UnityEngine;
using Player;
using HeroLoader = MobaPrototype.Player.HeroLoader;
using HeroSelector = MobaPrototype.Player.HeroSelector;
using HeroSkillSlot = MobaPrototype.Player.HeroSkillSlot;

namespace MobaPrototype.Managers
{
    [Serializable]
    /// <summary>
    /// Décrit les paramètres de spawn d'un héros dans la scène de match :
    /// définition runtime, point de départ, équipe et statut local/NPC.
    /// </summary>
    public class HeroSpawnConfig
    {
        public HeroDefinition heroDefinition;
        public Transform spawnPoint;
        public TeamId team;
        public bool isLocalPlayer;
    }

    [Serializable]
    /// <summary>
    /// Définit le mapping entre un identifiant de sélection pré-match
    /// et une HeroDefinition runtime utilisée par HeroSpawner.
    /// Ce pont permet de conserver une UI MVP simple tout en alimentant le runtime réel.
    /// </summary>
    public class HeroSelectionMapping
    {
        public string selectionHeroId;
        public HeroDefinition heroDefinition;
    }

    /// <summary>
    /// Orchestre le spawn des héros, le wiring runtime et l'initialisation du match.
    /// En V16, ce composant sait aussi appliquer un MatchLaunchContext confirmé
    /// pour reconfigurer le héros local à partir du choix effectué dans le flow pré-match.
    /// </summary>
    public class HeroSpawner : MonoBehaviour
    {
        [Header("Prefab générique")]
        [SerializeField] private GameObject heroPrefab;

        [Header("Configuration des 10 héros")]
        [SerializeField] private List<HeroSpawnConfig> spawnConfigs = new();

        [Header("PreMatch Runtime Mapping")]
        [SerializeField] private List<HeroSelectionMapping> localHeroMappings = new();

        [Header("References")]
        [SerializeField] private TeamManager teamManager;
        [SerializeField] private GameManager gameManager;

        [Header("Scene References")]
        [SerializeField] private Transform respawnPoint;
        [SerializeField] private VirtualJoystick joystick;
        [SerializeField] private PlayerCombatInputUI combatUI;

        private readonly List<GameObject> spawnedHeroes = new();
        private GameObject localHeroInstance;

        private void Awake()
        {
            if (teamManager == null)
#pragma warning disable CS0618
                teamManager = FindObjectOfType<TeamManager>();
#pragma warning restore CS0618

            if (gameManager == null)
#pragma warning disable CS0618
                gameManager = FindObjectOfType<GameManager>(true);
#pragma warning restore CS0618

            if (combatUI == null)
                Debug.LogWarning("[HeroSpawner] PlayerCombatInputUI non assigné dans l'Inspector.", this);
        }

        private void Start()
        {
            SpawnAllHeroes();
        }

        private void SpawnAllHeroes()
        {
            foreach (HeroSpawnConfig config in spawnConfigs)
            {
                if (config == null || config.heroDefinition == null || config.spawnPoint == null)
                {
                    Debug.LogWarning("[HeroSpawner] Config incomplète — héros ignoré.", this);
                    continue;
                }

                try
                {
                    SpawnHero(config);
                }
                catch (Exception e)
                {
                    Debug.LogError($"[HeroSpawner] Exception : {e.Message}\n{e.StackTrace}", this);
                }
            }

            if (gameManager != null)
                gameManager.InitializeMatch(spawnedHeroes, localHeroInstance);
            else
                Debug.LogWarning("[HeroSpawner] GameManager introuvable — match non initialisé.", this);
        }

        private void SpawnHero(HeroSpawnConfig config)
        {
            Vector3 targetPos = config.spawnPoint.position;

            GameObject hero = Instantiate(heroPrefab);
            hero.SetActive(false);

            CharacterController cc = hero.GetComponent<CharacterController>();
            if (cc != null)
                cc.enabled = false;

            hero.transform.position = targetPos;
            hero.transform.rotation = Quaternion.identity;

            HeroRuntime heroRuntime = hero.GetComponent<HeroRuntime>();
            if (heroRuntime != null)
            {
                heroRuntime.SetHeroDefinition(config.heroDefinition);
                heroRuntime.SetTeam(config.team);
            }

            hero.name = $"{config.heroDefinition.HeroDisplayName}_{config.team}";

            ConfigureHero(hero, config);

            hero.SetActive(true);

            if (cc != null)
            {
                cc.enabled = false;
                hero.transform.position = targetPos;
                cc.enabled = true;
            }

            if (config.isLocalPlayer)
            {
                localHeroInstance = hero;
                hero.tag = "Player";

                PlayerRespawn playerRespawn = hero.GetComponent<PlayerRespawn>();
                if (playerRespawn != null && respawnPoint != null)
                    playerRespawn.SetRespawnPoint(respawnPoint);
                else
                    Debug.LogWarning("[HeroSpawner] RespawnPoint non câblé.", this);

                PlayerMovement movement = hero.GetComponent<PlayerMovement>();
                if (movement != null && joystick != null)
                    movement.SetJoystick(joystick);
                else
                    Debug.LogWarning("[HeroSpawner] Joystick non câblé.", this);

                HeroLoader heroLoader = hero.GetComponent<HeroLoader>();
                if (heroLoader != null)
                {
                    Action onLoaded = null;
                    onLoaded = () =>
                    {
                        heroLoader.OnHeroLoaded -= onLoaded;

                        if (combatUI == null)
                        {
                            Debug.LogWarning("[HeroSpawner] PlayerCombatInputUI non assigné.", this);
                            return;
                        }

                        HeroSkillSlot slotBasic = hero.transform.Find("SlotBasic")?.GetComponent<HeroSkillSlot>();
                        HeroSkillSlot slotSkillOne = hero.transform.Find("SlotSkillOne")?.GetComponent<HeroSkillSlot>();
                        HeroSkillSlot slotSkillTwo = hero.transform.Find("SlotSkillTwo")?.GetComponent<HeroSkillSlot>();
                        HeroSkillSlot slotUltimate = hero.transform.Find("SlotUltimate")?.GetComponent<HeroSkillSlot>();

                        if (slotBasic == null || slotSkillOne == null || slotSkillTwo == null || slotUltimate == null)
                        {
                            Debug.LogWarning("[HeroSpawner] Un ou plusieurs HeroSkillSlot introuvables.", this);
                            return;
                        }

                        combatUI.SetSlots(slotBasic, slotSkillOne, slotSkillTwo, slotUltimate);
                    };

                    heroLoader.OnHeroLoaded += onLoaded;
                }
                else
                {
                    Debug.LogWarning("[HeroSpawner] HeroLoader introuvable.", this);
                }
            }
            else
            {
                ConfigureNpcHero(hero, config);
            }

            teamManager?.RegisterHero(hero, config.team);

            HeroWorldUI worldUI = hero.GetComponentInChildren<HeroWorldUI>();
            if (worldUI != null)
                worldUI.SetTeam(config.team == TeamId.TeamA);

            spawnedHeroes.Add(hero);
        }

        private void ConfigureHero(GameObject hero, HeroSpawnConfig config)
        {
            PlayerMovement movement = hero.GetComponent<PlayerMovement>();
            PlayerTargeting targeting = hero.GetComponent<PlayerTargeting>();
            PlayerRespawn respawn = hero.GetComponent<PlayerRespawn>();
            PlayerBasicAttack basicAttack = hero.GetComponent<PlayerBasicAttack>();
            HeroLoader heroLoader = hero.GetComponent<HeroLoader>();
            HeroSelector heroSelector = hero.GetComponent<HeroSelector>();

            bool isLocal = config.isLocalPlayer;

            if (movement != null) movement.enabled = isLocal;
            if (targeting != null) targeting.enabled = isLocal;
            if (respawn != null) respawn.enabled = isLocal;
            if (basicAttack != null) basicAttack.enabled = isLocal;
            if (heroLoader != null) heroLoader.enabled = isLocal;

            if (heroSelector != null)
                heroSelector.enabled = false;
        }

        private void ConfigureNpcHero(GameObject hero, HeroSpawnConfig config)
        {
            DisableLegacyEnemyScripts(hero);

            NpcHeroAI npcAi = hero.GetComponent<NpcHeroAI>();
            if (npcAi == null)
                npcAi = hero.AddComponent<NpcHeroAI>();

            npcAi.SetAnchorPosition(config.spawnPoint.position);

            NpcHeroRespawn npcRespawn = hero.GetComponent<NpcHeroRespawn>();
            if (npcRespawn == null)
                npcRespawn = hero.AddComponent<NpcHeroRespawn>();

            npcRespawn.SetRespawnPoint(config.spawnPoint);

            if (config.team == TeamId.TeamA)
            {
                npcAi.SetBehaviourMode(NpcBehaviourMode.HoldPosition);
            }
            else
            {
                npcAi.SetBehaviourMode(NpcBehaviourMode.Advance);

                Vector3 teamAFrontline = new Vector3(3f, config.spawnPoint.position.y, 3f);
                npcAi.SetAdvanceTargetPosition(teamAFrontline);
            }
        }

        private void DisableLegacyEnemyScripts(GameObject hero)
        {
            MonoBehaviour[] behaviours = hero.GetComponentsInChildren<MonoBehaviour>(true);

            foreach (MonoBehaviour behaviour in behaviours)
            {
                if (behaviour == null)
                    continue;

                string typeName = behaviour.GetType().Name;

                if (typeName == "EnemyAI" || typeName == "EnemyMovement" || typeName == "EnemyAttack")
                    behaviour.enabled = false;
            }
        }

        private HeroDefinition ResolveHeroDefinitionFromSelection(string selectionHeroId)
        {
            if (string.IsNullOrWhiteSpace(selectionHeroId))
                return null;
            Debug.Log($"[HeroSpawner] Looking for mapping: {selectionHeroId}");
            foreach (HeroSelectionMapping mapping in localHeroMappings)
            {
                Debug.Log($"[HeroSpawner] Checking mapping: {mapping.selectionHeroId}");
                if (mapping == null)
                    continue;

                if (string.Equals(mapping.selectionHeroId, selectionHeroId, StringComparison.OrdinalIgnoreCase))
                    return mapping.heroDefinition;
            }

            return null;
        }

        public bool TryApplyLaunchContext(MatchLaunchContext context)
        {
            if (context == null)
            {
                Debug.LogWarning("[HeroSpawner] Cannot apply launch context: context is null.", this);
                return false;
            }

            if (string.IsNullOrWhiteSpace(context.SelectedHeroId))
            {
                Debug.LogWarning("[HeroSpawner] Cannot apply launch context: SelectedHeroId is empty.", this);
                return false;
            }

            if (localHeroInstance == null)
            {
                Debug.LogWarning("[HeroSpawner] Cannot apply launch context: localHeroInstance is null.", this);
                return false;
            }

            HeroDefinition selectedDefinition = ResolveHeroDefinitionFromSelection(context.SelectedHeroId);
            if (selectedDefinition == null)
            {
                Debug.LogWarning($"[HeroSpawner] No HeroDefinition mapping found for selection id '{context.SelectedHeroId}'.", this);
                return false;
            }

            HeroRuntime heroRuntime = localHeroInstance.GetComponent<HeroRuntime>();
            if (heroRuntime == null)
            {
                Debug.LogWarning("[HeroSpawner] HeroRuntime missing on local hero.", this);
                return false;
            }

            heroRuntime.SetHeroDefinition(selectedDefinition);

            HeroLoader heroLoader = localHeroInstance.GetComponent<HeroLoader>();
            if (heroLoader != null)
            {
                heroLoader.enabled = false;
                heroLoader.enabled = true;
            }
            else
            {
                Debug.LogWarning("[HeroSpawner] HeroLoader missing on local hero.", this);
            }

            HeroSpawnConfig localConfig = spawnConfigs.Find(config => config != null && config.isLocalPlayer);
            if (localConfig != null)
            {
                localConfig.heroDefinition = selectedDefinition;
                localHeroInstance.name = $"{selectedDefinition.HeroDisplayName}_{localConfig.team}";
            }
            else
            {
                localHeroInstance.name = selectedDefinition.HeroDisplayName;
            }

            Debug.Log(
                $"[HeroSpawner] Launch context applied to local hero | " +
                $"Selection={context.SelectedHeroId} | RuntimeHero={selectedDefinition.HeroId}",
                this
            );

            return true;
        }
    }
}