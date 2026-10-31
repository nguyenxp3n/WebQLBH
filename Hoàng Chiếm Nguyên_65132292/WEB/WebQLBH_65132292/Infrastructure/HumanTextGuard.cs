using System.Text.RegularExpressions;

namespace DbQLBH_65132292.Infrastructure
{
    public static class HumanTextGuard
    {
        private static readonly Regex MultiWhitespaceRegex = new(@"\s+", RegexOptions.Compiled);

        private static readonly string[] SuspiciousMarkers =
        {
            "{\"",
            "\"model\"",
            "\"prompt\"",
            "\"tokens\"",
            "\"choices\"",
            "http://",
            "https://",
            "<script",
            "</",
            "```",
            "json",
            "payload"
        };

        public static string NormalizeInput(string? value)
        {
            if (string.IsNullOrWhiteSpace(value))
            {
                return string.Empty;
            }

            return MultiWhitespaceRegex.Replace(value.Trim(), " ");
        }

        public static string? NormalizeOptionalInput(string? value)
        {
            var normalized = NormalizeInput(value);
            return normalized.Length == 0 ? null : normalized;
        }

        public static bool LooksLikeMachinePayload(string? value)
        {
            if (string.IsNullOrWhiteSpace(value))
            {
                return false;
            }

            var normalized = NormalizeInput(value);
            var lower = normalized.ToLowerInvariant();

            foreach (var marker in SuspiciousMarkers)
            {
                if (lower.Contains(marker, StringComparison.Ordinal))
                {
                    return true;
                }
            }

            var specialCharacters = 0;
            foreach (var character in normalized)
            {
                if ("{}[]<>|`~\\".Contains(character, StringComparison.Ordinal))
                {
                    specialCharacters++;
                }
            }

            return specialCharacters >= 6 && specialCharacters >= normalized.Length / 8;
        }

        public static string ToSafeAddressDisplay(string? value)
        {
            var normalized = NormalizeInput(value);
            if (normalized.Length == 0 || LooksLikeMachinePayload(normalized))
            {
                return "Địa chỉ chưa cập nhật";
            }

            return normalized;
        }
    }
}