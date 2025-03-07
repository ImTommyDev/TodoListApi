using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace TodoListApi.Models
{
    public class Usuario
    {
        public int Id { get; set; }

        [MaxLength(100)]
        public string Nombre { get; set; } = string.Empty;

        public string Email { get; set; } = string.Empty;

        public string PasswordHash { get; set; } = string.Empty;

        public DateTime FechaCreacion { get; set; } = DateTime.UtcNow;

        // Relación con las tareas, un usuario puede tener una lista de tareas
        [JsonIgnore]
        public List<Tarea> Tareas { get; set; } = new();
    }
}
