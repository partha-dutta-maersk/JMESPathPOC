using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace JsonQueryPOC.Extensions
{
    public static class JsonConvertion
    {
        public static bool TryParseJson(this string? strInput, out JToken jToken)
        {
            jToken = new JObject();
            strInput = strInput?.Trim();
            if (string.IsNullOrWhiteSpace(strInput))
            {
                return false;
            }

            if ((!strInput.StartsWith("{") || !strInput.EndsWith("}")) && (!strInput.StartsWith("[") || !strInput.EndsWith("]")))
            {
                return false;
            }

            try
            {
                jToken = JToken.Parse(strInput);
                return true;
            }
            catch
            {
                return false;
            }
        }

        public static string ToStringIndented(this JToken value)
        {
            return value.ToString(Formatting.Indented);
        }
    }
}
