using System.Collections.Generic;
using Prolog;

namespace Prolog.Tests
{
    [TestFixture]
    public class TermTests
    {
        [Test]
        public void Atom_BasicProperties()
        {
            var atom = new Atom("tom");
            
            Assert.That(atom.Name, Is.EqualTo("tom"));
            Assert.That(atom.IsVariable, Is.False);
            Assert.That(atom.ToString(), Is.EqualTo("tom"));
        }

        [Test]
        public void Variable_BasicProperties()
        {
            var variable = new Variable("X");
            
            Assert.That(variable.Name, Is.EqualTo("X"));
            Assert.That(variable.IsVariable, Is.True);
            Assert.That(variable.ToString(), Is.EqualTo("X"));
        }

        [Test]
        public void Compound_BasicProperties()
        {
            var compound = new Compound("parent", new Atom("tom"), new Atom("bob"));
            
            Assert.That(compound.Functor, Is.EqualTo("parent"));
            Assert.That(compound.Arity, Is.EqualTo(2));
            Assert.That(compound.IsVariable, Is.False);
            Assert.That(compound.ToString(), Is.EqualTo("parent(tom, bob)"));
        }

        [Test]
        public void Variable_Substitution()
        {
            var variable = new Variable("X");
            var atom = new Atom("tom");
            var bindings = new Dictionary<string, Term> { { "X", atom } };
            
            var result = variable.Substitute(bindings);
            
            Assert.That(result, Is.EqualTo(atom));
        }

        [Test]
        public void Compound_Substitution()
        {
            var compound = new Compound("parent", new Variable("X"), new Atom("bob"));
            var bindings = new Dictionary<string, Term> { { "X", new Atom("tom") } };
            
            var result = compound.Substitute(bindings);
            
            Assert.That(result.ToString(), Is.EqualTo("parent(tom, bob)"));
        }

        [Test]
        public void Term_Equality()
        {
            var atom1 = new Atom("tom");
            var atom2 = new Atom("tom");
            var atom3 = new Atom("bob");
            
            Assert.That(atom1, Is.EqualTo(atom2));
            Assert.That(atom1, Is.Not.EqualTo(atom3));
            
            var var1 = new Variable("X");
            var var2 = new Variable("X");
            var var3 = new Variable("Y");
            
            Assert.That(var1, Is.EqualTo(var2));
            Assert.That(var1, Is.Not.EqualTo(var3));
        }

        [Test]
        public void Compound_Equality()
        {
            var compound1 = new Compound("parent", new Atom("tom"), new Atom("bob"));
            var compound2 = new Compound("parent", new Atom("tom"), new Atom("bob"));
            var compound3 = new Compound("parent", new Atom("alice"), new Atom("bob"));
            
            Assert.That(compound1, Is.EqualTo(compound2));
            Assert.That(compound1, Is.Not.EqualTo(compound3));
        }

        [Test]
        public void Variable_SubstitutionChain()
        {
            var variable = new Variable("X");
            var bindings = new Dictionary<string, Term> 
            { 
                { "X", new Variable("Y") },
                { "Y", new Atom("tom") }
            };
            
            var result = variable.Substitute(bindings);
            
            Assert.That(result.ToString(), Is.EqualTo("tom"));
        }
    }
}