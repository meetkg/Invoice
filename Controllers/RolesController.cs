using Invoice.Models.DTOs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

[Authorize(Roles = "Admin")]
[ApiController]
[Route("[controller]")]
public class RolesController : ControllerBase
{
    private readonly RoleManager<IdentityRole> _roleManager;

    public RolesController(RoleManager<IdentityRole> roleManager)
    {
        _roleManager = roleManager;
    }

    [HttpPost]
    public async Task<IActionResult> CreateRole([FromBody] RoleDto roleDto)
    {
        var role = new IdentityRole { Name = roleDto.Name };
        var result = await _roleManager.CreateAsync(role);

        if (result.Succeeded)
        {
            return Ok();
        }
        return BadRequest(result.Errors);
    }

    [HttpGet]
    public IActionResult GetAllRoles()
    {
        var roles = _roleManager.Roles.Select(r => new { r.Id, r.Name }).ToList();
        return Ok(roles);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateRole(string id, [FromBody] RoleDto roleDto)
    {
        var role = await _roleManager.FindByIdAsync(id);
        if (role == null) return NotFound();

        role.Name = roleDto.Name;
        var result = await _roleManager.UpdateAsync(role);

        if (result.Succeeded)
        {
            return Ok();
        }
        return BadRequest(result.Errors);
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteRole(string id)
    {
        var role = await _roleManager.FindByIdAsync(id);
        if (role == null) return NotFound();

        var result = await _roleManager.DeleteAsync(role);
        if (result.Succeeded)
        {
            return Ok();
        }
        return BadRequest(result.Errors);
    }
}
