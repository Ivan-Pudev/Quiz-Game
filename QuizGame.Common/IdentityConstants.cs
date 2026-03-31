namespace QuizGame.GCommon
{
    using System;
    using System.Collections.Generic;
    using System.Text;

    public static class IdentityConstants
    {
        public const int FullNameMinLength = 3;
        public const int FullNameMaxLength = 50;

        public const int PasswordMinLength = 6;
        public const int PasswordMaxLength = 15;

        public const int EmailMinLength = 6;
        public const int EmailMaxLength = 20;
        public const string PasswordErrorMessage = "Password must be at least 8 characters long and include uppercase, lowercase, a digit, and a special character.";

    }
}
