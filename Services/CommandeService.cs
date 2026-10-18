using PaniersFermiers.Models;

namespace PaniersFermiers.Services
{
    // Garde en mémoire la commande en cours de création (brouillon) et l'historique
    public static class CommandeService
    {
        private static int _prochainNumero = 8492;
        private static List<Commande> _commandes = new List<Commande>();

        // ----- Brouillon de la commande en cours -----
        public static Panier? PanierChoisi { get; set; }
        public static int Quantite { get; set; } = 1;
        public static PointRetrait? PointChoisi { get; set; }
        public static Creneau? CreneauChoisi { get; set; }
        public static string ModePaiement { get; set; } = "";

        public static int Total
        {
            get
            {
                if (PanierChoisi == null) return 0;
                return PanierChoisi.Prix * Quantite;
            }
        }

        public static void DemarrerReservation(Panier panier)
        {
            ViderBrouillon();
            PanierChoisi = panier;
        }

        public static void ViderBrouillon()
        {
            PanierChoisi = null;
            Quantite = 1;
            PointChoisi = null;
            CreneauChoisi = null;
            ModePaiement = "";
        }

        public static Commande ConfirmerCommande()
        {
            if (PanierChoisi == null || PointChoisi == null || CreneauChoisi == null)
            {
                throw new InvalidOperationException("Commande incomplète.");
            }

            Commande c = new Commande();
            c.Reference = "PF-" + _prochainNumero;
            _prochainNumero++;
            c.NomPanier = PanierChoisi.Nom;
            c.Quantite = Quantite;
            c.Total = Total;
            c.LieuRetrait = PointChoisi.Nom;
            c.JourRetrait = PointChoisi.Jour;
            c.CreneauRetrait = CreneauChoisi.libelle;
            c.ModePaiement = ModePaiement;
            c.Etape = 1;

            _commandes.Insert(0, c);   // la plus récente en premier
            ViderBrouillon();
            return c;
        }

        public static List<Commande> ObtenirCommandes()
        {
            return _commandes;
        }
    }
}
