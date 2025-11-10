namespace TagSelecta.Actions.FileActions;

public interface IFileAction<TSettings>
    where TSettings : BaseSettings
{
    Task ProcessFile(FileActionContext<TSettings> context);
}
