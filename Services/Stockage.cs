using Microsoft.Maui.Storage;

namespace PaniersFermiers.Services
{
    // Petit utilitaire : lire / écrire un fichier texte dans le dossier privé de l'application.
    // Les comptes, la session et les commandes y sont sauvegardés : ils survivent à la fermeture de l'appli.
    public static class Stockage
    {
        private static string Chemin(string nomFichier)
        {
            return Path.Combine(FileSystem.AppDataDirectory, nomFichier);
        }

        public static string Lire(string nomFichier)
        {
            try
            {
                string chemin = Chemin(nomFichier);
                if (!File.Exists(chemin)) return "";
                return File.ReadAllText(chemin);
            }
            catch (Exception)
            {
                return "";
            }
        }

        public static void Ecrire(string nomFichier, string contenu)
        {
            try
            {
                File.WriteAllText(Chemin(nomFichier), contenu);
            }
            catch (Exception)
            {
                // Si l'écriture échoue, l'application continue de fonctionner en mémoire
            }
        }

        public static void Supprimer(string nomFichier)
        {
            try
            {
                string chemin = Chemin(nomFichier);
                if (File.Exists(chemin)) File.Delete(chemin);
            }
            catch (Exception)
            {
            }
        }
    }
}
