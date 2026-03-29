namespace QuizGame.Data.Repository
{
    using Microsoft.EntityFrameworkCore;
    using QuizGame.Data.Models;
    using QuizGame.Data.Repository.Contracts;
    using System;
    using System.Collections.Generic;
    using System.Text;

    public class QuizRepository : BaseRepository, IQuizRepository
    {
        public QuizRepository(QuizGameDbContext dbContext) 
            : base(dbContext) {}

        public async Task<IEnumerable<Question>> GetAllQuestionsOrderByContentAsync()
        {
            return await DbContext
                .Questions
                .AsNoTracking()
                .OrderBy(q => q.Content)
                .ToListAsync();
        }

        public async Task<IEnumerable<Question>> GetQuestionsFromTheirIdsAsync(List<Guid> selectedQuestionsIds)
        {
            return await DbContext
                .Questions
                .Where(q => selectedQuestionsIds
                .Contains(q.Id))
                .ToListAsync();
        }

        public async Task<IEnumerable<Quiz>> GetAllQuizzesWithQuestionAnswersCategoriesAndLeaderboardAsync()
        {
            return await DbContext
                .Quizzes
                .AsNoTracking()
                .Include(q => q.Questions)
                    .ThenInclude(qq => qq.Answers)
                .Include(q => q.Questions)
                    .ThenInclude(qq => qq.Categories)
                .Include(q => q.Leaderboard)
                .AsSplitQuery()
                .ToListAsync();
        }

        public async Task<Quiz?> GetQuizWithQuestionsAnswersCategoriesAndLeaderboardByIdAsync(Guid? id)
        {
            return await DbContext
                .Quizzes
                .AsNoTracking()
                .Include(q => q.Questions)
                    .ThenInclude(qq => qq.Answers)
                .Include(q => q.Questions)
                    .ThenInclude(qq => qq.Categories)
                .Include(q => q.Leaderboard)
                .AsSplitQuery()
                .FirstOrDefaultAsync(q => q.Id == id);
        }

        public async Task<Quiz?> GetQuizWithQuestionsByIdAsync(Guid? id)
        {
            return await DbContext.Quizzes
                .Include(q => q.Questions)
                .FirstOrDefaultAsync(q => q.Id == id);
        }

        public async Task<bool> AddQuizAsync(Quiz quiz)
        {
            await DbContext.Quizzes.AddAsync(quiz);
            int resultCount = await SaveChangesAsync();

            return resultCount > 0;
        }

        public async Task<bool> UpdateQuizAsync(Quiz quiz)
        {
            DbContext.Quizzes.Update(quiz);
            int resultCount = await SaveChangesAsync();

            return resultCount > 0;
        }

        public async Task<bool> HardDeleteQuizAsync(Quiz quiz)
        {
            DbContext.Quizzes.Remove(quiz);
            int resultCount = await SaveChangesAsync();

            return resultCount > 0;
        }

        
    }
}
