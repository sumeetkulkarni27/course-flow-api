using CourseFlow.Application.DTOs;
using CourseFlow.Application.Interfaces.Courses;
using FluentValidation;

namespace CourseFlow.Application.DTOValidations
{
    public class CreateCourseValidator : AbstractValidator<CreateCourseDto>
    {
        public CreateCourseValidator(ICourseRepository repository)
        {
            RuleFor(x => x.Title)
                .NotNull()
                .NotEmpty()
                .MaximumLength(100)
                .MaximumLength(100)
                .MustAsync(async (title, cancellation) =>
                    !await repository.IsTitleDuplicateAsync(title))
                .WithMessage("The course title must be unique.");

            RuleFor(x => x.Description)
               .NotNull()
               .NotEmpty()
              .MaximumLength(500);
        }

    }

}
