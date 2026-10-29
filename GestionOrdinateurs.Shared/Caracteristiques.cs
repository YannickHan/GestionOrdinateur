namespace GestionOrdinateurs.Shared;

public class Caracteristiques
{
    public int RamGo { get; set; }
    public TypeOrdinateur TypeOrdi { get; set; }
    public int StockageGo { get; set; }
    public string Processeur { get; set; } = null!;

    private Caracteristiques()
    {
    }

    public Caracteristiques(int ramGo, int stockageGo, TypeOrdinateur typeOrdi, string processeur)
    {
        RamGo = ramGo;
        StockageGo = stockageGo;
        TypeOrdi = typeOrdi;
        Processeur = processeur;
    }
}