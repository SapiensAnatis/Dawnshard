using System;
using System.Collections;
using System.Collections.Generic;
using Photon.Hive.Plugin;

namespace DragaliaAPI.Photon.Plugin
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

        public static Uri GetUri(
            this IDictionary<string, string> dictionary,
            string key,
            UriKind kind
        )
        {
            if (!dictionary.TryGetValue(key, out string value))
            {
                throw new ArgumentException(
                    $"Dictionary did not contain required key {key}",
                    nameof(dictionary)
                );
            }

            return new Uri(value, kind);
        }

        public static int GetInt(this Hashtable hashtable, string key)
        {
            if (!hashtable.ContainsKey(key))
            {
                throw new ArgumentException(
                    $"Hashtable did not contain required key {key}",
                    nameof(hashtable)
                );
            }

            object value = hashtable[key];

            if (value is int intValue)
            {
                return intValue;
            }
            else if (value is string stringValue)
            {
                return int.Parse(stringValue);
            }
            else
            {
                throw new ArgumentException(
                    $"Could not convert hashtable value {value} under key {key} to int"
                );
            }
        }

        public static bool TryGetInt(this Hashtable hashtable, string key, out int value)
        {
            value = default;

            if (hashtable is null || !hashtable.ContainsKey(key))
            {
                return false;
            }

            object objValue = hashtable[key];

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

        public static bool TryGetInt(this PropertyBag<object> properties, string key, out int value)
        {
            value = default;

            if (properties is null || !properties.TryGetValue(key, out object objValue))
            {
                return false;
            }

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
    }
}
