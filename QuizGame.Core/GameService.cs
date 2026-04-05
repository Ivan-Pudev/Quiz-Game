namespace QuizGame.Core
{
    using QuizGame.Core.Contracts;
    using QuizGame.Data.Models;
    using QuizGame.Data.Repository.Contracts;
    using QuizGame.ViewModels.Game;
    using System;
    using System.Security.Claims;
    using System.Threading.Tasks;

    public class GameService : IGameService
    {
        private readonly IGameRepository _gameRepository;
        private readonly IQuizRepository _quizRepository;
        private readonly IQuizService _quizService;
        private readonly ILeaderboardRepository _leaderboardRepository;
        public GameService(IQuizRepository quizRepository, ILeaderboardRepository leaderboardRepository,
           IQuizService quizService,IGameRepository gameRepository)
        {
           _quizRepository = quizRepository;
            _leaderboardRepository = leaderboardRepository;
            _quizService = quizService;
            _gameRepository = gameRepository;
        }

        public async Task<Guid> StartAttemptAsync(Guid quizId, ClaimsPrincipal user)
        {
            string userId = user.FindFirstValue(ClaimTypes.NameIdentifier)
                ?? throw new Exception("User not logged in");

            Quiz? quiz = await _quizRepository.GetQuizWithQuestionsByIdAsync(quizId);

            if (quiz == null)
                throw new Exception("Quiz not found");

            int maxScore = quiz.Questions.Sum(q => q.Points);

            QuizAttempt attempt = new QuizAttempt
            {
                QuizId = quizId,
                UserId = Guid.Parse(userId),
                CurrentQuestionIndex = 0,
                Score = 0,
                MaxScore = maxScore,
                IsFinished = false,
            };

            bool isAddSuccessful = await _gameRepository.AddQuizAttemptAsync(attempt);

            if (!isAddSuccessful)
            {
                throw new InvalidOperationException();
            }

            return attempt.Id;
        }

        public async Task<PlayQuestionViewModel?> GetCurrentQuestionAsync(Guid attemptId)
        {
            QuizAttempt? attempt = await _gameRepository
                .GetQuizAttemptWithQuizQuestionsAndAnswersByIdAsync(attemptId);

            if (attempt == null) return null;
            if (attempt.IsFinished) return null;

            List<Question> questions = attempt.Quiz.Questions
                .OrderBy(q => q.Id)
                .ToList();

            if (attempt.CurrentQuestionIndex >= questions.Count)
                return null;

            Question question = questions[attempt.CurrentQuestionIndex];

            return new PlayQuestionViewModel
            {
                AttemptId = attempt.Id,
                QuizId = attempt.QuizId,
                QuestionId = question.Id,
                QuestionContent = question.Content,
                Answers = question.Answers.Select(a => new AnswerVm
                {
                    Id = a.Id,
                    Content = a.Content
                }).ToList(),
            };
        }

        public async Task SubmitAnswerAsync(Guid attemptId, Guid questionId, Guid selectedAnswerId)
        {
            QuizAttempt? attempt = await _gameRepository.GetQuizAttemptWithQuizQuestionsAndAnswersByIdAsync(attemptId);

            if (attempt == null) return;
            if (attempt.IsFinished) return;

            Question? question = attempt.Quiz.Questions.FirstOrDefault(q => q.Id == questionId);
            if (question == null) throw new Exception("Question not found in this quiz");

            Answer? selected = question.Answers.FirstOrDefault(a => a.Id == selectedAnswerId);
            if (selected == null)
                throw new Exception($"Selected answer not found. questionId={questionId}, selectedAnswerId={selectedAnswerId}");

            bool isCorrect = selected.IsCorrect;

            int earned = isCorrect ? question.Points : 0;

            attempt.Score += earned;

            AttemptAnswer newAttemptAnswer = new AttemptAnswer
            {
                QuizAttemptId = attempt.Id,
                QuestionId = questionId,
                SelectedAnswerId = selectedAnswerId,
                IsCorrect = isCorrect,
                EarnedPoints = earned
            };

            bool isAddSuccessful = await _gameRepository.AddAttemptAnswerAsync(newAttemptAnswer);

            if (!isAddSuccessful)
            {
                throw new InvalidOperationException();
            }

            attempt.CurrentQuestionIndex += 1;

            bool isNextQuestion = await _gameRepository.UpdateAttempAnswersAsync(newAttemptAnswer);

            if (!isNextQuestion)
            {
                throw new InvalidOperationException();
            }
        }

        public async Task<GameSummaryViewModel> FinishAttemptAsync(Guid attemptId)
        {
            QuizAttempt attempt = await _gameRepository
                .GetQuizAttemptWithQuizAndAnswersByIdAsync(attemptId)
                ?? throw new Exception("Attempt not found");

            if (!attempt.IsFinished)
            {
                attempt.IsFinished = true;
            }

            await _quizService.SubmitScoreAsync(attempt.QuizId, attempt.UserId, attempt.Score);

            Leaderboard? leaderboard = await _leaderboardRepository.GetLeaderboardWithEntriesAndUserByQuizIdAsync(attempt.QuizId);
            Guid leaderboardId = leaderboard!.Id;

            return new GameSummaryViewModel
            {
                QuizId = attempt.QuizId,
                QuizTitle = attempt.Quiz.Title,
                Score = attempt.Score,
                MaxScore = attempt.MaxScore,
                CorrectAnswers = attempt.Answers.Count(a => a.IsCorrect),
                TotalQuestions = attempt.Answers.Count,
                LeaderboardId = leaderboardId,
            };
        }
    }
}
