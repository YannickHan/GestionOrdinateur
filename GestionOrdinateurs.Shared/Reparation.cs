using System.ComponentModel.DataAnnotations.Schema;
using GestionOrdinateurs.Shared;

[Table ("Reparation")]
public class Reparation
{
    public int Id { get; set; }

    public int OrdinateurId { get; set; }

    public Ordinateur UnOrdinateur { get; set; } = null!;

    public string TypeReparation { get; set; } = null!;

    public DateTime DateDebut { get; set; }

    public String Technicien { get; set; } = null!;

    public string Statut { get; set; } = null!;

    // Constructeur utilisé par Entity Framework Core lors de la lecture en base.
    private Reparation()
    {
    }

    // Crée une réparation avec son ordinateur, son type, sa date et son statut.
    public Reparation(int id, int ordinateurId, Ordinateur unOrdinateur, string typeReparation, DateTime dateDebut, string statut, string technicien = "Technicien par défaut")
    {
        Id = id;
        OrdinateurId = ordinateurId;
        UnOrdinateur = unOrdinateur;
        TypeReparation = typeReparation;
        DateDebut = dateDebut;
        Statut = statut;
        Technicien = technicien;
    }
}