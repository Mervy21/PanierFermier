namespace PaniersFermiers.Models
{
    public class ProduitPanier
    {
        public string Icone { get; set; } = "";
        public string Libelle { get; set; } = "";
        public ProduitPanier(string icone, string libelle)
        {
            Icone = icone;
            Libelle = libelle;
        }
    }
}