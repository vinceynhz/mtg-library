using System;
using System.Threading.Tasks;

namespace MtgLibrary
{
    class Program
    {
        static async Task Main(string[] args)
        {
            MtgCli.Runner runner = new MtgCli.Runner();
            await runner.run(new string[]{"version"});
            Console.WriteLine("Enter 'help' to see list of available commands\n");
            
            var done = false;
            while(!done)
            {
                Console.Write("> ");
                var cliargs = Console.ReadLine();
                if(cliargs.Length != 0)
                {
                    int rc = await runner.run(cliargs.Split(' '));
                    done = rc == -1;
                }
            }
        }
    }
}
