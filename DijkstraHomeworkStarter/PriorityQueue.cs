﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

/// <summary>
/// IGME-106 - Game Development and Algorithmic Problem Solving
/// Homework 5
/// Class Description   : Priority Queue implementation
/// Author              : Benjamin Kleynhans
/// Modified By         : Benjamin Kleynhans
/// Date                : April 26, 2018
/// Filename            : PriorityQueue.cs
/// </summary>

namespace DijkstraHomeworkStarter
{
    class PriorityQueue
    {
        private List<Vertex> ObjectList { get; set; }
        private int count;
        
        /// <summary>
        /// Get the length/count of the queue
        /// </summary>
        public int Count
        {
            get { return ObjectList.Count; }
            set { count = value; }
        }

        /// <summary>
        /// Constructor
        /// </summary>
        public PriorityQueue()
        {
            ObjectList = new List<Vertex>();
        }

        /// <summary>
        /// Add a vertex to the queue
        /// </summary>
        /// <param name="passedObject">Vertex to add to the queue</param>
        public void Enqueue(Vertex passedObject)
        {
            ObjectList.Add(passedObject);
        }

        /// <summary>
        /// Remove the first vertex from the list/queue
        /// </summary>
        /// <returns>First vertex in the list that was removed</returns>
        public Vertex Dequeue()
        {
            Vertex returnValue = null;

            try
            {
                if (ObjectList.Count > 0)
                {
                    returnValue = ObjectList[0];
                    ObjectList.RemoveAt(0);
                }                
            }
            catch (Exception thisException)
            {
                if (thisException is ArgumentOutOfRangeException ||
                    thisException is IndexOutOfRangeException)
                {
                    throw new IndexOutOfRangeException(thisException.Message);
                }

            }

            if (ObjectList.Count > 0)
            {
                UpdatePriority();                                                           // Organize the queue so the next priority element is in front
            }

            return returnValue;
        }

        /// <summary>
        /// Returns the first element in the queue without removing it from the queue
        /// </summary>
        /// <returns>First vertex in the queue</returns>
        public Vertex Peek()
        {
            Vertex returnValue = null;

            try
            {
                if (ObjectList.Count > 0)
                {
                    returnValue = ObjectList[0];
                }                
            }
            catch (Exception thisException)
            {
                if (thisException is ArgumentOutOfRangeException ||
                    thisException is IndexOutOfRangeException)
                {
                    throw new IndexOutOfRangeException(thisException.Message);
                }
                
            }            

            return returnValue;
        }

        /// <summary>
        /// Moves the item with the higest priority to the front of the queue
        /// </summary>
        private void UpdatePriority()
        {            
            Vertex smallestVertex = ObjectList[0];

            foreach (Vertex listObject in ObjectList)
            {
                if (listObject.Distance < smallestVertex.Distance)
                {
                    smallestVertex = listObject;
                }
            }

            ObjectList.Remove(smallestVertex);
            ObjectList.Insert(0, smallestVertex);
        }

        /// <summary>
        /// Confirms whether the queue contains a value
        /// </summary>
        /// <param name="vertex">Value to check for</param>
        /// <returns>True or False depending on availability</returns>
        public bool Contains(Vertex vertex)
        {
            bool returnValue = false;

            if (ObjectList.Contains(vertex))
            {
                returnValue = true;
            }

            return returnValue;
        }

        /// <summary>
        /// Select a specific instance of a vertex from the queue
        /// </summary>
        /// <param name="vertex">Vertex of which the equivalent needs to be returned</param>
        /// <returns>The actual Vertex from the queue</returns>
        public Vertex VertexFromQueue(Vertex vertex)
        {
            Vertex returnValue = null;

            returnValue = ObjectList[GetIndex(vertex.Name)];

            return returnValue;
        }

        /// <summary>
        /// Return the numeric index of the specified object in the queue
        /// </summary>
        /// <param name="objectName">Name of the vertex to retrieve</param>
        /// <returns>Index of the vertex within the queue</returns>
        public int GetIndex(string objectName)
        {
            int returnValue = 0;

            foreach (Vertex vertex in ObjectList)
            {
                if (vertex.Name.Equals(objectName))
                {
                    returnValue = ObjectList.IndexOf(vertex);
                }
            }

            return returnValue;
        }

        /// <summary>
        /// Allows the replacement of a specific vertex in the queue based on vertex name
        /// </summary>
        /// <param name="vertex">Vertex that the one in the list needs to be replaced with</param>
        public void ReplaceVertex(Vertex vertex)
        {
            ObjectList[GetIndex(vertex.Name)] = vertex;
        }

        /// <summary>
        /// Allows the replacement of a specific vertex in teh queue based on index number
        /// </summary>
        /// <param name="index"></param>
        /// <param name="vertex"></param>
        public void UpdateVertexAt(int index, Vertex vertex)
        {
            ObjectList[index] = vertex;
        }

        /// <summary>
        /// ToString method
        /// </summary>
        /// <returns>All objects in the queue</returns>
        public override string ToString()
        {
            string returnValue = null;

            StringBuilder stringBuilder = new StringBuilder();

            foreach (Vertex vertex in ObjectList)
            {
                stringBuilder.Append(vertex.Name);
                stringBuilder.Append(" --> ");

                if (vertex.PreviousVertex != null)
                {
                    stringBuilder.Append(vertex.PreviousVertex.Name);
                }

                stringBuilder.Append(" --> ");
                stringBuilder.Append(vertex.Distance);
                stringBuilder.Append(" || ");
            }

            returnValue = stringBuilder.ToString();

            return returnValue;
        }
    }
}
