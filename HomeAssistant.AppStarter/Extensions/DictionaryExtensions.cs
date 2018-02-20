using System.Collections.Generic;
using System.Text.RegularExpressions;
using Rille.Hass.AppStarter;

// ReSharper disable once CheckNamespace
internal static class DictionaryExtensions
{
    /// <summary>
    /// Find apps matching the entity id. App-keys can have wildcard: *
    /// </summary>
    internal static List<IHassApp>  FindApps(this Dictionary<string, List<IHassApp>> apps, string entityId)
    {
        var matchedApps = new List<IHassApp>();
        foreach (var entry in apps)
        {
            // entityId "lights.home_1"
            // key: "lights.home*"

            if (Regex.IsMatch(entityId, entry.Key.WildcardToRegex()))
                matchedApps.AddRange(entry.Value);
        }
        return matchedApps;
    }

    internal static string WildcardToRegex(this string text)
    {
        return "^" + Regex.Escape(text).Replace("\\*", ".*") + "$";
    }
}
