namespace Prolog
{
    /// <summary>
    /// Interactive shell interface for the Prolog interpreter
    /// Handles user interaction, query processing, and result display
    /// </summary>
    public class PrologShell
    {
        readonly IParser parser;
        readonly IKnowledgeBase knowledgeBase;
        readonly IQueryEngine queryEngine;
        readonly PrettyPrinter prettyPrinter;
        bool isRunning;

        public PrologShell(IParser parser, IKnowledgeBase knowledgeBase, IQueryEngine queryEngine)
        {
            this.parser = parser ?? throw new ArgumentNullException(nameof(parser));
            this.knowledgeBase = knowledgeBase ?? throw new ArgumentNullException(nameof(knowledgeBase));
            this.queryEngine = queryEngine ?? throw new ArgumentNullException(nameof(queryEngine));
            this.prettyPrinter = new PrettyPrinter();
            this.isRunning = false;
        }

        /// <summary>
        /// Starts the interactive shell and processes user commands
        /// </summary>
        public void Run()
        {
            isRunning = true;
            DisplayWelcomeMessage();

            while (isRunning)
            {
                try
                {
                    Console.Write("?- ");
                    var input = Console.ReadLine();

                    if (string.IsNullOrWhiteSpace(input))
                        continue;

                    ProcessInput(input.Trim());
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error: {ex.Message}");
                }
            }

            Console.WriteLine("Goodbye!");
        }

        /// <summary>
        /// Processes user input - either commands or queries
        /// </summary>
        void ProcessInput(string input)
        {
            // Handle exit commands
            if (IsExitCommand(input))
            {
                isRunning = false;
                return;
            }

            // Handle help command
            if (input.Equals("help", StringComparison.OrdinalIgnoreCase) || input == "?")
            {
                DisplayHelp();
                return;
            }

            // Handle clear command
            if (input.Equals("clear", StringComparison.OrdinalIgnoreCase))
            {
                knowledgeBase.Clear();
                Console.WriteLine("Knowledge base cleared.");
                return;
            }

            // Handle stats command
            if (input.Equals("stats", StringComparison.OrdinalIgnoreCase))
            {
                DisplayStats();
                return;
            }

            // Handle program loading (facts and rules)
            if (!input.StartsWith("?-"))
            {
                LoadProgram(input);
                return;
            }

            // Handle queries
            ProcessQuery(input);
        }

        /// <summary>
        /// Loads a Prolog program (facts and rules) into the knowledge base
        /// </summary>
        void LoadProgram(string programText)
        {
            var parseResult = parser.ParseProgram(programText);

            if (!parseResult.Success)
            {
                Console.WriteLine($"Parse error: {parseResult.ErrorMessage}");
                return;
            }

            int addedCount = 0;
            foreach (var clause in parseResult.Clauses)
            {
                knowledgeBase.AddClause(clause);
                addedCount++;
                
                if (clause.IsFact)
                {
                    Console.WriteLine($"Added fact: {clause}");
                }
                else
                {
                    Console.WriteLine($"Added rule: {clause}");
                }
            }

            Console.WriteLine($"Successfully loaded {addedCount} clause(s).");
        }

        /// <summary>
        /// Processes a query and displays results with solution stepping
        /// </summary>
        void ProcessQuery(string queryText)
        {
            var parseResult = parser.ParseQuery(queryText);

            if (!parseResult.Success)
            {
                Console.WriteLine($"Parse error: {parseResult.ErrorMessage}");
                return;
            }

            var query = parseResult.Query;
            if (query == null)
            {
                Console.WriteLine("Error: No query found.");
                return;
            }

            Console.WriteLine($"Query: {query}");

            try
            {
                var solutions = queryEngine.Solve(query).ToList();

                if (solutions.Count == 0 || solutions.All(s => !s.IsSuccess))
                {
                    Console.WriteLine("No.");
                    return;
                }

                var successfulSolutions = solutions.Where(s => s.IsSuccess).ToList();
                
                if (successfulSolutions.Count == 1)
                {
                    // Single solution
                    DisplaySolution(successfulSolutions[0], 1, 1);
                    Console.WriteLine("Yes.");
                }
                else
                {
                    // Multiple solutions - allow stepping through them
                    StepThroughSolutions(successfulSolutions);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Query execution error: {ex.Message}");
            }
        }

        /// <summary>
        /// Allows user to step through multiple solutions
        /// </summary>
        void StepThroughSolutions(List<Solution> solutions)
        {
            int currentIndex = 0;

            while (currentIndex < solutions.Count)
            {
                DisplaySolution(solutions[currentIndex], currentIndex + 1, solutions.Count);

                if (currentIndex == solutions.Count - 1)
                {
                    // Last solution
                    Console.WriteLine("Yes.");
                    break;
                }

                // Ask user if they want more solutions
                Console.Write("More? (y/n): ");
                var response = Console.ReadLine()?.Trim().ToLower();

                if (response == "n" || response == "no")
                {
                    Console.WriteLine("Yes.");
                    break;
                }
                else if (response == "y" || response == "yes" || string.IsNullOrEmpty(response))
                {
                    currentIndex++;
                    continue;
                }
                else
                {
                    Console.WriteLine("Please enter 'y' for yes or 'n' for no.");
                }
            }
        }

        /// <summary>
        /// Displays a single solution with variable bindings
        /// </summary>
        void DisplaySolution(Solution solution, int solutionNumber, int totalSolutions)
        {
            if (!solution.IsSuccess)
            {
                Console.WriteLine("No solution");
                return;
            }

            if (solution.Bindings.Count == 0)
            {
                if (totalSolutions > 1)
                {
                    Console.WriteLine($"Solution {solutionNumber}/{totalSolutions}: true");
                }
                else
                {
                    Console.WriteLine("true");
                }
            }
            else
            {
                if (totalSolutions > 1)
                {
                    Console.WriteLine($"Solution {solutionNumber}/{totalSolutions}:");
                }

                foreach (var binding in solution.Bindings.OrderBy(kvp => kvp.Key))
                {
                    Console.WriteLine($"  {binding.Key} = {binding.Value}");
                }
            }
        }

        /// <summary>
        /// Displays welcome message and basic instructions
        /// </summary>
        void DisplayWelcomeMessage()
        {
            Console.WriteLine("Prolog Interpreter");
            Console.WriteLine("==================");
            Console.WriteLine();
            Console.WriteLine("Enter Prolog facts and rules, or queries starting with '?-'");
            Console.WriteLine("Type 'help' for more information, 'quit' to exit.");
            Console.WriteLine();
        }

        /// <summary>
        /// Displays help information
        /// </summary>
        void DisplayHelp()
        {
            Console.WriteLine();
            Console.WriteLine("Prolog Interpreter Help");
            Console.WriteLine("=======================");
            Console.WriteLine();
            Console.WriteLine("Commands:");
            Console.WriteLine("  help, ?          - Show this help message");
            Console.WriteLine("  quit, exit, bye  - Exit the interpreter");
            Console.WriteLine("  clear            - Clear the knowledge base");
            Console.WriteLine("  stats            - Show knowledge base statistics");
            Console.WriteLine();
            Console.WriteLine("Usage:");
            Console.WriteLine("  Facts:     parent(tom, bob).");
            Console.WriteLine("  Rules:     grandparent(X, Z) :- parent(X, Y), parent(Y, Z).");
            Console.WriteLine("  Queries:   ?- parent(X, bob).");
            Console.WriteLine();
            Console.WriteLine("Variables start with uppercase letters (X, Y, Person)");
            Console.WriteLine("Atoms start with lowercase letters (tom, bob, parent)");
            Console.WriteLine("Use semicolon (;) or 'y' to see more solutions");
            Console.WriteLine();
        }

        /// <summary>
        /// Displays knowledge base statistics
        /// </summary>
        void DisplayStats()
        {
            if (knowledgeBase is KnowledgeBase kb)
            {
                var stats = kb.GetStats();
                Console.WriteLine($"Knowledge Base Statistics:");
                Console.WriteLine($"  Total clauses: {stats.TotalClauses}");
                Console.WriteLine($"  Facts: {stats.FactCount}");
                Console.WriteLine($"  Rules: {stats.RuleCount}");
                Console.WriteLine($"  Unique functors: {stats.UniqueFunctors}");
            }
            else
            {
                Console.WriteLine($"Knowledge Base: {knowledgeBase.ClauseCount} clauses");
            }
        }

        /// <summary>
        /// Checks if the input is an exit command
        /// </summary>
        bool IsExitCommand(string input)
        {
            var exitCommands = new[] { "quit", "exit", "bye", "q" };
            return exitCommands.Contains(input.ToLower());
        }

        /// <summary>
        /// Stops the shell (for programmatic control)
        /// </summary>
        public void Stop()
        {
            isRunning = false;
        }

        /// <summary>
        /// Gets the current running state
        /// </summary>
        public bool IsRunning => isRunning;
    }
}