using Donation.Application.DTOs.Address.Response;
using MediatR;

namespace Donation.Application.Features.Addresses.Commands.CreateAddress;

public sealed record CreateAddressCommand(
    int AreaId,
    string Street,
    string Details,
    double Latitude,
    double Longitude) : IRequest<AddressResponse>;
