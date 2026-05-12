namespace TagSelecta.TagDataActions.Abstractions;

public interface ITagDataAction
{
    Task Execute(ITagDataActionExecuteContext context, CancellationToken token);

    Task<bool> BeforeExecute(TagDataActionSettings settings, CancellationToken token);

    FieldNameCompletion FieldNameCompletion => FieldNameCompletion.Disabled;
}

public enum FieldNameCompletion
{
    Disabled,
    Boolean,
    String,
}
