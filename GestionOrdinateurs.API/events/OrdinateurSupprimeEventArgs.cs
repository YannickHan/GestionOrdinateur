namespace GestionOrdinateurs.API.events;

using GestionOrdinateurs.Shared;

public class OrdinateurSupprimeEventArgs : EventArgs
{
    public Ordinateur Ordinateur { get; }

    // Constructeur pour initialiser les arguments de l'événement avec l'ordinateur supprimé.
    public OrdinateurSupprimeEventArgs(Ordinateur ordinateur)
    {
        Ordinateur = ordinateur;
    }
}