namespace PaniersFermiers.Models
{
    public class Utilisateur
    {
        public string NomComplet { get; set; } = "";
        public string Email { get; set; } = "";
        public string Telephone { get; set; } = "";
        // Simulation uniquement : en vrai, un mot de passe n'est jamais stocké en clair.
        public string MotDePasse { get; set; } = "";
    }
}
