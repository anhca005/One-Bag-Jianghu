using UnityEngine;
using UnityEngine.UI;

namespace OneBagJianghu.UI.Common
{
    public enum UiFontRole
    {
        Title,
        Body,
        Emphasis,
        Display
    }

    public static class UiFonts
    {
        private const string ResourceName = "UiFonts";
        private static UiFontRegistry registry;
        public static Font Display => Get(UiFontRole.Display);
        public static Font Title => Get(UiFontRole.Title);
        public static Font Body => Get(UiFontRole.Body);
        public static Font Emphasis => Get(UiFontRole.Emphasis);

        public static Font Get(UiFontRole role)
        {
            if (registry == null)
                registry = Resources.Load<UiFontRegistry>(ResourceName);
            if (registry == null)
            {
                Debug.LogError("UiFonts registry is missing. Rebuild the generated UI assets.");
                return null;
            }

            switch (role)
            {
                case UiFontRole.Display:
                    return registry.display;
                case UiFontRole.Title:
                    return registry.title;
                case UiFontRole.Emphasis:
                    return registry.emphasis;
                default:
                    return registry.body;
            }
        }

        public static void Apply(Text text, UiFontRole role)
        {
            if (text != null)
                text.font = Get(role);
        }

        public static void Reload() => registry = null;
    }
}