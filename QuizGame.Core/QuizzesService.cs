using Microsoft.EntityFrameworkCore;
using QuizGame.Core.Contracts;
using QuizGame.Data;
using QuizGame.Data.Models;
using QuizGame.ViewModels;
using QuizGame.ViewModels.Leaderboards;
using QuizGame.ViewModels.Quizzes;
using System;
using System.Collections.Generic;
using System.Text;

namespace QuizGame.Core
{
    public class QuizzesService : IQuizzesService
    {
        private readonly QuizGameDbContext _dbContext;
        public QuizzesService(QuizGameDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<CreateQuizViewModel> CreateQuizFormAsync()
        {
            IEnumerable<Question> allQuestions = await GetAllQuestionsAsync();

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

            await AddSelectedQuestions(newQuiz,viewModel.SelectedQuestionIds);
            await CreateLeaderboardAsync(newQuiz.Id);
        }
        public async Task AddSelectedQuestions(Quiz selectedQuiz,List<int> selectedIds)
        {
            List<Question> selectedQuestions = await GetQuestionsFromTheirIdsAsync(selectedIds);

            foreach (Question q in selectedQuestions)
            {
                selectedQuiz.Questions.Add(q);
            }

            await _dbContext.Quizzes.AddAsync(selectedQuiz);
            await _dbContext.SaveChangesAsync();
        }
        public async Task<IEnumerable<Question>> GetAllQuestionsAsync()
        {
            return await _dbContext
                .Questions
                .AsNoTracking()
                .OrderBy(q=>q.Content)
                .ToListAsync();
        }

        public async Task<IEnumerable<Quiz>> GetAllQuizzesAsync()
        {
            return await _dbContext
                .Quizzes
                .AsNoTracking()
                .Include(q => q.Questions)
                    .ThenInclude(qq => qq.Answers)
                .Include(q => q.Questions)
                    .ThenInclude(qq => qq.Categories)
                .Include(q=>q.Leaderboard)
                .ToListAsync();
        }

        public async Task<Quiz?> GetQuizByIdAsync(int? id)
        {
            return await _dbContext
                .Quizzes
                .AsNoTracking()
                .Include(q => q.Questions)
                    .ThenInclude(qq => qq.Answers)
                .Include(q => q.Questions)
                    .ThenInclude(qq => qq.Categories)
                .Include(q=>q.Leaderboard)
                .FirstOrDefaultAsync(q => q.Id == id);

        }

        public async Task<List<Question>> GetQuestionsFromTheirIdsAsync(List<int> selectedQuestionsIds)
        {
            return await _dbContext
                .Questions
                .Where(q => selectedQuestionsIds
                .Contains(q.Id))
                .ToListAsync();
        }

        public async Task<List<Answer>> GetAllAnswers()
        {
            return await 
                _dbContext
                .Answers
                .AsNoTracking()
                .ToListAsync();
        }
        public async Task<Leaderboard?> GetLeaderboardByQuizIdAsync(int quizId)
        {
            return await _dbContext.Leaderboards
                .AsNoTracking()
                .Include(l => l.Entries)
                    .ThenInclude(e => e.User)
                .FirstOrDefaultAsync(l => l.QuizId == quizId);
        }

        public async Task<List<LeaderboardRowVm>?> GetLeaderboardEntriesByIdAsync(int id)
        {
            var entries = await _dbContext.LeaderboardEntries
                .AsNoTracking()
                .Where(e => e.LeaderboardId == id)
                .Include(e => e.User)
                .OrderByDescending(e => e.Score)
                .ToListAsync();

            return entries
                .Select((e, index) => new LeaderboardRowVm
                {
                    Rank = index + 1,
                    UserName = e.User?.UserName ?? "(unknown)",
                    Score = e.Score
                })
                .ToList();
        }

        public async Task<List<LeaderboardRowVm>?> GetLeaderboardEntriesByQuizIdAsync(int quizId)
        {
            var entries = await _dbContext.LeaderboardEntries
                 .AsNoTracking()
                 .Include(le=>le.Leaderboard)
                 .Where(e => e.Leaderboard.QuizId == quizId)
                 .Include(e => e.User)
                 .OrderByDescending(e => e.Score)
                 .ToListAsync();

            return entries
                .Select((e, index) => new LeaderboardRowVm
                {
                    Rank = index + 1,
                    UserName = e.User?.UserName ?? "(unknown)",
                    Score = e.Score
                })
                .ToList();
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
                Categories = quizModel.Questions.Select(q=>q.Categories).ToList(),  
            };

            return viewModel;
        }

        public async Task<EditQuizViewModel> EditQuizGetDataFromForm(Quiz quizModel)
        {
            IEnumerable<Question> allQuestions = await GetAllQuestionsAsync();

            List<int> selectedIds = quizModel.Questions.Select(q => q.Id).ToList();

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

        public async Task UpdateQuizAsync(EditQuizViewModel viewModel,List<int> selectedQuestionId)
        {

            var quiz = await _dbContext.Quizzes
                .Include(q => q.Questions)
                .FirstOrDefaultAsync(q => q.Id == viewModel.Id);

            if (quiz == null)
                throw new InvalidOperationException("Quiz not found");

            quiz.Title = viewModel.Title;
            quiz.Description = viewModel.Description;
            quiz.StartTime = viewModel.StartTime;

            quiz.Questions.Clear(); 

            if (selectedQuestionId.Count > 0)
            {
                var questions = await _dbContext.Questions
                    .Where(q => selectedQuestionId.Contains(q.Id))
                    .ToListAsync();

                foreach (var q in questions)
                    quiz.Questions.Add(q);
            }

            await _dbContext.SaveChangesAsync();
        }
        
        public async Task DeleteQuizAsync(int id)
        {
            var quiz = await _dbContext.Quizzes
                .Include(q => q.Questions)
                .FirstOrDefaultAsync(q => q.Id == id);

            if (quiz == null)
                throw new InvalidOperationException("Quiz not found.");

            quiz.Questions.Clear();

            _dbContext.Quizzes.Remove(quiz);
            await _dbContext.SaveChangesAsync();
        }

        public async Task<Leaderboard> CreateLeaderboardAsync(int quizId)
        {
            var leaderboard = await _dbContext.Leaderboards
                .Include(l => l.Entries)
                .FirstOrDefaultAsync(l => l.QuizId == quizId);

            if (leaderboard != null) return leaderboard;

            Quiz? quiz = await _dbContext.Quizzes.FirstOrDefaultAsync(q => q.Id == quizId);

            leaderboard = new Leaderboard
            {
                QuizId = quizId,
                Title = quiz!.Title,
                Description = quiz.Description,
                LastUpdated = DateOnly.FromDateTime(DateTime.UtcNow)
            };

            await _dbContext.Leaderboards.AddAsync(leaderboard);
            await _dbContext.SaveChangesAsync();

            return leaderboard;
        }

        public async Task SubmitScoreAsync(int quizId, string userId, int score)
        {
            var leaderboard = await CreateLeaderboardAsync(quizId);

            LeaderboardEntry? entry = await _dbContext.LeaderboardEntries
                .FirstOrDefaultAsync(e => e.Id == leaderboard.Id && e.UserId == userId);

            if (entry == null)
            {
                entry = new LeaderboardEntry
                {
                    LeaderboardId = leaderboard.Id,
                    UserId = userId,
                    Score = score,
                };
                await _dbContext.LeaderboardEntries.AddAsync(entry);
            }
            else
            {
                if (score > entry.Score)
                    entry.Score = score;
            }

            leaderboard.LastUpdated = DateOnly.FromDateTime(DateTime.UtcNow);

            await RecalculateRanksAsync(entry.LeaderboardId);
        }

        private async Task RecalculateRanksAsync(int leaderboardId)
        {
            var entries = await _dbContext.LeaderboardEntries
                .Where(e => e.LeaderboardId == leaderboardId)
                .OrderByDescending(e => e.Score)
                .ThenBy(e => e.Id)
                .ToListAsync();

            for (int i = 0; i < entries.Count; i++)
                entries[i].Rank = i + 1;

            await _dbContext.SaveChangesAsync();
        }
    }
}
