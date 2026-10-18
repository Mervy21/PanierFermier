using System.Windows.Input;
using PaniersFermiers.Models;
using PaniersFermiers.Services;
using PaniersFermiers.Views;

namespace PaniersFermiers.ViewModels
{
    public class DetailPanierViewModel : BaseViewModel
    {
        private Panier _panier = new Panier();

        public string Nom { get; }
        public string Emoji { get; }
        public string PrixTexte { get; }
        public string PoidsTexte { get; }
        public string NombreVarietes { get; }
        public List<ProduitPanier> Contenu { get; }

        public ICommand ReserverCommand { get; }

        public DetailPanierViewModel()
        {
            Panier? choisi = CommandeService.PanierChoisi;
            if (choisi != null)
            {
                _panier = choisi;
            }

            Nom = _panier.Nom;
            Emoji = _panier.Emoji;
            PrixTexte = _panier.PrixTexte;
            PoidsTexte = _panier.PoidsTexte;
            Contenu = _panier.Contenu;
            NombreVarietes = _panier.Contenu.Count + " variétés locales";

            ReserverCommand = new Command(Reserver);
        }

        private async void Reserver()
        {
            // On repart d'une réservation propre avec ce panier
            CommandeService.DemarrerReservation(_panier);
            await Shell.Current.GoToAsync(nameof(ReservationPage));
        }
    }
}