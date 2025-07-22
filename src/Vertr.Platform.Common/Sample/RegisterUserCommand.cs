namespace Vertr.Platform.Common.Sample;

public record RegisterUserCommand(string FirstName, string LastName) : ICommand<Guid>;
