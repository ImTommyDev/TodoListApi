using System.ComponentModel.DataAnnotations;

namespace TodoListApi.Models
{
    public class Usuario
    {
        public int Id { get; set; }

        [Required, MaxLength(100)]
        public string Nombre { get; set; } = string.Empty;

        [Required, EmailAddress]
        public string Email { get; set; } = string.Empty;

        [Required]
        public string PasswordHash { get; set; } = string.Empty;

        public DateTime FechaCreacion { get; set; } = DateTime.UtcNow;

        // Relación con las tareas, un usuario puede tener una lista de tareas
        public List<Tarea> Tareas { get; set; } = new();
    }
}
