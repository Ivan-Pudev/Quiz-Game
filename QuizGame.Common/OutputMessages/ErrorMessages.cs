namespace QuizGame.GCommon.OutputMessages
{
    using System;
    using System.Collections.Generic;
    using System.Text;
    public static class ErrorMessages
    {
        public const string ErrorLoad = "Unable to load {0}.";
        public const string ErrorDisplayCreatePage = "Unable to open create {0} page.";
        public const string ErrorCreate = "Failed to create {0}.";
        public const string ErrorLoadDetails = "Unable to load {0} details page.";
        public const string ErrorDisplayEditPage = "Unable to open {0} edit page.";
        public const string ErrorUpdate = "Failed to update {0}.";
        public const string ErrorInvalidId = "Invalid {0} id.";
        public const string ErrorNotFound = "{0} not found.";
        public const string ErrorDelete = "Failed to delete {0}.";
        public const string ErrorLoadDeletedList = "Unable to load deleted {0}.";
        public const string ErrorRestore = "{0} cannot be restored.";
        public const string ErrorSoftDelete = "{0} cannot be removed.";
        public const string ErrorHardDelete = "{0} cannot be deleted.";
        public const string ErrorLoadLeaderboard = "Unable to load leaderboard for quiz.";
    }
}
