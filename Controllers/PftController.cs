using Microsoft.AspNetCore.Mvc;

namespace PFT.Controllers;

[ApiController]
[Route("api/pft")]
public class PftController : ControllerBase
{
    private readonly string _filePath = Path.Combine(Directory.GetCurrentDirectory(), "Data", "feed-log.txt");
    private readonly TimeZoneInfo _kievTimeZone = TimeZoneInfo.FindSystemTimeZoneById("FLE Standard Time");

    public PftController()
    {
        var directoryPath = Path.GetDirectoryName(_filePath);
        if (!Directory.Exists(directoryPath))
            Directory.CreateDirectory(directoryPath!);

        if (!System.IO.File.Exists(_filePath))
            System.IO.File.Create(_filePath).Dispose();
    }

    [HttpGet("last")]
    public IActionResult GetLastFeedingTime()
    {
        var lastFeedingTimeStr = System.IO.File.ReadLines(_filePath).LastOrDefault();

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
        System.IO.File.AppendAllText(_filePath, utcNow.ToString("o") + Environment.NewLine);
        return Ok("Feeding time updated!");
    }
}