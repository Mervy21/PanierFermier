using PaniersFermiers.Models;

namespace PaniersFermiers.Services
{
    // Garde en mémoire le panier composé par le client, son choix de retrait et l'historique
    public static class CommandeService
    {
        // ----- Règles de l'application -----
        public const int MontantMinimum = 2000;     // minimum de commande en FCFA
        public const double QuantiteMin = 0.5;      // kg
        public const double QuantiteMax = 20;       // kg
        public const double Pas = 0.5;              // on avance de 0.5 kg en 0.5 kg

        private static int _prochainNumero = 8492;
        private static List<Commande> _commandes = new List<Commande>();
        private static List<LignePanier> _lignes = new List<LignePanier>();

        // Produit affiché dans la fiche produit
        public static Produit? ProduitConsulte { get; set; }

        // Choix de retrait et de paiement
        public static PointRetrait? PointChoisi { get; set; }
        public static Creneau? CreneauChoisi { get; set; }
        public static string ModePaiement { get; set; } = "";

        // ----- Panier -----
        public static List<LignePanier> ObtenirLignes()
        {
            return _lignes;
        }

        public static int NombreArticles
        {
            get { return _lignes.Count; }
        }

        public static int Total
        {
            get
            {
                int somme = 0;
                foreach (LignePanier ligne in _lignes)
                {
                    somme = somme + ligne.Total;
                }
                return somme;
            }
        }

        public static double PoidsTotal
        {
            get
            {
                double poids = 0;
                foreach (LignePanier ligne in _lignes)
                {
                    poids = poids + ligne.QuantiteKg;
                }
                return poids;
            }
        }

        public static void AjouterProduit(Produit produit, double quantiteKg)
        {
            // Si le produit est déjà dans le panier, on additionne les quantités
            foreach (LignePanier ligne in _lignes)
            {
                if (ligne.Produit.Id == produit.Id)
                {
                    ModifierQuantite(ligne, ligne.QuantiteKg + quantiteKg);
                    return;
                }
            }
            _lignes.Add(new LignePanier(produit, quantiteKg));
        }

        public static void ModifierQuantite(LignePanier ligne, double nouvelleQuantite)
        {
            if (nouvelleQuantite < QuantiteMin) nouvelleQuantite = QuantiteMin;
            if (nouvelleQuantite > QuantiteMax) nouvelleQuantite = QuantiteMax;
            ligne.QuantiteKg = nouvelleQuantite;
        }

        public static void RetirerLigne(LignePanier ligne)
        {
            _lignes.Remove(ligne);
        }

        public static void ViderPanier()
        {
            _lignes.Clear();
        }

        public static void ViderBrouillon()
        {
            _lignes.Clear();
            PointChoisi = null;
            CreneauChoisi = null;
            ModePaiement = "";
        }

        // ----- Commandes -----
        public static Commande ConfirmerCommande()
        {
            if (_lignes.Count == 0 || PointChoisi == null || CreneauChoisi == null)
            {
                throw new InvalidOperationException("Commande incomplète.");
            }

            // Détail ligne par ligne
            string detail = "";
            foreach (LignePanier ligne in _lignes)
            {
                detail = detail + ligne.Emoji + " " + ligne.Nom + " — " + ligne.QuantiteTexte + " — " + ligne.TotalTexte + "\n";
            }

            Commande c = new Commande();
            c.Reference = "PF-" + _prochainNumero;
            _prochainNumero++;
            c.NombreArticles = _lignes.Count;
            c.PoidsKg = PoidsTotal;
            c.ArticlesTexte = detail.TrimEnd();
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
