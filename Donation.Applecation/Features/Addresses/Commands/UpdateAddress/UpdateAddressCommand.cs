using Donation.Application.DTOs.Address.Response;
using MediatR;

namespace Donation.Application.Features.Addresses.Commands.UpdateAddress;

public sealed record UpdateAddressCommand(
    int Id,
    int AreaId,
    string Street,
    string Details,
    double Latitude,
    double Longitude) : IRequest<AddressResponse?>;
