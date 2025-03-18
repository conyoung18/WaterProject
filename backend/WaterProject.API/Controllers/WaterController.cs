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
    public IEnumerable<Project> GetProjects()
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
        
        return _waterContext.Projects.ToList();
    }

    [HttpGet("FunctionalProjects")]
    public IEnumerable<Project> GetFunctionalProjects()
    {
        return _waterContext.Projects.Where(p => p.ProjectFunctionalityStatus == "Functional").ToList();
    }
}