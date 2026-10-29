namespace GestionOrdinateurs.API.events;

using GestionOrdinateurs.Shared;

public class OrdinateurAjouteEventArgs : EventArgs
{
    public Ordinateur Ordinateur { get; }

    // Constructeur pour initialiser les arguments de l'événement avec l'ordinateur ajouté.
    public OrdinateurAjouteEventArgs(Ordinateur ordinateur)
    {
        Ordinateur = ordinateur;
    }
}