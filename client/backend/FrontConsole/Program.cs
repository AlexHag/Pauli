using ElectronCgi.DotNet;

namespace FrontConsole;

public class Program
{
    public static void Main(string[] args)
    {
        var connection = new ConnectionBuilder()
            .WithLogging()
            .Build();
        
        connection.On<string, string>("greeting", name => "Hello " + name);

        connection.Listen();
    }
}