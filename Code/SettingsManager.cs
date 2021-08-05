using ShopWebData;
using System.Collections.Generic;
using System.Linq;

namespace ShopWebApp.Code
{
    public static class SettingsManager
    {
        static Dictionary<string, string> Dictionary = new Dictionary<string, string>();
        static int CallsCount = 0;
        static void RefreshValues()
        {
            Dictionary = new ShopWebContext().Settings.ToDictionary(s => s.Key, s => s.Value);
        }
        public static void StartCall()
        {
            if(CallsCount >= 3)
            {
                RefreshValues();
                CallsCount = 0;
            }
            else
            {
                CallsCount++;
            }
        }
        public static string GetValue(string Key)
        {
            if(Dictionary.Count == 0)
            {
                RefreshValues();
            }
            if (CallsCount == 0)
            {
                RefreshValues();
            }

            bool ValueFound = Dictionary.TryGetValue(Key, out string Value);
            if (ValueFound)
            {
                return Value;
            }
            else
            {
                return null;
            }
        }
    }
}
