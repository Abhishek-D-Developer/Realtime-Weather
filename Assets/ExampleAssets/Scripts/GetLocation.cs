using System.Collections;
using UnityEngine;
using UnityEngine.Networking;
using TMPro;
using SimpleJSON;
using System;

public class GetLocation : MonoBehaviour
{
    public string apiKey;
    public string currentWeatherApi = "api.openweathermap.org/data/2.5/weather?";
    public GameObject clock;
    [Header("UI")]
    public TextMeshProUGUI statusText;
    public TextMeshProUGUI location;
    public TextMeshProUGUI mainWeather;
    public TextMeshProUGUI description;
    public TextMeshProUGUI temp;
    public TextMeshProUGUI feels_like;
    public TextMeshProUGUI temp_min;
    public TextMeshProUGUI temp_max;
    public TextMeshProUGUI pressure;
    public TextMeshProUGUI humidity;
    public TextMeshProUGUI windspeed;
    private LocationInfo lastLocation;

    [Space]
    public TextMeshProUGUI time;
    public TextMeshProUGUI seconds;
    public TextMeshProUGUI am;
    public TextMeshProUGUI date;
    public TextMeshProUGUI month;
    public TextMeshProUGUI Year;
    public TextMeshProUGUI temperature;
    public TextMeshProUGUI _location;


    public LocationServiceStatus locationSTatus;
    void Start()
    {
        Invoke("EnableClock", 3);
        StartCoroutine(FetchLocationData());
    }

    void EnableClock()
    {
        clock.SetActive(true);
    }

    private IEnumerator FetchLocationData()
    {
        // First, check if user has location service enabled
        if (!Input.location.isEnabledByUser)
        {
            Debug.Log("location off"+ locationSTatus);
            //yield break;
        }            
        // Start service before querying location
        Input.location.Start();
        // Wait until service initializes
        int maxWait = 20;
        while (Input.location.status == LocationServiceStatus.Initializing && maxWait > 0)
        {
            yield return new WaitForSeconds(1);
            maxWait--;
        }
        // Service didn't initialize in 20 seconds
        if (maxWait < 1)
        {
            statusText.text = "Location Timed out";
            yield break;
        }
        // Connection has failed
        if (Input.location.status == LocationServiceStatus.Failed)
        {
            statusText.text = "Unable to determine device location";
            yield break;
        }
        else
        {
            lastLocation = Input.location.lastData;
            UpdateWeatherData();
        }
        Input.location.Stop();
    }
    private void UpdateWeatherData()
    {
        StartCoroutine(FetchWeatherDataFromApi(lastLocation.latitude.ToString(), lastLocation.longitude.ToString()));
    }
    private IEnumerator FetchWeatherDataFromApi(string latitude, string longitude)
    {
        string url = currentWeatherApi + "lat=" + latitude + "&lon=" + longitude + "&appid=" + apiKey + "&units=metric";
        UnityWebRequest fetchWeatherRequest = UnityWebRequest.Get(url);
        yield return fetchWeatherRequest.SendWebRequest();
        if (fetchWeatherRequest.isNetworkError || fetchWeatherRequest.isHttpError)
        {
            //Check and print error
            statusText.text = fetchWeatherRequest.error;
        }
        else
        {
            Debug.Log(fetchWeatherRequest.downloadHandler.text);
            var response = JSON.Parse(fetchWeatherRequest.downloadHandler.text);
            location.text = response["name"];
            mainWeather.text = response["weather"][0]["main"];
            description.text = response["weather"][0]["description"];
            temp.text = response["main"]["temp"] + " C";
            feels_like.text = "Feels like " + response["main"]["feels_like"] + " C";
            temp_min.text = "Min is " + response["main"]["temp_min"] + " C";
            temp_max.text = "Max is " + response["main"]["temp_max"] + " C";
            pressure.text = "Pressure is " + response["main"]["pressure"] + " Pa";
            humidity.text = response["main"]["humidity"] + " % Humidity";
            windspeed.text = "Windspeed is " + response["wind"]["speed"] + " Km/h";


            temperature.text = response["main"]["temp_max"] + "° C";

        }
    }

    private void Update()
    {

        time.text = System.DateTime.Now.ToString("h:mm");
        seconds.text = System.DateTime.Now.ToString("ss");
        date.text = System.DateTime.Now.ToString("dd");
        month.text = System.DateTime.Now.ToString("MMMM");
        Year.text = System.DateTime.Now.ToString("yyy");

        DateTime currentTime = DateTime.Now;
        Debug.Log(currentTime.Hour);
        if (currentTime.Hour >= 12)
            am.text = "PM";
        else
            am.text = "AM";
        
    }
}