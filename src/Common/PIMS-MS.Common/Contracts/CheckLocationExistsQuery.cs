using MediatR;

namespace PIMS_MS.Common.Contracts;

public record class CheckLocationExistsQuery(Guid Location) : IRequest<bool>;