using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SmallGame.GameObjects;

namespace SmallGame.Collections
{
    public class SampleUseCase
    {
        public static void Main()
        {
            var tree = new BinTree<BasicObject, float>(gob => gob.Mass);

            var fifteen = new BasicObject() {Mass = 15};
            var fifteen2 = new BasicObject() { Mass = 15, Inertia = 2};

            tree.Add(new BasicObject() { Mass = 10 });
            tree.Add(new BasicObject() { Mass = 5 });
            tree.Add(fifteen);
            tree.Add(fifteen2);

            tree.Add(new BasicObject() { Mass = 3 });
            tree.Add(new BasicObject() { Mass = 10 });

            tree.Add(new BasicObject() { Mass = 12 });
            tree.Add(new BasicObject() { Mass = 3 });
            tree.Add(new BasicObject() { Mass = 17 });
            tree.Add(new BasicObject() { Mass = 25 });
            tree.Add(new BasicObject() { Mass = 8 });

            var set = tree.Find(3, 25);
        }
    }

    public class BinTree<T, C> where C : IComparable<C>
    {
        public List<T> Elements { get; set; }
        public BinTree<T, C> Left { get; set; }
        public BinTree<T, C> Right { get; set; }

        public Func<T, C> DataProducer { get; set; }

        //public BinTree(Func<T, C> dataProducer) : this(default(T), dataProducer) { }

        public BinTree(Func<T, C> dataProducer)
        {
            Elements = new List<T>();
            DataProducer = dataProducer;

        }


        public List<T> Find(C minQuery, C maxQuery)
        {
            var firstElem = Elements.FirstOrDefault();
            if (Elements.Count == 0)
            {
                return new List<T>();
            }
            else if (DataProducer(firstElem).CompareTo(minQuery) < 0)
            {
                return Right == null ? new List<T>() : Right.Find(minQuery, maxQuery);
            }
            else if (DataProducer(firstElem).CompareTo(maxQuery) > 0)
            {

                return Left == null ? new List<T>() : Left.Find(minQuery, maxQuery);
            }
            else
            {
                var returnSet = Elements
                    .Concat(Left == null ? new List<T>() : Left.Find(minQuery, maxQuery))
                    .Concat(Right == null ? new List<T>() : Right.Find(minQuery, maxQuery))
                    .ToList();
                return returnSet;
            }
        } 

        public List<T> Find(C query)
        {
            var firstElem = Elements.FirstOrDefault();
            if (Elements.Count == 0)
            {
                return new List<T>();
            }
            else if (DataProducer(firstElem).CompareTo(query) == 0)
            {
                return Elements;
            }
            else if (DataProducer(firstElem).CompareTo(query) > 0)
            {
                return Left == null ? new List<T>() : Left.Find(query);
            }
            else
            {
                return Right == null ? new List<T>() : Right.Find(query);
            }
        }

        public void Delete(T elem)
        {
            var firstElem = Elements.FirstOrDefault();
           
            if (Elements.Count == 0)
            {
                // do nothing.
            }
            else if (DataProducer(firstElem).CompareTo(DataProducer(elem)) == 0)
            {
                // we found it!
                Elements.Remove(elem);
                if (Elements.Count == 0)
                {
                    
                    if (Left == null && Right == null)
                    {
                        // we give no fucks. 
                        // but, there will be some empty lists awaiting new entries. 
                    }
                    else if (Left != null && Right == null)
                    {
                        Elements = Left.Elements;
                        Right = Left.Right;
                        Left = Left.Left;
                        // we shift the left branch to become this branch. 
                    }
                    else if (Left == null && Right != null)
                    {
                        Elements = Right.Elements;
                        Left = Right.Left;
                        Right = Right.Right;
                        // we shift the right branch to become this branch. 
                    }
                    else if (Left != null && Right != null)
                    {
                        Elements = Left.Elements;

                        var leftMost = Right.Left;
                        while (leftMost != null)
                        {
                            leftMost = leftMost.Left;
                        }
                        leftMost.Left = Left.Right;

                        Left = Left.Left;
                    }
                }
            }
            else if (DataProducer(firstElem).CompareTo(DataProducer(elem)) > 0)
            {
                if (Left != null)
                {
                    Left.Delete(elem);
                }
            }
            else if (Right != null)
            {
                Right.Delete(elem);
            }
        }

        public void Add(T elem)
        {
            var firstElem = Elements.FirstOrDefault();
            
            if (Elements.Count == 0 || DataProducer(firstElem).CompareTo(DataProducer(elem)) == 0 )
            {
                Elements.Add(elem);
            }
            else if (DataProducer(firstElem).CompareTo(DataProducer(elem)) > 0)
            {
                if (Left == null)
                {
                    Left = new BinTree<T, C>(DataProducer);
                    Left.Elements.Add(elem);
                }
                else
                {
                    Left.Add(elem);
                }
            }
            else
            {
                if (Right == null)
                {
                    Right = new BinTree<T, C>(DataProducer);
                    Right.Elements.Add(elem);
                }
                else
                {
                    Right.Add(elem);
                }
            }

        }

    }
}
