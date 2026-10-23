using PaniersFermiers.Views;

namespace PaniersFermiers
{
    public partial class AppShell : Shell
    {
        public AppShell()
        {
            InitializeComponent();

            Routing.RegisterRoute(nameof(RegisterPage), typeof(RegisterPage));
            Routing.RegisterRoute(nameof(ReservationPage), typeof(ReservationPage));
            Routing.RegisterRoute(nameof(RetraitPage), typeof(RetraitPage));
            Routing.RegisterRoute(nameof(PaiementPage), typeof(PaiementPage));
            Routing.RegisterRoute(nameof(SuiviPage), typeof(SuiviPage));
            Routing.RegisterRoute(nameof(ProduitPage), typeof(ProduitPage));

        }
    }
}
