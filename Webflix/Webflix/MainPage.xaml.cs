using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Storage;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Imaging;
using Windows.UI.Xaml.Navigation;
using Windows.Web.Http;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace Webflix
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page, INotifyPropertyChanged
    {
        private MoviesRepository _repo;

        private Movie[] _inTheaters;
        public Movie[] InTheaters
        {
            get { return _inTheaters; }
            set { _inTheaters = value; OnPropertyChanged("InTheaters"); }
        }

        private Movie[] _topRentals;
        public Movie[] TopRentals
        {
            get { return _topRentals; }
            set { _topRentals = value; OnPropertyChanged("TopRentals"); }
        }

        private Movie[] _newReleases;
        public Movie[] NewReleases
        {
            get { return _newReleases; }
            set { _newReleases = value; OnPropertyChanged("NewReleases"); }
        }

        public IEnumerable<Movie> AllItems
        {
            get
            {
                return InTheaters.Union(TopRentals).Union(NewReleases);
            }
        }

        public ObservableCollection<Movie> Suggestions { get; private set; }

        public MainPage()
        {
            this.InitializeComponent();

            _repo = new MoviesRepository();
            DataContext = this;
            Suggestions = new ObservableCollection<Movie>();
        }

        protected override async void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
            InTheaters = await _repo.GetInTheatersMovies();
            TopRentals = await _repo.GetTopRentalMovies();
            NewReleases = await _repo.GetNewReleasesMovies();
        }


        private void AutoSuggestBox_TextChanged(AutoSuggestBox sender, AutoSuggestBoxTextChangedEventArgs args)
        {
            // TODO: not being called from flyout
            if (args.Reason == AutoSuggestionBoxTextChangeReason.UserInput)
            {
                Suggestions.Clear();
                foreach (var Item in AllItems)
                {
                    if (Item.title.IndexOf(sender.Text, StringComparison.OrdinalIgnoreCase) >= 0)
                    {
                        Suggestions.Add(Item);
                    }
                }
                
            }
        }


        private void AutoSuggestBox_QuerySubmitted(AutoSuggestBox sender, AutoSuggestBoxQuerySubmittedEventArgs args)
        {
            Movie FoundItem = null;

            if (args.ChosenSuggestion != null && args.ChosenSuggestion is Movie)
            {
                FoundItem = args.ChosenSuggestion as Movie;
            }
            else if (String.IsNullOrEmpty(args.QueryText) == false)
            {
                foreach (var Item in AllItems)
                {
                    if (Item.title.Equals(args.QueryText, StringComparison.OrdinalIgnoreCase))
                    {
                        FoundItem = Item;
                        break;
                    }
                }
            }

            ShowItem(FoundItem);
        }

        async private void ShowItem(Movie model)
        {
            var MyDialog = new ContentDialog();

            if (model != null)
            {
                var MyImage = new Image();
                MyImage.Source = new BitmapImage(new Uri(model.posters.primary));
                MyImage.HorizontalAlignment = HorizontalAlignment.Stretch;
                MyImage.VerticalAlignment = VerticalAlignment.Stretch;
                MyImage.Stretch = Windows.UI.Xaml.Media.Stretch.UniformToFill;
                MyDialog.Title = String.Format("You have selected {0}", model.title);
                MyDialog.Content = MyImage;
            }
            else
            {
                MyDialog.Title = "No item found";
            }

            MyDialog.PrimaryButtonText = "OK";
            await Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, async () => await MyDialog.ShowAsync());

        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged(string propertyName)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }
    }

    public class MoviesRepository
    {
        public Task<Movie[]> GetInTheatersMovies()
        {
            return GetFromFile("ms-appx:///Data/in_theaters.json");
        }

        public Task<Movie[]> GetTopRentalMovies()
        {
            return GetFromFile("ms-appx:///Data/top_rentals.json");
        }

        public Task<Movie[]> GetNewReleasesMovies()
        {
            return GetFromFile("ms-appx:///Data/new_releases.json");
        }

        private async Task<Movie[]> GetFromFile(string fileUrl)
        {
            var file = await StorageFile.GetFileFromApplicationUriAsync(new Uri(fileUrl));
            var json = await FileIO.ReadTextAsync(file);
            var root = JsonConvert.DeserializeObject<Rootobject>(json);
            return root.results;
        }
    }


    public class Rootobject
    {
        public Movie[] results { get; set; }
        public Counts counts { get; set; }
    }

    public class Counts
    {
        public int total { get; set; }
        public int count { get; set; }
    }

    public class Movie
    {
        public int popcornScore { get; set; }
        public int tomatoScore { get; set; }
        public string mpaaRating { get; set; }
        public string runtime { get; set; }
        public string synopsisType { get; set; }
        public string url { get; set; }
        public int id { get; set; }
        public string tomatoIcon { get; set; }
        public string title { get; set; }
        public string theaterReleaseDate { get; set; }
        public string synopsis { get; set; }
        public string dvdReleaseDate { get; set; }
        public string[] actors { get; set; }
        public Posters posters { get; set; }

        public override string ToString()
        {
            return title;
        }
    }

    public class Posters
    {
        public string primary { get; set; }
        public string secondary { get; set; }
    }

}
