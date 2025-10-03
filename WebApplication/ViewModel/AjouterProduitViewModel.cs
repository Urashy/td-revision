using System.ComponentModel;
using System.Runtime.CompilerServices;
using WebApplication.Models;
using WebApplication.Services;

namespace WebApplication.ViewModels
{
    public class AjouterProduitViewModel : INotifyPropertyChanged
    {
        private readonly IGenericService<Produit> _produitService;
        private readonly IGenericService<Marque> _marqueService;
        private readonly IGenericService<TypeProduit> _typeService;
        private readonly IGenericService<Image> _imageService;
        private readonly INotificationService _notificationService;

        public event PropertyChangedEventHandler? PropertyChanged;

        #region Propriétés principales

        private Produit _produit = new();
        public Produit Produit
        {
            get => _produit;
            set => SetProperty(ref _produit, value);
        }

        private List<Image> _images = new();
        public List<Image> Images
        {
            get => _images;
            set => SetProperty(ref _images, value);
        }

        private int? _stockMin = 0;
        public int? StockMin
        {
            get => _stockMin;
            set
            {
                SetProperty(ref _stockMin, value);
                OnPropertyChanged(nameof(AfficherAlerteStockMin));
            }
        }

        private int? _stockMax = 100;
        public int? StockMax
        {
            get => _stockMax;
            set
            {
                SetProperty(ref _stockMax, value);
                OnPropertyChanged(nameof(AfficherAlerteStockMax));
            }
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

        #region Propriétés Modal Marque

        private bool _showModalMarque;
        public bool ShowModalMarque
        {
            get => _showModalMarque;
            set => SetProperty(ref _showModalMarque, value);
        }

        private Marque _nouvelleMarque = new();
        public Marque NouvelleMarque
        {
            get => _nouvelleMarque;
            set => SetProperty(ref _nouvelleMarque, value);
        }

        private string _errorMessageMarque = "";
        public string ErrorMessageMarque
        {
            get => _errorMessageMarque;
            set => SetProperty(ref _errorMessageMarque, value);
        }

        private bool _isSubmittingMarque;
        public bool IsSubmittingMarque
        {
            get => _isSubmittingMarque;
            set => SetProperty(ref _isSubmittingMarque, value);
        }

        #endregion

        #region Propriétés Modal Type

        private bool _showModalType;
        public bool ShowModalType
        {
            get => _showModalType;
            set => SetProperty(ref _showModalType, value);
        }

        private TypeProduit _nouveauType = new();
        public TypeProduit NouveauType
        {
            get => _nouveauType;
            set => SetProperty(ref _nouveauType, value);
        }

        private string _errorMessageType = "";
        public string ErrorMessageType
        {
            get => _errorMessageType;
            set => SetProperty(ref _errorMessageType, value);
        }

        private bool _isSubmittingType;
        public bool IsSubmittingType
        {
            get => _isSubmittingType;
            set => SetProperty(ref _isSubmittingType, value);
        }

        #endregion

        #region Propriétés UI

        private string _errorMessage = "";
        public string ErrorMessage
        {
            get => _errorMessage;
            set => SetProperty(ref _errorMessage, value);
        }

        private string _successMessage = "";
        public string SuccessMessage
        {
            get => _successMessage;
            set => SetProperty(ref _successMessage, value);
        }

        private bool _isSubmitting;
        public bool IsSubmitting
        {
            get => _isSubmitting;
            set => SetProperty(ref _isSubmitting, value);
        }

        public bool AfficherAlerteStockMin =>
            StockMin.HasValue && Produit.Stock.HasValue && Produit.Stock.Value < StockMin.Value;

        public bool AfficherAlerteStockMax =>
            StockMax.HasValue && Produit.Stock.HasValue && Produit.Stock.Value > StockMax.Value;

        #endregion

        public AjouterProduitViewModel(
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

        #region Méthodes d'initialisation

        public async Task InitialiserAsync()
        {
            try
            {
                await ChargerDonneesReferencesAsync();
            }
            catch (Exception ex)
            {
                ErrorMessage = $"Erreur lors du chargement des données : {ex.Message}";
                Console.WriteLine($"Erreur InitialiserAsync : {ex}");
            }
        }

        private async Task ChargerDonneesReferencesAsync()
        {
            var marquesResponse = await _marqueService.GetAllAsync();
            MarquesDisponibles = marquesResponse?.ToList() ?? new List<Marque>();

            var typesResponse = await _typeService.GetAllAsync();
            TypesDisponibles = typesResponse?.ToList() ?? new List<TypeProduit>();

            Console.WriteLine($"Marques chargées : {MarquesDisponibles.Count}");
            Console.WriteLine($"Types chargés : {TypesDisponibles.Count}");
        }

        #endregion

        #region Gestion des images

        public void AjouterImage()
        {
            Images.Add(new Image
            {
                NomImage = "",
                UrlPhoto = "",
                IdProduit = 0
            });
            OnPropertyChanged(nameof(Images));
        }

        public void SupprimerImage(int index)
        {
            if (index >= 0 && index < Images.Count)
            {
                Images.RemoveAt(index);
                OnPropertyChanged(nameof(Images));
            }
        }

        #endregion

        #region Gestion Modal Marque

        public void OuvrirModalMarque()
        {
            NouvelleMarque = new Marque();
            ErrorMessageMarque = "";
            ShowModalMarque = true;
        }

        public void FermerModalMarque()
        {
            ShowModalMarque = false;
            NouvelleMarque = new Marque();
            ErrorMessageMarque = "";
            IsSubmittingMarque = false;
        }

        public async Task AjouterMarqueAsync()
        {
            IsSubmittingMarque = true;
            ErrorMessageMarque = "";

            try
            {
                if (string.IsNullOrWhiteSpace(NouvelleMarque.Nom))
                {
                    ErrorMessageMarque = "Le nom de la marque est obligatoire.";
                    return;
                }

                if (MarquesDisponibles?.Any(m => m.Nom?.Equals(NouvelleMarque.Nom, StringComparison.OrdinalIgnoreCase) == true) == true)
                {
                    ErrorMessageMarque = "Cette marque existe déjà.";
                    return;
                }

                await _marqueService.AddAsync(NouvelleMarque);
                await ChargerDonneesReferencesAsync();

                Produit.Marque = NouvelleMarque.Nom;
                OnPropertyChanged(nameof(Produit));

                FermerModalMarque();
                _notificationService.ShowSuccess("Marque ajoutée avec succès");
            }
            catch (Exception ex)
            {
                ErrorMessageMarque = $"Erreur lors de l'ajout de la marque : {ex.Message}";
                Console.WriteLine($"Erreur AjouterMarqueAsync : {ex}");
            }
            finally
            {
                IsSubmittingMarque = false;
            }
        }

        #endregion

        #region Gestion Modal Type

        public void OuvrirModalType()
        {
            NouveauType = new TypeProduit();
            ErrorMessageType = "";
            ShowModalType = true;
        }

        public void FermerModalType()
        {
            ShowModalType = false;
            NouveauType = new TypeProduit();
            ErrorMessageType = "";
            IsSubmittingType = false;
        }

        public async Task AjouterTypeAsync()
        {
            IsSubmittingType = true;
            ErrorMessageType = "";

            try
            {
                if (string.IsNullOrWhiteSpace(NouveauType.Nom))
                {
                    ErrorMessageType = "Le nom du type de produit est obligatoire.";
                    return;
                }

                if (TypesDisponibles?.Any(t => t.Nom?.Equals(NouveauType.Nom, StringComparison.OrdinalIgnoreCase) == true) == true)
                {
                    ErrorMessageType = "Ce type de produit existe déjà.";
                    return;
                }

                await _typeService.AddAsync(NouveauType);
                await ChargerDonneesReferencesAsync();

                Produit.Type = NouveauType.Nom;
                OnPropertyChanged(nameof(Produit));

                FermerModalType();
                _notificationService.ShowSuccess("Type ajouté avec succès");
            }
            catch (Exception ex)
            {
                ErrorMessageType = $"Erreur lors de l'ajout du type : {ex.Message}";
                Console.WriteLine($"Erreur AjouterTypeAsync : {ex}");
            }
            finally
            {
                IsSubmittingType = false;
            }
        }

        #endregion

        #region Validation et Soumission

        public async Task<bool> AjouterProduitAsync()
        {
            IsSubmitting = true;
            ErrorMessage = "";
            SuccessMessage = "";

            try
            {
                if (!ValiderProduit())
                    return false;

                Produit.StockMini = StockMin;
                Produit.StockMaxi = StockMax;

                await _produitService.AddAsync(Produit);

                var produitCree = await _produitService.GetByNameAsync(Produit.Nom);

                if (produitCree != null)
                {
                    await AjouterImagesAsync(produitCree.Id);
                    SuccessMessage = $"Le produit '{Produit.Nom}' a été ajouté avec succès !";
                    _notificationService.ShowSuccess(SuccessMessage);
                    return true;
                }
                else
                {
                    ErrorMessage = "Le produit a été créé mais impossible de récupérer son ID pour ajouter les images.";
                    return false;
                }
            }
            catch (Exception ex)
            {
                ErrorMessage = $"Erreur lors de l'ajout du produit : {ex.Message}";
                _notificationService.ShowError(ErrorMessage);
                Console.WriteLine($"Erreur AjouterProduitAsync : {ex}");
                return false;
            }
            finally
            {
                IsSubmitting = false;
            }
        }

        private bool ValiderProduit()
        {
            if (string.IsNullOrWhiteSpace(Produit.Nom))
            {
                ErrorMessage = "Le nom du produit est obligatoire.";
                return false;
            }

            if (string.IsNullOrWhiteSpace(Produit.Marque))
            {
                ErrorMessage = "La marque est obligatoire.";
                return false;
            }

            if (string.IsNullOrWhiteSpace(Produit.Type))
            {
                ErrorMessage = "Le type de produit est obligatoire.";
                return false;
            }

            if (StockMax.HasValue && Produit.Stock.HasValue && Produit.Stock.Value > StockMax.Value)
            {
                ErrorMessage = "Le stock initial ne peut pas dépasser le stock maximum.";
                return false;
            }

            if (StockMin.HasValue && StockMax.HasValue && StockMin.Value > StockMax.Value)
            {
                ErrorMessage = "Le stock minimum ne peut pas être supérieur au stock maximum.";
                return false;
            }

            return true;
        }

        private async Task AjouterImagesAsync(int produitId)
        {
            foreach (var image in Images.Where(img => !string.IsNullOrEmpty(img.NomImage) && !string.IsNullOrEmpty(img.UrlPhoto)))
            {
                try
                {
                    image.IdProduit = produitId;
                    await _imageService.AddAsync(image);
                    Console.WriteLine($"Image ajoutée : {image.NomImage}");
                }
                catch (Exception imgEx)
                {
                    Console.WriteLine($"Erreur lors de l'ajout de l'image {image.NomImage} : {imgEx.Message}");
                }
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