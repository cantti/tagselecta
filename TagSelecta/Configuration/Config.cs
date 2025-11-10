using System.Text.RegularExpressions;

namespace TagSelecta.Configuration;

public class Config : IConfig
{
    public List<string> CleanExcept
    {
        get
        {
            var env = Environment.GetEnvironmentVariable("TAGSELECTA_CLEAN_EXCEPT");

            if (!string.IsNullOrEmpty(env))
            {
                return [.. Regex.Split(env, @"\W+")];
            }
            else
            {
                return [];
            }
        }
    }
}
