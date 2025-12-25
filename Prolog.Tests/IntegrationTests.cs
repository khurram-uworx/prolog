using NUnit.Framework;
using Prolog;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Prolog.Tests
{
    /// <summary>
    /// Integration tests that verify end-to-end functionality using sample Prolog programs
    /// Tests the complete workflow: parse program → load into knowledge base → execute queries
    /// </summary>
    [TestFixture]
    public class IntegrationTests
    {
        private IParser parser;
        private IKnowledgeBase knowledgeBase;
        private IUnificationEngine unificationEngine;
        private IQueryEngine queryEngine;

        [SetUp]
        public void SetUp()
        {
            parser = new Parser();
            knowledgeBase = new KnowledgeBase();
            unificationEngine = new UnificationEngine();
            queryEngine = new QueryEngine(knowledgeBase, unificationEngine);
        }

        #region Family Tree Integration Tests

        [Test]
        public void FamilyTree_LoadProgram_ParsesSuccessfully()
        {
            // Act
            var result = parser.ParseProgram(TestPrograms.FamilyTree);

            // Assert
            Assert.That(result.Success, Is.True, $"Failed to parse family tree program: {result.ErrorMessage}");
            Assert.That(result.Clauses.Count, Is.GreaterThan(0), "Should have parsed multiple clauses");
            
            // Verify we have both facts and rules
            var facts = result.Clauses.Where(c => c.IsFact).ToList();
            var rules = result.Clauses.Where(c => !c.IsFact).ToList();
            
            Assert.That(facts.Count, Is.GreaterThan(0), "Should have parsed facts");
            Assert.That(rules.Count, Is.GreaterThan(0), "Should have parsed rules");
        }

        [Test]
        public void FamilyTree_SimpleParentQuery_ReturnsCorrectResults()
        {
            // Arrange
            LoadProgramIntoKnowledgeBase(TestPrograms.FamilyTree);
            var queryResult = parser.ParseQuery("?- parent(tom, X).");
            Assert.That(queryResult.Success, Is.True, $"Query should parse successfully: {queryResult.ErrorMessage}");

            // Act
            var solutions = queryEngine.Solve(queryResult.Query).ToList();

            // Assert
            Assert.That(solutions.Count, Is.EqualTo(2), "Tom should have 2 children");
            Assert.That(solutions.All(s => s.IsSuccess), Is.True);
            
            var children = solutions.Select(s => s.Bindings["X"]).Cast<Atom>().Select(a => a.Name).ToList();
            Assert.That(children, Contains.Item("bob"));
            Assert.That(children, Contains.Item("alice"));
        }

        [Test]
        public void FamilyTree_GrandparentQuery_ReturnsCorrectResults()
        {
            // Arrange
            LoadProgramIntoKnowledgeBase(TestPrograms.FamilyTree);
            var queryResult = parser.ParseQuery("?- grandparent(tom, X).");
            Assert.That(queryResult.Success, Is.True, $"Query should parse successfully: {queryResult.ErrorMessage}");

            // Act
            var solutions = queryEngine.Solve(queryResult.Query).ToList();

            // Assert
            Assert.That(solutions.Count, Is.EqualTo(4), "Tom should have 4 grandchildren");
            Assert.That(solutions.All(s => s.IsSuccess), Is.True);
            
            var grandchildren = solutions.Select(s => s.Bindings["X"]).Cast<Atom>().Select(a => a.Name).ToList();
            Assert.That(grandchildren, Contains.Item("charlie"));
            Assert.That(grandchildren, Contains.Item("diana"));
            Assert.That(grandchildren, Contains.Item("eve"));
            Assert.That(grandchildren, Contains.Item("frank"));
        }

        [Test]
        public void FamilyTree_AncestorQuery_ReturnsCorrectResults()
        {
            // Arrange
            LoadProgramIntoKnowledgeBase(TestPrograms.FamilyTree);
            // Test a simpler case: Tom is ancestor of Bob (direct parent)
            var queryResult = parser.ParseQuery("?- ancestor(tom, bob).");
            Assert.That(queryResult.Success, Is.True, $"Query should parse successfully: {queryResult.ErrorMessage}");

            // Act
            var solutions = queryEngine.Solve(queryResult.Query).ToList();

            // Assert
            Assert.That(solutions.Count, Is.EqualTo(1), "Tom should be ancestor of Bob (direct parent)");
            Assert.That(solutions[0].IsSuccess, Is.True);
        }

        [Test]
        public void FamilyTree_GenderBasedQuery_ReturnsCorrectResults()
        {
            // Arrange
            LoadProgramIntoKnowledgeBase(TestPrograms.FamilyTree);
            var queryResult = parser.ParseQuery("?- father(tom, X).");
            Assert.That(queryResult.Success, Is.True, $"Query should parse successfully: {queryResult.ErrorMessage}");

            // Act
            var solutions = queryEngine.Solve(queryResult.Query).ToList();

            // Assert
            Assert.That(solutions.Count, Is.EqualTo(2), "Tom should be father of 2 children");
            Assert.That(solutions.All(s => s.IsSuccess), Is.True);
        }

        [Test]
        public void FamilyTree_SiblingQuery_ReturnsCorrectResults()
        {
            // Arrange
            LoadProgramIntoKnowledgeBase(TestPrograms.FamilyTree);
            var queryResult = parser.ParseQuery("?- sibling(bob, X).");
            Assert.That(queryResult.Success, Is.True, $"Query should parse successfully: {queryResult.ErrorMessage}");

            // Act
            var solutions = queryEngine.Solve(queryResult.Query).ToList();

            // Assert
            Assert.That(solutions.Count, Is.EqualTo(1), "Bob should have 1 sibling");
            Assert.That(solutions.All(s => s.IsSuccess), Is.True);
            
            var siblings = solutions.Select(s => s.Bindings["X"]).Cast<Atom>().Select(a => a.Name).ToList();
            Assert.That(siblings, Contains.Item("alice"));
        }

        [Test]
        public void FamilyTree_SiblingQuery_ExcludesSelf()
        {
            // Arrange - Test that the \= operator correctly excludes self from sibling results
            LoadProgramIntoKnowledgeBase(TestPrograms.FamilyTree);
            var queryResult = parser.ParseQuery("?- sibling(X, Y).");
            Assert.That(queryResult.Success, Is.True, $"Query should parse successfully: {queryResult.ErrorMessage}");

            // Act
            var solutions = queryEngine.Solve(queryResult.Query).ToList();

            // Assert
            Assert.That(solutions.Count, Is.GreaterThan(0), "Should find sibling relationships");
            Assert.That(solutions.All(s => s.IsSuccess), Is.True);
            
            // Verify that no solution has X = Y (person is not their own sibling)
            foreach (var solution in solutions)
            {
                var x = ((Atom)solution.Bindings["X"]).Name;
                var y = ((Atom)solution.Bindings["Y"]).Name;
                Assert.That(x, Is.Not.EqualTo(y), $"Person {x} should not be their own sibling");
            }
        }

        #endregion

        #region Simple Rules Integration Tests

        [Test]
        public void SimpleRules_LoadProgram_ParsesSuccessfully()
        {
            // Act
            var result = parser.ParseProgram(TestPrograms.SimpleRules);

            // Assert
            Assert.That(result.Success, Is.True, $"Failed to parse simple rules program: {result.ErrorMessage}");
            Assert.That(result.Clauses.Count, Is.GreaterThan(0));
        }

        [Test]
        public void SimpleRules_HappyQuery_ReturnsCorrectResults()
        {
            // Arrange
            LoadProgramIntoKnowledgeBase(TestPrograms.SimpleRules);
            var queryResult = parser.ParseQuery("?- happy(X).");
            Assert.That(queryResult.Success, Is.True, $"Query should parse successfully: {queryResult.ErrorMessage}");

            // Act
            var solutions = queryEngine.Solve(queryResult.Query).ToList();

            // Assert
            Assert.That(solutions.Count, Is.EqualTo(2), "Mary and John should be happy (they like wine)");
            Assert.That(solutions.All(s => s.IsSuccess), Is.True);
            
            var happyPeople = solutions.Select(s => s.Bindings["X"]).Cast<Atom>().Select(a => a.Name).ToList();
            Assert.That(happyPeople, Contains.Item("mary"));
            Assert.That(happyPeople, Contains.Item("john"));
        }

        [Test]
        public void SimpleRules_LikesQuery_ReturnsCorrectResults()
        {
            // Arrange
            LoadProgramIntoKnowledgeBase(TestPrograms.SimpleRules);
            var queryResult = parser.ParseQuery("?- likes(mary, X).");
            Assert.That(queryResult.Success, Is.True, $"Query should parse successfully: {queryResult.ErrorMessage}");

            // Act
            var solutions = queryEngine.Solve(queryResult.Query).ToList();

            // Assert
            Assert.That(solutions.Count, Is.EqualTo(2), "Mary likes 2 things");
            Assert.That(solutions.All(s => s.IsSuccess), Is.True);
            
            var marysLikes = solutions.Select(s => s.Bindings["X"]).Cast<Atom>().Select(a => a.Name).ToList();
            Assert.That(marysLikes, Contains.Item("food"));
            Assert.That(marysLikes, Contains.Item("wine"));
        }

        #endregion

        #region Logic Puzzle Integration Tests

        [Test]
        public void LogicPuzzle_LoadProgram_ParsesSuccessfully()
        {
            // Act
            var result = parser.ParseProgram(TestPrograms.LogicPuzzle);

            // Assert
            Assert.That(result.Success, Is.True, $"Failed to parse logic puzzle program: {result.ErrorMessage}");
            Assert.That(result.Clauses.Count, Is.GreaterThan(0));
        }

        [Test]
        public void LogicPuzzle_RedObjectQuery_ReturnsCorrectResults()
        {
            // Arrange
            LoadProgramIntoKnowledgeBase(TestPrograms.LogicPuzzle);
            var queryResult = parser.ParseQuery("?- red_object(X).");
            Assert.That(queryResult.Success, Is.True, $"Query should parse successfully: {queryResult.ErrorMessage}");

            // Act
            var solutions = queryEngine.Solve(queryResult.Query).ToList();

            // Assert
            Assert.That(solutions.Count, Is.EqualTo(1), "Only ball should be red");
            Assert.That(solutions[0].IsSuccess, Is.True);
            Assert.That(((Atom)solutions[0].Bindings["X"]).Name, Is.EqualTo("ball"));
        }

        [Test]
        public void LogicPuzzle_RoundObjectQuery_ReturnsCorrectResults()
        {
            // Arrange
            LoadProgramIntoKnowledgeBase(TestPrograms.LogicPuzzle);
            var queryResult = parser.ParseQuery("?- round_object(X).");
            Assert.That(queryResult.Success, Is.True, $"Query should parse successfully: {queryResult.ErrorMessage}");

            // Act
            var solutions = queryEngine.Solve(queryResult.Query).ToList();

            // Assert
            Assert.That(solutions.Count, Is.EqualTo(1), "Only ball should be round");
            Assert.That(solutions[0].IsSuccess, Is.True);
            Assert.That(((Atom)solutions[0].Bindings["X"]).Name, Is.EqualTo("ball"));
        }

        [Test]
        public void LogicPuzzle_ColoredShapeQuery_ReturnsCorrectResults()
        {
            // Arrange
            LoadProgramIntoKnowledgeBase(TestPrograms.LogicPuzzle);
            var queryResult = parser.ParseQuery("?- colored_shape(blue, X).");
            Assert.That(queryResult.Success, Is.True, $"Query should parse successfully: {queryResult.ErrorMessage}");

            // Act
            var solutions = queryEngine.Solve(queryResult.Query).ToList();

            // Assert
            Assert.That(solutions.Count, Is.EqualTo(1), "Blue should map to square");
            Assert.That(solutions[0].IsSuccess, Is.True);
            Assert.That(((Atom)solutions[0].Bindings["X"]).Name, Is.EqualTo("square"));
        }

        #endregion

        #region Math Relations Integration Tests

        [Test]
        public void MathRelations_LoadProgram_ParsesSuccessfully()
        {
            // Act
            var result = parser.ParseProgram(TestPrograms.MathRelations);

            // Assert
            Assert.That(result.Success, Is.True, $"Failed to parse math relations program: {result.ErrorMessage}");
            Assert.That(result.Clauses.Count, Is.GreaterThan(0));
        }

        [Test]
        public void MathRelations_LessThanQuery_ReturnsCorrectResults()
        {
            // Arrange
            LoadProgramIntoKnowledgeBase(TestPrograms.MathRelations);
            var queryResult = parser.ParseQuery("?- less_than(zero, X).");
            Assert.That(queryResult.Success, Is.True, $"Query should parse successfully: {queryResult.ErrorMessage}");

            // Act
            var solutions = queryEngine.Solve(queryResult.Query).ToList();

            // Assert
            Assert.That(solutions.Count, Is.GreaterThan(0), "Zero should be less than other numbers");
            Assert.That(solutions.All(s => s.IsSuccess), Is.True);
            
            var greaterNumbers = solutions.Select(s => s.Bindings["X"]).Cast<Atom>().Select(a => a.Name).ToList();
            Assert.That(greaterNumbers, Contains.Item("one"));
        }

        [Test]
        public void MathRelations_SuccessorQuery_ReturnsCorrectResults()
        {
            // Arrange
            LoadProgramIntoKnowledgeBase(TestPrograms.MathRelations);
            var queryResult = parser.ParseQuery("?- succ(one, X).");
            Assert.That(queryResult.Success, Is.True, $"Query should parse successfully: {queryResult.ErrorMessage}");

            // Act
            var solutions = queryEngine.Solve(queryResult.Query).ToList();

            // Assert
            Assert.That(solutions.Count, Is.EqualTo(1), "One should have exactly one successor");
            Assert.That(solutions[0].IsSuccess, Is.True);
            Assert.That(((Atom)solutions[0].Bindings["X"]).Name, Is.EqualTo("two"));
        }

        #endregion

        #region End-to-End Workflow Tests

        [Test]
        public void EndToEnd_CompleteWorkflow_WorksCorrectly()
        {
            // Test the complete workflow: parse → load → query → get results
            
            // Step 1: Parse a program
            var parseResult = parser.ParseProgram(TestPrograms.FamilyTree);
            Assert.That(parseResult.Success, Is.True, "Program should parse successfully");

            // Step 2: Load into knowledge base
            foreach (var clause in parseResult.Clauses)
            {
                knowledgeBase.AddClause(clause);
            }
            Assert.That(knowledgeBase.ClauseCount, Is.GreaterThan(0), "Knowledge base should contain clauses");

            // Step 3: Parse and execute a query
            var queryResult = parser.ParseQuery("?- parent(X, Y).");
            Assert.That(queryResult.Success, Is.True, "Query should parse successfully");

            // Step 4: Get solutions
            var solutions = queryEngine.Solve(queryResult.Query).ToList();
            Assert.That(solutions.Count, Is.GreaterThan(0), "Should find parent relationships");
            Assert.That(solutions.All(s => s.IsSuccess), Is.True, "All solutions should be successful");

            // Step 5: Verify solution structure
            foreach (var solution in solutions)
            {
                Assert.That(solution.Bindings.ContainsKey("X"), Is.True, "Should bind X variable");
                Assert.That(solution.Bindings.ContainsKey("Y"), Is.True, "Should bind Y variable");
                Assert.That(solution.Bindings["X"], Is.InstanceOf<Atom>(), "X should be bound to atom");
                Assert.That(solution.Bindings["Y"], Is.InstanceOf<Atom>(), "Y should be bound to atom");
            }
        }

        [Test]
        public void EndToEnd_MultiplePrograms_WorkIndependently()
        {
            // Test that different programs can be loaded and queried independently
            
            // Load first program
            LoadProgramIntoKnowledgeBase(TestPrograms.SimpleRules);
            var queryResult1 = parser.ParseQuery("?- likes(mary, X).");
            Assert.That(queryResult1.Success, Is.True, $"Query should parse successfully: {queryResult1.ErrorMessage}");
            var solutions1 = queryEngine.Solve(queryResult1.Query).ToList();
            Assert.That(solutions1.Count, Is.EqualTo(2), "Mary should like 2 things");

            // Clear and load second program
            knowledgeBase.Clear();
            LoadProgramIntoKnowledgeBase(TestPrograms.LogicPuzzle);
            var queryResult2 = parser.ParseQuery("?- color(X).");
            Assert.That(queryResult2.Success, Is.True, $"Query should parse successfully: {queryResult2.ErrorMessage}");
            var solutions2 = queryEngine.Solve(queryResult2.Query).ToList();
            Assert.That(solutions2.Count, Is.EqualTo(3), "Should have 3 colors");

            // Verify first program queries no longer work
            var queryResult3 = parser.ParseQuery("?- likes(mary, X).");
            Assert.That(queryResult3.Success, Is.True, $"Query should parse successfully: {queryResult3.ErrorMessage}");
            var solutions3 = queryEngine.Solve(queryResult3.Query).ToList();
            Assert.That(solutions3.Count, Is.EqualTo(0), "Mary likes query should fail after clearing");
        }

        [Test]
        public void EndToEnd_ErrorHandling_WorksCorrectly()
        {
            // Test error handling in the complete workflow
            
            // Test invalid program parsing
            var invalidProgram = "invalid syntax here !!!";
            var parseResult = parser.ParseProgram(invalidProgram);
            Assert.That(parseResult.Success, Is.False, "Invalid program should fail to parse");
            Assert.That(parseResult.ErrorMessage, Is.Not.Null.And.Not.Empty, "Should provide error message");

            // Test invalid query parsing
            var invalidQuery = "invalid query ???";
            var queryResult = parser.ParseQuery(invalidQuery);
            Assert.That(queryResult.Success, Is.False, "Invalid query should fail to parse");
            Assert.That(queryResult.ErrorMessage, Is.Not.Null.And.Not.Empty, "Should provide error message");

            // Test query against empty knowledge base
            knowledgeBase.Clear();
            var validQueryResult = parser.ParseQuery("?- parent(X, Y).");
            Assert.That(validQueryResult.Success, Is.True, "Valid query should parse");
            
            var solutions = queryEngine.Solve(validQueryResult.Query).ToList();
            Assert.That(solutions.Count, Is.EqualTo(0), "Should return no solutions for empty knowledge base");
        }

        #endregion

        #region Helper Methods

        /// <summary>
        /// Helper method to load a program string into the knowledge base
        /// </summary>
        private void LoadProgramIntoKnowledgeBase(string program)
        {
            var result = parser.ParseProgram(program);
            Assert.That(result.Success, Is.True, $"Failed to parse program: {result.ErrorMessage}");
            
            foreach (var clause in result.Clauses)
            {
                knowledgeBase.AddClause(clause);
            }
        }

        #endregion
    }
}