using UnityEngine;

namespace exiii.Unity
{
    public class RegexAttribute : PropertyAttribute
    {
        public readonly string Pattern;
        public readonly string HelpMessage;

        public RegexAttribute(string pattern, string helpMessage)
        {
            Pattern = pattern;
            HelpMessage = helpMessage;
        }
    }
}