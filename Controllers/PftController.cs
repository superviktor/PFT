using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc;

namespace PFT.Controllers;

[ApiController]
[Route("api/pft")]
public class PftController : ControllerBase
{
    private const string DateFormat = "yyyy-MM-dd";
    private static readonly string _dataDirectoryPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile), "data");
    private static readonly string _dataFilePath = Path.Combine(_dataDirectoryPath, "feed-log.txt");

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
        var lastMealTime = allLines.LastOrDefault();

        if (string.IsNullOrWhiteSpace(lastMealTime))
            return Ok("The pet hasn't been fed yet!");

        var today = DateTime.UtcNow; 
        var todayMeals = allLines.Count(line => line.StartsWith(today.ToString(DateFormat)));
        var yesterday = today.AddDays(-1); 
        var yesterdayMeals = allLines.Count(line => line.StartsWith(yesterday.ToString(DateFormat)));

        return Ok(new { lastMealTime, todayMeals, yesterdayMeals });
    }

    [HttpPost("feed")]
    public IActionResult Feed()
    {
        var utcNow = DateTime.UtcNow;
        System.IO.File.AppendAllText(_dataFilePath, utcNow.ToString("o") + Environment.NewLine);

        return Ok();
    }

    [HttpGet("meals")]
    public IActionResult GetMealsTime([Required] DateTime date)
    {
        var allLines = System.IO.File.ReadLines(_dataFilePath);
        var meals = allLines.Where(line => line.StartsWith(date.ToString(DateFormat)));

        return Ok(meals);
    }
}