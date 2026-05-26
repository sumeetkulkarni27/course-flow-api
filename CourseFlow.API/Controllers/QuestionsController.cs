using CourseFlow.Application.DTOs;
using CourseFlow.Application.Interfaces.QuestionsChoice;
using Microsoft.AspNetCore.Mvc;

namespace CourseFlow.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class QuestionsController : ControllerBase
    {
        private readonly IQuestionService _service;

        public QuestionsController(IQuestionService service)
        {
            _service = service;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<QuestionDto>>> GetQuestions()
        {
            return Ok(await _service.GetAllQuestionsAsync());
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<QuestionDto>> GetQuestion(int id)
        {
            var question = await _service.GetQuestionByIdAsync(id);
            return question == null ? NotFound() : Ok(question);
        }

        [HttpPost]
        public async Task<IActionResult> CreateQuestion([FromBody] CreateQuestionDto dto)
        {
            await _service.AddQuestionAsync(dto);
            return CreatedAtAction(nameof(GetQuestion), new { id = dto.CourseId }, dto);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateQuestion(int id, [FromBody] UpdateQuestionDto dto)
        {
            await _service.UpdateQuestionAsync(id, dto);
            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteQuestion(int id)
        {
            await _service.DeleteQuestionAsync(id);
            return NoContent();
        }


        [HttpPost("CreateQuestionChoices")]
        public async Task<IActionResult> CreateQuestionChoices([FromBody] QuestionDto dto)
        {
            var createdResource = await _service.AddQuestionAndChoicesAsync(dto);
            return CreatedAtAction(nameof(GetQuestion), new { id = createdResource.QuestionId }, createdResource);
        }

        [HttpPut("UpdateQuestionAndChoices/{id}")]
        public async Task<IActionResult> UpdateQuestionAndChoices(int id, [FromBody] QuestionDto dto)
        {
            await _service.UpdateQuestionAndChoicesAsync(id, dto);
            return NoContent();
        }
    }

}
