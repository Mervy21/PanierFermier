<<<<<<< HEAD
﻿using PaniersFermiers.ViewModels;
=======
using PaniersFermiers.ViewModels;
>>>>>>> origin/main

namespace PaniersFermiers.Views
{
    public partial class CataloguePage : ContentPage
    {
        public CataloguePage()
        {
            InitializeComponent();
            BindingContext = new CatalogueViewModel();
        }

<<<<<<< HEAD
        // Appelée chaque fois que la page redevient visible (retour du panier, de la fiche produit...)
=======
        // Appel�e chaque fois que la page redevient visible (retour du panier, de la fiche produit...)
>>>>>>> origin/main
        protected override void OnAppearing()
        {
            base.OnAppearing();
            CatalogueViewModel vm = (CatalogueViewModel)BindingContext;
            vm.Actualiser();
        }
    }
}
