using Prolog;
using Prolog.Samples;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Prolog.Samples
{
    /// <summary>
    /// Sample program demonstrator for the Prolog interpreter
    /// Shows how to load and query different sample programs
    /// </summary>
    class Program
    {
        static IParser parser = null!;
        static IKnowledgeBase knowledgeBase = null!;
        static IUnificationEngine unificationEngine = null!;
        static IQueryEngine queryEngine = null!;

        static void Main(string[] args)
        {
            Console.WriteLine("=== Prolog Interpreter Sample Programs ===");
            Console.WriteLine();

            try
            {
                // Initialize components
                InitializeComponents();

                // Show available programs
                ShowAvailablePrograms();

                // Run interactive demo
                RunInteractiveDemo();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
                Console.WriteLine("Press any key to exit...");
                Console.ReadKey();
            }
        }

        /// <summary>
        /// Initialize all Prolog interpreter components
        /// </summary>
        static void InitializeComponents()
        {
            Console.WriteLine("Initializing Prolog interpreter components...");
            
            parser = new Parser();
            knowledgeBase = new KnowledgeBase();
            unificationEngine = new UnificationEngine();
            queryEngine = new QueryEngine(knowledgeBase, unificationEngine);
            
            Console.WriteLine("Components initialized successfully!");
            Console.WriteLine();
        }

        /// <summary>
        /// Show all available sample programs
        /// </summary>
        static void ShowAvailablePrograms()
        {
            Console.WriteLine("Available Sample Programs:");
            Console.WriteLine("1. Family Tree - Demonstrates parent-child relationships and family rules");
            Console.WriteLine("2. Simple Rules - Shows basic facts and logical derivations");
            Console.WriteLine("3. Logic Puzzle - Object properties and reasoning");
            Console.WriteLine("4. Math Relations - Number relationships and basic arithmetic");
            Console.WriteLine();
            Console.WriteLine("Additional Options:");
            Console.WriteLine("5. Run all demos - Execute all sample programs in sequence");
            Console.WriteLine("6. Custom program - Enter your own Prolog program");
            Console.WriteLine();
        }

        /// <summary>
        /// Run interactive demonstration of sample programs
        /// </summary>
        static void RunInteractiveDemo()
        {
            var programs = SamplePrograms.GetAllPrograms();
            var queries = SamplePrograms.GetSampleQueries();

            while (true)
            {
                Console.WriteLine("Choose an option:");
                Console.WriteLine("1. Run Family Tree demo");
                Console.WriteLine("2. Run Simple Rules demo");
                Console.WriteLine("3. Run Logic Puzzle demo");
                Console.WriteLine("4. Run Math Relations demo");
                Console.WriteLine("5. Run all demos");
                Console.WriteLine("6. Custom program (enter your own)");
                Console.WriteLine("0. Exit");
                Console.Write("Enter choice (0-6): ");

                var choice = Console.ReadLine()?.Trim();

                switch (choice)
                {
                    case "1":
                        RunProgramDemo("FamilyTree", programs["FamilyTree"], queries["FamilyTree"]);
                        break;
                    case "2":
                        RunProgramDemo("SimpleRules", programs["SimpleRules"], queries["SimpleRules"]);
                        break;
                    case "3":
                        RunProgramDemo("LogicPuzzle", programs["LogicPuzzle"], queries["LogicPuzzle"]);
                        break;
                    case "4":
                        RunProgramDemo("MathRelations", programs["MathRelations"], queries["MathRelations"]);
                        break;
                    case "5":
                        RunAllDemos();
                        break;
                    case "6":
                        RunCustomProgram();
                        break;
                    case "0":
                        Console.WriteLine("Goodbye!");
                        return;
                    default:
                        Console.WriteLine("Invalid choice. Please try again.");
                        break;
                }

                Console.WriteLine();
                Console.WriteLine("Press any key to continue...");
                Console.ReadKey();
                Console.Clear();
                ShowAvailablePrograms();
            }
        }

        /// <summary>
        /// Run a demonstration of a specific program
        /// </summary>
        static void RunProgramDemo(string programName, string programText, List<string> sampleQueries)
        {
            Console.WriteLine($"=== {programName} Demo ===");
            Console.WriteLine();

            // Clear previous program
            knowledgeBase.Clear();

            // Load the program
            Console.WriteLine("Loading program...");
            var parseResult = parser.ParseProgram(programText);
            
            if (!parseResult.Success)
            {
                Console.WriteLine($"Error parsing program: {parseResult.ErrorMessage}");
                return;
            }

            foreach (var clause in parseResult.Clauses)
            {
                knowledgeBase.AddClause(clause);
            }

            Console.WriteLine($"Program loaded successfully! ({knowledgeBase.ClauseCount} clauses)");
            Console.WriteLine();

            // Show program content (first few lines)
            Console.WriteLine("Program content (excerpt):");
            var lines = programText.Split('\n').Where(l => !string.IsNullOrWhiteSpace(l) && !l.Trim().StartsWith("%")).Take(5);
            foreach (var line in lines)
            {
                Console.WriteLine($"  {line.Trim()}");
            }
            Console.WriteLine("  ...");
            Console.WriteLine();

            // Run sample queries
            Console.WriteLine("Running sample queries:");
            Console.WriteLine();

            foreach (var queryText in sampleQueries)
            {
                RunSingleQuery(queryText);
                Console.WriteLine();
            }
        }

        /// <summary>
        /// Run all program demonstrations
        /// </summary>
        static void RunAllDemos()
        {
            var programs = SamplePrograms.GetAllPrograms();
            var queries = SamplePrograms.GetSampleQueries();

            foreach (var program in programs)
            {
                RunProgramDemo(program.Key, program.Value, queries[program.Key]);
                Console.WriteLine();
                Console.WriteLine("=" + new string('=', 50));
                Console.WriteLine();
            }
        }

        /// <summary>
        /// Allow user to enter a custom program
        /// </summary>
        static void RunCustomProgram()
        {
            Console.WriteLine("=== Custom Program ===");
            Console.WriteLine("Enter your Prolog program (end with empty line):");
            Console.WriteLine("Example: parent(tom, bob).");
            Console.WriteLine();

            var programLines = new List<string>();
            string? line;
            
            while (!string.IsNullOrWhiteSpace(line = Console.ReadLine()))
            {
                programLines.Add(line);
            }

            if (programLines.Count == 0)
            {
                Console.WriteLine("No program entered.");
                return;
            }

            var programText = string.Join("\n", programLines);

            // Clear and load program
            knowledgeBase.Clear();
            var parseResult = parser.ParseProgram(programText);
            
            if (!parseResult.Success)
            {
                Console.WriteLine($"Error parsing program: {parseResult.ErrorMessage}");
                return;
            }

            foreach (var clause in parseResult.Clauses)
            {
                knowledgeBase.AddClause(clause);
            }

            Console.WriteLine($"Program loaded successfully! ({knowledgeBase.ClauseCount} clauses)");
            Console.WriteLine();

            // Interactive query mode
            Console.WriteLine("Enter queries (empty line to finish):");
            Console.WriteLine("Example: parent(tom, X).");
            Console.WriteLine();

            while (!string.IsNullOrWhiteSpace(line = Console.ReadLine()))
            {
                RunSingleQuery(line);
                Console.WriteLine();
            }
        }

        /// <summary>
        /// Execute a single query and display results
        /// </summary>
        static void RunSingleQuery(string queryText)
        {
            Console.WriteLine($"Query: {queryText}");

            try
            {
                var queryResult = parser.ParseQuery(queryText);
                
                if (!queryResult.Success)
                {
                    Console.WriteLine($"  Error parsing query: {queryResult.ErrorMessage}");
                    return;
                }

                var solutions = queryEngine.Solve(queryResult.Query).ToList();

                if (solutions.Count == 0)
                {
                    Console.WriteLine("  Result: No solutions found.");
                }
                else
                {
                    Console.WriteLine($"  Result: {solutions.Count} solution(s) found:");
                    
                    for (int i = 0; i < Math.Min(solutions.Count, 10); i++) // Limit to first 10 solutions
                    {
                        var solution = solutions[i];
                        if (solution.Bindings.Count == 0)
                        {
                            Console.WriteLine($"    {i + 1}. Yes (no variable bindings)");
                        }
                        else
                        {
                            var bindingStrings = solution.Bindings.Select(kvp => $"{kvp.Key} = {kvp.Value}");
                            Console.WriteLine($"    {i + 1}. {string.Join(", ", bindingStrings)}");
                        }
                    }

                    if (solutions.Count > 10)
                    {
                        Console.WriteLine($"    ... and {solutions.Count - 10} more solutions");
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"  Error executing query: {ex.Message}");
            }
        }
    }
}