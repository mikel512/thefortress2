using System.Collections.Generic;

namespace DataAccessLibrary.Utilities
{
    public static class Pairing
    {
        public static KeyValuePair<string, object> Of(string key, object value)
        {
            return new KeyValuePair<string, object>(key, value);
        }
    }
}