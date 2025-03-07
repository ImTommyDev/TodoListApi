using System.Security.Cryptography;
using System.Text;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TodoListApi.DataAccess;
using TodoListApi.DTOs;
using TodoListApi.Models;
using TodoListApi.Services;

namespace TodoListApi.Controllers
{
    [Authorize] //Para proteger controladores o endpoints
    [Route("api/auth")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly AppDbContext _context;
        private readonly JwtService _jwtService;

        public AuthController(AppDbContext context, JwtService jwtService)
        {
            _context = context;
            _jwtService = jwtService;
        }

        [HttpPost("register")]
        [AllowAnonymous]
        public async Task<IActionResult> Register([FromBody] RegistroDto registroRequest)
        {
            // Verificar si el email ya está registrado
            var existingUser = await _context.Usuarios.FirstOrDefaultAsync(u =>
                u.Email == registroRequest.Email
            );
            if (existingUser != null)
            {
                return BadRequest(new { message = "El usuario ya existe" });
            }

            // Crear el usuario con la contraseña hasheada
            var usuario = new Usuario
            {
                Nombre = registroRequest.Nombre,
                Email = registroRequest.Email,
                PasswordHash = BCrypt.Net.BCrypt.HashPassword(registroRequest.Password),
            };

            _context.Usuarios.Add(usuario);
            await _context.SaveChangesAsync();

            return Ok(new { message = "Usuario registrado exitosamente" });
        }

        [HttpPost("login")]
        [AllowAnonymous]
        public async Task<IActionResult> Login([FromBody] LoginDto loginRequest)
        {
            var user = await _context.Usuarios.FirstOrDefaultAsync(u =>
                u.Email == loginRequest.Email
            );

            if (user == null || !BCrypt.Net.BCrypt.Verify(loginRequest.Password, user.PasswordHash))
            {
                return Unauthorized(new { message = "Credenciales inválidas" });
            }

            var token = _jwtService.GenerarToken(user.Id, user.Email);
            return Ok(new { token });
        }
    }
}
