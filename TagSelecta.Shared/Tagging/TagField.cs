namespace TagSelecta.Shared.Tagging;

public class TagField
{
    public TagField(string key, List<string> text)
    {
        Key = key;
        Text = text;
    }

    public string Key { get; }
    public List<string> Text { get; }
}
