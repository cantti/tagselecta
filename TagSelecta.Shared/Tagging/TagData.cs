using TagLib;

namespace TagSelecta.Shared.Tagging;

public class TagData
{
    private readonly List<TagField> _fields = [];

    public List<Picture> Picture { get; set; } = [];

    public IReadOnlyList<TagField> Fields =>
        _fields.OrderBy(cf => cf.Key).Select(x => new TagField(x.Key, x.Text)).ToList();

    public void Clear()
    {
        _fields.Clear();
        Picture.Clear();
    }

    public void SetValue(string key, IEnumerable<string> value)
    {
        key = key.NormalizeKey();
        var valueList = value.Where(x => !string.IsNullOrWhiteSpace(x)).ToList();

        var index = _fields.FindIndex(cf => cf.Key == key);

        if (valueList.Count == 0)
        {
            if (index >= 0)
            {
                _fields.RemoveAt(index);
            }
        }
        else
        {
            var replacement = new TagField(key, valueList);
            if (index < 0)
            {
                _fields.Add(replacement);
            }
            else
            {
                _fields[index] = replacement;
            }
        }
    }

    public void SetValue(string key, string value)
    {
        SetValue(key, [value]);
    }

    public void RemoveField(string key)
    {
        key = key.NormalizeKey();
        var index = _fields.FindIndex(cf => cf.Key == key);
        if (index >= 0)
        {
            _fields.RemoveAt(index);
        }
    }

    public List<string> GetValue(string key)
    {
        key = key.NormalizeKey();
        return _fields.Find(cf => cf.Key == key)?.Text ?? [];
    }

    public string GetValueFirst(string key)
    {
        return GetValue(key).FirstOrDefault() ?? "";
    }

    public TagData Clone()
    {
        return TagDataCloner.Clone(this);
    }
}
