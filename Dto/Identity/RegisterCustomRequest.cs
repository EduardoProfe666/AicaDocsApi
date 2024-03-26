using AicaDocsApi.Database;

namespace AicaDocsApi.Dto.Identity;

public class RegisterCustomRequest
{
    public required string Email { get; init; }
    public required string Password { get; init; }
    public required string FullName { get; init; }
    public required PossibleRole Role { get; init; }
}