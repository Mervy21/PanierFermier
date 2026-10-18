using PaniersFermiers.Services;

namespace PaniersFermiers.Models
{
    public class Commande
    {
        public string Reference { get; set; } = "";      // "PF-8492"
        public string NomPanier { get; set; } = "";
        public int Quantite { get; set; }
        public int Total { get; set; }
        public string LieuRetrait { get; set; } = "";
        public string JourRetrait { get; set; } = "";
        public string CreneauRetrait { get; set; } = "";
        public string ModePaiement { get; set; } = "";
        public int Etape { get; set; } = 1;              // 1 = Validée, 2 = Récoltée, 3 = Au relais

        public string ReferenceAffichee
        {
            get { return "Commande #" + Reference; }
        }

        public string TotalTexte
        {
            get { return Outils.Fcfa(Total); }
        }

        public string ResumeTexte
        {
            get { return Quantite + " × " + NomPanier; }
        }

        public string StatutTexte
        {
            get
            {
                if (Etape == 1) return "Validée";
                if (Etape == 2) return "Récoltée";
                return "Prête à être retirée";
            }
        }
    }
}
