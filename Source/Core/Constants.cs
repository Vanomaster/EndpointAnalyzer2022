namespace Core;

public static class Constants
{
    public const string ProgramName = "Endpoint analyzer 2022";
    public static readonly string ProgramExePath = Directory.GetCurrentDirectory();
    public static readonly string UserDirectoryPath = Environment.GetFolderPath(Environment.SpecialFolder.Personal);
    public static readonly string SourceDbDirectoryPath = @$"{ProgramExePath}\..\..\..\..\..\Db";
}