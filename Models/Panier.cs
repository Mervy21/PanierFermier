using PaniersFermiers.Services;
namespace PaniersFermiers.Models
{
    public class Panier
    {
        public int Id { get; set; }
        public string Nom { get; set; } = "";
        public string Description { get; set; } = "";
        public string Categorie { get; set; } = "";
        public double PoidsKg { get; set; }
        public int Prix { get; set; }
        public string Emoji { get; set; } = "";
        public List<ProduitPanier> Contenu { get; set; } = new List<ProduitPanier>();
        public string PrixTexte
        {
            get { return Outils.Fcfa(Prix); }
        }
        public string PoidsTexte
        {
            get { return Outils.Kg(PoidsKg); }
        }
    }
}
