using FluentValidation;
using LotteryApp.RequestDto;

namespace LotteryApp.Validations
{
    public class RequestLotteryTicketValidator : AbstractValidator<RequestLotteryTicket>
    {
        public RequestLotteryTicketValidator()
        {
            RuleFor(x => x.Numbers)
                .NotNull()
                .WithMessage("Numbers cannot be null");

            RuleFor(x => x.Numbers)
                .Must(numbers => numbers != null && numbers.Count > 0)
                .WithMessage("At least one number is required");

            RuleForEach(x => x.Numbers)
                .InclusiveBetween((byte)1, (byte)100)
                .WithMessage("Each number must be between 1 and 100");

            RuleFor(x => x.Numbers)
                .Must(numbers => numbers == null || numbers.GroupBy(n => n)
                .All(g => g.Count() == 1))
                .WithMessage("Duplicated numbers are not allowed");
        }

    }
}
