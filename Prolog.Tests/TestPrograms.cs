using System;
using System.Collections.Generic;

namespace Prolog.Tests
{
    /// <summary>
    /// Contains test data with sample Prolog programs for integration testing
    /// </summary>
    public static class TestPrograms
    {
        /// <summary>
        /// Family tree program with facts and rules for testing relationships
        /// </summary>
        public static readonly string FamilyTree = @"
% Facts about parent relationships
parent(tom, bob).
parent(tom, alice).
parent(bob, charlie).
parent(bob, diana).
parent(alice, eve).
parent(alice, frank).
parent(charlie, george).
parent(diana, helen).

% Facts about gender
male(tom).
male(bob).
male(charlie).
male(frank).
male(george).
female(alice).
female(diana).
female(eve).
female(helen).

% Rules for derived relationships
grandparent(X, Z) :- parent(X, Y), parent(Y, Z).
father(X, Y) :- parent(X, Y), male(X).
mother(X, Y) :- parent(X, Y), female(X).
grandfather(X, Z) :- grandparent(X, Z), male(X).
grandmother(X, Z) :- grandparent(X, Z), female(X).
ancestor(X, Y) :- parent(X, Y).
ancestor(X, Y) :- parent(X, Z), ancestor(Z, Y).
";

        /// <summary>
        /// Simple rules program for testing basic logical relationships
        /// </summary>
        public static readonly string SimpleRules = @"
% Facts about likes relationships
likes(mary, food).
likes(mary, wine).
likes(john, wine).
likes(john, mary).

% Facts about animals
animal(cat).
animal(dog).
animal(bird).

% Facts about pets
pet(fluffy).
pet(rover).
pet(tweety).

% Rules for derived relationships
happy(X) :- likes(X, wine).
social(X) :- likes(X, Y), likes(Y, X).
";

        /// <summary>
        /// Logic puzzle program for testing complex reasoning
        /// </summary>
        public static readonly string LogicPuzzle = @"
% Facts about colors
color(red).
color(blue).
color(green).

% Facts about shapes
shape(circle).
shape(square).
shape(triangle).

% Facts about objects
object(ball, red, circle).
object(box, blue, square).
object(hat, green, triangle).

% Rules for queries
red_object(X) :- object(X, red, _).
round_object(X) :- object(X, _, circle).
colored_shape(Color, Shape) :- object(_, Color, Shape).
";

        /// <summary>
        /// Mathematical relations program for testing arithmetic-like relationships
        /// </summary>
        public static readonly string MathRelations = @"
% Facts about numbers
number(zero).
number(one).
number(two).
number(three).

% Facts about successor relationships
succ(zero, one).
succ(one, two).
succ(two, three).

% Rules for mathematical relationships
less_than(X, Y) :- succ(X, Y).
less_than(X, Z) :- succ(X, Y), less_than(Y, Z).
add(zero, X, X).
add(X, Y, Z) :- succ(A, X), succ(B, Z), add(A, Y, B).
";

        /// <summary>
        /// Gets all test programs as a dictionary for easy access
        /// </summary>
        public static Dictionary<string, string> GetAllPrograms()
        {
            return new Dictionary<string, string>
            {
                { "FamilyTree", FamilyTree },
                { "SimpleRules", SimpleRules },
                { "LogicPuzzle", LogicPuzzle },
                { "MathRelations", MathRelations }
            };
        }
    }
}