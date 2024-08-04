using FluentValidation;

namespace ConsumerPedidos.src.Validators
{
    public class ItemValidator : AbstractValidator<Item>
    {
        public ItemValidator() 
        {
            RuleFor(x => x.Produto)
            .NotEmpty().WithMessage("O produto não pode ser vazio")
            .Length(1, 100).WithMessage("O produto deve ter entre 1 e 100 caracteres");

            RuleFor(x => x.Quantidade)
                .GreaterThan(0).WithMessage("A quantidade não pode ser menor do que zero");

            RuleFor(x => x.Preco)
                .GreaterThan(0).WithMessage("O preço não pode ser menor do que zero");
        }
    }
}
