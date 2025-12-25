using Prolog;

namespace Prolog
{
    class Program
    {
        static void Main(string[] args)
        {
            try
            {
                // Create the core components
                var parser = new Parser();
                var knowledgeBase = new KnowledgeBase();
                var unificationEngine = new UnificationEngine();
                var queryEngine = new QueryEngine(knowledgeBase, unificationEngine);

                // Create and start the interactive shell
                var shell = new PrologShell(parser, knowledgeBase, queryEngine);
                shell.Run();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Fatal error: {ex.Message}");
                Console.WriteLine("Press any key to exit...");
                Console.ReadKey();
            }
        }
    }
}
