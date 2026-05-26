using CourseFlow.Application.DTOs;

namespace CourseFlow.Application.Interfaces.QuestionsChoice
{
    public interface IChoiceService
    {
        Task<IEnumerable<ChoiceDto>> GetAllChoicesAsync(int questionId);
        Task<ChoiceDto?> GetChoiceByIdAsync(int choiceId);
        Task AddChoiceAsync(CreateChoiceDto dto);
        Task UpdateChoiceAsync(int choiceId, UpdateChoiceDto dto);
        Task UpdateUserChoiceAsync(int choiceId, UpdateUserChoice dto);
        Task DeleteChoiceAsync(int choiceId);
    }

}
