using ConsumerPedidos.src.Validators;
using FluentValidation;

public class PedidoValidator : AbstractValidator<Pedido>
{
    public PedidoValidator()
    {
        RuleFor(x => x.CodigoPedido)
            .GreaterThan(0).WithMessage("O código do pedido deve ser maior que zero");

        RuleFor(x => x.CodigoCliente)
            .GreaterThan(0).WithMessage("O código do cliente deve ser maior que zero");

        RuleFor(x => x.Itens)
            .NotEmpty().WithMessage("A lista de itens não pode ser vazia")
            .NotNull().WithMessage("A lista de itens não pode ser nula");
            

        RuleForEach(x => x.Itens)
            .SetValidator(new ItemValidator());
    }
}
