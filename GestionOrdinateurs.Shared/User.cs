using System.ComponentModel.DataAnnotations;

namespace GestionOrdinateurs.Shared;


public enum RoleEnum
{
    Gestionnaire,
    Admin
}

public class User
{
    [Key]
    public int Id { get; set; }

    [Required]
	public string Login { get; set; } = string.Empty;

    public RoleEnum Role { get; set; }
}