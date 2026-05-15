using System.Collections.Generic;
using MobaPrototype.Managers;
using MobaPrototype.Systems.Health;
using MobaPrototype.Systems.Heroes;
using UnityEngine;

namespace MobaPrototype.UI
{
    /// <summary>
    /// Crée et met à jour les marqueurs de la minimap à partir du GameManager.
    /// V1.1 : lisibilité améliorée, clamp, rotation du joueur local.
    /// </summary>
    public class MinimapPresenter : MonoBehaviour
    {
        [Header("References")]
        [SerializeField] private GameManager gameManager;
        [SerializeField] private MinimapBounds minimapBounds;
        [SerializeField] private RectTransform markersRoot;

        [Header("Marker Prefabs")]
        [SerializeField] private MinimapMarkerView localPlayerMarkerPrefab;
        [SerializeField] private MinimapMarkerView allyMarkerPrefab;
        [SerializeField] private MinimapMarkerView enemyMarkerPrefab;

        [Header("Marker Sizes")]
        [SerializeField] private float localPlayerMarkerSize = 14f;
        [SerializeField] private float allyMarkerSize = 10f;
        [SerializeField] private float enemyMarkerSize = 10f;

        [Header("Optional Styling")]
        [SerializeField] private bool overrideMarkerColors = false;
        [SerializeField] private Color localPlayerColor = Color.green;
        [SerializeField] private Color allyColor = Color.blue;
        [SerializeField] private Color enemyColor = Color.red;

        [Header("Behaviour")]
        [SerializeField] private bool hideDeadHeroes = true;
        [SerializeField] private bool clampMarkersInsideMinimap = true;
        [SerializeField] private float clampPadding = 6f;
        [SerializeField] private bool rotateLocalPlayerMarker = true;

        private readonly Dictionary<GameObject, MinimapMarkerView> markerByHero = new();
        private HeroRuntime localHeroRuntime;
        private bool isInitialized;

        private void Awake()
        {
            if (gameManager == null)
                gameManager = GameManager.Instance;
        }

        private void OnEnable()
        {
            if (gameManager == null)
                gameManager = GameManager.Instance;

            if (gameManager != null)
            {
                gameManager.OnMatchStarted += HandleMatchStarted;
                gameManager.OnMatchEnded += HandleMatchEnded;
            }
        }

        private void OnDisable()
        {
            if (gameManager != null)
            {
                gameManager.OnMatchStarted -= HandleMatchStarted;
                gameManager.OnMatchEnded -= HandleMatchEnded;
            }
        }

        private void Start()
        {
            TryInitialize();
        }

        private void Update()
        {
            if (!isInitialized)
            {
                TryInitialize();
                return;
            }

            if (NeedsRebuild())
                RebuildMarkers();

            UpdateMarkerPositions();
        }

        private void HandleMatchStarted()
        {
            TryInitialize(forceRebuild: true);
        }

        private void HandleMatchEnded(MatchResult result)
        {
            UpdateMarkerPositions();
        }

        private void TryInitialize(bool forceRebuild = false)
        {
            if (gameManager == null || minimapBounds == null || markersRoot == null)
                return;

            if (!gameManager.IsMatchReady())
                return;

            GameObject localHeroObject = gameManager.LocalHero;
            if (localHeroObject == null)
                return;

            localHeroRuntime = localHeroObject.GetComponent<HeroRuntime>();
            if (localHeroRuntime == null)
                return;

            if (!isInitialized || forceRebuild)
            {
                RebuildMarkers();
                isInitialized = true;
            }
        }

        private bool NeedsRebuild()
        {
            if (gameManager == null)
                return false;

            if (markerByHero.Count != gameManager.AllHeroes.Count)
                return true;

            foreach (GameObject hero in gameManager.AllHeroes)
            {
                if (hero == null || !markerByHero.ContainsKey(hero))
                    return true;
            }

            return false;
        }

        private void RebuildMarkers()
        {
            ClearMarkers();

            if (gameManager == null || localHeroRuntime == null)
                return;

            foreach (GameObject heroObject in gameManager.AllHeroes)
            {
                if (heroObject == null)
                    continue;

                HeroRuntime heroRuntime = heroObject.GetComponent<HeroRuntime>();
                if (heroRuntime == null)
                    continue;

                MinimapMarkerView prefab = ResolvePrefab(heroObject, heroRuntime);
                if (prefab == null)
                    continue;

                MinimapMarkerView marker = Instantiate(prefab, markersRoot);
                marker.name = $"MinimapMarker_{heroObject.name}";

                ApplyMarkerSize(marker, heroObject, heroRuntime);

                if (overrideMarkerColors)
                    ApplyColorOverride(marker, heroObject, heroRuntime);

                markerByHero[heroObject] = marker;
            }

            UpdateMarkerPositions();
        }

        private MinimapMarkerView ResolvePrefab(GameObject heroObject, HeroRuntime heroRuntime)
        {
            if (heroObject == gameManager.LocalHero)
                return localPlayerMarkerPrefab;

            bool sameTeam = heroRuntime.Team == localHeroRuntime.Team;
            return sameTeam ? allyMarkerPrefab : enemyMarkerPrefab;
        }

        private void ApplyMarkerSize(MinimapMarkerView marker, GameObject heroObject, HeroRuntime heroRuntime)
        {
            if (marker == null)
                return;

            if (heroObject == gameManager.LocalHero)
            {
                marker.SetSize(localPlayerMarkerSize);
                return;
            }

            bool sameTeam = heroRuntime.Team == localHeroRuntime.Team;
            marker.SetSize(sameTeam ? allyMarkerSize : enemyMarkerSize);
        }

        private void ApplyColorOverride(MinimapMarkerView marker, GameObject heroObject, HeroRuntime heroRuntime)
        {
            if (heroObject == gameManager.LocalHero)
            {
                marker.SetColor(localPlayerColor);
                return;
            }

            bool sameTeam = heroRuntime.Team == localHeroRuntime.Team;
            marker.SetColor(sameTeam ? allyColor : enemyColor);
        }

        private void UpdateMarkerPositions()
        {
            if (markersRoot == null || minimapBounds == null)
                return;

            Rect rect = markersRoot.rect;

            foreach (KeyValuePair<GameObject, MinimapMarkerView> pair in markerByHero)
            {
                GameObject heroObject = pair.Key;
                MinimapMarkerView marker = pair.Value;

                if (heroObject == null || marker == null)
                    continue;

                bool visible = ShouldDisplayHero(heroObject);
                marker.SetVisible(visible);

                if (!visible)
                    continue;

                Vector2 normalized = minimapBounds.WorldToNormalized(heroObject.transform.position);

                float anchoredX = (normalized.x - 0.5f) * rect.width;
                float anchoredY = (normalized.y - 0.5f) * rect.height;

                if (clampMarkersInsideMinimap)
                {
                    float halfWidth = rect.width * 0.5f;
                    float halfHeight = rect.height * 0.5f;

                    anchoredX = Mathf.Clamp(anchoredX, -halfWidth + clampPadding, halfWidth - clampPadding);
                    anchoredY = Mathf.Clamp(anchoredY, -halfHeight + clampPadding, halfHeight - clampPadding);
                }

                marker.SetAnchoredPosition(new Vector2(anchoredX, anchoredY));

                if (heroObject == gameManager.LocalHero)
                    UpdateLocalPlayerMarkerRotation(heroObject, marker);
                else
                    marker.SetRotation(0f);
            }
        }

        private void UpdateLocalPlayerMarkerRotation(GameObject heroObject, MinimapMarkerView marker)
        {
            if (!rotateLocalPlayerMarker || heroObject == null || marker == null)
            {
                if (marker != null)
                    marker.SetRotation(0f);

                return;
            }

            Vector3 forward = heroObject.transform.forward;
            Vector2 flatForward = new Vector2(forward.x, forward.z);

            if (flatForward.sqrMagnitude <= 0.0001f)
            {
                marker.SetRotation(0f);
                return;
            }

            float angle = Mathf.Atan2(flatForward.y, flatForward.x) * Mathf.Rad2Deg;

            // Ajustement pour que le sprite "pointe" vers le haut si nécessaire.
            marker.SetRotation(angle - 90f);
        }

        private bool ShouldDisplayHero(GameObject heroObject)
        {
            if (heroObject == null)
                return false;

            if (!heroObject.activeInHierarchy)
                return false;

            if (!hideDeadHeroes)
                return true;

            HealthSystem healthSystem = heroObject.GetComponent<HealthSystem>();
            if (healthSystem == null)
                return true;

            return !healthSystem.IsDead;
        }

        private void ClearMarkers()
        {
            foreach (KeyValuePair<GameObject, MinimapMarkerView> pair in markerByHero)
            {
                if (pair.Value != null)
                    Destroy(pair.Value.gameObject);
            }

            markerByHero.Clear();
        }
    }
}