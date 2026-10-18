using NUnit.Framework;
using OneBagJianghu.UI.Popups;

namespace OneBagJianghu.Tests.UI
{
    public sealed class ArenaAccountLogicTests
    {
        [Test]
        public void Validate_RejectsMismatchedRepeatedPassword()
        {
            // Arrange
            const string expected = "Mật khẩu nhập lại chưa khớp";

            // Act
            string error = AccountFormLogic.Validate("kiem_khach", "secret1", "secret2", "A", true);

            // Assert
            Assert.That(error, Is.EqualTo(expected));
        }

        [Test]
        public void ShouldPersistToken_ReturnsFalseWhenRememberIsOff()
        {
            // Arrange
            const bool remember = false;

            // Act
            bool persist = AccountFormLogic.ShouldPersistToken(remember);

            // Assert
            Assert.That(persist, Is.False);
        }
    }
}
