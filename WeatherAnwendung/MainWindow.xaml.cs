using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Xml;
using System.Xml.Serialization;


namespace WeatherAnwendung
{
    /// <summary>
    /// Interaktionslogik für MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private string apiKey = "your API key";

        private List<WheatherData> cityList = new List<WheatherData>();

        readonly List<string> TempList = new List<string> { "Celsius", "Fahrenheit", "Kelvin"};
        private List<string> citynames = new List<string>();

        private WheatherData aktuelleCityDaten;

        public MainWindow()
        {
            InitializeComponent();

            lb_wheatherData.ItemsSource = cityList;
            cb_temperatur.ItemsSource = TempList;
            tb_stadtName.Focus();
        }
        private void getWheatherDataFrom(string cityname)
        {
            try
            {
                var currentCulture = System.Globalization.CultureInfo.InstalledUICulture;
                var numberFormat = (System.Globalization.NumberFormatInfo)currentCulture.NumberFormat.Clone();
                numberFormat.NumberDecimalSeparator = ".";

                string url = "https://api.openweathermap.org/data/2.5/weather?q=" + cityname + "&units=metric&mode=xml&appid=" + apiKey;
                XmlDocument responseAPI = new XmlDocument();
                responseAPI.Load(url);

                WheatherData city = new WheatherData
                {
                    cityID = int.Parse(responseAPI["current"]["city"].Attributes["id"].Value),
                    cityName = responseAPI["current"]["city"].Attributes["name"].Value,
                    countryName = responseAPI["current"]["city"]["country"].InnerText,
                    aktuellTemp = double.Parse(responseAPI["current"]["temperature"].Attributes["value"].Value, numberFormat),
                    minTemp = double.Parse(responseAPI["current"]["temperature"].Attributes["min"].Value, numberFormat),
                    maxTemp = double.Parse(responseAPI["current"]["temperature"].Attributes["max"].Value, numberFormat),
                    clouds = responseAPI["current"]["clouds"].Attributes["name"].Value,
                    lastAbdate = DateTime.Parse(responseAPI["current"]["lastupdate"].Attributes["value"].Value)
                };

                if (IsCityExist(city.cityID))
                {
                    //MessageBox.Show("Sie haben bereits diese Stadt hinzugefügt, bitte tragen Sie noch einen anderen Stadtnamen ein", "Warnung", MessageBoxButton.OK, MessageBoxImage.Warning);
                    //return;
                    throw new CityAlreadyException();
                }
                cityList.Add(city);

                if (!citynames.Contains(cityname))
                {
                    citynames.Add(cityname);
                }
                lb_wheatherData.Items.Refresh();
            }
            catch (System.Net.WebException we)
            {
                //MessageBox.Show("kein gültiger Stadtname oder keine Internetverbindung");
                MessageBox.Show(we.Message);
                return;
            }
            catch (CityAlreadyException cae) //meine eigene Exception
            {
                MessageBox.Show(cae.Message,"Meine Eigene Exception", MessageBoxButton.OK,MessageBoxImage.Exclamation);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Irdendein Fehler ist aufgetreten");
            }
        }
        private void bt_hinzufügen_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                lab_message.Visibility = Visibility.Hidden;
                lab_aktualisiert.Visibility = Visibility.Hidden;
                if (tb_stadtName.Text != "")
                {
                    getWheatherDataFrom(tb_stadtName.Text);
                    FillinTheBlanks(cityList[cityList.Count() - 1]);
                    tb_stadtName.Text = "";
                }
                else
                {
                    MessageBox.Show("Bitte tragen Sie einen Stadtnamen ein!", "", MessageBoxButton.OK, MessageBoxImage.Information);
                }
            }
            catch (Exception)
            {

            }
        }
        private void bt_leeren_Click(object sender, RoutedEventArgs e)
        {
            lab_message.Visibility = Visibility.Hidden;
            lab_aktualisiert.Visibility = Visibility.Hidden;
            tb_stadtName.Text = "";
            tb_LandName.Text = "";
            tb_Temp.Text = "";
            tb_MaxTemp.Text = "";
            tb_MinTemp.Text = "";
            tb_StadtID.Text = "";
            tb_letzteAktual.Text = "";
            tb_wetterZustand.Text = "";
            cb_temperatur.SelectedItem = "";
            aktuelleCityDaten = null;
            cityList.Clear();
            citynames.Clear();

            lb_wheatherData.Items.Refresh();

            tb_stadtName.Focus();
        }
        private void bt_export_Click(object sender, RoutedEventArgs e)
        {
            //TextWriter dataWriter = null;
            XmlSerializer wetterSerializer = new XmlSerializer(typeof(List<WheatherData>));
            TextWriter dataWriter = new StreamWriter("C:\\Users\\admin\\Desktop\\Wetterbericht.xml");
            wetterSerializer.Serialize(dataWriter, cityList );
            dataWriter.Close();
            lab_message.Visibility = Visibility.Visible;
        }
        private void bt_aktualisieren_Click(object sender, RoutedEventArgs e)
        {
            cityList.Clear();
            foreach (var item in citynames)
            {
                getWheatherDataFrom(item);
                MessageBox.Show("Stadt: "+item+" wird aktualisiert...");
            }
            lb_wheatherData.Items.Refresh();
            lab_aktualisiert.Visibility = Visibility.Visible;
            //MessageBox.Show("Daten wurden erfolgreich aktualisiert...");
        }
        private void bt_exit_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("Vielen Dank, dass Sie unser Programm verwendet haben.");
            Environment.Exit(0);
        }
        private void FillinTheBlanks(WheatherData city)
        {
            tb_stadtName.Text = city.cityName;
            tb_StadtID.Text = city.cityID.ToString();
            tb_Temp.Text = city.aktuellTemp.ToString();
            tb_MaxTemp.Text = city.maxTemp.ToString();
            tb_MinTemp.Text = city.minTemp.ToString();
            tb_letzteAktual.Text = city.lastAbdate.ToString();

            aktuelleCityDaten = city;
            cb_temperatur.SelectedItem = "Celsius";

            switch (city.clouds)
            {
                case "broken clouds":
                    tb_wetterZustand.Text = "teilweise wolkig";
                    break;
                case "clear sky":
                    tb_wetterZustand.Text = "wolkenlos";
                    break;
                case "overcast clouds":
                    tb_wetterZustand.Text = "stark bewölkt";
                    break;
                case "few clouds":
                    tb_wetterZustand.Text = "leicht bewölkt";
                    break;
                case "scattered clouds":
                    tb_wetterZustand.Text = "vereinzelte Wolken";
                    break;
            }

            switch (city.countryName)
            {
                case "DE":
                    tb_LandName.Text = "Deutschland";
                    break;
                case "TR":
                    tb_LandName.Text = "Türkei";
                    break;
                case "ES":
                    tb_LandName.Text = "Spanien";
                    break;
                case "FR":
                    tb_LandName.Text = "Frankreich";
                    break;
                case "US":
                    tb_LandName.Text = "Vereinigte Staaten von Amerika";
                    break;
                case "IT":
                    tb_LandName.Text = "Italien";
                    break;
                case "JP":
                    tb_LandName.Text = "Japan";
                    break;
            }
        }
        private void SendDataToTheBoxes(object sender, SelectionChangedEventArgs e)
        {
          
            WheatherData selected = (WheatherData)lb_wheatherData.SelectedItem;
            if (selected != null)
            {
                FillinTheBlanks(selected);
            }
        }
        private bool IsCityExist(int cityID)
        {
            foreach (var item in cityList)
            {
                if (item.cityID==cityID)
                {
                    return true;
                }
            }
            return false;
        }
        private void cb_temperatur_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (aktuelleCityDaten == null)
            {
                return;
            }
            if (cb_temperatur.SelectedItem.ToString() == "Kelvin")
            {
                tb_Temp.Text = (aktuelleCityDaten.aktuellTemp + 273.15).ToString();
                tb_MinTemp.Text = (aktuelleCityDaten.minTemp + 273.15).ToString();
                tb_MaxTemp.Text = (aktuelleCityDaten.maxTemp + 273.15).ToString();
            }
            else if (cb_temperatur.SelectedItem.ToString() == "Fahrenheit")
            {
                tb_Temp.Text = (aktuelleCityDaten.aktuellTemp * 1.8 + 32).ToString();
                tb_MinTemp.Text = (aktuelleCityDaten.minTemp * 1.8 + 32).ToString();
                tb_MaxTemp.Text = (aktuelleCityDaten.maxTemp * 1.8 + 32).ToString();
            }
            else
            {
                tb_Temp.Text = aktuelleCityDaten.aktuellTemp.ToString();
                tb_MaxTemp.Text = aktuelleCityDaten.maxTemp.ToString();
                tb_MinTemp.Text = aktuelleCityDaten.minTemp.ToString();
            }

        }

        
    }
}
