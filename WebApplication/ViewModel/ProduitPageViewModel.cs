using System.ComponentModel;
using System.Runtime.CompilerServices;
using WebApplication.Models;
using WebApplication.Services;

namespace WebApplication.ViewModels
{
    public class ProduitPageViewModel : INotifyPropertyChanged
    {
        private readonly IGenericService<Produit> _produitService;
        private readonly IGenericService<Marque> _marqueService;
        private readonly IGenericService<TypeProduit> _typeService;
        private readonly IGenericService<Image> _imageService;
        private readonly INotificationService _notificationService;

        public event PropertyChangedEventHandler? PropertyChanged;

        #region Propriétés principales

        private IEnumerable<ProduitSimple> _produits = new List<ProduitSimple>();
        public IEnumerable<ProduitSimple> Produits
        {
            get => _produits;
            set => SetProperty(ref _produits, value);
        }

        private IEnumerable<ProduitSimple> _produitsFiltres = new List<ProduitSimple>();
        public IEnumerable<ProduitSimple> ProduitsFiltres
        {
            get => _produitsFiltres;
            set => SetProperty(ref _produitsFiltres, value);
        }

        private Dictionary<int, List<Image>> _imagesProduits = new();
        public Dictionary<int, List<Image>> ImagesProduits
        {
            get => _imagesProduits;
            set => SetProperty(ref _imagesProduits, value);
        }

        private List<Marque> _marquesDisponibles = new();
        public List<Marque> MarquesDisponibles
        {
            get => _marquesDisponibles;
            set => SetProperty(ref _marquesDisponibles, value);
        }

        private List<TypeProduit> _typesDisponibles = new();
        public List<TypeProduit> TypesDisponibles
        {
            get => _typesDisponibles;
            set => SetProperty(ref _typesDisponibles, value);
        }

        #endregion

        #region Propriétés de filtrage

        private string _searchNom = "";
        public string SearchNom
        {
            get => _searchNom;
            set
            {
                if (SetProperty(ref _searchNom, value))
                {
                    _ = AppliquerFiltresAsync();
                }
            }
        }

        private string _selectedType = "";
        public string SelectedType
        {
            get => _selectedType;
            set
            {
                if (SetProperty(ref _selectedType, value))
                {
                    _ = AppliquerFiltresAsync();
                }
            }
        }

        private string _selectedMarque = "";
        public string SelectedMarque
        {
            get => _selectedMarque;
            set
            {
                if (SetProperty(ref _selectedMarque, value))
                {
                    _ = AppliquerFiltresAsync();
                }
            }
        }

        #endregion

        #region Propriétés Modal Détails

        private bool _showDetailModal;
        public bool ShowDetailModal
        {
            get => _showDetailModal;
            set => SetProperty(ref _showDetailModal, value);
        }

        private Produit? _produitSelectionne;
        public Produit? ProduitSelectionne
        {
            get => _produitSelectionne;
            set => SetProperty(ref _produitSelectionne, value);
        }

        #endregion

        #region Propriétés Modal Modification

        private bool _showEditModal;
        public bool ShowEditModal
        {
            get => _showEditModal;
            set => SetProperty(ref _showEditModal, value);
        }

        private Produit? _produitAModifier;
        public Produit? ProduitAModifier
        {
            get => _produitAModifier;
            set => SetProperty(ref _produitAModifier, value);
        }

        private List<Image> _imagesModification = new();
        public List<Image> ImagesModification
        {
            get => _imagesModification;
            set => SetProperty(ref _imagesModification, value);
        }

        private List<Image> _imagesASupprimer = new();
        public List<Image> ImagesASupprimer
        {
            get => _imagesASupprimer;
            set => SetProperty(ref _imagesASupprimer, value);
        }

        private string _nouvelleImageNom = "";
        public string NouvelleImageNom
        {
            get => _nouvelleImageNom;
            set => SetProperty(ref _nouvelleImageNom, value);
        }

        private string _nouvelleImageUrl = "";
        public string NouvelleImageUrl
        {
            get => _nouvelleImageUrl;
            set => SetProperty(ref _nouvelleImageUrl, value);
        }

        private string _errorMessageEdit = "";
        public string ErrorMessageEdit
        {
            get => _errorMessageEdit;
            set => SetProperty(ref _errorMessageEdit, value);
        }

        private bool _isSubmittingEdit;
        public bool IsSubmittingEdit
        {
            get => _isSubmittingEdit;
            set => SetProperty(ref _isSubmittingEdit, value);
        }

        #endregion

        #region Propriétés Modal Suppression

        private bool _showDeleteModal;
        public bool ShowDeleteModal
        {
            get => _showDeleteModal;
            set => SetProperty(ref _showDeleteModal, value);
        }

        private (int id, string nom)? _produitASupprimer;
        public (int id, string nom)? ProduitASupprimer
        {
            get => _produitASupprimer;
            set
            {
                SetProperty(ref _produitASupprimer, value);
                OnPropertyChanged(nameof(NomProduitASupprimer));
            }
        }

        public string NomProduitASupprimer => ProduitASupprimer?.nom ?? "";

        private bool _isSubmittingDelete;
        public bool IsSubmittingDelete
        {
            get => _isSubmittingDelete;
            set => SetProperty(ref _isSubmittingDelete, value);
        }

        #endregion

        private ProduitService ProduitService => (ProduitService)_produitService;

        public ProduitPageViewModel(
            IGenericService<Produit> produitService,
            IGenericService<Marque> marqueService,
            IGenericService<TypeProduit> typeService,
            IGenericService<Image> imageService,
            INotificationService notificationService)
        {
            _produitService = produitService;
            _marqueService = marqueService;
            _typeService = typeService;
            _imageService = imageService;
            _notificationService = notificationService;
        }

        #region Initialisation

        public async Task InitialiserAsync()
        {
            try
            {
                await ChargerProduitsAsync();
                await ChargerDonneesReferencesAsync();
                await ChargerImagesAsync();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Erreur lors du chargement : {ex.Message}");
                _notificationService.ShowError($"Erreur lors du chargement : {ex.Message}");
            }
        }

        private async Task ChargerProduitsAsync()
        {
            try
            {
                ProduitsFiltres = await ProduitService.GetFilteredAsync(
                    searchTerm: string.IsNullOrEmpty(SearchNom) ? null : SearchNom,
                    marque: string.IsNullOrEmpty(SelectedMarque) ? null : SelectedMarque,
                    type: string.IsNullOrEmpty(SelectedType) ? null : SelectedType
                );

                Produits = ProduitsFiltres;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Erreur ChargerProduitsAsync: {ex.Message}");
                ProduitsFiltres = new List<ProduitSimple>();
            }
        }

        private async Task ChargerDonneesReferencesAsync()
        {
            var marquesResponse = await _marqueService.GetAllAsync();
            MarquesDisponibles = marquesResponse?.ToList() ?? new List<Marque>();

            var typesResponse = await _typeService.GetAllAsync();
            TypesDisponibles = typesResponse?.ToList() ?? new List<TypeProduit>();
        }

        private async Task ChargerImagesAsync()
        {
            try
            {
                var allImages = await _imageService.GetAllAsync();
                if (allImages != null && ProduitsFiltres != null)
                {
                    ImagesProduits = allImages
                        .GroupBy(img => img.IdProduit)
                        .ToDictionary(g => g.Key, g => g.ToList());
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Erreur lors du chargement des images : {ex.Message}");
                ImagesProduits = new Dictionary<int, List<Image>>();
            }
        }

        #endregion

        #region Filtrage

        public async Task AppliquerFiltresAsync()
        {
            await ChargerProduitsAsync();
        }

        public async Task EffacerFiltresAsync()
        {
            SearchNom = "";
            SelectedType = "";
            SelectedMarque = "";

            await ChargerProduitsAsync();
            await ChargerImagesAsync();
        }

        #endregion

        #region Modal Détails

        public async Task VoirDetailsAsync(int produitId)
        {
            try
            {
                ProduitSelectionne = await _produitService.GetByIdAsync(produitId);
                ShowDetailModal = true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Erreur lors du chargement des détails : {ex.Message}");
                _notificationService.ShowError($"Erreur lors du chargement des détails : {ex.Message}");
            }
        }

        public void FermerDetailModal()
        {
            ShowDetailModal = false;
            ProduitSelectionne = null;
        }

        #endregion

        #region Modal Modification

        public async Task OuvrirModificationAsync(int produitId)
        {
            try
            {
                FermerDetailModal();
                ProduitAModifier = await _produitService.GetByIdAsync(produitId);
                ErrorMessageEdit = "";

                // Charger les images existantes
                if (ImagesProduits.ContainsKey(produitId))
                {
                    ImagesModification = ImagesProduits[produitId].Select(img => new Image
                    {
                        IdImage = img.IdImage,
                        NomImage = img.NomImage,
                        UrlPhoto = img.UrlPhoto,
                        IdProduit = img.IdProduit
                    }).ToList();
                }
                else
                {
                    ImagesModification = new List<Image>();
                }

                ImagesASupprimer = new List<Image>();
                NouvelleImageNom = "";
                NouvelleImageUrl = "";

                ShowEditModal = true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Erreur lors du chargement pour modification : {ex.Message}");
                ErrorMessageEdit = $"Erreur lors du chargement : {ex.Message}";
                _notificationService.ShowError($"Erreur lors du chargement : {ex.Message}");
            }
        }

        public void FermerEditModal()
        {
            ShowEditModal = false;
            ProduitAModifier = null;
            ErrorMessageEdit = "";
            IsSubmittingEdit = false;
            ImagesModification.Clear();
            ImagesASupprimer.Clear();
            NouvelleImageNom = "";
            NouvelleImageUrl = "";
        }

        public void AjouterImageTemporaire()
        {
            if (string.IsNullOrWhiteSpace(NouvelleImageNom))
            {
                _notificationService.ShowWarning("Veuillez saisir un nom pour l'image");
                return;
            }

            if (string.IsNullOrWhiteSpace(NouvelleImageUrl))
            {
                _notificationService.ShowWarning("Veuillez saisir une URL pour l'image");
                return;
            }

            var nouvelleImage = new Image
            {
                IdImage = 0,
                NomImage = NouvelleImageNom,
                UrlPhoto = NouvelleImageUrl,
                IdProduit = ProduitAModifier?.IdProduit ?? 0
            };

            ImagesModification.Add(nouvelleImage);
            OnPropertyChanged(nameof(ImagesModification));

            NouvelleImageNom = "";
            NouvelleImageUrl = "";

            _notificationService.ShowSuccess("Image ajoutée à la liste");
        }

        public void SupprimerImageModification(Image image)
        {
            if (image.IdImage > 0)
            {
                ImagesASupprimer.Add(image);
            }

            ImagesModification.Remove(image);
            OnPropertyChanged(nameof(ImagesModification));
            _notificationService.ShowInfo("Image marquée pour suppression");
        }

        public async Task<bool> ConfirmerModificationAsync()
        {
            IsSubmittingEdit = true;
            ErrorMessageEdit = "";

            try
            {
                // Mettre à jour le produit
                await _produitService.UpdateAsync(ProduitAModifier.Id, ProduitAModifier);

                // Supprimer les images marquées
                foreach (var imageASupprimer in ImagesASupprimer)
                {
                    try
                    {
                        await _imageService.DeleteAsync(imageASupprimer.IdImage);
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"Erreur lors de la suppression de l'image {imageASupprimer.IdImage}: {ex.Message}");
                    }
                }

                // Ajouter les nouvelles images
                var nouvellesImages = ImagesModification.Where(img => img.IdImage == 0).ToList();
                foreach (var nouvelleImage in nouvellesImages)
                {
                    try
                    {
                        nouvelleImage.IdProduit = ProduitAModifier.IdProduit;
                        await _imageService.AddAsync(nouvelleImage);
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"Erreur lors de l'ajout de l'image {nouvelleImage.NomImage}: {ex.Message}");
                    }
                }

                // Recharger les données
                await InitialiserAsync();

                _notificationService.ShowSuccess($"Le produit '{ProduitAModifier.Nom}' a été modifié avec succès");
                FermerEditModal();
                return true;
            }
            catch (Exception ex)
            {
                ErrorMessageEdit = $"Erreur lors de la modification : {ex.Message}";
                Console.WriteLine($"Erreur ConfirmerModificationAsync : {ex}");
                _notificationService.ShowError($"Erreur lors de la modification : {ex.Message}");
                return false;
            }
            finally
            {
                IsSubmittingEdit = false;
            }
        }

        #endregion

        #region Modal Suppression

        public void OuvrirSuppression(int produitId, string nomProduit)
        {
            ProduitASupprimer = (produitId, nomProduit);
            ShowDeleteModal = true;
        }

        public void FermerDeleteModal()
        {
            ShowDeleteModal = false;
            ProduitASupprimer = null;
            IsSubmittingDelete = false;
        }

        public async Task<bool> ConfirmerSuppressionAsync()
        {
            if (ProduitASupprimer == null) return false;

            IsSubmittingDelete = true;

            try
            {
                // Supprimer les images associées
                if (ImagesProduits.TryGetValue(ProduitASupprimer.Value.id, out var images))
                {
                    foreach (var image in images)
                    {
                        await _imageService.DeleteAsync(image.Id);
                    }
                }

                // Supprimer le produit
                await _produitService.DeleteAsync(ProduitASupprimer.Value.id);

                _notificationService.ShowSuccess($"Le produit '{ProduitASupprimer.Value.nom}' a été supprimé avec succès");

                await InitialiserAsync();
                FermerDeleteModal();
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Erreur lors de la suppression : {ex.Message}");
                _notificationService.ShowError($"Erreur lors de la suppression : {ex.Message}");
                return false;
            }
            finally
            {
                IsSubmittingDelete = false;
            }
        }

        #endregion

        #region INotifyPropertyChanged

        protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        protected bool SetProperty<T>(ref T field, T value, [CallerMemberName] string? propertyName = null)
        {
            if (EqualityComparer<T>.Default.Equals(field, value))
                return false;

            field = value;
            OnPropertyChanged(propertyName);
            return true;
        }

        #endregion
    }
}