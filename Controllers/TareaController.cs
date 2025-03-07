using System.Security.Claims;
using System.Text.Json;
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

            // Configurar las opciones de serialización para evitar el ciclo infinito
            var options = new JsonSerializerOptions
            {
                ReferenceHandler = System.Text.Json.Serialization.ReferenceHandler.Preserve,
            };

            // Serializar las tareas con las opciones configuradas
            var json = JsonSerializer.Serialize(tareas, options);

            return Ok(json);
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

            // Configurar las opciones de serialización para evitar el ciclo infinito
            var options = new JsonSerializerOptions
            {
                ReferenceHandler = System.Text.Json.Serialization.ReferenceHandler.Preserve,
            };

            var json = JsonSerializer.Serialize(tarea, options);

            return Ok(json);
        }

        // Crear una nueva tarea
        // Crear una nueva tarea
        [HttpPost]
        [HttpPost]
        public async Task<IActionResult> CreateTarea([FromBody] Tarea tarea)
        {
            // Obtener el userId desde el token JWT
            var userId = GetUserIdFromClaims();

            // Asignar el UserId del usuario autenticado a la tarea
            tarea.UserId = userId;

            // Asegurarse de que la tarea no cree un nuevo usuario
            // Se asume que la propiedad Usuario en la tarea está solo para las relaciones, no para la creación.
            tarea.Usuario = null; // Asegurarse de que no se asigna un usuario nuevo en la tarea

            // Agregar la tarea al contexto de la base de datos
            _context.Tareas.Add(tarea);

            // Guardar cambios en la base de datos
            await _context.SaveChangesAsync();

            // Retornar la tarea recién creada, incluyendo la URL para obtenerla
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
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier); // Busca el claim con el userId (NameIdentifier)

            if (userIdClaim == null)
            {
                throw new InvalidOperationException("El userId no se encuentra en los claims.");
            }

            return int.Parse(userIdClaim.Value); // Convierte el valor del claim a entero (userId)
        }
    }
}
