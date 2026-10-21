using FluentValidation;
using SportBook.Core.DTOs;

namespace SportBook.Services.Validators
{
    public class RegistrarUsuarioDtoValidator :
        AbstractValidator<RegistrarUsuarioDto>
    {
        public RegistrarUsuarioDtoValidator()
        {
            RuleFor(x => x.Nombre)
                .NotEmpty()
                .MaximumLength(50);

            RuleFor(x => x.Apellido)
                .NotEmpty()
                .MaximumLength(50);

            RuleFor(x => x.Email)
                .NotEmpty()
                .EmailAddress()
                .MaximumLength(100);

            RuleFor(x => x.Password)
                .NotEmpty()
                .MinimumLength(8)
                .MaximumLength(100);

            RuleFor(x => x.Telefono)
                .MaximumLength(20);
        }
    }
}