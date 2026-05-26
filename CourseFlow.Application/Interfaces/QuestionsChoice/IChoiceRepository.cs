using CourseFlow.Domain.Entities;

namespace CourseFlow.Application.Interfaces.QuestionsChoice
{
    public interface IChoiceRepository
    {
        Task<IEnumerable<Choice>> GetAllChoicesAsync(int questionId);
        Task<Choice?> GetChoiceByIdAsync(int id);
        Task AddChoiceAsync(Choice choice);
        Task UpdateChoiceAsync(Choice choice);
        Task DeleteChoiceAsync(Choice choice);
    }
}
