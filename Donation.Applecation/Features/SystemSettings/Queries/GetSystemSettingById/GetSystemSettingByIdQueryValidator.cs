using FluentValidation;

namespace Donation.Application.Features.SystemSettings.Queries.GetSystemSettingById;

public sealed class GetSystemSettingByIdQueryValidator : AbstractValidator<GetSystemSettingByIdQuery>
{
    public GetSystemSettingByIdQueryValidator()
    {
        RuleFor(x => x.Id).NotEmpty();
    }
}
