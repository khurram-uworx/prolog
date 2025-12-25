using Prolog;

namespace Prolog.Console
{
    class Program
    {
        static void Main(string[] args)
        {
            // Set up graceful shutdown handling
            System.Console.CancelKeyPress += OnCancelKeyPress;
            
            try
            {
                // Create the core components with error handling
                var parser = CreateParser();
                var knowledgeBase = CreateKnowledgeBase();
                var unificationEngine = CreateUnificationEngine();
                var queryEngine = CreateQueryEngine(knowledgeBase, unificationEngine);

                // Create and start the interactive shell
                var shell = new PrologShell(parser, knowledgeBase, queryEngine);
                
                System.Console.WriteLine("Starting Prolog Interpreter...");
                shell.Run();
                
                System.Console.WriteLine("Prolog Interpreter terminated normally.");
            }
            catch (OutOfMemoryException ex)
            {
                System.Console.WriteLine("Error: Out of memory. The program or query may be too complex.");
                System.Console.WriteLine($"Details: {ex.Message}");
                Environment.Exit(1);
            }
            catch (StackOverflowException ex)
            {
                System.Console.WriteLine("Error: Stack overflow. The query may have infinite recursion.");
                System.Console.WriteLine($"Details: {ex.Message}");
                Environment.Exit(1);
            }
            catch (ArgumentException ex)
            {
                System.Console.WriteLine($"Configuration error: {ex.Message}");
                System.Console.WriteLine("Please check your system configuration and try again.");
                Environment.Exit(1);
            }
            catch (Exception ex)
            {
                System.Console.WriteLine($"Fatal error: {ex.Message}");
                System.Console.WriteLine($"Type: {ex.GetType().Name}");
                
                if (ex.InnerException != null)
                {
                    System.Console.WriteLine($"Inner exception: {ex.InnerException.Message}");
                }
                
                System.Console.WriteLine("\nPress any key to exit...");
                System.Console.ReadKey();
                Environment.Exit(1);
            }
        }

        /// <summary>
        /// Creates the parser component with error handling
        /// </summary>
        static IParser CreateParser()
        {
            try
            {
                return new Parser();
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException("Failed to create parser component", ex);
            }
        }

        /// <summary>
        /// Creates the knowledge base component with error handling
        /// </summary>
        static IKnowledgeBase CreateKnowledgeBase()
        {
            try
            {
                return new KnowledgeBase();
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException("Failed to create knowledge base component", ex);
            }
        }

        /// <summary>
        /// Creates the unification engine component with error handling
        /// </summary>
        static IUnificationEngine CreateUnificationEngine()
        {
            try
            {
                return new UnificationEngine();
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException("Failed to create unification engine component", ex);
            }
        }

        /// <summary>
        /// Creates the query engine component with error handling
        /// </summary>
        static IQueryEngine CreateQueryEngine(IKnowledgeBase knowledgeBase, IUnificationEngine unificationEngine)
        {
            try
            {
                if (knowledgeBase == null)
                    throw new ArgumentNullException(nameof(knowledgeBase));
                if (unificationEngine == null)
                    throw new ArgumentNullException(nameof(unificationEngine));

                return new QueryEngine(knowledgeBase, unificationEngine);
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException("Failed to create query engine component", ex);
            }
        }

        /// <summary>
        /// Handles Ctrl+C gracefully
        /// </summary>
        static void OnCancelKeyPress(object? sender, ConsoleCancelEventArgs e)
        {
            System.Console.WriteLine("\nReceived interrupt signal. Shutting down gracefully...");
            e.Cancel = true; // Prevent immediate termination
            
            // Give a moment for cleanup
            System.Threading.Thread.Sleep(100);
            
            System.Console.WriteLine("Goodbye!");
            Environment.Exit(0);
        }
    }
}