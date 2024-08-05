using FluentValidation;

namespace ConsumerPedidos.src.Validators
{
    public class ItemValidator : AbstractValidator<Item>
    {
        public ItemValidator() 
        {
            RuleFor(x => x.Produto)
            .NotEmpty().WithMessage("O nome do produto não pode ser vazio")
            .DependentRules(() =>
            {
                RuleFor(x => x.Produto)
                .Length(1, 100).WithMessage("O nome do produto deve ter entre 1 e 100 caracteres");

            });

            RuleFor(x => x.Quantidade)
                .GreaterThan(0).WithMessage("A quantidade de produtos no item do pedido não pode ser menor do que zero");

            RuleFor(x => x.Preco)
                .GreaterThan(0).WithMessage("O preço no item do pedido não pode ser menor do que zero");
        }
    }
}
