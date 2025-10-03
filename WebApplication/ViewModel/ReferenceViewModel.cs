using System.ComponentModel;
using System.Runtime.CompilerServices;
using WebApplication.Models;
using WebApplication.Services;

namespace WebApplication.ViewModels
{
    public class ReferenceViewModel : INotifyPropertyChanged
    {
        private readonly IGenericService<Marque> _marqueService;
        private readonly IGenericService<TypeProduit> _typeService;
        private readonly INotificationService _notificationService;
        private readonly HttpClient _httpClient;

        public event PropertyChangedEventHandler? PropertyChanged;

        #region Propriétés principales

        private IEnumerable<Marque>? _marques;
        public IEnumerable<Marque>? Marques
        {
            get => _marques;
            set
            {
                SetProperty(ref _marques, value);
                OnPropertyChanged(nameof(NombreMarques));
            }
        }

        private IEnumerable<TypeProduit>? _typesProduit;
        public IEnumerable<TypeProduit>? TypesProduit
        {
            get => _typesProduit;
            set
            {
                SetProperty(ref _typesProduit, value);
                OnPropertyChanged(nameof(NombreTypes));
            }
        }

        public int NombreMarques => Marques?.Count() ?? 0;
        public int NombreTypes => TypesProduit?.Count() ?? 0;

        #endregion

        #region Propriétés Modal Marque

        private bool _showModalMarque;
        public bool ShowModalMarque
        {
            get => _showModalMarque;
            set => SetProperty(ref _showModalMarque, value);
        }

        private bool _modeEditionMarque;
        public bool ModeEditionMarque
        {
            get => _modeEditionMarque;
            set
            {
                SetProperty(ref _modeEditionMarque, value);
                OnPropertyChanged(nameof(TitreModalMarque));
                OnPropertyChanged(nameof(TexteBoutonMarque));
                OnPropertyChanged(nameof(IconeBoutonMarque));
            }
        }

        private Marque _marqueEnCours = new();
        public Marque MarqueEnCours
        {
            get => _marqueEnCours;
            set => SetProperty(ref _marqueEnCours, value);
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

        public string TitreModalMarque => ModeEditionMarque ? "Modifier la marque" : "Ajouter une marque";
        public string TexteBoutonMarque => IsSubmittingMarque
            ? (ModeEditionMarque ? "Modification..." : "Ajout...")
            : (ModeEditionMarque ? "Modifier" : "Ajouter");
        public string IconeBoutonMarque => ModeEditionMarque ? "fa-save" : "fa-plus";

        #endregion

        #region Propriétés Modal Type

        private bool _showModalType;
        public bool ShowModalType
        {
            get => _showModalType;
            set => SetProperty(ref _showModalType, value);
        }

        private bool _modeEditionType;
        public bool ModeEditionType
        {
            get => _modeEditionType;
            set
            {
                SetProperty(ref _modeEditionType, value);
                OnPropertyChanged(nameof(TitreModalType));
                OnPropertyChanged(nameof(TexteBoutonType));
                OnPropertyChanged(nameof(IconeBoutonType));
            }
        }

        private TypeProduit _typeEnCours = new();
        public TypeProduit TypeEnCours
        {
            get => _typeEnCours;
            set => SetProperty(ref _typeEnCours, value);
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

        public string TitreModalType => ModeEditionType ? "Modifier le type de produit" : "Ajouter un type de produit";
        public string TexteBoutonType => IsSubmittingType
            ? (ModeEditionType ? "Modification..." : "Ajout...")
            : (ModeEditionType ? "Modifier" : "Ajouter");
        public string IconeBoutonType => ModeEditionType ? "fa-save" : "fa-plus";

        #endregion

        #region Propriétés Modal Suppression Marque

        private bool _showModalSuppressionMarque;
        public bool ShowModalSuppressionMarque
        {
            get => _showModalSuppressionMarque;
            set => SetProperty(ref _showModalSuppressionMarque, value);
        }

        private Marque? _marqueASupprimer;
        public Marque? MarqueASupprimer
        {
            get => _marqueASupprimer;
            set => SetProperty(ref _marqueASupprimer, value);
        }

        private bool _isSubmittingSuppressionMarque;
        public bool IsSubmittingSuppressionMarque
        {
            get => _isSubmittingSuppressionMarque;
            set => SetProperty(ref _isSubmittingSuppressionMarque, value);
        }

        private int _nombreProduitsMarque;
        public int NombreProduitsMarque
        {
            get => _nombreProduitsMarque;
            set => SetProperty(ref _nombreProduitsMarque, value);
        }

        #endregion

        #region Propriétés Modal Suppression Type

        private bool _showModalSuppressionType;
        public bool ShowModalSuppressionType
        {
            get => _showModalSuppressionType;
            set => SetProperty(ref _showModalSuppressionType, value);
        }

        private TypeProduit? _typeASupprimer;
        public TypeProduit? TypeASupprimer
        {
            get => _typeASupprimer;
            set => SetProperty(ref _typeASupprimer, value);
        }

        private bool _isSubmittingSuppressionType;
        public bool IsSubmittingSuppressionType
        {
            get => _isSubmittingSuppressionType;
            set => SetProperty(ref _isSubmittingSuppressionType, value);
        }

        private int _nombreProduitsType;
        public int NombreProduitsType
        {
            get => _nombreProduitsType;
            set => SetProperty(ref _nombreProduitsType, value);
        }

        #endregion

        public ReferenceViewModel(
            IGenericService<Marque> marqueService,
            IGenericService<TypeProduit> typeService,
            INotificationService notificationService,
            HttpClient httpClient)
        {
            _marqueService = marqueService;
            _typeService = typeService;
            _notificationService = notificationService;
            _httpClient = httpClient;
        }

        #region Initialisation

        public async Task InitialiserAsync()
        {
            await ChargerDonneesAsync();
        }

        private async Task ChargerDonneesAsync()
        {
            try
            {
                Marques = await _marqueService.GetAllAsync();
                TypesProduit = await _typeService.GetAllAsync();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Erreur lors du chargement des données : {ex.Message}");
                _notificationService.ShowError("Erreur lors du chargement des données");
            }
        }

        #endregion

        #region Gestion des Marques

        public void OuvrirModalAjoutMarque()
        {
            ModeEditionMarque = false;
            MarqueEnCours = new Marque();
            ErrorMessageMarque = "";
            ShowModalMarque = true;
        }

        public void OuvrirModalModificationMarque(Marque marque)
        {
            ModeEditionMarque = true;
            MarqueEnCours = new Marque
            {
                IdMarque = marque.IdMarque,
                Nom = marque.Nom,
                NbProduits = marque.NbProduits
            };
            ErrorMessageMarque = "";
            ShowModalMarque = true;
        }

        public void FermerModalMarque()
        {
            ShowModalMarque = false;
            ModeEditionMarque = false;
            MarqueEnCours = new Marque();
            ErrorMessageMarque = "";
            IsSubmittingMarque = false;
        }

        public async Task<bool> ValiderMarqueAsync()
        {
            IsSubmittingMarque = true;
            ErrorMessageMarque = "";

            try
            {
                if (string.IsNullOrWhiteSpace(MarqueEnCours.Nom))
                {
                    ErrorMessageMarque = "Le nom de la marque est obligatoire.";
                    return false;
                }

                var marqueExistante = Marques?.FirstOrDefault(m =>
                    m.Nom?.Equals(MarqueEnCours.Nom, StringComparison.OrdinalIgnoreCase) == true &&
                    m.IdMarque != MarqueEnCours.IdMarque);

                if (marqueExistante != null)
                {
                    ErrorMessageMarque = "Cette marque existe déjà.";
                    return false;
                }

                if (ModeEditionMarque)
                {
                    await _marqueService.UpdateAsync(MarqueEnCours.IdMarque, MarqueEnCours);
                    _notificationService.ShowSuccess("Marque modifiée avec succès");
                }
                else
                {
                    await _marqueService.AddAsync(MarqueEnCours);
                    _notificationService.ShowSuccess("Marque ajoutée avec succès");
                }

                await ChargerDonneesAsync();
                FermerModalMarque();
                return true;
            }
            catch (Exception ex)
            {
                ErrorMessageMarque = $"Erreur : {ex.Message}";
                _notificationService.ShowError("Erreur lors de l'opération");
                return false;
            }
            finally
            {
                IsSubmittingMarque = false;
            }
        }

        public async Task OuvrirModalSuppressionMarque(Marque marque)
        {
            try
            {
                // Récupérer le nombre de produits liés à cette marque
                var response = await _httpClient.GetAsync($"api/Marque/GetProduitsCount/{marque.IdMarque}");
                if (response.IsSuccessStatusCode)
                {
                    var content = await response.Content.ReadAsStringAsync();
                    NombreProduitsMarque = int.Parse(content);
                }
                else
                {
                    NombreProduitsMarque = 0;
                }

                MarqueASupprimer = marque;
                ShowModalSuppressionMarque = true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Erreur lors de la récupération du nombre de produits : {ex.Message}");
                _notificationService.ShowError("Erreur lors de la préparation de la suppression");
            }
        }

        public void FermerModalSuppressionMarque()
        {
            ShowModalSuppressionMarque = false;
            MarqueASupprimer = null;
            IsSubmittingSuppressionMarque = false;
            NombreProduitsMarque = 0;
        }

        public async Task<bool> ConfirmerSuppressionMarqueAsync()
        {
            if (MarqueASupprimer == null) return false;

            IsSubmittingSuppressionMarque = true;

            try
            {
                await _marqueService.DeleteAsync(MarqueASupprimer.IdMarque);
                await ChargerDonneesAsync();

                var message = NombreProduitsMarque > 0
                    ? $"Marque et {NombreProduitsMarque} produit(s) supprimé(s) avec succès"
                    : "Marque supprimée avec succès";

                _notificationService.ShowSuccess(message);

                FermerModalSuppressionMarque();
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Erreur lors de la suppression : {ex.Message}");
                _notificationService.ShowError("Erreur lors de la suppression");
                return false;
            }
            finally
            {
                IsSubmittingSuppressionMarque = false;
            }
        }

        #endregion

        #region Gestion des Types

        public void OuvrirModalAjoutType()
        {
            ModeEditionType = false;
            TypeEnCours = new TypeProduit();
            ErrorMessageType = "";
            ShowModalType = true;
        }

        public void OuvrirModalModificationType(TypeProduit type)
        {
            ModeEditionType = true;
            TypeEnCours = new TypeProduit
            {
                IdTypeProduit = type.IdTypeProduit,
                Nom = type.Nom,
                NbProduits = type.NbProduits
            };
            ErrorMessageType = "";
            ShowModalType = true;
        }

        public void FermerModalType()
        {
            ShowModalType = false;
            ModeEditionType = false;
            TypeEnCours = new TypeProduit();
            ErrorMessageType = "";
            IsSubmittingType = false;
        }

        public async Task<bool> ValiderTypeAsync()
        {
            IsSubmittingType = true;
            ErrorMessageType = "";

            try
            {
                if (string.IsNullOrWhiteSpace(TypeEnCours.Nom))
                {
                    ErrorMessageType = "Le nom du type de produit est obligatoire.";
                    return false;
                }

                var typeExistant = TypesProduit?.FirstOrDefault(t =>
                    t.Nom?.Equals(TypeEnCours.Nom, StringComparison.OrdinalIgnoreCase) == true &&
                    t.IdTypeProduit != TypeEnCours.IdTypeProduit);

                if (typeExistant != null)
                {
                    ErrorMessageType = "Ce type de produit existe déjà.";
                    return false;
                }

                if (ModeEditionType)
                {
                    await _typeService.UpdateAsync(TypeEnCours.IdTypeProduit, TypeEnCours);
                    _notificationService.ShowSuccess("Type de produit modifié avec succès");
                }
                else
                {
                    await _typeService.AddAsync(TypeEnCours);
                    _notificationService.ShowSuccess("Type de produit ajouté avec succès");
                }

                await ChargerDonneesAsync();
                FermerModalType();
                return true;
            }
            catch (Exception ex)
            {
                ErrorMessageType = $"Erreur : {ex.Message}";
                _notificationService.ShowError("Erreur lors de l'opération");
                return false;
            }
            finally
            {
                IsSubmittingType = false;
            }
        }

        public async Task OuvrirModalSuppressionType(TypeProduit type)
        {
            try
            {
                // Récupérer le nombre de produits liés à ce type
                var response = await _httpClient.GetAsync($"api/TypeProduit/GetProduitsCount/{type.IdTypeProduit}");
                if (response.IsSuccessStatusCode)
                {
                    var content = await response.Content.ReadAsStringAsync();
                    NombreProduitsType = int.Parse(content);
                }
                else
                {
                    NombreProduitsType = 0;
                }

                TypeASupprimer = type;
                ShowModalSuppressionType = true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Erreur lors de la récupération du nombre de produits : {ex.Message}");
                _notificationService.ShowError("Erreur lors de la préparation de la suppression");
            }
        }

        public void FermerModalSuppressionType()
        {
            ShowModalSuppressionType = false;
            TypeASupprimer = null;
            IsSubmittingSuppressionType = false;
            NombreProduitsType = 0;
        }

        public async Task<bool> ConfirmerSuppressionTypeAsync()
        {
            if (TypeASupprimer == null) return false;

            IsSubmittingSuppressionType = true;

            try
            {
                await _typeService.DeleteAsync(TypeASupprimer.IdTypeProduit);
                await ChargerDonneesAsync();

                var message = NombreProduitsType > 0
                    ? $"Type de produit et {NombreProduitsType} produit(s) supprimé(s) avec succès"
                    : "Type de produit supprimé avec succès";

                _notificationService.ShowSuccess(message);

                FermerModalSuppressionType();
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Erreur lors de la suppression : {ex.Message}");
                _notificationService.ShowError("Erreur lors de la suppression");
                return false;
            }
            finally
            {
                IsSubmittingSuppressionType = false;
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