using TagSelecta.Shared.Tagging;
using TagSelecta.TagDataActions.Abstractions;

namespace TagSelecta.TagDataActions.ClearExcept;

[TagDataActionInfo(
    "clearexcept",
    AllowRemainingArguments = true,
    FieldNameCompletion = FieldNameCompletion.Boolean
)]
public class ClearExceptAction : ITagDataAction<ClearExceptSettings>
{
    private HashSet<string> _fieldsToKeep = [];

    public Task<bool> BeforeExecute(ClearExceptSettings settings, CancellationToken token)
    {
        _fieldsToKeep = BuildFieldsToKeep(settings);
        return Task.FromResult(true);
    }

    public Task Execute(
        TagDataActionExecuteContext<ClearExceptSettings> context,
        CancellationToken token
    )
    {
        var tagData = context.Target.CurrentTagData;

        foreach (var field in tagData.Fields)
        {
            if (!_fieldsToKeep.Contains(field.Key))
            {
                tagData.SetValue(field.Key, "");
            }
        }

        if (!context.Settings.Picture)
        {
            tagData.ClearPicture();
        }

        context.Target.UpdateTagData(tagData);

        return Task.CompletedTask;
    }

    private static HashSet<string> BuildFieldsToKeep(ClearExceptSettings settings)
    {
        var fieldsToKeep = new HashSet<string>(StringComparer.OrdinalIgnoreCase);

        var settingProperties = typeof(ClearExceptSettings)
            .GetProperties()
            .Where(x => x.PropertyType == typeof(bool))
            .ToDictionary(x => x.Name, x => x, StringComparer.OrdinalIgnoreCase);

        foreach (var field in FieldName.All())
        {
            if ((bool)settingProperties[field].GetValue(settings)!)
            {
                fieldsToKeep.Add(field);
            }
        }

        foreach (var key in settings.Key)
        {
            fieldsToKeep.Add(key.NormalizeKey());
        }

        return fieldsToKeep;
    }
}
