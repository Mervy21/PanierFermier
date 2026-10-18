using PaniersFermiers.Models;

namespace PaniersFermiers.Services
{
    public static class CatalogueService
    {
        private static List<Panier> _paniers = CreerPaniers();

        public static List<Panier> ObtenirTous()
        {
            return _paniers;
        }

        private static List<Panier> CreerPaniers()
        {
            List<Panier> liste = new List<Panier>();

            liste.Add(new Panier
            {
                Id = 1,
                Nom = "Panier Maraîcher",
                Description = "Pommes de terre, carottes fanes, poireaux d'automne, salade...",
                Categorie = "Légumes",
                PoidsKg = 4.5,
                Prix = 25000,
                Emoji = "🥕",
                Contenu = new List<ProduitPanier>
                {
                    new ProduitPanier("🥔", "1 kg Pommes de terre"),
                    new ProduitPanier("🥕", "800 g Carottes"),
                    new ProduitPanier("🌱", "2 Poireaux"),
                    new ProduitPanier("🥬", "2 kg de Salades"),
                    new ProduitPanier("🧅", "500 g Oignons")
                }
            });

            liste.Add(new Panier
            {
                Id = 2,
                Nom = "Panier Fruité & Douceur",
                Description = "Pommes bio craquantes, poires de saison, prunes locales mûries au soleil...",
                Categorie = "Fruits",
                PoidsKg = 3,
                Prix = 18000,
                Emoji = "🍎",
                Contenu = new List<ProduitPanier>
                {
                    new ProduitPanier("🍎", "1 kg Pommes bio"),
                    new ProduitPanier("🍐", "1 kg Poires de saison"),
                    new ProduitPanier("🍑", "500 g Prunes locales"),
                    new ProduitPanier("🍇", "500 g Raisins")
                }
            });

            liste.Add(new Panier
            {
                Id = 3,
                Nom = "Grand Panier Famille",
                Description = "Légumes de saison variés pour la semaine + généreuse sélection de fruits...",
                Categorie = "Mixte",
                PoidsKg = 7,
                Prix = 40000,
                Emoji = "🧺",
                Contenu = new List<ProduitPanier>
                {
                    new ProduitPanier("🥔", "1 kg Pommes de terre"),
                    new ProduitPanier("🥕", "1 kg Carottes"),
                    new ProduitPanier("🍅", "1 kg Tomates"),
                    new ProduitPanier("🥬", "1 Chou"),
                    new ProduitPanier("🍎", "1 kg Pommes"),
                    new ProduitPanier("🍐", "1 kg Poires"),
                    new ProduitPanier("🍌", "1 kg Bananes")
                }
            });

            return liste;
        }
    }
}