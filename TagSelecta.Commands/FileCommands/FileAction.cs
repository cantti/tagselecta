namespace TagSelecta.Commands.FileCommands;

public abstract class FileAction<TSettings>
    where TSettings : BaseSettings
{
    protected virtual void ProcessFile(FileActionContext<TSettings> context) { }

    public virtual Task ProcessFileAsync(FileActionContext<TSettings> context)
    {
        ProcessFile(context);
        return Task.CompletedTask;
    }
}
