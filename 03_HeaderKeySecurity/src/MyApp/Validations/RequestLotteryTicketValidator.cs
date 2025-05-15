using FluentValidation;
using LotteryApp.Constants;
using LotteryApp.RequestDto;

namespace LotteryApp.Validations
{
    public class RequestLotteryTicketValidator : AbstractValidator<LotteryRequest>
    {
        public RequestLotteryTicketValidator()
        {
            RuleFor(x => x).NotNull().WithMessage("Request body cannot be null.");

            RuleFor(x => x.Numbers)
                .NotNull()
                .WithMessage("Numbers cannot be null");

            RuleFor(x => x.Numbers)
                .Must(numbers => numbers != null && numbers.Count == LotteryRules.NumberOfBallsToPick)
                .WithMessage($"{LotteryRules.NumberOfBallsToPick} numbers are required");

            RuleForEach(x => x.Numbers)
                .InclusiveBetween(LotteryRules.MinLotteryNumber, LotteryRules.MaxLotteryNumber)
                .WithMessage($"Each number must be between {LotteryRules.MinLotteryNumber} and {LotteryRules.MaxLotteryNumber}");

            RuleFor(x => x.Numbers)
                .Must(numbers => numbers == null || numbers.GroupBy(n => n)
                .All(g => g.Count() == 1))
                .WithMessage("Duplicated numbers are not allowed");
        }

    }
}
