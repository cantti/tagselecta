using TagLib;

namespace TagSelecta.Shared.Tagging;

public class TagData
{
    private readonly List<TagField> _fields = [];

    private readonly List<TagPicture> _picture = [];

    public IReadOnlyList<TagPicture> Picture => _picture.AsReadOnly();

    public IReadOnlyList<TagField> Fields =>
        _fields
            .OrderBy(cf => cf.Key)
            .Select(x => new TagField(x.Key, x.Text))
            .ToList()
            .AsReadOnly();

    public void Clear()
    {
        _fields.Clear();
    }

    public void ClearPicture()
    {
        _picture.Clear();
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

    public TagData Clone()
    {
        return TagDataCloner.Clone(this);
    }

    public void AddPicture(TagPicture picture)
    {
        _picture.Add(picture);
    }

    public void RemovePicture(PictureType type)
    {
        _picture.RemoveAll(x => x.Type == type);
    }
}
