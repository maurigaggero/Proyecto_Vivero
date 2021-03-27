using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Proyecto_Vivero.Server.Data;
using Proyecto_Vivero.Shared;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Proyecto_Vivero.Server.Common;

namespace Proyecto_Vivero.Server.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RolesController : ControllerBase
    {
        private readonly UserManager<ApplicationUser> userManager;
        private readonly RoleManager<IdentityRole> roleManager;

        public RolesController(UserManager<ApplicationUser> userManager,
            RoleManager<IdentityRole> roleManager)
        {
            this.roleManager = roleManager;
            this.userManager = userManager;
        }

        [HttpGet]
        public List<IdentityRole> ListaRoles()
        {
            var claims = User.Claims.ToArray();

            return roleManager.Roles.ToList();
        }

        [HttpGet("usuarios")]
        public List<ApplicationUser> ListaUsuarios()
        {
            return userManager.Users.ToList();
        }

        [HttpGet("rol/{nombreRol}")]
        public async Task<IdentityRole> Rol(string nombreRol)
        {
            return await roleManager.FindByNameAsync(nombreRol);
        }

        [HttpPost("{usuarioId}/{create}")]
        public async Task<ActionResult> AddRolUser(IdentityRole rol, string usuarioId, bool create)
        {
            try
            {
                var user = await userManager.FindByIdAsync(usuarioId);
                var role = rol.Name;

                if (create == true)
                {
                    await userManager.AddToRoleAsync(user, role);
                    return Ok();
                }
                else
                {
                    await userManager.RemoveFromRoleAsync(user, role);
                    return Ok();
                }
            }
            catch (Exception)
            {
                return NotFound();
            }
        }
    }
}