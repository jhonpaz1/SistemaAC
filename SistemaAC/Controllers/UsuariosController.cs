using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using SistemaAC.Data;
using SistemaAC.Models;

namespace SistemaAC.Controllers
{
    public class UsuariosController : Controller
    {
        private readonly ApplicationDbContext _context;
        UserManager<ApplicationUser> _userManager;
        RoleManager<IdentityRole> _roleManager;
        UsuarioRole _usrioRole;
        public List<SelectListItem> usuarioRole;

        public UsuariosController(ApplicationDbContext context, 
            UserManager<ApplicationUser> userManager,
            RoleManager<IdentityRole> roleManager)
        {
            _context = context;
            _userManager = userManager;
            _roleManager = roleManager;
            _usrioRole = new UsuarioRole();
            usuarioRole = new List<SelectListItem>();
        }

        // GET: Usuarios
        public async Task<IActionResult> Index()
        {
            //ID decalrada e inicializada como vacía
            var ID = "";
            //Objeto de tipo List que depende de la clase usuario
            List<Usuario> usuario = new List<Usuario>();
            //Almacenar los datos de la tabla en el objeto
            var appUsuario = await _context.ApplicationUser.ToListAsync();
            // Recorrer todos los valores del objeto appUsuario
            foreach (var Data in appUsuario)
            {
                ID = Data.Id;
                usuarioRole = await _usrioRole.GetRole(_userManager, _roleManager, ID);

                usuario.Add(new Usuario()
                {
                    Id = Data.Id,
                    UserName = Data.UserName,
                    PhoneNumber = Data.PhoneNumber,
                    Email = Data.Email,
                    Role = usuarioRole[0].Text
                });
            }
            //return View(await _context.ApplicationUser.ToListAsync());
            return View(usuario.ToList());
        }

        //public async Task<List<ApplicationUser>> GetUsuario(string id)
        public async Task<List<Usuario>> GetUsuario(string id)
        {
            //List<ApplicationUser> usuario = new List<ApplicationUser>();
            //var appUsuario = await _context.ApplicationUser.SingleOrDefaultAsync(m => m.Id == id);
            //usuario.Add(appUsuario);

            //Declaro un objeto list que depende de la clase usuario
            List<Usuario> usuario = new List<Usuario>();
            var appUsuario = await _context.ApplicationUser.SingleOrDefaultAsync(m=>m.Id == id);
            usuarioRole = await _usrioRole.GetRole(_userManager, _roleManager, id);

            usuario.Add(new Usuario()
            {
                Id = appUsuario.Id,
                UserName = appUsuario.UserName,
                PhoneNumber = appUsuario.PhoneNumber,
                Email = appUsuario.Email,
                Role = usuarioRole[0].Text,
                RoleId = usuarioRole[0].Value,
                EmailConfirmed = appUsuario.EmailConfirmed,
                LockoutEnabled = appUsuario.LockoutEnabled,
                LockoutEnd = appUsuario.LockoutEnd,
                NormalizedEmail = appUsuario.NormalizedEmail,
                NormalizedUserName = appUsuario.NormalizedUserName,
                PasswordHash = appUsuario.PasswordHash,
                PhoneNumberConfirmed = appUsuario.PhoneNumberConfirmed,
                SecurityStamp = appUsuario.SecurityStamp,
                TwoFactorEnabled = appUsuario.TwoFactorEnabled,
                AccessFailedCount = appUsuario.AccessFailedCount,
                ConcurrencyStamp = appUsuario.ConcurrencyStamp
            });
            return usuario;
        }

        public async Task<string> EditUsuario(string id, string userName, string email,
           string phoneNumber, int accessFailedCount, string concurrencyStamp, bool emailConfirmed,
           bool lockoutEnabled, DateTimeOffset lockoutEnd, string normalizedEmail,
           string normalizedUserName, string passwordHash, bool phoneNumberConfirmed,
           string securityStamp, bool twoFactorEnabled, ApplicationUser applicationUser)
        {
            var resp = "";
            try
            {
                applicationUser = new ApplicationUser
                {
                    Id = id,
                    UserName = userName,
                    Email = email,
                    PhoneNumber = phoneNumber,
                    EmailConfirmed = emailConfirmed,
                    LockoutEnabled = lockoutEnabled,
                    LockoutEnd = lockoutEnd,
                    NormalizedEmail = normalizedEmail,
                    NormalizedUserName = normalizedUserName,
                    PasswordHash = passwordHash,
                    PhoneNumberConfirmed = phoneNumberConfirmed,
                    SecurityStamp = securityStamp,
                    TwoFactorEnabled = twoFactorEnabled,
                    AccessFailedCount = accessFailedCount,
                    ConcurrencyStamp = concurrencyStamp
                };
                //Actualizamos los datos
                _context.Update(applicationUser);
                await _context.SaveChangesAsync();
                resp = "Save";
            }
            catch
            {
                resp = "No Save";
            }

            return resp;
        }

        private bool ApplicationUserExists(string id)
        {
            return _context.ApplicationUser.Any(e => e.Id == id);
        }
    }
}
