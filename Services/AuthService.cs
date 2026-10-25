using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using PaniersFermiers.Models;

namespace PaniersFermiers.Services
{
    // - les mots de passe sont stockés sous forme d'empreinte SHA-256 (jamais en clair)
    // - les comptes créés sont sauvegardés sur l'appareil (fichier comptes.json)
    // - la session est mémorisée : l'utilisateur reste connecté au prochain lancement
    public static class AuthService
    {
        private const string FichierComptes = "comptes.json";
        private const string FichierSession = "session.txt";

        // Compte de démo : demo@paniers.cm / 12345678
        private static List<Utilisateur> _comptes = ChargerComptes();

        public static Utilisateur? UtilisateurConnecte { get; private set; }

        // Transforme un mot de passe en empreinte (SHA-256, en hexadécimal)
        public static string Hacher(string motDePasse)
        {
            byte[] octets = SHA256.HashData(Encoding.UTF8.GetBytes(motDePasse));
            return Convert.ToHexString(octets);
        }

        public static bool Connecter(string identifiant, string motDePasse)
        {
            string id = identifiant.Trim().ToLower();
            string tel = Outils.NettoyerTelephone(identifiant);
            string empreinte = Hacher(motDePasse);

            foreach (Utilisateur u in _comptes)
            {
                bool memeIdentifiant = (u.Email.ToLower() == id) || (u.Telephone == tel);
                if (memeIdentifiant && u.MotDePasse == empreinte)
                {
                    OuvrirSession(u);
                    return true;
                }
            }
            return false;
        }

        public static bool EmailExiste(string email)
        {
            foreach (Utilisateur u in _comptes)
            {
                if (u.Email.ToLower() == email.Trim().ToLower()) return true;
            }
            return false;
        }

        public static bool TelephoneExiste(string telephone)
        {
            string tel = Outils.NettoyerTelephone(telephone);
            foreach (Utilisateur u in _comptes)
            {
                if (u.Telephone == tel) return true;
            }
            return false;
        }

        // Le RegisterViewModel envoie le mot de passe en clair : on le hache ici avant de l'enregistrer
        public static void Inscrire(Utilisateur utilisateur)
        {
            utilisateur.MotDePasse = Hacher(utilisateur.MotDePasse);
            utilisateur.Telephone = Outils.NettoyerTelephone(utilisateur.Telephone);
            _comptes.Add(utilisateur);
            SauvegarderComptes();
            OuvrirSession(utilisateur);
        }

        public static void Deconnecter()
        {
            UtilisateurConnecte = null;
            Stockage.Supprimer(FichierSession);
        }

        // Appelé au démarrage : renvoie true si un utilisateur était déjà connecté
        public static bool RestaurerSession()
        {
            string email = Stockage.Lire(FichierSession).Trim();
            if (email == "") return false;

            foreach (Utilisateur u in _comptes)
            {
                if (u.Email.ToLower() == email)
                {
                    UtilisateurConnecte = u;
                    return true;
                }
            }
            return false;
        }

        private static void OuvrirSession(Utilisateur u)
        {
            UtilisateurConnecte = u;
            Stockage.Ecrire(FichierSession, u.Email.ToLower());
        }

        // ----- Sauvegarde des comptes -----
        private static List<Utilisateur> ChargerComptes()
        {
            List<Utilisateur> liste = new List<Utilisateur>();
            try
            {
                string brut = Stockage.Lire(FichierComptes);
                if (brut != "")
                {
                    List<Utilisateur>? lue = JsonSerializer.Deserialize<List<Utilisateur>>(brut);
                    if (lue != null) liste = lue;
                }
            }
            catch (Exception)
            {
                liste = new List<Utilisateur>();
            }

            // Le compte de démo existe toujours
            bool demoPresent = false;
            foreach (Utilisateur u in liste)
            {
                if (u.Email == "demo@paniers.cm") demoPresent = true;
            }
            if (!demoPresent)
            {
                liste.Insert(0, new Utilisateur
                {
                    NomComplet = "Client Démo",
                    Email = "demo@paniers.cm",
                    Telephone = "690000000",
                    MotDePasse = Hacher("12345678")
                });
            }
            return liste;
        }

        private static void SauvegarderComptes()
        {
            Stockage.Ecrire(FichierComptes, JsonSerializer.Serialize(_comptes));
        }
    }
}
