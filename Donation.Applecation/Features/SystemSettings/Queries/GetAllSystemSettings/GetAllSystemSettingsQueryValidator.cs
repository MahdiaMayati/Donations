using FluentValidation;

namespace Donation.Application.Features.SystemSettings.Queries.GetAllSystemSettings;

public sealed class GetAllSystemSettingsQueryValidator : AbstractValidator<GetAllSystemSettingsQuery>
{
    public GetAllSystemSettingsQueryValidator()
    {
        RuleFor(x => x.Page).GreaterThanOrEqualTo(1);
        RuleFor(x => x.Limit).InclusiveBetween(1, 100);
        RuleFor(x => x.Type).MaximumLength(100).When(x => !string.IsNullOrWhiteSpace(x.Type));
    }
}
