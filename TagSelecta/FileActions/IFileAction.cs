namespace TagSelecta.FileActions;

public interface IFileAction<TSettings>
    where TSettings : BaseSettings
{
    Task<bool> ProcessTagData(FileActionContext<TSettings> context);
}
