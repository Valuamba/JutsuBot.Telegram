using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;

namespace JutsuForms.Server.TgBotFramework.Helpers
{
    public static class StageHelper
    {
        public static bool IsStage(this string stage, string expectedStage)
        {
            if (expectedStage is null)
                return false;

            return stage.Split('?').ElementAtOrDefault(0)?.Equals(expectedStage.Split('?').ElementAtOrDefault(0)) ?? false;
        }

        public static string AddParameter(this string stage, string parameterName, object value)
        {
            var decodedParameters = DecodeQueryParameters(stage) ?? new Dictionary<string, string>();

            if (decodedParameters is not null)
            {
                if (decodedParameters.ContainsKey(parameterName))
                {
                    decodedParameters[parameterName] = value.ToString();
                }
                else
                {
                    decodedParameters.Add(parameterName, value.ToString());
                }
            }

           return MergeStageWithParameters(stage, decodedParameters);
        }

        private static string MergeStageWithParameters(string stage, Dictionary<string, string> parameters)
        {
            var stageName = GetStageName(stage);

            return $"{stageName}?{string.Join('&', parameters.Select(d => $"{d.Key}:{d.Value}"))}";
        }

        private static string GetStageName(string stage)
        {
            return stage
                .Split('?')
                .ElementAtOrDefault(0);
        }

        public static T GetParameter<T>(this string stage, string parameter)
        {
            var decodedParameters = DecodeQueryParameters(stage);
            var stringValue = decodedParameters[parameter];

            return TConverter.ChangeType<T>(stringValue);
        }

        public static Dictionary<string, string> DecodeQueryParameters(this string stage)
        {
            if (stage == null)
                throw new ArgumentNullException("uri");

            if (stage.Length == 0)
                return new Dictionary<string, string>();

            return stage
                .Split('?')
                .ElementAtOrDefault(1)?
                .TrimStart('?')
                .Split(new[] { '&', ';' }, StringSplitOptions.RemoveEmptyEntries)
                .Select(parameter => parameter.Split(new[] { '=' }, StringSplitOptions.RemoveEmptyEntries))
                .GroupBy(parts => parts[0],
                         parts => parts.Length > 2 ? string.Join("=", parts, 1, parts.Length - 1) : (parts.Length > 1 ? parts[1] : ""))
                .ToDictionary(grouping => grouping.Key,
                              grouping => string.Join(",", grouping));
        }
    }
}
