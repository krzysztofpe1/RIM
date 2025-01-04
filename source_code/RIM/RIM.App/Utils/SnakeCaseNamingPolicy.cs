using System.Text.Json;

namespace RIM.App.Utils;

public class SnakeCaseNamingPolicy : JsonNamingPolicy
{
    public override string ConvertName(string name)
    {
        // conversion of PascalCase to snake_case
        return string.Concat(name.Select((c, i) =>
            i > 0 && char.IsUpper(c) ? "_" + c.ToString().ToLower() : c.ToString().ToLower()));
    }
}