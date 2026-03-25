using Microsoft.EntityFrameworkCore;
using QuizGame.Data.Models;
using QuizGame.Data.Repository.Contracts;

namespace QuizGame.Data.Repository
{
    public class GameRepository : BaseRepository, IGameRepository
    {
        public GameRepository(QuizGameDbContext dbContext) 
            : base(dbContext) { }

        public async Task<QuizAttempt?> GetQuizAttemptWithQuizQuestionAndAnswersByIdAsync(Guid attemptId)
        {
            return await DbContext.QuizAttempts
                .Include(a => a.Quiz)
                    .ThenInclude(q => q.Questions)
                        .ThenInclude(qn => qn.Answers)
                .FirstOrDefaultAsync(a => a.Id == attemptId);
        }

        public async Task<bool> AddQuizAttemptAsync(QuizAttempt attempt)
        {
            await DbContext.QuizAttempts.AddAsync(attempt);
            int resultCount = await SaveChangesAsync();

            return resultCount > 0;
        }

        public async Task<bool> AddAttemptAnswerAsync(AttemptAnswer answer)
        {
            await DbContext.AttemptAnswers.AddAsync(answer);
            int resultCount = await SaveChangesAsync();

            return resultCount > 0;
        }

        public async Task<bool> UpdateAttempAnswersAsync(AttemptAnswer answer)
        {
            DbContext.AttemptAnswers.Update(answer);
            int resultCount = await SaveChangesAsync();

            return resultCount > 0;
        }
    }
}
