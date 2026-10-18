using PaniersFermiers.Models;

namespace PaniersFermiers.Services
{
    public static class AuthService
    {
        // Comptes en mémoire (simulation). Compte de démo : demo@paniers.cm / 12345678
        private static List<Utilisateur> _comptes = new List<Utilisateur>
        {
            new Utilisateur
            {
                NomComplet = "Compte Démo",
                Email = "demo@paniers.cm",
                Telephone = "690000000",
                MotDePasse = "12345678"
            }
        };

        public static Utilisateur? UtilisateurConnecte { get; private set; }

        public static bool Connecter(string identifiant, string motDePasse)
        {
            string id = identifiant.Trim().ToLower();
            string tel = Outils.NettoyerTelephone(identifiant);

            foreach (Utilisateur u in _comptes)
            {
                bool memeIdentifiant = (u.Email.ToLower() == id) || (u.Telephone == tel);
                if (memeIdentifiant && u.MotDePasse == motDePasse)
                {
                    UtilisateurConnecte = u;
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

        public static void Inscrire(Utilisateur utilisateur)
        {
            _comptes.Add(utilisateur);
            UtilisateurConnecte = utilisateur;
        }

        public static void Deconnecter()
        {
            UtilisateurConnecte = null;
        }
    }
}
