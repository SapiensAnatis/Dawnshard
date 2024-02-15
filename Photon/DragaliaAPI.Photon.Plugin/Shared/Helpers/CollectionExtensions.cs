using System;
using System.Collections;
using System.Collections.Generic;
using DragaliaAPI.Photon.Plugin.Shared.Constants;
using Photon.Hive.Plugin;

namespace DragaliaAPI.Photon.Plugin.Shared.Helpers
{
    internal static class CollectionExtensions
    {
        public static int GetInt(this IDictionary<string, string> dictionary, string key)
        {
            if (!dictionary.TryGetValue(key, out string value))
            {
                throw new ArgumentException(
                    $"Dictionary did not contain required key {key}",
                    nameof(dictionary)
                );
            }

            return int.Parse(value);
        }

        public static bool TryGetBool(
            this IDictionary<string, string> dictionary,
            string key,
            out bool result
        )
        {
            result = false;

            return dictionary.TryGetValue(key, out string stringValue)
                && bool.TryParse(stringValue, out result);
        }

        public static long GetLong(this IDictionary<string, string> dictionary, string key)
        {
            if (!dictionary.TryGetValue(key, out string value))
            {
                throw new ArgumentException(
                    $"Dictionary did not contain required key {key}",
                    nameof(dictionary)
                );
            }

            return long.Parse(value);
        }

        public static Uri GetUri(
            this IDictionary<string, string> dictionary,
            string key,
            UriKind kind
        )
        {
            if (!dictionary.TryGetValue(key, out string value))
            {
                throw new ArgumentException(
                    $"Configuration did not contain required key {key}",
                    nameof(dictionary)
                );
            }

            return new Uri(value, kind);
        }

        public static int GetInt(this Hashtable hashtable, string key)
        {
            if (!hashtable.TryGetInt(key, out int value))
            {
                throw new ArgumentException(
                    $"Hashtable did not contain required integer value for key {key}",
                    nameof(key)
                );
            }

            return value;
        }

        public static long GetLong(this Hashtable hashtable, string key)
        {
            if (!hashtable.TryGetLong(key, out long value))
            {
                throw new ArgumentException(
                    $"Hashtable did not contain required integer value for key {key}",
                    nameof(key)
                );
            }

            return value;
        }

        public static bool TryGetValue(this Hashtable hashtable, string key, out object value)
        {
            value = default;

            if (hashtable is null || !hashtable.ContainsKey(key))
                return false;

            value = hashtable[key];
            return true;
        }

        public static bool TryGetInt(this Hashtable hashtable, string key, out int value)
        {
            value = default;

            if (!hashtable.TryGetValue(key, out object objValue))
                return false;

            if (objValue is int intValue)
            {
                value = intValue;
                return true;
            }
            else if (objValue is string stringValue)
            {
                value = int.Parse(stringValue);
                return true;
            }

            return false;
        }

        public static bool TryGetLong(this Hashtable hashtable, string key, out long value)
        {
            value = default;

            if (!hashtable.TryGetValue(key, out object objValue))
                return false;

            if (objValue is long longValue)
            {
                value = longValue;
                return true;
            }
            else if (objValue is string stringValue)
            {
                value = long.Parse(stringValue);
                return true;
            }

            return false;
        }

        public static bool TryGetInt(this PropertyBag<object> properties, string key, out int value)
        {
            value = default;

            if (properties is null || !properties.TryGetValue(key, out object objValue))
                return false;

            if (objValue is int intValue)
            {
                value = intValue;
                return true;
            }
            else if (objValue is string stringValue)
            {
                value = int.Parse(stringValue);
                return true;
            }
            else
            {
                return false;
            }
        }

        public static int GetInt(this PropertyBag<object> properties, string key)
        {
            if (!properties.TryGetInt(key, out int value))
            {
                throw new ArgumentException(
                    $"PropertyBag did not contain required integer value for key {key}",
                    nameof(key)
                );
            }

            return value;
        }

        public static int GetIntOrDefault(this PropertyBag<object> properties, string key)
        {
            if (!properties.TryGetInt(key, out int value))
            {
                return 0;
            }

            return value;
        }

        public static bool TryGetBool(
            this PropertyBag<object> properties,
            string key,
            out bool value
        )
        {
            value = default;

            if (properties is null || !properties.TryGetValue(key, out object objValue))
                return false;

            if (objValue is bool boolValue)
            {
                value = boolValue;
                return true;
            }

            return false;
        }

        public static void InitializeViewerId(this Hashtable actorProperties)
        {
            if (!actorProperties.ContainsKey(ActorPropertyKeys.ViewerId))
            {
                actorProperties.Add(
                    ActorPropertyKeys.ViewerId,
                    actorProperties[ActorPropertyKeys.PlayerId]
                );
            }
        }
    }
}
