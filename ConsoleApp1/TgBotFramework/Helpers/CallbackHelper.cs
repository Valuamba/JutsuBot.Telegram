using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace JutsuForms.Server.TgBotFramework.Helpers
{
    public static class CallbackHelper 
    {
        public static bool IsCallbackCommand(this string callback, string expectedCallback)
        {
            if (expectedCallback is null)
                return false;

            var callbackParameters = DecodeCallbackDataParameters(callback);
            var expectedCallbackParameters = DecodeCallbackDataParameters(expectedCallback);

            var callbackCommand = GetCallbackCommand(callback);
            var expectedCallbackCommand = GetCallbackCommand(expectedCallback);

            return callbackCommand == expectedCallbackCommand
                && callbackParameters.Count == expectedCallbackParameters.Count
                && !callbackParameters.Keys.Except(expectedCallbackParameters.Keys).Any();
        }

        private static string GetCallbackCommand(string callback)
        {
            return callback?
                .Split('?')
                .ElementAtOrDefault(0);
        }

        private static Dictionary<string, string> DecodeCallbackDataParameters(this string callback)
        {
            if (callback == null)
                throw new ArgumentNullException("uri");

            if (callback.Length == 0)
                return new Dictionary<string, string>();

            return callback
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
