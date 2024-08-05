using ConsumerPedidos.src.Validators;
using FluentValidation;

public class PedidoValidator : AbstractValidator<Pedido>
{
    public PedidoValidator()
    {
        RuleFor(x => x.CodigoPedido)
            .GreaterThan(0).WithMessage("O código do pedido não pode ser zero");

        RuleFor(x => x.CodigoCliente)
            .GreaterThan(0).WithMessage("O código do cliente não pode ser zero");

        RuleFor(x => x.Itens).
            Must(itens => itens != null && itens.Any())
            .WithMessage("A lista de itens não pode ser vazia");
            

        RuleForEach(x => x.Itens)
            .SetValidator(new ItemValidator());
    }
}
