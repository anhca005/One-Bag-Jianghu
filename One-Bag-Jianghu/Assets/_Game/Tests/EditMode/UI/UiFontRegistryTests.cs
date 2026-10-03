using NUnit.Framework;
using OneBagJianghu.UI.Common;
using UnityEditor;
using UnityEngine;

namespace OneBagJianghu.Tests.UI
{
    public sealed class UiFontRegistryTests
    {
        private const string VietnameseSample =
"Nhất Nang Giang Hồ ạảấầẩẫậắằẳẵặẹẻẽếềểễệỉịọỏốồổỗộớờởỡợụủứừửữựỳỵỷỹđơư ≤ •";
        [Test]
        public void RegistryAssignsFourFontsWithVietnameseGlyphCoverage()
        {
            UiFontRegistry registry =
AssetDatabase.LoadAssetAtPath<UiFontRegistry>("Assets/_Game/Resources/UiFonts.asset");
            Assert.That(registry,
Is.Not.Null,
"Rebuild the generated common UI assets.");
            AssertFont(registry.display, "Assets/_Game/Fonts/Charm-Bold.ttf");
            AssertFont(registry.title, "Assets/_Game/Fonts/AlegreyaSC-Bold.ttf");
            AssertFont(registry.body, "Assets/_Game/Fonts/Alegreya-Regular.ttf");
            AssertFont(registry.emphasis, "Assets/_Game/Fonts/Alegreya-Bold.ttf");
        }

        private static void AssertFont(Font font, string expectedAssetPath)
        {
            Assert.That(font,
Is.Not.Null,
expectedAssetPath + " is not assigned in UiFonts.asset.");
            Assert.That(AssetDatabase.GetAssetPath(font), Is.EqualTo(expectedAssetPath));
            foreach (char character in VietnameseSample)
            {
                Assert.That(font.HasCharacter(character),
Is.True,
font.name +
" is missing glyph U+" +
((int)character).ToString("X4") +
" ('" +
character +
"').");
            }
        }
    }
}