using PaniersFermiers.ViewModels;

namespace PaniersFermiers.Views
{
    public partial class DetailPanierPage : ContentPage
    {
        public DetailPanierPage()
        {
            InitializeComponent();
            BindingContext = new DetailPanierViewModel();
        }
    }
}
