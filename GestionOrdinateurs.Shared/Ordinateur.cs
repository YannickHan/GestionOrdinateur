using System.ComponentModel.DataAnnotations.Schema;

namespace GestionOrdinateurs.Shared;

public enum EtatOrdinateur
{
    EnService,
    EnPanne,
    EnMaintenance,
    HorsService,
    Lent,
    Reparable
}

[Table ("Ordinateur")]
public class Ordinateur
{
    public int Id { get; init; }
    public string Nom { get; set; } = null!;
    public string Marque { get; private set; } = null!;
    public EtatOrdinateur Etat { get; set; }
    public decimal Prix { get; set; } 
    public int Stock { get; set; }
    public bool Vendu { get; set; } = false;
    public Caracteristiques Caracteristiques { get; set; } = null!;
    public List<Reparation> Reparations { get; set; } = new();

    private Ordinateur()
    {
    }

    public Ordinateur(int id, string nom, string marque, EtatOrdinateur etat, decimal prix = 0, int stock = 0, Caracteristiques caracteristiques = null!)
    {
        this.Id = id;
        this.Nom = nom;
        this.Marque = marque;
        this.Etat = etat;
        this.Prix = prix;
        this.Stock = stock;
        this.Caracteristiques = caracteristiques;
    }

    public void Allumer()
    {
        this.Etat = EtatOrdinateur.EnService;
    }
    public void Eteindre()
    {
        this.Etat = EtatOrdinateur.HorsService;
    }
}
