using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;

HttpClient client = new HttpClient();
client.BaseAddress = new Uri("http://localhost:5190/"); // Adapter avec ton vrai port !

string? jwtToken = null;

async Task Register(string email, string password, string name)
{
    var registerDto = new
    {
        Name = name,
        Email = email,
        Password = password
    };

    var content = new StringContent(
        JsonSerializer.Serialize(registerDto),
        Encoding.UTF8, "application/json");

    var response = await client.PostAsync("api/users/register", content);
    Console.WriteLine($"Register: {response.StatusCode}");
    Console.WriteLine(await response.Content.ReadAsStringAsync());
}

async Task Login(string email, string password)
{
    var loginDto = new
    {
        Email = email,
        Password = password
    };

    var content = new StringContent(
        JsonSerializer.Serialize(loginDto),
        Encoding.UTF8, "application/json");

    var response = await client.PostAsync("api/users/login", content);
    Console.WriteLine($"Login: {response.StatusCode}");

    var body = await response.Content.ReadAsStringAsync();
    Console.WriteLine(body);

    if (!response.IsSuccessStatusCode)
    {
        Console.WriteLine("Login failed, skipping token parsing.");
        return;
    }

    var json = JsonDocument.Parse(body);
    jwtToken = json.RootElement.GetProperty("token").GetString();
    client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", jwtToken);
}

async Task CreateProject(string name, string description)
{
    var content = new StringContent(
        JsonSerializer.Serialize(new { Name = name, Description = description }),
        Encoding.UTF8, "application/json");

    var response = await client.PostAsync("api/projects", content);
    Console.WriteLine($"Create Project: {response.StatusCode}");
    Console.WriteLine(await response.Content.ReadAsStringAsync());
}

async Task GetProjects()
{
    var response = await client.GetAsync("api/projects");
    Console.WriteLine($"Get Projects: {response.StatusCode}");
    Console.WriteLine(await response.Content.ReadAsStringAsync());
}

async Task CreateTask(int projectId, string title)
{
    var content = new StringContent(
        JsonSerializer.Serialize(new { ProjectId = projectId, Title = title }),
        Encoding.UTF8, "application/json");

    var response = await client.PostAsync("api/tasks", content);
    Console.WriteLine($"Create Task: {response.StatusCode}");
    Console.WriteLine(await response.Content.ReadAsStringAsync());
}

async Task GetTasks()
{
    var response = await client.GetAsync("api/tasks");
    Console.WriteLine($"Get Tasks: {response.StatusCode}");
    Console.WriteLine(await response.Content.ReadAsStringAsync());
}

async Task MainAsync()
{
    Console.WriteLine("Testing Register...");
    await Register("test@example.com", "P@ssw0rd!", "Test User");

    Console.WriteLine("Testing Login...");
    await Login("test@example.com", "P@ssw0rd!");

    if (jwtToken == null)
    {
        Console.WriteLine("Authentication failed, aborting...");
        return;
    }

    Console.WriteLine("Testing Create Project...");
    await CreateProject("Projet de Test", "Ceci est un projet de test.");

    Console.WriteLine("Testing Get Projects...");
    await GetProjects();

    Console.WriteLine("Testing Create Task...");
    await CreateTask(1, "Première tâche");

    Console.WriteLine("Testing Get Tasks...");
    await GetTasks();
}

await MainAsync();
