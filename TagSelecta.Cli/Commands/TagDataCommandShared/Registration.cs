using Microsoft.Extensions.DependencyInjection;

namespace TagSelecta.Cli.Commands.TagDataCommandShared;

public static class Registration
{
    public static void AddCommonTagDataServices(this IServiceCollection services)
    {
        services.AddTransient<IUserActionReader, UserActionReader>();
    }
}
