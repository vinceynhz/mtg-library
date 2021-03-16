using System;
using System.Threading.Tasks;

namespace MtgLibrary
{
    class Program
    {
        static async Task Main(string[] args)
        {
            MtgCli.Runner runner = new MtgCli.Runner();
            await runner.run(new string[] { "version" });
            Console.WriteLine("Enter 'help' to see list of available commands\n");
            var done = false;
            while (!done)
            {
                string prompt = (runner.GetCurrentPlayerName() ?? "") + "> ";
                Console.Write(prompt);
                var cliargs = Console.ReadLine();
                if (cliargs.Length != 0)
                {
                    cliargs = cliargs.Trim();
                    if ("exit".Equals(cliargs.ToLower()))
                    {
                        Console.WriteLine("Bye!\n");
                        done = true;
                    }
                    else
                    {
                        int rc = await runner.run(cliargs.Split(' '));
                        Console.WriteLine($"{rc}\n");
                    }
                }
            }
        }
    }
}
