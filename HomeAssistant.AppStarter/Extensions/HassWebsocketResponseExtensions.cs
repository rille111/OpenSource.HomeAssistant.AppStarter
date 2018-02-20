using Newtonsoft.Json.Linq;

// ReSharper disable once CheckNamespace
internal static class HassWebsocketResponseExtensions
{
    internal static bool IsAuthMessage(this JToken source)
    {
        if (source["type"] == null)
            return false;
        if ( ((string)source["type"]).StartsWith("auth"))
            return true;

        return false;
    }


    internal static bool IsResult(this JToken source)
    {
        if (source["type"] == null)
            return false;
        if ((string)source["type"] == "result")
            return true;

        return false;
    }


    internal static bool IsEvent(this JToken source)
    {
        if (source["type"] == null)
            return false;
        if ((string)source["type"] == "event")
            return true;

        return false;
    }

    internal static bool IsStateChangeEvent(this JToken source)
    {
        if (source["event"]["event_type"] == null)
            return false;
        if ((string) source["event"]["event_type"] == "state_changed")
            return true;

        return false;
    }

    internal static bool IsClickEvent(this JToken source)
    {
        if (source["event"]["event_type"] == null)
            return false;
        if ((string)source["event"]["event_type"] == "click")
            return true;

        return false;
    }

    internal static string ExtractEntityId(this JToken source)
    {
        return (string) source["event"]["data"]["entity_id"];
    }

    internal static bool IsTheMostRelevantStateChangeMessage(this JToken source)
    {
        var entId = source.ExtractEntityId();
        var deeperEntId = "";
        if (source["event"]["data"]["new_state"].HasValues)
        {
            deeperEntId = (string)source["event"]["data"]["new_state"]["entity_id"];
        }
        else if (source["event"]["data"]["old_state"].HasValues)
        {
            deeperEntId = (string)source["event"]["data"]["old_state"]["entity_id"];
        }
        
        // Outer entity_id must be same as inner because otherwise it's a (for us) irrelevant message due to some internal Hass workings. 
        // Example: we can get several messages, one that has entity id: light.mylight and also light.mylight_2. Dunno why.
        return entId == deeperEntId;
    }

    internal static bool HasNewState(this JToken source)
    {
        if (source["event"]["data"]["new_state"] == null)
            return false;
        if (source["event"]["data"]["new_state"].Type == JTokenType.Null)
            return false;

        return true;
    }

    internal static bool HasNewStateWithLastTriggered(this JToken source)
    {
        if (source["event"]["data"]["new_state"]["attributes"]["last_triggered"] == null)
            return false;
        if (source["event"]["data"]["new_state"]["attributes"]["last_triggered"].Type == JTokenType.Null)
            return false;

        return true;
    }
    
}
