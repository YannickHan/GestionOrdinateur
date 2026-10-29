using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GestionOrdinateurs.Shared;

[Table ("User")]
public class User
{
    public int Id { get; set; }

    [Required]
	public string Login { get; set; } = string.Empty;
    
    [Required]
    public string Role { get; set; } = RoleEnum.Gestionnaire;
}