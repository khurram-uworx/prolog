using System;
using System.Collections.Generic;

namespace Prolog.Samples
{
    /// <summary>
    /// Contains sample Prolog programs for demonstration and testing purposes
    /// </summary>
    public static class SamplePrograms
    {
        /// <summary>
        /// Family tree program demonstrating parent-child relationships and derived rules
        /// </summary>
        public static readonly string FamilyTree = @"
% Family Tree Example
% This program demonstrates basic facts and rules for family relationships

% Facts: Direct parent-child relationships
parent(tom, bob).
parent(tom, alice).
parent(bob, charlie).
parent(bob, diana).
parent(alice, eve).
parent(alice, frank).
parent(charlie, george).
parent(diana, helen).

% Facts: Gender information
male(tom).
male(bob).
male(charlie).
male(frank).
male(george).
female(alice).
female(diana).
female(eve).
female(helen).

% Rules: Derived relationships
% A grandparent is someone who is a parent of a parent
grandparent(X, Z) :- parent(X, Y), parent(Y, Z).

% Siblings share the same parent but are different people
sibling(X, Y) :- parent(Z, X), parent(Z, Y), X \= Y.

% A father is a male parent
father(X, Y) :- parent(X, Y), male(X).

% A mother is a female parent
mother(X, Y) :- parent(X, Y), female(X).

% A grandfather is a male grandparent
grandfather(X, Z) :- grandparent(X, Z), male(X).

% A grandmother is a female grandparent
grandmother(X, Z) :- grandparent(X, Z), female(X).

% Ancestor relationship (recursive)
ancestor(X, Y) :- parent(X, Y).
ancestor(X, Y) :- parent(X, Z), ancestor(Z, Y).
";

        /// <summary>
        /// Simple rules program demonstrating basic logical relationships
        /// </summary>
        public static readonly string SimpleRules = @"
% Simple Rules Example
% This program demonstrates basic facts and simple derived rules

% Facts: Who likes what
likes(mary, food).
likes(mary, wine).
likes(john, wine).
likes(john, mary).

% Facts: Animal classifications
animal(cat).
animal(dog).
animal(bird).

% Facts: Pet names
pet(fluffy).
pet(rover).
pet(tweety).

% Rules: Derived properties
% Someone is happy if they like wine
happy(X) :- likes(X, wine).

% Two people are social if they like each other
social(X) :- likes(X, Y), likes(Y, X).

% A mammal is an animal that is not a bird
mammal(X) :- animal(X), X \= bird.
";

        /// <summary>
        /// Logic puzzle program for testing complex reasoning
        /// </summary>
        public static readonly string LogicPuzzle = @"
% Logic Puzzle Example
% This program demonstrates object properties and logical reasoning

% Facts: Available colors
color(red).
color(blue).
color(green).

% Facts: Available shapes
shape(circle).
shape(square).
shape(triangle).

% Facts: Objects with their properties (name, color, shape)
object(ball, red, circle).
object(box, blue, square).
object(hat, green, triangle).

% Rules: Property queries
% Find objects by color
red_object(X) :- object(X, red, _).
blue_object(X) :- object(X, blue, _).
green_object(X) :- object(X, green, _).

% Find objects by shape
round_object(X) :- object(X, _, circle).
square_object(X) :- object(X, _, square).
triangular_object(X) :- object(X, _, triangle).

% Find color-shape combinations
colored_shape(Color, Shape) :- object(_, Color, Shape).
";

        /// <summary>
        /// Mathematical relations program for testing arithmetic-like relationships
        /// </summary>
        public static readonly string MathRelations = @"
% Mathematical Relations Example
% This program demonstrates number relationships and basic arithmetic

% Facts: Number names
number(zero).
number(one).
number(two).
number(three).

% Facts: Successor relationships (next number)
succ(zero, one).
succ(one, two).
succ(two, three).

% Rules: Mathematical relationships
% Less than relationship (direct and transitive)
less_than(X, Y) :- succ(X, Y).
less_than(X, Z) :- succ(X, Y), less_than(Y, Z).

% Addition (simplified for small numbers)
add(zero, X, X).
add(X, Y, Z) :- succ(A, X), succ(B, Z), add(A, Y, B).
";

        /// <summary>
        /// Gets all sample programs as a dictionary for easy access
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

        /// <summary>
        /// Gets sample queries for each program to demonstrate functionality
        /// </summary>
        public static Dictionary<string, List<string>> GetSampleQueries()
        {
            return new Dictionary<string, List<string>>
            {
                {
                    "FamilyTree", new List<string>
                    {
                        "parent(tom, X).",           // Who are Tom's children?
                        "grandparent(tom, X).",      // Who are Tom's grandchildren?
                        "ancestor(tom, george).",    // Is Tom an ancestor of George?
                        "father(X, bob).",           // Who is Bob's father?
                        "sibling(X, Y).",            // Who are siblings?
                        "grandmother(X, helen)."     // Who is Helen's grandmother?
                    }
                },
                {
                    "SimpleRules", new List<string>
                    {
                        "likes(mary, X).",           // What does Mary like?
                        "happy(X).",                 // Who is happy?
                        "social(X).",                // Who is social?
                        "mammal(X).",                // What are mammals?
                        "animal(X).",                // What are animals?
                        "pet(X)."                    // What are pets?
                    }
                },
                {
                    "LogicPuzzle", new List<string>
                    {
                        "red_object(X).",            // What objects are red?
                        "round_object(X).",          // What objects are round?
                        "colored_shape(blue, X).",   // What shape is blue?
                        "object(X, red, circle).",   // What red circular objects exist?
                        "color(X).",                 // What colors exist?
                        "shape(X)."                  // What shapes exist?
                    }
                },
                {
                    "MathRelations", new List<string>
                    {
                        "succ(one, X).",             // What comes after one?
                        "less_than(zero, X).",       // What is greater than zero?
                        "less_than(X, three).",      // What is less than three?
                        "add(one, one, X).",         // What is one plus one?
                        "number(X).",                // What are the numbers?
                        "add(X, Y, two)."            // What adds up to two?
                    }
                }
            };
        }
    }
}