using PaniersFermiers.Models;
using System.Globalization;
using System.Text.Json;

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

        // Les commandes et le numéro suivant sont sauvegardés sur l'appareil
        private const string FichierCommandes = "commandes.json";
        private const string FichierNumero = "numero.txt";
        private static int _prochainNumero = ChargerNumero();
        private static List<Commande> _commandes = ChargerCommandes();
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
                detail = detail + "• " + ligne.Nom + " — " + ligne.QuantiteTexte + " — " + ligne.TotalTexte + "\n";
            }

            Commande c = new Commande();
            c.Reference = "PF-" + _prochainNumero;
            _prochainNumero++;
            Stockage.Ecrire(FichierNumero, _prochainNumero.ToString());
            c.EmailClient = AuthService.UtilisateurConnecte?.Email.ToLower() ?? "";
            c.DateCommande = DateTime.Now;
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
            SauvegarderCommandes();
            ViderBrouillon();
            return c;
        }

        public static List<Commande> ObtenirCommandes()
        {
            // Chaque client ne voit que ses propres commandes
            string email = AuthService.UtilisateurConnecte?.Email.ToLower() ?? "";
            List<Commande> resultat = new List<Commande>();
            foreach (Commande c in _commandes)
            {
                if (c.EmailClient == email) resultat.Add(c);
            }
            return resultat;
        }

        // Statistique pour la page Profil : total dépensé par le client connecté
        public static int TotalDepense()
        {
            int somme = 0;
            foreach (Commande c in ObtenirCommandes())
            {
                somme = somme + c.Total;
            }
            return somme;
        }

        // Annulation (seulement avant la récolte)
        public static bool AnnulerCommande(Commande commande)
        {
            if (!commande.PeutEtreAnnulee) return false;
            _commandes.Remove(commande);
            SauvegarderCommandes();
            return true;
        }

        // À appeler quand l'étape d'une commande change
        public static void MettreAJourCommande(Commande commande)
        {
            SauvegarderCommandes();
        }

        private static void SauvegarderCommandes()
        {
            Stockage.Ecrire(FichierCommandes, JsonSerializer.Serialize(_commandes));
        }

        private static int ChargerNumero()
        {
            int numero;
            if (int.TryParse(Stockage.Lire(FichierNumero).Trim(), out numero))
            {
                return numero;
            }
            return 8492;
        }

        private static List<Commande> ChargerCommandes()
        {
            try
            {
                string brut = Stockage.Lire(FichierCommandes);
                if (brut == "") return new List<Commande>();
                List<Commande>? lue = JsonSerializer.Deserialize<List<Commande>>(brut);
                if (lue == null) return new List<Commande>();
                return lue;
            }
            catch (Exception)
            {
                return new List<Commande>();
            }
        }
    }
}
