using System.Text.RegularExpressions;
using OneBagJianghu.Net;

namespace OneBagJianghu.UI.Popups
{
    public static class AccountFormLogic
    {
        private static readonly Regex UsernamePattern = new Regex("^[a-z0-9_]{3,20}$");

        public static string Validate(string username,
            string password,
            string repeatedPassword,
            string heroName,
            bool registration)
        {
            if (!UsernamePattern.IsMatch(username ?? string.Empty))
            {
                return OnlineProtocol.LocalizeError("invalid_name");
            }

            int passwordLength = password?.Length ?? 0;
            if (passwordLength < 6 || passwordLength > 64)
            {
                return OnlineProtocol.LocalizeError("weak_password");
            }

            if (registration && password != repeatedPassword)
            {
                return "Mật khẩu nhập lại chưa khớp";
            }

            if (registration && (string.IsNullOrWhiteSpace(heroName) || heroName.Trim().Length > 24))
            {
                return "Tên Hiệp Khách phải có từ 1 đến 24 ký tự";
            }

            return string.Empty;
        }

        public static bool ShouldPersistToken(bool rememberLogin)
        {
            return rememberLogin;
        }
    }
}
