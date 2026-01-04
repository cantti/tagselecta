using Microsoft.Extensions.DependencyInjection;
using TagSelecta.Cli.Commands.TagDataCommandShared.BulkWrite;
using TagSelecta.Cli.Commands.TagDataCommandShared.InteractiveWrite;

namespace TagSelecta.Cli.Commands.TagDataCommandShared;

public static class Registration
{
    public static void AddCommonTagDataServices(this IServiceCollection services)
    {
        services.AddTransient<IUserActionReader, UserActionReader>();
        services.AddTransient<IInteractiveWriter, InteractiveWriter>();
        services.AddTransient<IBulkWriter, BulkWriter>();
    }
}
