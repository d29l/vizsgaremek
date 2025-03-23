using System.Text.RegularExpressions;

namespace ProjektBackend.PasswordHandler
{
    public class PasswordPolicy
    {
        private static int MinimumLength = 8;
        private static int UpperCaseLength = 1;
        private static int LowerCaseLength = 1;
        private static int NumericLength = 1;

        public static bool IsValid(string password)
        {
            if (string.IsNullOrEmpty(password) || password.Length < MinimumLength)
                return false;

            if (UpperCaseCount(password) < UpperCaseLength)
                return false;

            if (LowerCaseCount(password) < LowerCaseLength)
                return false;

            if (NumericCount(password) < NumericLength)
                return false;


            return true;
        }

        private static int UpperCaseCount(string password)
        {
            return Regex.Matches(password, "[A-Z]").Count;
        }

        private static int LowerCaseCount(string password)
        {
            return Regex.Matches(password, "[a-z]").Count;
        }

        private static int NumericCount(string password)
        {
            return Regex.Matches(password, "[0-9]").Count;
        }
    }

}
