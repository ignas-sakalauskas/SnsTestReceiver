using System.Text;

namespace SnsTestReceiver.Api.Helpers
{
    public static class LogSanitizer
    {
        public static string SanitizeForLog(this string value)
        {
            if (string.IsNullOrEmpty(value))
            {
                return value;
            }

            var builder = new StringBuilder(value.Length);

            foreach (var ch in value)
            {
                if (!char.IsControl(ch))
                {
                    builder.Append(ch);
                }
            }

            return builder.ToString();
        }
    }
}
