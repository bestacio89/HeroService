using UnityEngine;

/// <summary>
/// Helper pour désérialiser des tableaux JSON avec JsonUtility.
///
/// Problème : JsonUtility.FromJson ne supporte pas les tableaux JSON directs.
///   [{"id":"1"},{"id":"2"}]  ← JsonUtility ne peut pas lire ça directement.
///
/// Solution : wrapper le tableau dans un objet { "items": [...] }
/// avant de le désérialiser, puis extraire le tableau.
///
/// Utilisation :
///   HeroListItemDto[] heroes = JsonHelper.FromJsonArray&lt;HeroListItemDto&gt;(json);
/// </summary>
public static class JsonHelper
{
  /// <summary>
  /// Désérialise un tableau JSON en tableau d'objets typés.
  /// Compatible avec les réponses API qui retournent un tableau JSON direct.
  /// </summary>
  public static T[] FromJsonArray<T>(string json)
  {
    // Wrap le tableau dans un objet pour que JsonUtility puisse le lire
    string wrappedJson = $"{{\"items\":{json}}}";
    var wrapper = JsonUtility.FromJson<JsonArrayWrapper<T>>(wrappedJson);
    return wrapper?.items ?? System.Array.Empty<T>();
  }

  [System.Serializable]
  private class JsonArrayWrapper<T>
  {
    public T[] items;
  }
}