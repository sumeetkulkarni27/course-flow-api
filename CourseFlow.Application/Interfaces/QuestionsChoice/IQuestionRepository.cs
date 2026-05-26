using CourseFlow.Application.DTOs;
using CourseFlow.Domain.Entities;

namespace CourseFlow.Application.Interfaces.QuestionsChoice
{
    public interface IQuestionRepository
    {
        Task<IEnumerable<Question>> GetAllQuestionsAsync();
        Task<Question?> GetQuestionByIdAsync(int id);
        Task<Question> AddQuestionAsync(Question question);
        Task UpdateQuestionAsync(Question question);
        Task DeleteQuestionAsync(Question question);
        Task UpdateQuestionAndChoicesAsync(int id, QuestionDto dto);
    }
}
