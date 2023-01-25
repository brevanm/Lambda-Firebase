using System.Text.Json.Serialization;
using Google.Cloud.Firestore;

namespace Lambda_Firebase.Models;

[FirestoreData]
public class DiningHall
{
    [FirestoreProperty]
    public string date { get; set; }
    [FirestoreProperty]
    public Categories[] breakfast { get; set; }
    [FirestoreProperty]
    public Categories[] lunch { get; set; }
    [FirestoreProperty]
    public Categories[] dinner { get; set; }
}

[FirestoreData]
public class RootObject
{
    [FirestoreProperty]
    public string status { get; set; }
    [FirestoreProperty]
    public double request_time { get; set; }
    [FirestoreProperty]
    public int records { get; set; }
    [FirestoreProperty]
    public bool allergen_filter { get; set; }
    [FirestoreProperty]
    public Menu menu { get; set; }
    [FirestoreProperty]
    public Periods[] periods { get; set; }
    [FirestoreProperty]
    public bool closed { get; set; }

    public string getLunchId()
    {
        return periods[1].id;
    }
    public string getDinnerId()
    {
        return periods[2].id;
    }
}

[FirestoreData]
public class Menu
{
    public string id { get; set; }
    [FirestoreProperty]
    public string date { get; set; }
    [FirestoreProperty]
    public object name { get; set; }
    [FirestoreProperty]
    public object from_date { get; set; }
    [FirestoreProperty]
    public object to_date { get; set; }
    [FirestoreProperty]
    public Periods1 periods { get; set; }
}

[FirestoreData]
public class Periods1
{
    [FirestoreProperty]
    public string id { get; set; }
    [FirestoreProperty]
    public string name { get; set; }
    [FirestoreProperty]
    public int sort_order { get; set; }
    [FirestoreProperty]
    public Categories[] categories { get; set; }
}

[FirestoreData]
public class Categories
{
    [FirestoreProperty]
    public string id { get; set; }
    [FirestoreProperty]
    public string name { get; set; }
    [FirestoreProperty]
    public int sort_order { get; set; }
    [FirestoreProperty]
    public Items[] items { get; set; }
}

[FirestoreData]
public class Items
{
    [FirestoreProperty]
    public string id { get; set; }
    [FirestoreProperty]
    public string name { get; set; }
    public string mrn { get; set; }
    public string rev { get; set; }
    public string mrn_full { get; set; }
    [FirestoreProperty]
    public string desc { get; set; }
    public int webtrition_id { get; set; }
    [FirestoreProperty]
    public int sort_order { get; set; }
    public string portion { get; set; }
    public object qty { get; set; }
    public string ingredients { get; set; }
    public Nutrients[] nutrients { get; set; }
    public Filters[] filters { get; set; }
    public object[] custom_allergens { get; set; }
    public int calories { get; set; }
}

[FirestoreData]
public class Nutrients
{
    [FirestoreProperty]
    public string id { get; set; }
    [FirestoreProperty]
    public string name { get; set; }
    [FirestoreProperty]
    public string value { get; set; }
    [FirestoreProperty]
    public string uom { get; set; }
    [FirestoreProperty]
    public string value_numeric { get; set; }
}

[FirestoreData]
public class Filters
{
    [FirestoreProperty]
    public string id { get; set; }
    [FirestoreProperty]
    public string name { get; set; }
    [FirestoreProperty]
    public string type { get; set; }
    [FirestoreProperty]
    public bool icon { get; set; }
    [FirestoreProperty]
    public string remote_file_name { get; set; }
    [FirestoreProperty]
    public object[] custom_icons { get; set; }
}

[FirestoreData]
public class Periods
{
    [FirestoreProperty]
    public string id { get; set; }
    [FirestoreProperty]
    public string name { get; set; }
    [FirestoreProperty]
    public int sort_order { get; set; }
}

