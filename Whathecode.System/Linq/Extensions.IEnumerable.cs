﻿using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using Whathecode.System.Extensions;


namespace Whathecode.System.Linq
{
    public static partial class Extensions
    {
        /// <summary>
        ///   Merges three sequences by using the specified predicate function.
        /// </summary>
        /// <typeparam name = "TFirst">The type of the elements of the first input sequence.</typeparam>
        /// <typeparam name = "TSecond">The type of the elements of the second input sequence.</typeparam>
        /// <typeparam name = "TThird">The type of the elements of the third input sequence.</typeparam>
        /// <typeparam name = "TResult">The type of the elements of the result sequence.</typeparam>
        /// <param name = "first">The first sequence to merge.</param>
        /// <param name = "second">The second sequence to merge.</param>
        /// <param name = "third">The third sequence to merge.</param>
        /// <param name = "resultSelector">A function that specifies how to merge the elements from the three sequences.</param>
        /// <returns>An <see cref = "IEnumerable{T}" /> that contains merged elements of three input sequences.</returns>
        public static IEnumerable<TResult> Zip<TFirst, TSecond, TThird, TResult>(
            this IEnumerable<TFirst> first,
            IEnumerable<TSecond> second,
            IEnumerable<TThird> third,
            Func<TFirst, TSecond, TThird, TResult> resultSelector )
        {
            Contract.Requires( first != null && second != null && third != null && resultSelector != null );

            using ( IEnumerator<TFirst> iterator1 = first.GetEnumerator() )
            using ( IEnumerator<TSecond> iterator2 = second.GetEnumerator() )
            using ( IEnumerator<TThird> iterator3 = third.GetEnumerator() )
            {
                while ( iterator1.MoveNext() && iterator2.MoveNext() && iterator3.MoveNext() )
                {
                    yield return resultSelector( iterator1.Current, iterator2.Current, iterator3.Current );
                }
            }
        }

        /// <summary>
        ///   Returns a specified number of random elements without returning a same element twice.
        /// </summary>
        /// <typeparam name = "T">The type of the elements of the input sequence.</typeparam>
        /// <param name = "source">The source for this extension method.</param>
        /// <param name = "count">The number of elements to return.</param>
        /// <returns>A sequence of random elements from the given sequence.</returns>
        public static IEnumerable<T> TakeRandom<T>( this IEnumerable<T> source, int count )
        {
            Contract.Requires( source != null );
            Contract.Requires( count <= source.Count() );

            List<T> remainingElements = source.ToList();
            Random random = new Random();

            for ( int taken = 0; taken < count; ++taken )
            {
                int randomIndex = remainingElements.GetIndexInterval().GetValueAt( random.NextDouble() );
                yield return remainingElements[ randomIndex ];

                remainingElements.RemoveAt( randomIndex );
            }
        }

        /// <summary>
        ///   Returns all combinations of a chosen amount of selected elements in the sequence.
        /// </summary>
        /// <typeparam name = "T">The type of the elements of the input sequence.</typeparam>
        /// <param name = "source">The source for this extension method.</param>
        /// <param name = "select">The amount of elements to select for every combination.</param>
        /// <param name = "repetition">True when repetition of elements is allowed.</param>
        /// <returns>All combinations of a chosen amount of selected elements in the sequence.</returns>
        public static IEnumerable<IEnumerable<T>> Combinations<T>( this IEnumerable<T> source, int select, bool repetition = false )
        {
            Contract.Requires( source != null );
            Contract.Requires( select >= 0 );

            return select == 0
                       ? new[] { new T[0] }
                       : source.SelectMany( ( element, index ) =>
                           source.Skip( repetition ? index : index + 1 )
                                 .Combinations( select - 1, repetition )
                                 .Select( c => new[] { element }.Concat( c ) ) );
        }

        /// <summary>
        ///   Returns whether the sequence contains a certain amount of elements.
        /// </summary>
        /// <typeparam name = "T">The type of the elements of the input sequence.</typeparam>
        /// <param name = "source">The source for this extension method.</param>
        /// <param name = "count">The amount of elements the sequence should contain.</param>
        /// <returns>True when the sequence contains the specified amount of elements, false otherwise.</returns>
        public static bool CountOf<T>( this IEnumerable<T> source, int count )
        {
            Contract.Requires( source != null );
            Contract.Requires( count >= 0 );

        	return source.Take( count + 1 ).Count() == count;
        }
    }
}