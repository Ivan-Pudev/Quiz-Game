using Microsoft.EntityFrameworkCore;
using QuizGame.Core.Contracts;
using QuizGame.Data;
using QuizGame.Data.Models;
using QuizGame.Data.Repository.Contracts;
using QuizGame.ViewModels;
using QuizGame.ViewModels.Leaderboards;
using QuizGame.ViewModels.Quizzes;
using System;
using System.Collections.Generic;
using System.Text;

namespace QuizGame.Core
{
    public class QuizService : IQuizService
    {
        private readonly IQuizRepository _quizRepository;
        private readonly ILeaderboardRepository _leaderboardRepository;
        public QuizService(IQuizRepository quizRepository, ILeaderboardRepository leaderboardRepository)
        {
            _quizRepository = quizRepository;
            _leaderboardRepository = leaderboardRepository;
        }

        public async Task<Quiz?> GetQuizByIdAsync(Guid? id)
        {
            return await _quizRepository.GetQuizByIdAsync(id);
        }

        public async Task<IEnumerable<Quiz>> GetAllQuizzesAsync()
        {
            return await _quizRepository.GetAllQuizzesWithQuestionAnswersCategoriesAndLeaderboardAsync();
        }

        public async Task<IEnumerable<Question>> GetAllQuestionsAsync()
        {
            return await _quizRepository.GetAllQuestionsOrderByContentAsync();
        }

        public async Task<CreateQuizViewModel> CreateQuizFormAsync()
        {
            IEnumerable<Question> allQuestions = await _quizRepository.GetAllQuestionsOrderByContentAsync();

            CreateQuizViewModel quizViewModel = new CreateQuizViewModel()
            {
                Questions = allQuestions.ToList()
            };

            return quizViewModel;
        }

        public async Task CreateQuizAsync(CreateQuizViewModel viewModel)
        {
            Quiz newQuiz = new Quiz()
            {
                Title = viewModel.Title,
                Description = viewModel.Description,
                StartTime = viewModel.StartTime,
                Questions = new List<Question>(),
            };

            await AddSelectedQuestions(newQuiz, viewModel.SelectedQuestionIds);
            await CreateLeaderboardAsync(newQuiz.Id);
        }
        public async Task AddSelectedQuestions(Quiz selectedQuiz, List<Guid> selectedIds)
        {
            IEnumerable<Question> selectedQuestions = await _quizRepository
                .GetQuestionsFromTheirIdsAsync(selectedIds);

            selectedQuiz.Questions = selectedQuestions.ToList();

            bool isAddedSuccessful = await _quizRepository.AddQuizAsync(selectedQuiz);

             if (!isAddedSuccessful)
            {
                throw new InvalidOperationException();
            }
        }

        public DetailsQuizViewModel ShowQuizDetails(Quiz quizModel)
        {
            DetailsQuizViewModel viewModel = new DetailsQuizViewModel()
            {
                Id = quizModel.Id,
                Title = quizModel.Title,
                Description = quizModel.Description,
                StartTime = quizModel.StartTime,
                Questions = quizModel.Questions,
                Answers = quizModel.Questions.Select(q => q.Answers).ToList(),
                Categories = quizModel.Questions.Select(q => q.Categories).ToList(),
            };

            return viewModel;
        }

        public async Task<EditQuizViewModel> EditQuizGetDataFromForm(Quiz quizModel)
        {
            IEnumerable<Question> allQuestions = await _quizRepository
                .GetAllQuestionsOrderByContentAsync();

            List<Guid> selectedIds = quizModel.Questions.Select(q => q.Id).ToList();

            EditQuizViewModel viewModel = new EditQuizViewModel()
            {
                Title = quizModel.Title,
                Description = quizModel.Description,
                StartTime = quizModel.StartTime,
                SelectedQuestions = allQuestions
                .Select(q => new QuestionSelectionViewModel()
                {
                    QuestionId = q.Id,
                    Content = q.Content,
                    IsSelected = selectedIds.Contains(q.Id),
                    Points = q.Points,
                })
                .ToList(),
            };

            return viewModel;
        }

        public async Task EditQuizAsync(EditQuizViewModel viewModel, List<Guid> selectedQuestionId)
        {

            Quiz? quiz = await _quizRepository.GetQuizWithQuestionsByIdAsync(viewModel.Id);

            if (quiz == null)
                throw new InvalidOperationException("Quiz not found");

            quiz.Title = viewModel.Title;
            quiz.Description = viewModel.Description;
            quiz.StartTime = viewModel.StartTime;

            if (selectedQuestionId.Count > 0)
            {
                quiz.Questions.Clear();

                IEnumerable<Question> questions = await _quizRepository
                    .GetQuestionsFromTheirIdsAsync(selectedQuestionId);

                quiz.Questions = questions.ToList();

                bool isUpdateSuccessful = await _quizRepository.UpdateQuizAsync(quiz);

                if (!isUpdateSuccessful)
                {
                    throw new InvalidOperationException();
                }

                //bool isUpdateSuccessful;
                //foreach (Question question in questions)
                //{
                //    quiz.Questions.Add(question);

                //    isUpdateSuccessful = await _quizRepository.UpdateQuizAsync(quiz);

                //    if (!isUpdateSuccessful)
                //    {
                //        throw new InvalidOperationException();
                //    }
                //}
            }
        }

        public async Task DeleteQuizAsync(Guid id)
        {
            Quiz? quiz = await _quizRepository
                .GetQuizWithQuestionsByIdAsync(id);

            if (quiz == null)
                throw new InvalidOperationException("Quiz not found.");

            quiz.Questions.Clear();

            bool isDeleteSuccessful = await _quizRepository.HardDeleteQuizAsync(quiz);

            if (!isDeleteSuccessful)
            {
                throw new InvalidOperationException();
            }
        }

        public async Task<Leaderboard> CreateLeaderboardAsync(Guid quizId)
        {
            Leaderboard? leaderboard = await _leaderboardRepository
                .GetLeaderboardsWithEntriesByQuizIdAsync(quizId);

            if (leaderboard != null) return leaderboard;

            Quiz? quiz = await _quizRepository.GetQuizByIdAsync(quizId);

            leaderboard = new Leaderboard
            {
                QuizId = quizId,
                Title = quiz!.Title,
                Description = quiz.Description,
                LastUpdated = DateOnly.FromDateTime(DateTime.UtcNow)
            };

            bool isAddSuccessful = await _leaderboardRepository.AddLeaderboardAsync(leaderboard);

            if (!isAddSuccessful)
            {
                throw new InvalidOperationException();
            }

            return leaderboard;
        }

        public async Task SubmitScoreAsync(Guid quizId, Guid userId, int score)
        {
            var leaderboard = await CreateLeaderboardAsync(quizId);

            LeaderboardEntry? entry = await _leaderboardRepository
                .GetLeaderboardEntryForUserByIdAsync(leaderboard.Id, userId);

            if (entry == null)
            {
                entry = new LeaderboardEntry
                {
                    LeaderboardId = leaderboard.Id,
                    UserId = userId,
                    Score = score,
                };

                bool isAddedSuccessful = await _leaderboardRepository.AddLeaderboardEntryAsync(entry);

                if (!isAddedSuccessful)
                {
                    throw new InvalidOperationException();
                }
            }
            else
            {
                if (score > entry.Score)
                    entry.Score = score;
            }

            leaderboard.LastUpdated = DateOnly.FromDateTime(DateTime.UtcNow);

            await RecalculateRanksAsync(entry.LeaderboardId);
        }

        private async Task RecalculateRanksAsync(Guid leaderboardId)
        {
            List<LeaderboardEntry> entries = await _leaderboardRepository
                .GetLeaderboardEntriesOrderedByScoreByLeaderboardIdAsync(leaderboardId);

            for (int i = 0; i < entries.Count; i++)
            {
                entries[i].Rank = i + 1;

                bool isRankedSuccessful = await _leaderboardRepository.UpdateLeaderboardEntriesAsync(entries[i]);

                if (!isRankedSuccessful)
                {
                    throw new InvalidOperationException();
                }

            }
        }
    }
}
