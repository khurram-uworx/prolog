using Prolog;

namespace Prolog.Tests
{
    [TestFixture]
    public class ParserTests
    {
        Parser parser;

        [SetUp]
        public void SetUp()
        {
            parser = new Parser();
        }

        [Test]
        public void Parser_ParsesSimpleFact()
        {
            var result = parser.ParseProgram("parent(tom, bob).");
            
            Assert.That(result.Success, Is.True);
            Assert.That(result.Clauses.Count, Is.EqualTo(1));
            
            var clause = result.Clauses[0];
            Assert.That(clause.IsFact, Is.True);
            Assert.That(clause.Head.ToString(), Is.EqualTo("parent(tom, bob)"));
        }

        [Test]
        public void Parser_ParsesMultipleFacts()
        {
            var result = parser.ParseProgram("parent(tom, bob). parent(bob, alice).");
            
            Assert.That(result.Success, Is.True);
            Assert.That(result.Clauses.Count, Is.EqualTo(2));
            
            Assert.That(result.Clauses[0].Head.ToString(), Is.EqualTo("parent(tom, bob)"));
            Assert.That(result.Clauses[1].Head.ToString(), Is.EqualTo("parent(bob, alice)"));
        }

        [Test]
        public void Parser_ParsesSimpleRule()
        {
            var result = parser.ParseProgram("grandparent(X, Z) :- parent(X, Y), parent(Y, Z).");
            
            Assert.That(result.Success, Is.True);
            Assert.That(result.Clauses.Count, Is.EqualTo(1));
            
            var clause = result.Clauses[0];
            Assert.That(clause.IsFact, Is.False);
            Assert.That(clause.Head.ToString(), Is.EqualTo("grandparent(X, Z)"));
            Assert.That(clause.Body.Count, Is.EqualTo(2));
            Assert.That(clause.Body[0].ToString(), Is.EqualTo("parent(X, Y)"));
            Assert.That(clause.Body[1].ToString(), Is.EqualTo("parent(Y, Z)"));
        }

        [Test]
        public void Parser_ParsesAtom()
        {
            var result = parser.ParseProgram("tom.");
            
            Assert.That(result.Success, Is.True);
            Assert.That(result.Clauses.Count, Is.EqualTo(1));
            
            var clause = result.Clauses[0];
            Assert.That(clause.Head, Is.InstanceOf<Atom>());
            Assert.That(clause.Head.ToString(), Is.EqualTo("tom"));
        }

        [Test]
        public void Parser_ParsesVariable()
        {
            var result = parser.ParseProgram("likes(X, Y) :- person(X), person(Y).");
            
            Assert.That(result.Success, Is.True);
            var clause = result.Clauses[0];
            
            var compound = clause.Head as Compound;
            Assert.That(compound, Is.Not.Null);
            Assert.That(compound.Arguments[0], Is.InstanceOf<Variable>());
            Assert.That(compound.Arguments[1], Is.InstanceOf<Variable>());
        }

        [Test]
        public void Parser_ParsesCompoundTerm()
        {
            var result = parser.ParseProgram("likes(tom, mary).");
            
            Assert.That(result.Success, Is.True);
            var clause = result.Clauses[0];
            
            var compound = clause.Head as Compound;
            Assert.That(compound, Is.Not.Null);
            Assert.That(compound.Functor, Is.EqualTo("likes"));
            Assert.That(compound.Arity, Is.EqualTo(2));
            Assert.That(compound.Arguments[0].ToString(), Is.EqualTo("tom"));
            Assert.That(compound.Arguments[1].ToString(), Is.EqualTo("mary"));
        }

        [Test]
        public void Parser_ParsesNestedCompoundTerm()
        {
            var result = parser.ParseProgram("likes(person(tom), person(mary)).");
            
            Assert.That(result.Success, Is.True);
            var clause = result.Clauses[0];
            
            var compound = clause.Head as Compound;
            Assert.That(compound, Is.Not.Null);
            Assert.That(compound.Arguments[0], Is.InstanceOf<Compound>());
            Assert.That(compound.Arguments[1], Is.InstanceOf<Compound>());
        }

        [Test]
        public void Parser_ParsesSimpleQuery()
        {
            var result = parser.ParseQuery("?- parent(X, bob).");
            
            Assert.That(result.Success, Is.True);
            Assert.That(result.Query, Is.Not.Null);
            Assert.That(result.Query.ToString(), Is.EqualTo("parent(X, bob)"));
        }

        [Test]
        public void Parser_ParsesCompoundQuery()
        {
            var result = parser.ParseQuery("?- parent(X, Y), parent(Y, Z).");
            
            Assert.That(result.Success, Is.True);
            Assert.That(result.Query, Is.Not.Null);
            
            var compound = result.Query as Compound;
            Assert.That(compound, Is.Not.Null);
            Assert.That(compound.Functor, Is.EqualTo(","));
            Assert.That(compound.Arguments.Count, Is.EqualTo(2));
        }

        [Test]
        public void Parser_HandlesWhitespaceAndComments()
        {
            var program = @"
                % This is a comment
                parent(tom, bob).   % Another comment
                
                % Rule with comment
                grandparent(X, Z) :- 
                    parent(X, Y),    % First goal
                    parent(Y, Z).    % Second goal
            ";
            
            var result = parser.ParseProgram(program);
            
            Assert.That(result.Success, Is.True);
            Assert.That(result.Clauses.Count, Is.EqualTo(2));
        }

        [Test]
        public void Parser_ReportsErrorForInvalidSyntax()
        {
            var result = parser.ParseProgram("parent(tom bob).");
            
            Assert.That(result.Success, Is.False);
            Assert.That(result.ErrorMessage, Is.Not.Empty);
        }

        [Test]
        public void Parser_ReportsErrorForMissingDot()
        {
            var result = parser.ParseProgram("parent(tom, bob)");
            
            Assert.That(result.Success, Is.False);
            Assert.That(result.ErrorMessage, Contains.Substring("Expected '.' or ':-'"));
        }

        [Test]
        public void Parser_ReportsErrorForMissingRightParen()
        {
            var result = parser.ParseProgram("parent(tom, bob.");
            
            Assert.That(result.Success, Is.False);
            Assert.That(result.ErrorMessage, Contains.Substring("Expected ')'"));
        }

        [Test]
        public void Parser_ReportsErrorForQueryWithoutQuestionMark()
        {
            var result = parser.ParseQuery("parent(X, bob).");
            
            Assert.That(result.Success, Is.False);
            Assert.That(result.ErrorMessage, Contains.Substring("Query must start with '?-'"));
        }

        [Test]
        public void Parser_ReportsErrorForQueryWithoutDot()
        {
            var result = parser.ParseQuery("?- parent(X, bob)");
            
            Assert.That(result.Success, Is.False);
            Assert.That(result.ErrorMessage, Contains.Substring("Query must end with '.'"));
        }

        [Test]
        public void Parser_HandlesEmptyProgram()
        {
            var result = parser.ParseProgram("");
            
            Assert.That(result.Success, Is.True);
            Assert.That(result.Clauses.Count, Is.EqualTo(0));
        }

        [Test]
        public void Parser_ParsesZeroArityCompound()
        {
            var result = parser.ParseProgram("fact().");
            
            Assert.That(result.Success, Is.True);
            var clause = result.Clauses[0];
            
            var compound = clause.Head as Compound;
            Assert.That(compound, Is.Not.Null);
            Assert.That(compound.Functor, Is.EqualTo("fact"));
            Assert.That(compound.Arity, Is.EqualTo(0));
        }

        [Test]
        public void Parser_ParsesNotEqualOperator()
        {
            var result = parser.ParseProgram("sibling(X, Y) :- parent(Z, X), parent(Z, Y), X \\= Y.");
            
            Assert.That(result.Success, Is.True);
            Assert.That(result.Clauses.Count, Is.EqualTo(1));
            
            var clause = result.Clauses[0];
            Assert.That(clause.IsFact, Is.False);
            Assert.That(clause.Body.Count, Is.EqualTo(3));
            
            // The third goal should be the not-equal constraint
            var notEqualGoal = clause.Body[2] as Compound;
            Assert.That(notEqualGoal, Is.Not.Null);
            Assert.That(notEqualGoal.Functor, Is.EqualTo("\\="));
            Assert.That(notEqualGoal.Arity, Is.EqualTo(2));
            Assert.That(notEqualGoal.Arguments[0].ToString(), Is.EqualTo("X"));
            Assert.That(notEqualGoal.Arguments[1].ToString(), Is.EqualTo("Y"));
        }

        [Test]
        public void Parser_ParsesNotEqualInQuery()
        {
            var result = parser.ParseQuery("?- X \\= Y.");
            
            Assert.That(result.Success, Is.True);
            Assert.That(result.Query, Is.Not.Null);
            
            var compound = result.Query as Compound;
            Assert.That(compound, Is.Not.Null);
            Assert.That(compound.Functor, Is.EqualTo("\\="));
            Assert.That(compound.Arity, Is.EqualTo(2));
        }
    }
}