using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TodoListApi.DataAccess;
using TodoListApi.Models;

namespace TodoListApi.Controllers
{
    [Authorize] // Asegura que solo los usuarios autenticados puedan acceder a este controlador
    [Route("api/tareas")]
    [ApiController]
    public class TareaController : ControllerBase
    {
        private readonly AppDbContext _context;

        public TareaController(AppDbContext context)
        {
            _context = context;
        }

        // Obtener todas las tareas del usuario autenticado
        [HttpGet]
        public async Task<IActionResult> GetTareas()
        {
            var userId = GetUserIdFromClaims();
            var tareas = await _context
                .Tareas.Where(t => t.UserId == userId) // Solo tareas del usuario autenticado
                .ToListAsync();

            return Ok(tareas);
        }

        // Obtener una tarea por ID
        [HttpGet("{id}")]
        public async Task<IActionResult> GetTareaById(int id)
        {
            var userId = GetUserIdFromClaims();
            var tarea = await _context.Tareas.FirstOrDefaultAsync(t =>
                t.Id == id && t.UserId == userId
            ); // Solo tareas del usuario autenticado

            if (tarea == null)
            {
                return NotFound(new { message = "Tarea no encontrada" });
            }

            return Ok(tarea);
        }

        // Crear una nueva tarea
        [HttpPost]
        public async Task<IActionResult> CreateTarea([FromBody] Tarea tarea)
        {
            var userId = GetUserIdFromClaims();
            tarea.UserId = userId; // Asignar el usuario autenticado a la tarea

            _context.Tareas.Add(tarea);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetTareaById), new { id = tarea.Id }, tarea);
        }

        // Actualizar una tarea existente
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateTarea(int id, [FromBody] Tarea tarea)
        {
            var userId = GetUserIdFromClaims();
            var existingTarea = await _context.Tareas.FirstOrDefaultAsync(t =>
                t.Id == id && t.UserId == userId
            );

            if (existingTarea == null)
            {
                return NotFound(new { message = "Tarea no encontrada" });
            }

            existingTarea.Titulo = tarea.Titulo;
            existingTarea.Descripcion = tarea.Descripcion;
            existingTarea.Completada = tarea.Completada;
            existingTarea.FechaCreacion = tarea.FechaCreacion;

            _context.Tareas.Update(existingTarea);
            await _context.SaveChangesAsync();

            return Ok(existingTarea);
        }

        // Eliminar una tarea
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteTarea(int id)
        {
            var userId = GetUserIdFromClaims();
            var tarea = await _context.Tareas.FirstOrDefaultAsync(t =>
                t.Id == id && t.UserId == userId
            );

            if (tarea == null)
            {
                return NotFound(new { message = "Tarea no encontrada" });
            }

            _context.Tareas.Remove(tarea);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        // Obtener el userId desde el token JWT
        private int GetUserIdFromClaims()
        {
            var userId = User.FindFirstValue("sub");
            return int.Parse(userId);
        }
    }
}
