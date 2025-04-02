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
    public IActionResult GetProjects(int pageHowMany = 10, int pageNumber = 1, [FromQuery] List<string>projectTypes = null)
    {
        var query = _waterContext.Projects.AsQueryable();

        if (projectTypes != null && projectTypes.Any())
        {
            query = query.Where(p => projectTypes.Contains(p.ProjectType));
        }
        
        string? FavProjectType = Request.Cookies["FavProjectType"];
        Console.WriteLine("---------COOKIE--------\n" + FavProjectType);
        
        HttpContext.Response.Cookies.Append("FavoriteProjectType", "Borehole Well and Hand Pump", new CookieOptions
        {
            HttpOnly = true, 
            Secure = true,
            SameSite = SameSiteMode.Strict,
            Expires = DateTime.Now.AddMinutes(1)
        });
        
        var totalNumProjects = query.Count();
        
        var something = query
            .Skip((pageNumber - 1) * pageHowMany)
            .Take(pageHowMany)
            .ToList();

            var someObject = new
            {
                Projects = something,
                TotalNumProjects = totalNumProjects
            };

            return Ok(someObject);
    }

    [HttpGet("GetProjectTypes")]
    public IActionResult GetProjectTypes()
    {
        var projectTypes = _waterContext.Projects
            .Select(p => p.ProjectType)
            .Distinct()
            .ToList();
        
        return Ok(projectTypes);
    }

    [HttpPost("AddProject")]
    public IActionResult AddProject([FromBody] Project newProject)
    {
        _waterContext.Projects.Add(newProject);
        _waterContext.SaveChanges();
        return Ok(newProject);
    }

    [HttpPut("UpdateProject/{projectId}")]
    public IActionResult UpdateProject(int projectId, [FromBody] Project updatedProject)
    {
        var existingProject = _waterContext.Projects.Find(projectId);

        existingProject.ProjectName = updatedProject.ProjectName;
        existingProject.ProjectType = updatedProject.ProjectType;
        existingProject.ProjectRegionalProgram = updatedProject.ProjectRegionalProgram;
        existingProject.ProjectImpact = updatedProject.ProjectImpact;
        existingProject.ProjectPhase = updatedProject.ProjectPhase;
        existingProject.ProjectFunctionalityStatus = updatedProject.ProjectFunctionalityStatus;

        _waterContext.Projects.Update(existingProject);
        _waterContext.SaveChanges();

        return Ok(existingProject);
    }

    [HttpDelete("DeleteProject/{projectId}")]
    public IActionResult DeleteProject(int projectId)
    {
        var project = _waterContext.Projects.Find(projectId);

        if (project == null)
        {
            return NotFound(new {message = "Project not found"});
        }

        _waterContext.Projects.Remove(project);
        _waterContext.SaveChanges();

        return NoContent();
    }
}