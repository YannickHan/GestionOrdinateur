using GestionOrdinateurs.Shared;
namespace GestionOrdinateurs.API.services.interfaces;

public interface IOrdinateurService
{
    public bool Add(int id, string nom, string marque, int ramGo, int stockageGo, TypeOrdinateur typeOrdi, string processeur, EtatOrdinateur etat, decimal prix = 0, int stock = 0);
    public bool Remove(int id);
    public bool Update(int id, string nom, int ramGo, int stockageGo, TypeOrdinateur typeOrdi, string processeur, EtatOrdinateur etat, decimal prix = 0, int stock = 0);
    public IReadOnlyList<Ordinateur> GetAll();
    public IReadOnlyList<Ordinateur> GetById(int id);
    public IReadOnlyList<Ordinateur> GetByMarque(string marque);
    public string Diagnostiquer(Ordinateur ordinateur);
    public (int TotalOrdinateur, int OrdiDisponibles, decimal ValeurStock, int NbReparations) CalculerStatistiques();
}