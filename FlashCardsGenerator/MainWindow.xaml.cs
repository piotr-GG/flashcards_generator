using System.Collections.ObjectModel;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Microsoft.Web.WebView2.Core;
using Microsoft.Web.WebView2.WinForms;
using Microsoft.Win32;
using static System.Net.WebRequestMethods;
using Microsoft.VisualBasic;

namespace FlashCardsGenerator
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private const string ENG_URL_PREFIX = "https://www.diki.pl/slownik-angielskiego";
        public ObservableCollection<FlashcardItem> Items { get; set; }

        public MainWindow()
        {
            InitializeComponent();
            InitializeWebView();

            Items = [];
            OverviewTable.ItemsSource = Items;
        }

        private async void InitializeWebView()
        {
            try
            {
                await Browser.EnsureCoreWebView2Async();
                Browser.CoreWebView2.Navigate(ENG_URL_PREFIX);

                await Browser.CoreWebView2.AddScriptToExecuteOnDocumentCreatedAsync(@"
                    document.addEventListener('keydown', function(event) {
                        if (['1', '2', '3', '4'].includes(event.key)) {
                            window.chrome.webview.postMessage(event.key);
                        }
                    });
                ");

                Browser.CoreWebView2.WebMessageReceived += CoreWebView2_WebMessageReceived;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"WebView Initialization failed with message: {ex.Message}", "Error");
            }
        }

        private void CoreWebView2_WebMessageReceived(object? sender, CoreWebView2WebMessageReceivedEventArgs e)
        {
            
            string key = e.TryGetWebMessageAsString();
            if (!string.IsNullOrEmpty(key))
            {
                HandleKeyPress(key);
            }
        }

        private void SearchForButton_Click(object sender, RoutedEventArgs e)
        {
            string searchPhrase = SearchPhraseBox.Text;
            if (!string.IsNullOrWhiteSpace(searchPhrase))
            {
                string new_url = ENG_URL_PREFIX + "\\?q=" + searchPhrase;

                try
                {
                    Browser.CoreWebView2.Navigate(new_url);
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"WebView Initialization failed with message: {ex.Message}", "Error");
                }
            }
        }

        private async void HandleKeyPress(string key)
        {
            string selectedText = await GetSelectedTextInBrowser();
            switch (key)
            {
                case "1":
                    foreignWordBox.Text = selectedText;
                    break;
                case "2":
                    meaningWordBox.Text = selectedText;
                    break;
                case "3":
                    foreignExampleBox.Text = selectedText;
                    break;
                case "4":
                    meaningExampleBox.Text = selectedText;
                    break;
                default:
                    break;
            }
        }
        private async void mainWindow_KeyUp(object sender, KeyEventArgs e)
        {
            string selectedText = await GetSelectedTextInBrowser();
            Console.WriteLine($"Pressing {e.Key}");
            switch(e.Key)
            {
                case Key.Enter:
                    break;
                case Key.D1:
                    foreignWordBox.Text = selectedText;
                    break;
                case Key.D2:
                    meaningWordBox.Text = selectedText;
                    break;
                case Key.D3:
                    foreignExampleBox.Text = selectedText;  
                    break;
                case Key.D4:
                    meaningExampleBox.Text = selectedText;
                    break;
                default:
                    break;
            }
        }


        private async Task<string> GetSelectedTextInBrowser()
        {
            string response = await Browser.ExecuteScriptAsync("window.getSelection().toString()");
            return response.Substring(1, response.Length - 2).Trim();
        }

        private async void AddButton_Click(object sender, RoutedEventArgs e)
        {
            string originalWord = foreignWordBox.Text;
            string translatedWord = meaningWordBox.Text;
            string example = foreignExampleBox.Text;
            string translatedExample = meaningExampleBox.Text;

            AddElementToTable(originalWord, translatedWord, example, translatedExample);    
        }

        private void AddElementToTable(string originalWord, string translatedWord, string originalExample,  string translatedExample)
        {
            Items.Add(new FlashcardItem(originalWord, translatedWord, originalExample, translatedExample));
        }

        private void ExportToCSV_Click(object sender, RoutedEventArgs e)
        {
            SaveFileDialog saveFileDialog = new SaveFileDialog
            {
                Filter = "CSV files (*.csv)|*.csv|All files(*.*)|*.*",
                DefaultExt=".csv",
                Title="Save CSV file"
            };
            
            if (saveFileDialog.ShowDialog() == true)
            {
                string filePath = saveFileDialog.FileName;
                try
                {
                    CSVWriter.WriteToCSV<FlashcardItem>(filePath, Items);
                    MessageBox.Show($"Successfully saved CSV file under {filePath} location", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Failed to save CSV file with error message : {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        private void ImportFromCSV_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog
            {
                Filter = "CSV files (*.csv)|*.csv|All files(*.*)|*.*",
                DefaultExt = ".csv",
                Title = "Import from CSV file"
            };

            if (openFileDialog.ShowDialog() == true)
            {
                string filePath = openFileDialog.FileName;
                try
                {
                    string separator = Interaction.InputBox("Please specify the separator used in CSV file: ", "Specify separator", ";");
                    if (!string.IsNullOrEmpty(separator))
                    { 
                        MessageBoxResult result = MessageBox.Show("Do you want to overwrite the existing cards?", "Overwrite cards", MessageBoxButton.YesNo, MessageBoxImage.Question);

                        List<string> csvRows = CSVReader.ReadFromCSV(filePath);
                        ObservableCollection<FlashcardItem> temp = [];
                        foreach (string csvRow in csvRows)
                        {
                            
                            temp.Add(FlashcardFactory.FromCSV(csvRow, separator));
                        }

                        if (result == MessageBoxResult.Yes)
                        {
                            Items.Clear();
                        } 

                        foreach(FlashcardItem item in temp)
                        {
                            Items.Add(item);
                        }
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Failed to import data from CSV file with error message : {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }


    }
}