using PaniersFermiers.ViewModels;
namespace PaniersFermiers.Views
{
    public partial class CataloguePage : ContentPage
    {
        public CataloguePage()
        {
            InitializeComponent();
            BindingContext = new CatalogueViewModel();
        }
    }
}