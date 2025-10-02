namespace WebApplication.E2ETests.Fixtures;

public static class TestData
{
    public static class Products
    {
        public static readonly string ExistingProductName = "Produit Test 1";
        public static readonly string ExistingProductMarque = "MarqueTest";
        public static readonly string ExistingProductType = "TypeTest";

        public static readonly string NewProductName = "Nouveau Produit E2E";
        public static readonly string NewProductMarque = "Marque E2E";
        public static readonly string NewProductType = "Type E2E";
    }

    public static class Images
    {
        public static readonly string TestImageUrl = "https://via.placeholder.com/300";
        public static readonly string TestImageName = "Image Test";
    }

    public static class Filters
    {
        public static readonly string SearchTerm = "Test";

        public static readonly string ValidMarque = Products.ExistingProductMarque;
        public static readonly string ValidType = Products.ExistingProductType;
    }
}