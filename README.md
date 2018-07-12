# Miki.Configuration
[![](https://img.shields.io/nuget/dt/Miki.Configuration.svg?style=for-the-badge)](https://www.nuget.org/packages/Miki.Configuration)
[![](https://img.shields.io/discord/259343729586864139.svg?style=for-the-badge&logo=discord)](https://discord.gg/XpG4kwE)

A configuration manager system

## Usage
To use Miki.Configuration, you should create and construct your system with services at launch time as shown below.
```cs
class MyService
{
  [Configurable]
  public string ConnectionUri { get; set; } = "http://localhost/";

  public void TryConnect() 
  {
    ...
  }
}

...

class Program
{
  MyService service;

  public static void Main(string[] args)
  {
    ConfigurationManager configuration = new ConfigurationManager();
    
    service = new MyService();
    configuration.RegisterType(service);  
  
    // saves values set to all [Configurable]'s
    configuration.ExportAsync(
      new JsonSerializationProvider(),
      "./config.json"
    ).GetAwaiter().GetResult();
  
    // loads in values set to all [Configurable]'s
    configuration.ImportAsync(
      new JsonSerializationProvider(),
      "./config.json"
    ).GetAwaiter().GetResult();
    
    service.TryConnect();
  }
}
```
