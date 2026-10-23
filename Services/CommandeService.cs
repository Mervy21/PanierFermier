using PaniersFermiers.Models;
using System.Globalization;
using Microsoft.Maui.Storage;

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
            foreach (LignePanier ligne in _lignes)
            {
                if (ligne.Produit.Id == produit.Id)
                {
                    ModifierQuantite(ligne, ligne.QuantiteKg + quantiteKg);
                    return;
                }
            }
            _lignes.Add(new LignePanier(produit, quantiteKg));
            SauvegarderPanier();
        }

        public static void ModifierQuantite(LignePanier ligne, double nouvelleQuantite)
        {
            if (nouvelleQuantite < QuantiteMin) nouvelleQuantite = QuantiteMin;
            if (nouvelleQuantite > QuantiteMax) nouvelleQuantite = QuantiteMax;
            ligne.QuantiteKg = nouvelleQuantite;
            SauvegarderPanier();
        }

        public static void RetirerLigne(LignePanier ligne)
        {
            _lignes.Remove(ligne);
            SauvegarderPanier();
        }

        public static void ViderPanier()
        {
            _lignes.Clear();
            SauvegarderPanier();
        }

        public static void ViderBrouillon()
        {
            _lignes.Clear();
            PointChoisi = null;
            CreneauChoisi = null;
            ModePaiement = "";
            SauvegarderPanier();
        }

        // ----- Sauvegarde du panier sur le téléphone -----
        private const string ClePanier = "panier_v1";

        public static void SauvegarderPanier()
        {
            List<string> morceaux = new List<string>();
            foreach (LignePanier l in _lignes)
                morceaux.Add(l.Produit.Id + ":" + l.QuantiteKg.ToString(CultureInfo.InvariantCulture));
            Preferences.Default.Set(ClePanier, string.Join(";", morceaux));
        }

        public static void ChargerPanier()
        {
            _lignes.Clear();
            string brut = Preferences.Default.Get(ClePanier, "");
            if (brut == "") return;
            foreach (string morceau in brut.Split(';'))
            {
                string[] p = morceau.Split(':');
                if (p.Length != 2) continue;
                if (!int.TryParse(p[0], out int id)) continue;
                if (!double.TryParse(p[1], NumberStyles.Any, CultureInfo.InvariantCulture, out double kg)) continue;
                Produit? produit = CatalogueService.ObtenirTous().FirstOrDefault(x => x.Id == id);
                if (produit != null && produit.Disponible) _lignes.Add(new LignePanier(produit, kg));
            }
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
