namespace GestionOrdinateurs.API.events;

using GestionOrdinateurs.Shared;

public class OrdinateurMisAJourEventArgs : EventArgs
{
    public Ordinateur Ordinateur { get; }

    // Constructeur pour initialiser les arguments de l'événement avec l'ordinateur mis à jour.
    public OrdinateurMisAJourEventArgs(Ordinateur ordinateur)
    {
        Ordinateur = ordinateur;
    }
}