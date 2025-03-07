using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace TodoListApi.Models
{
    public class Tarea
    {
        public int Id { get; set; }

        [Required, MaxLength(255)]
        public string Titulo { get; set; } = string.Empty;

        public string Descripcion { get; set; } = string.Empty;

        public bool Completada { get; set; } = false;

        public DateTime FechaCreacion { get; set; } = DateTime.UtcNow;

        // Clave foránea para el usuario
        [ForeignKey("Usuario")]
        public int UserId { get; set; }

        [JsonIgnore]
        public Usuario Usuario { get; set; } = new Usuario();
    }
}
