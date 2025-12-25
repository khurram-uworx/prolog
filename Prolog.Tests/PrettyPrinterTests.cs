using Prolog;

namespace Prolog.Tests
{
    [TestFixture]
    public class PrettyPrinterTests
    {
        PrettyPrinter printer;
        Parser parser;

        [SetUp]
        public void SetUp()
        {
            printer = new PrettyPrinter();
            parser = new Parser();
        }

        [Test]
        public void PrettyPrinter_FormatsAtom()
        {
            var atom = new Atom("tom");
            var result = printer.FormatTerm(atom);
            
            Assert.That(result, Is.EqualTo("tom"));
        }

        [Test]
        public void PrettyPrinter_FormatsVariable()
        {
            var variable = new Variable("X");
            var result = printer.FormatTerm(variable);
            
            Assert.That(result, Is.EqualTo("X"));
        }

        [Test]
        public void PrettyPrinter_FormatsCompoundTerm()
        {
            var compound = new Compound("parent", new Atom("tom"), new Atom("bob"));
            var result = printer.FormatTerm(compound);
            
            Assert.That(result, Is.EqualTo("parent(tom, bob)"));
        }

        [Test]
        public void PrettyPrinter_FormatsNestedCompoundTerm()
        {
            var nested = new Compound("likes", 
                new Compound("person", new Atom("tom")), 
                new Compound("food", new Atom("pizza")));
            var result = printer.FormatTerm(nested);
            
            Assert.That(result, Is.EqualTo("likes(person(tom), food(pizza))"));
        }

        [Test]
        public void PrettyPrinter_FormatsZeroArityCompound()
        {
            var compound = new Compound("fact", new List<Term>());
            var result = printer.FormatTerm(compound);
            
            Assert.That(result, Is.EqualTo("fact"));
        }

        [Test]
        public void PrettyPrinter_FormatsFact()
        {
            var fact = new Clause(new Compound("parent", new Atom("tom"), new Atom("bob")));
            var result = printer.FormatClause(fact);
            
            Assert.That(result, Is.EqualTo("parent(tom, bob)."));
        }

        [Test]
        public void PrettyPrinter_FormatsRule()
        {
            var head = new Compound("grandparent", new Variable("X"), new Variable("Z"));
            var body = new List<Term>
            {
                new Compound("parent", new Variable("X"), new Variable("Y")),
                new Compound("parent", new Variable("Y"), new Variable("Z"))
            };
            var rule = new Clause(head, body);
            var result = printer.FormatClause(rule);
            
            Assert.That(result, Is.EqualTo("grandparent(X, Z) :- parent(X, Y), parent(Y, Z)."));
        }

        [Test]
        public void PrettyPrinter_FormatsProgram()
        {
            var clauses = new List<Clause>
            {
                new Clause(new Compound("parent", new Atom("tom"), new Atom("bob"))),
                new Clause(new Compound("parent", new Atom("bob"), new Atom("alice")))
            };
            var result = printer.FormatProgram(clauses);
            
            Assert.That(result, Is.EqualTo("parent(tom, bob).\nparent(bob, alice)."));
        }

        [Test]
        public void PrettyPrinter_FormatsSimpleQuery()
        {
            var query = new Compound("parent", new Variable("X"), new Atom("bob"));
            var result = printer.FormatQuery(query);
            
            Assert.That(result, Is.EqualTo("?- parent(X, bob)."));
        }

        [Test]
        public void PrettyPrinter_FormatsCompoundQuery()
        {
            var query = new Compound(",", 
                new Compound("parent", new Variable("X"), new Variable("Y")),
                new Compound("parent", new Variable("Y"), new Variable("Z")));
            var result = printer.FormatQuery(query);
            
            Assert.That(result, Is.EqualTo("?- parent(X, Y), parent(Y, Z)."));
        }

        [Test]
        public void PrettyPrinter_FormatsProgramPretty()
        {
            var head = new Compound("grandparent", new Variable("X"), new Variable("Z"));
            var body = new List<Term>
            {
                new Compound("parent", new Variable("X"), new Variable("Y")),
                new Compound("parent", new Variable("Y"), new Variable("Z"))
            };
            var rule = new Clause(head, body);
            var clauses = new List<Clause> { rule };
            
            var result = printer.FormatProgramPretty(clauses);
            
            Assert.That(result, Is.EqualTo("grandparent(X, Z) :-\n    parent(X, Y),\n    parent(Y, Z)."));
        }

        [Test]
        public void PrettyPrinter_RoundTripSimpleFact()
        {
            var original = "parent(tom, bob).";
            
            // Parse then print
            var parseResult = parser.ParseProgram(original);
            Assert.That(parseResult.Success, Is.True);
            
            var formatted = printer.FormatProgram(parseResult.Clauses);
            
            // Parse again
            var secondParseResult = parser.ParseProgram(formatted);
            Assert.That(secondParseResult.Success, Is.True);
            
            // Should be equivalent
            Assert.That(secondParseResult.Clauses.Count, Is.EqualTo(parseResult.Clauses.Count));
            Assert.That(secondParseResult.Clauses[0], Is.EqualTo(parseResult.Clauses[0]));
        }

        [Test]
        public void PrettyPrinter_RoundTripComplexRule()
        {
            var original = "grandparent(X, Z) :- parent(X, Y), parent(Y, Z).";
            
            // Parse then print
            var parseResult = parser.ParseProgram(original);
            Assert.That(parseResult.Success, Is.True);
            
            var formatted = printer.FormatProgram(parseResult.Clauses);
            
            // Parse again
            var secondParseResult = parser.ParseProgram(formatted);
            Assert.That(secondParseResult.Success, Is.True);
            
            // Should be equivalent
            Assert.That(secondParseResult.Clauses[0], Is.EqualTo(parseResult.Clauses[0]));
        }

        [Test]
        public void PrettyPrinter_RoundTripNestedTerms()
        {
            var original = "likes(person(tom), food(pizza)).";
            
            // Parse then print
            var parseResult = parser.ParseProgram(original);
            Assert.That(parseResult.Success, Is.True);
            
            var formatted = printer.FormatProgram(parseResult.Clauses);
            
            // Parse again
            var secondParseResult = parser.ParseProgram(formatted);
            Assert.That(secondParseResult.Success, Is.True);
            
            // Should be equivalent
            Assert.That(secondParseResult.Clauses[0], Is.EqualTo(parseResult.Clauses[0]));
        }

        [Test]
        public void PrettyPrinter_RoundTripMultipleClauses()
        {
            var original = "parent(tom, bob). parent(bob, alice). grandparent(X, Z) :- parent(X, Y), parent(Y, Z).";
            
            // Parse then print
            var parseResult = parser.ParseProgram(original);
            Assert.That(parseResult.Success, Is.True);
            
            var formatted = printer.FormatProgram(parseResult.Clauses);
            
            // Parse again
            var secondParseResult = parser.ParseProgram(formatted);
            Assert.That(secondParseResult.Success, Is.True);
            
            // Should be equivalent
            Assert.That(secondParseResult.Clauses.Count, Is.EqualTo(parseResult.Clauses.Count));
            for (int i = 0; i < parseResult.Clauses.Count; i++)
            {
                Assert.That(secondParseResult.Clauses[i], Is.EqualTo(parseResult.Clauses[i]));
            }
        }

        [Test]
        public void PrettyPrinter_RoundTripQuery()
        {
            var original = "?- parent(X, bob).";
            
            // Parse then print
            var parseResult = parser.ParseQuery(original);
            Assert.That(parseResult.Success, Is.True);
            
            var formatted = printer.FormatQuery(parseResult.Query!);
            
            // Parse again
            var secondParseResult = parser.ParseQuery(formatted);
            Assert.That(secondParseResult.Success, Is.True);
            
            // Should be equivalent
            Assert.That(secondParseResult.Query, Is.EqualTo(parseResult.Query));
        }

        [Test]
        public void PrettyPrinter_RoundTripCompoundQuery()
        {
            var original = "?- parent(X, Y), parent(Y, Z).";
            
            // Parse then print
            var parseResult = parser.ParseQuery(original);
            Assert.That(parseResult.Success, Is.True);
            
            var formatted = printer.FormatQuery(parseResult.Query!);
            
            // Parse again
            var secondParseResult = parser.ParseQuery(formatted);
            Assert.That(secondParseResult.Success, Is.True);
            
            // Should be equivalent
            Assert.That(secondParseResult.Query, Is.EqualTo(parseResult.Query));
        }
    }
}