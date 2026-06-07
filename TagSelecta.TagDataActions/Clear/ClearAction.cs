using TagSelecta.Shared.Tagging;
using TagSelecta.TagDataActions.Abstractions;

namespace TagSelecta.TagDataActions.Clear;

[TagDataActionInfo(
    "clear",
    AllowRemainingArguments = true,
    FieldNameCompletion = FieldNameCompletion.Boolean
)]
public class ClearAction : ITagDataAction<ClearSettings>
{
    private HashSet<string> _fieldsToRemove = [];

    public Task<bool> BeforeExecute(ClearSettings settings, CancellationToken token)
    {
        _fieldsToRemove = BuildFieldsToRemove(settings);
        return Task.FromResult(true);
    }

    public Task Execute(TagDataActionExecuteContext<ClearSettings> context, CancellationToken token)
    {
        var tagData = context.Target.CurrentTagData;

        foreach (var field in tagData.Fields)
        {
            if (_fieldsToRemove.Contains(field.Key))
            {
                tagData.SetValue(field.Key, "");
            }
        }

        if (context.Settings.Picture)
        {
            tagData.ClearPicture();
        }

        context.Target.UpdateTagData(tagData);

        return Task.CompletedTask;
    }

    private static HashSet<string> BuildFieldsToRemove(ClearSettings settings)
    {
        var fieldsToKeep = new HashSet<string>(StringComparer.OrdinalIgnoreCase);

        var settingProperties = typeof(ClearSettings)
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
