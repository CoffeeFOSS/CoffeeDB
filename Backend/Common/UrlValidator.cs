using System.Text.RegularExpressions;

public static partial class UrlValidator
{
  private const string UrlPattern =
      @"^https?:\/\/(([\w-]+\.)+[\w]{2,})(\/?|[\/?.#][^\s,]*[^. ,])$";

  private static readonly Regex UrlRegex =
      MyRegex();

  public static bool IsValidUrl(string url)
  {
    if (string.IsNullOrWhiteSpace(url)) return false;

    return UrlRegex.IsMatch(url.Trim());
  }

  [GeneratedRegex(UrlPattern, RegexOptions.IgnoreCase | RegexOptions.Compiled, "en-CA")]
  private static partial Regex MyRegex();
}