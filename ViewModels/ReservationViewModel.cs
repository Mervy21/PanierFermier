using System.Windows.Input;
using PaniersFermiers.Models;
using PaniersFermiers.Services;
using PaniersFermiers.Views;

namespace PaniersFermiers.ViewModels
{
    public class ReservationViewModel : BaseViewModel
    {
        private Panier _panier = new Panier();
        private int _quantite = 1;
        private bool _vide = false;

        public string Nom { get { return _panier.Nom; } }
        public string Emoji { get { return _panier.Emoji; } }

        public string PrixKgTexte
        {
            get
            {
                if (_panier.PoidsKg <= 0) return "";
                int prixKg = (int)Math.Round(_panier.Prix / _panier.PoidsKg);
                return Outils.Fcfa(prixKg) + "/kg";
            }
        }

        public int Quantite
        {
            get { return _quantite; }
            set
            {
                SetProperty(ref _quantite, value);
                CommandeService.Quantite = value;
                OnPropertyChanged(nameof(PeutDiminuer));
                OnPropertyChanged(nameof(PeutAugmenter));
                OnPropertyChanged(nameof(TotalTexte));
                OnPropertyChanged(nameof(NombreArticlesTexte));
            }
        }

        public bool PeutDiminuer { get { return _quantite > 1; } }
        public bool PeutAugmenter { get { return _quantite < 10; } }

        public string TotalTexte
        {
            get { return Outils.Fcfa(_panier.Prix * _quantite); }
        }

        public string NombreArticlesTexte
        {
            get { return "Sous-total produits (" + _quantite + (_quantite > 1 ? " articles)" : " article)"); }
        }

        // État "panier vide" (après Supprimer)
        public bool Vide
        {
            get { return _vide; }
            set
            {
                SetProperty(ref _vide, value);
                OnPropertyChanged(nameof(ContenuVisible));
            }
        }

        public bool ContenuVisible { get { return !_vide; } }

        public ICommand AugmenterCommand { get; }
        public ICommand DiminuerCommand { get; }
        public ICommand SupprimerCommand { get; }
        public ICommand ChoisirPointCommand { get; }
        public ICommand VoirPaniersCommand { get; }

        public ReservationViewModel()
        {
            Panier? choisi = CommandeService.PanierChoisi;
            if (choisi == null)
            {
                _vide = true;
            }
            else
            {
                _panier = choisi;
                _quantite = CommandeService.Quantite;
            }

            AugmenterCommand = new Command(Augmenter);
            DiminuerCommand = new Command(Diminuer);
            SupprimerCommand = new Command(Supprimer);
            ChoisirPointCommand = new Command(ChoisirPoint);
            VoirPaniersCommand = new Command(VoirPaniers);
        }

        private void Augmenter()
        {
            if (PeutAugmenter) Quantite = _quantite + 1;
        }

        private void Diminuer()
        {
            if (PeutDiminuer) Quantite = _quantite - 1;
        }

        private void Supprimer()
        {
            CommandeService.ViderBrouillon();
            Vide = true;
        }

        private async void ChoisirPoint()
        {
            await Shell.Current.GoToAsync(nameof(RetraitPage));
        }

        private async void VoirPaniers()
        {
            await Shell.Current.GoToAsync("//CataloguePage");
        }
    }
}