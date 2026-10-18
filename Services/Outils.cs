using System.Globalization;
namespace PaniersFermiers.Services
{
    public static class Outils
    {
        public static string Fcfa(int montant)
        {
            return montant.ToString("N0", CultureInfo.InvariantCulture) + " FCFA";
        }
        public static string Kg(double poids)
        {
            return poids.ToString(CultureInfo.InvariantCulture) + " kg";
        }
        public static bool EmailValide(string email)
        {
            if (email.Contains(" ")) return false;
            int arobase = email.IndexOf('@');
            if (arobase < 1) return false;
            int point = email.LastIndexOf('.');
            return point > arobase + 1 && point < email.Length - 1;
        }
        public static string NettoyerTelephone(string telephone)
        {
            string tel = telephone.Replace(" ", "").Replace("-", "");
            if (tel.StartsWith("+237"))
            {
                tel = tel.Substring(4);
            }
            return tel;
        }
        public static bool TelephoneValide(string telephone)
        {
            string tel = NettoyerTelephone(telephone);
            if (tel.Length != 9) return false;
            if (tel[0] != '6') return false;
            foreach (char c in tel)
            {
                if (!char.IsDigit(c)) return false;
            }
            return true;
        }
    }
}
