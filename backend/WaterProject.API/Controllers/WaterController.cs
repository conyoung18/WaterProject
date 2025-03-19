using System.Reflection;
using Microsoft.AspNetCore.Mvc;
using WaterProject.API.Data;

namespace WaterProject.API.Controllers;

[Route("[controller]")]
[ApiController]
public class WaterController : ControllerBase
{
    private WaterDbContext _waterContext;
    
    public WaterController(WaterDbContext temp)
    {
        _waterContext = temp;
    }
    
    [HttpGet("AllProjects")]
    public IActionResult GetProjects(int pageHowMany = 10, int pageNumber = 1)
    {
        string? FavProjectType = Request.Cookies["FavProjectType"];
        Console.WriteLine("---------COOKIE--------\n" + FavProjectType);
        
        HttpContext.Response.Cookies.Append("FavoriteProjectType", "Borehole Well and Hand Pump", new CookieOptions
        {
            HttpOnly = true, 
            Secure = true,
            SameSite = SameSiteMode.Strict,
            Expires = DateTime.Now.AddMinutes(1)
        });
        
        var something = _waterContext.Projects
            .Skip((pageNumber - 1) * pageHowMany)
            .Take(pageHowMany)
            .ToList();

            var totalNumProjects = _waterContext.Projects.Count();

            var someObject = new
            {
                Projects = something,
                TotalNumProjects = totalNumProjects
            };

            return Ok(someObject);
    }

    [HttpGet("FunctionalProjects")]
    public IEnumerable<Project> GetFunctionalProjects()
    {
        return _waterContext.Projects.Where(p => p.ProjectFunctionalityStatus == "Functional").ToList();
    }
}