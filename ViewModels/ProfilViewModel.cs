using System.Windows.Input;
using PaniersFermiers.Models;
using PaniersFermiers.Services;
using PaniersFermiers.Views;

namespace PaniersFermiers.ViewModels
{
    // Page profil du client : informations, statistiques, aide et déconnexion
    public class ProfilViewModel : BaseViewModel
    {
        private Utilisateur? U { get { return AuthService.UtilisateurConnecte; } }

        public string Initiales { get { return U?.Initiales ?? "?"; } }
        public string NomComplet { get { return U?.NomComplet ?? ""; } }
        public string Email { get { return U?.Email ?? ""; } }
        public string Telephone { get { return U == null ? "" : "+237 " + U.Telephone; } }

        public string NombreCommandesTexte
        {
            get { return CommandeService.ObtenirCommandes().Count.ToString(); }
        }

        public string TotalDepenseTexte
        {
            get { return Outils.Fcfa(CommandeService.TotalDepense()); }
        }

        public ICommand MesCommandesCommand { get; }
        public ICommand AideCommand { get; }
        public ICommand DeconnexionCommand { get; }

        public ProfilViewModel()
        {
            MesCommandesCommand = new Command(OuvrirMesCommandes);
            AideCommand = new Command(Aide);
            DeconnexionCommand = new Command(Deconnexion);
        }

        public void Actualiser()
        {
            OnPropertyChanged(nameof(NombreCommandesTexte));
            OnPropertyChanged(nameof(TotalDepenseTexte));
        }

        private async void OuvrirMesCommandes()
        {
            await Shell.Current.GoToAsync(nameof(SuiviPage));
        }

        private async void Aide()
        {
            await Shell.Current.DisplayAlert("Aide & contact",
                "Une question sur votre panier ?\n\nTéléphone : 6 90 00 00 00\nE-mail : contact@paniers.cm\n\nDu lundi au samedi, 8h - 18h.",
                "OK");
        }

        private async void Deconnexion()
        {
            bool confirme = await Shell.Current.DisplayAlert("Déconnexion",
                "Voulez-vous vous déconnecter ?", "Se déconnecter", "Annuler");
            if (!confirme) return;
            CommandeService.ViderPanier();   // le panier en cours ne doit pas passer au client suivant
            AuthService.Deconnecter();
            await Shell.Current.GoToAsync("//LoginPage");
        }
    }
}