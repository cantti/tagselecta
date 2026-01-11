namespace TagSelecta.App.TagDataActions.FileCommands;

public abstract class FileAction<TSettings>
    where TSettings : BaseSettings
{
    protected virtual void ProcessFile(IFileActionContext<TSettings> context) { }

    public virtual Task ProcessFileAsync(IFileActionContext<TSettings> context)
    {
        ProcessFile(context);
        return Task.CompletedTask;
    }
}
