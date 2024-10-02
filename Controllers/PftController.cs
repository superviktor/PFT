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
        var lastFeedingTimeStr = System.IO.File.ReadLines(_dataFilePath).LastOrDefault();

        if (string.IsNullOrWhiteSpace(lastFeedingTimeStr))
            return Ok("The pet hasn't been fed yet!");

        var lastFeedingTimeUtc = DateTime.Parse(lastFeedingTimeStr, null, System.Globalization.DateTimeStyles.RoundtripKind);
        var kievTime = TimeZoneInfo.ConvertTimeFromUtc(lastFeedingTimeUtc, _kievTimeZone);
        var formattedTime = kievTime.ToString("dd/M/yyyy HH:mm");

        return Ok(formattedTime);
    }

    [HttpPost("feed")]
    public IActionResult Feed()
    {
        var utcNow = DateTime.UtcNow;
        System.IO.File.AppendAllText(_dataFilePath, utcNow.ToString("o") + Environment.NewLine);

        return Ok("Feeding time updated!");
    }
}