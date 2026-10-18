using System.Collections.ObjectModel;
using System.Windows.Input;
using PaniersFermiers.Models;
using PaniersFermiers.Services;
using PaniersFermiers.Views;

namespace PaniersFermiers.ViewModels
{
    public class CatalogueViewModel : BaseViewModel
    {
        private List<Panier> _tousLesPaniers = new List<Panier>();
        private string _filtreActif = "Tous";
        private string _texteTous = "Tous";
        private bool _estChargement = false;
        private string _erreurChargement = "";

      
        public ObservableCollection<Panier> Paniers { get; } = new ObservableCollection<Panier>();

        public string FiltreActif
        {
            get { return _filtreActif; }
            set { SetProperty(ref _filtreActif, value); }
        }

        public string TexteTous
        {
            get { return _texteTous; }
            set { SetProperty(ref _texteTous, value); }
        }

        public bool EstChargement
        {
            get { return _estChargement; }
            set
            {
                SetProperty(ref _estChargement, value);
                OnPropertyChanged(nameof(ListeVisible));
            }
        }

        public string ErreurChargement
        {
            get { return _erreurChargement; }
            set
            {
                SetProperty(ref _erreurChargement, value);
                OnPropertyChanged(nameof(AErreur));
                OnPropertyChanged(nameof(ListeVisible));
            }
        }

        public bool AErreur
        {
            get { return _erreurChargement != ""; }
        }

        public bool ListeVisible
        {
            get { return !_estChargement && _erreurChargement == ""; }
        }

        public ICommand FiltrerCommand { get; }
        public ICommand OuvrirPanierCommand { get; }
        public ICommand ReessayerCommand { get; }
        public ICommand MesCommandesCommand { get; }

        public CatalogueViewModel()
        {
            FiltrerCommand = new Command<string>(Filtrer);
            OuvrirPanierCommand = new Command<Panier>(OuvrirPanier);
            ReessayerCommand = new Command(Charger);
            MesCommandesCommand = new Command(OuvrirMesCommandes);
            Charger();
        }

        private async void Charger()
        {
            EstChargement = true;
            ErreurChargement = "";
            try
            {
                await Task.Delay(700);   // simulation du chargement
                _tousLesPaniers = CatalogueService.ObtenirTous();
                TexteTous = "Tous (" + _tousLesPaniers.Count + ")";
                AppliquerFiltre();
            }
            catch (Exception)
            {
                ErreurChargement = "Impossible de charger les paniers. Réessayez.";
            }
            EstChargement = false;
        }

        private void Filtrer(string categorie)
        {
            FiltreActif = categorie;
            AppliquerFiltre();
        }

        private void AppliquerFiltre()
        {
            Paniers.Clear();
            foreach (Panier p in _tousLesPaniers)
            {
                if (FiltreActif == "Tous" || p.Categorie == FiltreActif)
                {
                    Paniers.Add(p);
                }
            }
        }

        private async void OuvrirPanier(Panier panier)
        {
            CommandeService.DemarrerReservation(panier);
            await Shell.Current.GoToAsync(nameof(DetailPanierPage));
        }

        private async void OuvrirMesCommandes()
        {
            await Shell.Current.GoToAsync(nameof(SuiviPage));
        }

    }
}