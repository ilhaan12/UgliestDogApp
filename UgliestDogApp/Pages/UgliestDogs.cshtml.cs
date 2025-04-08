using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Data.Sqlite;
using System.Collections.Generic;

public class UgliestDogsModel : PageModel
{
    // List of dogs to be displayed in the dropdown
    public List<SelectListItem> DogList { get; set; }

    // The selected dog object containing detailed information
    public Dog SelectedDog { get; set; }

    // On GET request, load the list of dogs to display in the dropdown
    public void OnGet()
    {
        LoadDogList(); // Loads the list of dogs
    }

    // On POST request, load the list again and set the selected dog based on the user selection
    public void OnPost(string selectedDog)
    {
        LoadDogList(); // Reload dog list after selection
        if (!string.IsNullOrEmpty(selectedDog))
        {
            // Set the selected dog by its ID
            SelectedDog = GetDogById(int.Parse(selectedDog));
        }
    }

    // Method to load the list of dogs from the database and populate the DogList property
    private void LoadDogList()
    {
        DogList = new List<SelectListItem>(); // Initialize an empty list to hold dog options
        using (var connection = new SqliteConnection("Data Source=UgliestDogs.db"))
        {
            connection.Open(); // Open connection to the SQLite database
            var command = connection.CreateCommand();
            command.CommandText = "SELECT Id, Name FROM Dogs"; // Query to fetch dog names and ids
            using (var reader = command.ExecuteReader())
            {
                // Iterate through the results and add each dog to the list
                while (reader.Read())
                {
                    DogList.Add(new SelectListItem
                    {
                        Value = reader.GetInt32(0).ToString(), // Set Id as the value
                        Text = reader.GetString(1) // Set Name as the text
                    });
                }
            }
        }
    }

    // Method to fetch dog details by Id from the database
    private Dog GetDogById(int id)
    {
        using (var connection = new SqliteConnection("Data Source=UgliestDogs.db"))
        {
            connection.Open(); // Open the database connection
            var command = connection.CreateCommand();
            command.CommandText = "SELECT * FROM Dogs WHERE Id = @Id"; // Query to fetch a specific dog by its Id
            command.Parameters.AddWithValue("@Id", id); // Use parameterized query to prevent SQL injection
            using (var reader = command.ExecuteReader())
            {
                if (reader.Read())
                {
                    // If the dog exists, create and return a Dog object with the retrieved data
                    return new Dog
                    {
                        Id = reader.GetInt32(0),
                        Name = reader.GetString(1),
                        Breed = reader.GetString(2),
                        Year = reader.GetInt32(3),
                        ImageFileName = reader.GetString(4)
                    };
                }
            }
        }
        return null; // Return null if no dog found
    }
}

// Dog class to represent the dog entity in the database
public class Dog
{
    public int Id { get; set; } // Unique identifier for the dog
    public string Name { get; set; } // Name of the dog
    public string Breed { get; set; } // Breed of the dog
    public int Year { get; set; } // Year of the contest
    public string ImageFileName { get; set; } // Filename of the dog's image
}
