using System.Globalization;
using Microsoft.AspNetCore.Mvc;

namespace PFT.Controllers;

[ApiController]
[Route("api/pft")]
public class PftController : ControllerBase
{
    private static readonly string _dataDirectoryPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile), "data");
    private static readonly string _dataFilePath = Path.Combine(_dataDirectoryPath, "feed-log.txt");

    private readonly TimeZoneInfo _kievTimeZone = TimeZoneInfo.FindSystemTimeZoneById("FLE Standard Time");

    public PftController()
    {
        if (!Directory.Exists(_dataDirectoryPath))
            Directory.CreateDirectory(_dataDirectoryPath);

        if (!System.IO.File.Exists(_dataFilePath))
            System.IO.File.Create(_dataFilePath).Dispose();
    }

    [HttpGet("last")]
    public IActionResult GetLastFeedingTime()
    {
        var allLines = System.IO.File.ReadLines(_dataFilePath);
        var lastLine = allLines.LastOrDefault();

        if (string.IsNullOrWhiteSpace(lastLine))
            return Ok("The pet hasn't been fed yet!");

        var lastMealUtc = DateTime.Parse(lastLine, CultureInfo.InvariantCulture, DateTimeStyles.RoundtripKind);
        var lastMealTime = TimeZoneInfo.ConvertTimeFromUtc(lastMealUtc, _kievTimeZone).ToString("d MMM HH:mm");

        var todayMeals = allLines.Count(line => DateTime.TryParse(line, CultureInfo.InvariantCulture, out DateTime date) && date.Date == DateTime.UtcNow.Date);
        var yesterdayMeals = allLines.Count(line => DateTime.TryParse(line, CultureInfo.InvariantCulture, out DateTime date) && date.Date == DateTime.UtcNow.AddDays(-1).Date);

        return Ok(new { lastMealTime, todayMeals, yesterdayMeals });
    }

    [HttpPost("feed")]
    public IActionResult Feed()
    {
        var utcNow = DateTime.UtcNow;
        System.IO.File.AppendAllText(_dataFilePath, utcNow.ToString("o") + Environment.NewLine);

        return Ok();
    }
}