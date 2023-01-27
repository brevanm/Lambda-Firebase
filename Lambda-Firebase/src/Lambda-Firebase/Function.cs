using Amazon.Lambda.CloudWatchEvents;
using Amazon.Lambda.Core;
using Google.Cloud.Firestore;
using Lambda_Firebase.Models;
using Newtonsoft.Json;
using RestSharp;
using RestSharp.Serializers.SystemTextJson;

// Assembly attribute to enable the Lambda function's JSON input to be converted into a .NET class.
[assembly: LambdaSerializer(typeof(Amazon.Lambda.Serialization.SystemTextJson.DefaultLambdaJsonSerializer))]

namespace Lambda_Firebase;

public class Function
{
    private readonly RestClient _dineOnCampusClient;
    private readonly string[] halls = { "Sargent", "Allison", "Elder", "Plex West" };
    private readonly Dictionary<string, string> ids;
    public Dictionary<string, DiningHall> data;
    public string date;

    public Function()
    {
        _dineOnCampusClient = new RestClient("https://api.dineoncampus.com/v1");
        
        ids = new Dictionary<string, string>
        {
            { "Allison", "5b33ae291178e909d807593d" },
            { "Sargent", "5b33ae291178e909d807593e" },
            { "Elder", "5d113c924198d409c34fdf5c" },
            { "Plex West", "5bae7de3f3eeb60c7d3854ba" },
        };

        data = new Dictionary<string, DiningHall>
        {
            { "Allison", new DiningHall() },
            { "Sargent", new DiningHall() },
            { "Elder", new DiningHall() },
            { "Plex West", new DiningHall() },
        };
    }
 
    public async Task FunctionHandler(CloudWatchEvent<object> input, ILambdaContext context)
    {
        date = DateTime.Now.ToString("yyyy-MM-dd");
        context.Logger.LogInformation($"Received call at {date}");

        FirestoreDb db = await FirestoreDb.CreateAsync("dininginformate");
        CollectionReference collection = db.Collection("Dining Halls");
        
        context.Logger.LogInformation($"Starting breaky fetches");
        // Fetch breakfast menus
        var tasks = halls.Select((hall) => CallApi($"location/{ids[hall]}/periods?platform=0&date={date}")).ToArray();
        context.Logger.LogInformation($"Awaiting breakfast responses");
        await Task.WhenAll(tasks);
        context.Logger.LogInformation($"Received breakfast responses");
        
        var lunchIds = new List<string>();
        var dinnerIds = new List<string>();
        // Add breakfast menus to data
        int index = 0;
        foreach (var t in tasks) {
            var postResponse = await t;
            lunchIds.Add(postResponse.getLunchId());
            dinnerIds.Add(postResponse.getDinnerId());
            data[halls[index]].breakfast = postResponse.menu.periods.categories;
            index++;
        }
        context.Logger.LogInformation($"Unpacked breakfast responses");
        context.Logger.LogInformation($"Calling lunch and dinner");

        index = 0;
        var lunchTasks = new List<Task<RootObject>>();
        var dinnerTasks = new List<Task<RootObject>>();
        foreach (var hall in halls)
        {
            lunchTasks.Add(CallApi($"location/{ids[hall]}/periods/{lunchIds[index]}?platform=0&date={date}"));
            dinnerTasks.Add(CallApi($"location/{ids[hall]}/periods/{dinnerIds[index]}?platform=0&date={date}"));
            index++;
        }
        await Task.WhenAll(lunchTasks);
        await Task.WhenAll(dinnerTasks);

        context.Logger.LogInformation($"Received lunch and dinner");
        index = 0;
        foreach (var hall in halls)
        {
            var lunchResponse = await lunchTasks[index];
            var dinnerResponse = await dinnerTasks[index];
            data[hall].lunch = lunchResponse.menu.periods.categories;
            data[hall].dinner = dinnerResponse.menu.periods.categories;
            data[hall].date = dinnerResponse.menu.date;
            index++;
        }
        context.Logger.LogInformation($"Unpacked lunch and dinner responses");

        foreach (var hall in halls)
        {
            DocumentReference doc = collection.Document(hall);
            Dictionary<string, object> updates = new Dictionary<string, object>
            {
                { "menu", data[hall] }
            };
            await doc.UpdateAsync(updates);
        }
        context.Logger.LogInformation($"Successfully wrote to firebase");
    }

    private async Task<RootObject> CallApi(string url)
    {
        var request = new RestRequest(url, Method.Get);
        var response = await _dineOnCampusClient.GetAsync<RootObject>(request);
        if (response == null)
        {
            throw new Exception("Error fetching data");
        }
        return response;
    }
}