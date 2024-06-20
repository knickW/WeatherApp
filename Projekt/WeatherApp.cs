using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using System.Windows.Forms;
using Newtonsoft.Json.Linq;

namespace Projekt
{
	public partial class WeatherApp : Form
	{
		private const string openMeteoApiUrl = "https://api.open-meteo.com/v1/forecast";
		private const string azureMapsApiKey = "xxx"; //change this

		public WeatherApp()
		{
			InitializeComponent();
		}

		private async void submitButton_Click(object sender, EventArgs e)
		{
			string cityName = cityNameTextBox.Text;

			if (string.IsNullOrWhiteSpace(cityName))
			{
				MessageBox.Show("Wprowadź nazwę miasta.", "Błąd", MessageBoxButtons.OK, MessageBoxIcon.Error);
				return;
			}

			try
			{
				// Uzyskanie współrzędnych geograficznych na podstawie nazwy miasta z Azure Maps
				var coordinates = await GetCoordinates(cityName);

				if (coordinates != null)
				{
					// Pobranie danych pogodowych za pomocą API OpenMeteo
					var weatherInfo = await GetOpenMeteoWeather(coordinates.Latitude, coordinates.Longitude);

					// Przetwarzanie i wyświetlanie danych pogodowych
					string weatherInfoText = $"Pogoda w {cityName} ({coordinates.Latitude};{coordinates.Longitude}):\n" +
											 $"Aktualna temperatura: {weatherInfo.Current.Temperature2m}°C\n" +
											 $"Aktualna prędkość wiatru: {weatherInfo.Current.WindSpeed10m} km/h\n" +
											 "\nPrognoza godzinowa:\n";

					for (int i = 0; i < weatherInfo.Hourly.Time.Count; i++)
					{
						weatherInfoText += $"{weatherInfo.Hourly.Time[i]} - Temperatura: {weatherInfo.Hourly.Temperature2m[i]}°C, Wilgotność: {weatherInfo.Hourly.RelativeHumidity2m[i]}%, Prędkość wiatru: {weatherInfo.Hourly.WindSpeed10m[i]} km/h\n";
					}

					// Wyświetlanie w oknie Response
					Response.Text = weatherInfoText;
				}
				else
				{
					MessageBox.Show("Nie udało się uzyskać współrzędnych geograficznych dla podanego miasta.", "Błąd", MessageBoxButtons.OK, MessageBoxIcon.Error);
				}
			}
			catch (Exception ex)
			{
				MessageBox.Show($"Wystąpił błąd: {ex.Message}", "Błąd", MessageBoxButtons.OK, MessageBoxIcon.Error);
			}
		}

		private async Task<Coordinates> GetCoordinates(string cityName)
		{
			using (HttpClient httpClient = new HttpClient())
			{
				// Utwórz zapytanie do usługi Geocoding Azure Maps
				string geocodingUrl = $"https://atlas.microsoft.com/search/address/json?subscription-key={azureMapsApiKey}&api-version=1.0&query={cityName}";

				// Wyślij zapytanie HTTP GET
				string response = await httpClient.GetStringAsync(geocodingUrl);

				// Przetwórz odpowiedź JSON
				JObject jsonResponse = JObject.Parse(response);

				// Sprawdź, czy zapytanie zakończyło się sukcesem
				if (jsonResponse["results"]?.Count() > 0)
				{
					// Pobierz współrzędne geograficzne z odpowiedzi
					double latitude = (double)jsonResponse["results"][0]["position"]["lat"];
					double longitude = (double)jsonResponse["results"][0]["position"]["lon"];

					return new Coordinates(latitude, longitude);
				}
				else
				{
					return null;
				}
			}
		}

		private async Task<OpenMeteoWeatherInfo> GetOpenMeteoWeather(double latitude, double longitude)
		{
			using (HttpClient client = new HttpClient())
			{
				// Zamień przecinki na kropki w parametrze zapytania
				string latitudeString = latitude.ToString().Replace(',', '.');
				string longitudeString = longitude.ToString().Replace(',', '.');

				// Budowanie adresu URL dla zapytania o prognozę pogody za pomocą API OpenMeteo
				string apiUrl = $"{openMeteoApiUrl}?latitude={latitudeString}&longitude={longitudeString}&current=temperature_2m,wind_speed_10m&hourly=temperature_2m,relative_humidity_2m,wind_speed_10m";

				// Wyślij zapytanie HTTP GET do API OpenMeteo
				string response = await client.GetStringAsync(apiUrl);

				// Wyświetlenie informacji o wysłanym zapytaniu
				Response.Text = $"Wysłane zapytanie do OpenMeteo API:\n\n{apiUrl}";

				// Przetworzenie odpowiedzi JSON
				JObject jsonResponse = JObject.Parse(response);

				// Pobranie danych pogodowych z odpowiedzi
				var currentWeather = jsonResponse["current"];
				var hourlyForecast = jsonResponse["hourly"];

				// Zwróć obiekt zawierający informacje o pogodzie
				return new OpenMeteoWeatherInfo
				{
					Current = new OpenMeteoWeatherData
					{
						Temperature2m = currentWeather.Value<double>("temperature_2m"),
						WindSpeed10m = currentWeather.Value<double>("wind_speed_10m")
					},
					Hourly = new OpenMeteoHourlyData
					{
						Time = hourlyForecast["time"].ToObject<List<string>>(),
						Temperature2m = hourlyForecast["temperature_2m"].ToObject<List<double>>(),
						RelativeHumidity2m = hourlyForecast["relative_humidity_2m"].ToObject<List<int>>(),
						WindSpeed10m = hourlyForecast["wind_speed_10m"].ToObject<List<double>>()
					}
				};
			}
		}
	}

	public class OpenMeteoWeatherInfo
	{
		public OpenMeteoWeatherData Current { get; set; }
		public OpenMeteoHourlyData Hourly { get; set; }
	}

	public class OpenMeteoWeatherData
	{
		public double Temperature2m { get; set; }
		public double WindSpeed10m { get; set; }
	}

	public class OpenMeteoHourlyData
	{
		public List<string> Time { get; set; }
		public List<double> Temperature2m { get; set; }
		public List<int> RelativeHumidity2m { get; set; }
		public List<double> WindSpeed10m { get; set; }
	}

	internal class Coordinates
	{
		private double latitude;
		private double longitude;

		public double Latitude
		{
			get { return latitude; }
			set { latitude = value; }
		}

		public double Longitude
		{
			get { return longitude; }
			set { longitude = value; }
		}

		public Coordinates(double latitude, double longitude)
		{
			this.latitude = latitude;
			this.longitude = longitude;
		}
	}
}
