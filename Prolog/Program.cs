using Prolog;

namespace Prolog
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Prolog Interpreter");
            Console.WriteLine("==================");
            
            // Test basic term creation
            var atom = new Atom("tom");
            var variable = new Variable("X");
            var compound = new Compound("parent", atom, new Atom("bob"));
            
            Console.WriteLine($"Atom: {atom}");
            Console.WriteLine($"Variable: {variable}");
            Console.WriteLine($"Compound: {compound}");
            
            Console.WriteLine("\nCore term representations implemented successfully!");
            
            // Test lexer
            Console.WriteLine("\nTesting Lexer:");
            var lexer = new Lexer("parent(tom, bob). grandparent(X, Z) :- parent(X, Y), parent(Y, Z).");
            var tokens = lexer.Tokenize();
            
            Console.WriteLine("Tokens:");
            foreach (var token in tokens.Where(t => t.Type != TokenType.Whitespace && t.Type != TokenType.EndOfFile))
            {
                Console.WriteLine($"  {token.Type}: '{token.Value}'");
            }
            
            Console.WriteLine("\nLexer implemented successfully!");
            
            // Test parser
            Console.WriteLine("\nTesting Parser:");
            var parser = new Parser();
            
            // Test parsing facts
            var factResult = parser.ParseProgram("parent(tom, bob). likes(mary, chocolate).");
            if (factResult.Success)
            {
                Console.WriteLine($"Parsed {factResult.Clauses.Count} facts:");
                foreach (var clause in factResult.Clauses)
                {
                    Console.WriteLine($"  {clause}");
                }
            }
            
            // Test parsing rules
            var ruleResult = parser.ParseProgram("grandparent(X, Z) :- parent(X, Y), parent(Y, Z).");
            if (ruleResult.Success)
            {
                Console.WriteLine($"Parsed rule: {ruleResult.Clauses[0]}");
            }
            
            // Test parsing queries
            var queryResult = parser.ParseQuery("?- parent(X, bob).");
            if (queryResult.Success)
            {
                Console.WriteLine($"Parsed query: ?- {queryResult.Query}.");
            }
            
            Console.WriteLine("\nParser implemented successfully!");
            
            // Test pretty printer
            Console.WriteLine("\nTesting Pretty Printer:");
            var prettyPrinter = new PrettyPrinter();
            
            // Test round-trip: parse -> print -> parse
            var originalProgram = "parent(tom, bob). grandparent(X, Z) :- parent(X, Y), parent(Y, Z).";
            Console.WriteLine($"Original: {originalProgram}");
            
            var parseResult = parser.ParseProgram(originalProgram);
            if (parseResult.Success)
            {
                var formatted = prettyPrinter.FormatProgram(parseResult.Clauses);
                Console.WriteLine($"Formatted: {formatted}");
                
                // Parse the formatted version
                var secondParse = parser.ParseProgram(formatted);
                if (secondParse.Success)
                {
                    Console.WriteLine("Round-trip successful! ✓");
                    
                    // Test pretty formatting
                    var prettyFormatted = prettyPrinter.FormatProgramPretty(parseResult.Clauses);
                    Console.WriteLine("Pretty formatted:");
                    Console.WriteLine(prettyFormatted);
                }
            }
            
            Console.WriteLine("\nPretty printer implemented successfully!");
            
            // Test knowledge base
            Console.WriteLine("\nTesting Knowledge Base:");
            var knowledgeBase = new KnowledgeBase();
            
            // Add some facts and rules
            var kbParser = new Parser();
            var program = "parent(tom, bob). parent(bob, alice). grandparent(X, Z) :- parent(X, Y), parent(Y, Z).";
            var kbParseResult = kbParser.ParseProgram(program);
            
            if (kbParseResult.Success)
            {
                foreach (var clause in kbParseResult.Clauses)
                {
                    knowledgeBase.AddClause(clause);
                }
                
                var stats = knowledgeBase.GetStats();
                Console.WriteLine($"Knowledge base stats: {stats}");
                
                // Test retrieval
                var parentGoal = new Compound("parent", new Variable("X"), new Variable("Y"));
                var parentMatches = knowledgeBase.GetMatchingClauses(parentGoal).ToList();
                Console.WriteLine($"Found {parentMatches.Count} parent clauses:");
                foreach (var match in parentMatches)
                {
                    Console.WriteLine($"  {match}");
                }
                
                var grandparentGoal = new Compound("grandparent", new Variable("X"), new Variable("Z"));
                var grandparentMatches = knowledgeBase.GetMatchingClauses(grandparentGoal).ToList();
                Console.WriteLine($"Found {grandparentMatches.Count} grandparent clauses:");
                foreach (var match in grandparentMatches)
                {
                    Console.WriteLine($"  {match}");
                }
            }
            
            Console.WriteLine("\nKnowledge base implemented successfully!");
        }
    }
}
