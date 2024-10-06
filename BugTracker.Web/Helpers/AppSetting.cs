namespace BugTracker.Web.Helpers;

public static class AppSetting
{
    public static string BaseUrl
    {
        get
        {
            var config = new ConfigurationBuilder().SetBasePath(AppDomain.CurrentDomain.BaseDirectory).AddJsonFile("appsettings.json").Build();
            var baseUrl = config.GetSection("BaseUrl").GetSection("BugTrackerAPI").Value;
            return baseUrl ?? string.Empty;
        }
    }
}
