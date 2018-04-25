using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Graphics;

namespace DijkstraHomeworkStarter
{
	class Graph
	{
		#region Fields
		// Assets
		private Texture2D pixel;
		private Texture2D vertexTexture;
		private SpriteFont font;

		// The list of vertices in the graph
		// Note that the index in this list is also the vertex's row/col in the matrix.
		List<Vertex> vertices;

		// The dictionary of vertices, to look up their indices quickly
		Dictionary<String, int> vertNameToIndex;

		// Adjacency matrix
		int[,] adjMatrix;

		// A matrix of colors for the edges.
		// We'll use this for drawing the result of Dijkstra's algorithm.
		Color[,] edgeColor;

		// The currently selected source vertex
		Vertex selectedVertex;
        #endregion

        #region Constants
        // The maximum number of vertices
        public const int MAX_VERTICES = 10;

		// The "width" of an edge when drawn
		public const int EDGE_WIDTH = 2;

		// Colors
		public Color NORMAL_EDGE_COLOR = Color.Brown;
		public Color HIGHLIGHT_EDGE_COLOR = Color.Orange;
		public Color VERTEX_NAME_COLOR = Color.CornflowerBlue;
		public Color VERTEX_DIST_COLOR = Color.Green;
		#endregion

		#region Properties
		/// <summary>
		/// Gets or sets the sprite font for drawing text
		/// </summary>
		public SpriteFont Font { get { return font; } set { font = value; } }

		/// <summary>
		/// Gets or sets the texture for the vertices
		/// </summary>
		public Texture2D VertexTexture { get { return vertexTexture; } set { vertexTexture = value; } }
		#endregion

		#region Constructor
		/// <summary>
		/// Creates a new graph
		/// </summary>
		/// <param name="device">The graphics device for this XNA game</param>
		public Graph(GraphicsDevice device)
		{
			// Set up the vertex list & dictionary
			vertices = new List<Vertex>();
			vertNameToIndex = new Dictionary<String, int>();
			selectedVertex = null;

			// Set up the adjacency matrix
			adjMatrix = new int[MAX_VERTICES, MAX_VERTICES];
			edgeColor = new Color[MAX_VERTICES, MAX_VERTICES];

			// Create a 1x1 white pixel texture
			pixel = new Texture2D(device, 1, 1);
			pixel.SetData<Color>(new Color[] { Color.White });
        }
		#endregion

		#region Add Methods
		/// <summary>
		/// Adds a vertex to the graph
		/// </summary>
		/// <param name="vert">The vert to add</param>
		public void AddVertex(Vertex vert)
		{
			// Add the vertex if we're below the maximum
			if (vertices.Count < MAX_VERTICES)
			{
				vertNameToIndex.Add(vert.Name, vertices.Count);
				vertices.Add(vert);
			}
		}

		/// <summary>
		/// Adds a directed (one-way) edge to the graph
		/// </summary>
		/// <param name="vert1">The starting vert</param>
		/// <param name="vert2">The ending vert</param>
		/// <param name="weight">The weight of this edge</param>
		public void AddDirectedEdge(int vert1, int vert2, int weight)
		{
			// Add the edge
			adjMatrix[vert1, vert2] = weight;
			// Set a default color
			edgeColor[vert1, vert2] = NORMAL_EDGE_COLOR;
		}

		/// <summary>
		/// Adds a directed (one-way) edge to the graph
		/// </summary>
		/// <param name="vert1">The starting vert</param>
		/// <param name="vert2">The ending vert</param>
		/// <param name="weight">The weight of this edge</param>
		public void AddDirectedEdge(String vert1, String vert2, int weight)
		{
			// Add the edge if the verts exist
			if (vertNameToIndex.ContainsKey(vert1) && vertNameToIndex.ContainsKey(vert2))
			{
				adjMatrix[vertNameToIndex[vert1], vertNameToIndex[vert2]] = weight;
				edgeColor[vertNameToIndex[vert1], vertNameToIndex[vert2]] = NORMAL_EDGE_COLOR;
			}
		}

		/// <summary>
		/// Adds an undirected (two-way) edge to the graph
		/// </summary>
		/// <param name="vert1">One of the verts</param>
		/// <param name="vert2">The other vert</param>
		/// <param name="weight">The weight of this edge</param>
		public void AddUndirectedEdge(int vert1, int vert2, int weight)
		{
			// Add the edge in both directions
			AddDirectedEdge(vert1, vert2, weight);
			AddDirectedEdge(vert2, vert1, weight);
		}

		/// <summary>
		/// Adds an undirected (two-way) edge to the graph
		/// </summary>
		/// <param name="vert1">One of the verts</param>
		/// <param name="vert2">The other vert</param>
		/// <param name="weight">The weight of this edge</param>
		public void AddUndirectedEdge(String vert1, String vert2, int weight)
		{
			// Add the edge in both directions
			AddDirectedEdge(vert1, vert2, weight);
			AddDirectedEdge(vert2, vert1, weight);
		}
		#endregion

		#region Draw Methods
		/// <summary>
		/// Highlights an edge
		/// </summary>
		/// <param name="vert1">The first vertex that defines the edge</param>
		/// <param name="vert2">The second vertex that defines the edge</param>
		public void HighlightEdge(int vert1, int vert2)
		{
			edgeColor[vert1, vert2] = HIGHLIGHT_EDGE_COLOR;
		}

		/// <summary>
		/// Draws the graph's edges to the screen
		/// </summary>
		/// <param name="sb">
		/// The sprite batch to use when drawing.  This method assumes that
		/// the sprite batch's Begin() has already been called.
		/// </param>
		public void DrawEdges(SpriteBatch sb)
		{
			// Loop through the adjacency matrix and draw any edges.
			// Note: Since the vertices list can never have more vertices than
			//       MAX_VERTICES, but it could have fewer, we'll just loop
			//       enough times to cover all the verts, not necessarily through
			//       the entire adjacency matrix
			for (int row = 0; row < vertices.Count; row++)
			{
				for (int col = 0; col < vertices.Count; col++)
				{
					// Check for an edge
					if (adjMatrix[row, col] > 0)
					{
						// Found an edge, so draw it
						DrawOneEdge(vertices[row], vertices[col], edgeColor[row, col], sb);
					}
				}
			}
		}

		/// <summary>
		/// Helper method for drawing a single edge
		/// </summary>
		/// <param name="v1">The first vertex</param>
		/// <param name="v2">The second vertex</param>
		/// <param name="color">The color of the edge & weight</param>
		/// <param name="sb">The spritebatch to use when drawing</param>
		private void DrawOneEdge(Vertex v1, Vertex v2, Color color, SpriteBatch sb)
		{
			// Calculate the scale of the edge in pixels
			Vector2 scale = new Vector2(Vector2.Distance(v2.Position, v1.Position), EDGE_WIDTH);

			// Calculate the rotation
			float rotation = (float)Math.Atan2(v2.Position.Y - v1.Position.Y, v2.Position.X - v1.Position.X);

			// Draw
			sb.Draw(
				pixel,
				v1.Position,
				null,
				color,
				rotation,
				Vector2.Zero,
				scale,
				SpriteEffects.None,
				0.0f
			);

			// Get the edge's weight
			int weight = adjMatrix[vertNameToIndex[v1.Name], vertNameToIndex[v2.Name]];

			// Draw above the center
			Vector2 pos = v1.Position;
			pos.X += (v2.Position.X - v1.Position.X) / 2.0f;
			pos.Y += (v2.Position.Y - v1.Position.Y) / 2.0f;

			// Draw the text
			sb.DrawString(font, weight.ToString(), pos, Color.White);
		}

		/// <summary>
		/// Draws the graph's vertices to the screen
		/// </summary>
		/// <param name="sb">
		/// The sprite batch to use when drawing.  This method assumes that
		/// the sprite batch's Begin() has already been called.
		/// </param>
		public void DrawVertices(SpriteBatch sb)
		{
			// Loop through the vertices and draw them to the screen
			foreach (Vertex vert in vertices)
			{
				// Offset the position (centering the graphic)
				Vector2 pos = vert.Position;
				pos.X -= vertexTexture.Width / 2.0f;
				pos.Y -= vertexTexture.Height / 2.0f;

				// Draw this vert
				if (vert == selectedVertex)
				{
					sb.Draw(vertexTexture, pos, Color.Orange);
				}
				else if (GetVertexUnderMouse() == vert)
				{
					sb.Draw(vertexTexture, pos, Color.Yellow);
				}
				else
				{
					sb.Draw(vertexTexture, pos, Color.White);
				}

				// Draw the name
				Vector2 namePos = vert.Position;
				namePos.Y += (int)(vertexTexture.Height / 2.0f);
				namePos.X -= (int)(font.MeasureString(vert.Name).X / 2.0f);
				sb.DrawString(font, vert.Name, namePos, VERTEX_NAME_COLOR);

				// Draw the distance
				if (vert.Distance < int.MaxValue && vert.Distance > 0)
				{
					Vector2 distPos = namePos;
					distPos.X += font.MeasureString(vert.Name).X;
					sb.DrawString(font, " // " + vert.Distance.ToString(), distPos, VERTEX_DIST_COLOR);
				}
			}
		}

		/// <summary>
		/// Resets each vertex's visited property and resets the edge colors,
		/// preparing for another Dijkstra's.
		/// </summary>
		public void ResetPaths()
		{
			// Loop and reset the vertex and the edge colors
			for (int i = 0; i < vertices.Count; i++)
			{
				vertices[i].Reset();
				for (int j = 0; j < vertices.Count; j++)
				{
					if (adjMatrix[i, j] > 0)
					{
						edgeColor[i, j] = NORMAL_EDGE_COLOR;
					}
				}
			}
		}
		#endregion

		#region Mouse Methods
		/// <summary>
		/// Returns the vertex under the mouse, if there is one
		/// </summary>
		/// <returns>A vertex, or null</returns>
		public Vertex GetVertexUnderMouse()
		{
			// Get the mouse state
			MouseState ms = Mouse.GetState();

			// Loop through the vertices
			foreach (Vertex v in vertices)
			{
				// Figure out this vert's rectangle on the screen
				Rectangle vertRect = new Rectangle();
				vertRect.X = (int)(v.Position.X - (vertexTexture.Width / 2.0f));
				vertRect.Y = (int)(v.Position.Y - (vertexTexture.Height / 2.0f));
				vertRect.Width = vertexTexture.Width;
				vertRect.Height = vertexTexture.Height;

				// Test the mouse pos
				if (ms.X >= vertRect.Left && ms.X <= vertRect.Right &&
					ms.Y >= vertRect.Top && ms.Y <= vertRect.Bottom)
				{
					// The mouse is over this vertex
					return v;
				}
			}

			// Nothing found
			return null;
		}

		/// <summary>
		/// Selects a vertex
		/// </summary>
		/// <param name="v">The vertex to select</param>
		public void SelectVertex(Vertex v)
		{
			// Set the selected vertex
			selectedVertex = v;

			// Was there one?
			if (selectedVertex != null)
				FindAndHighlightShortestPaths(selectedVertex);
		}
		#endregion

		#region Student Methods
		/// <summary>
		/// This should highlight the tree of shortest paths from the vertex to all other vertices,
		/// using Dijkstra's algorithm.
		/// </summary>
		public void FindAndHighlightShortestPaths(Vertex v)
		{
            // Implement this method!
            v.Permanent = true;
            v.Distance = 0;

            bool nonPermanentVertex;            
            List<Vertex> adjacentsList = new List<Vertex>();

            PriorityQueue priorityQueue = new PriorityQueue();
            priorityQueue.Enqueue(v);

            do
            {
                int vertexCounter = 0;
                nonPermanentVertex = false;

                do
                {
                    if (!vertices[vertexCounter].Permanent)
                    {
                        nonPermanentVertex = true;
                    }

                    vertexCounter++;

                } while ((!nonPermanentVertex) && (vertexCounter < vertices.Count));

                adjacentsList = GetAdjacents(priorityQueue.Peek(), priorityQueue);                

                priorityQueue.Dequeue();

                adjacentsList.Clear();

            } while (nonPermanentVertex);

			// To highlight a path, use the HighlightPath method.  It takes two vertices as parameters.
		}

        private void UpdateDistances(Vertex rootNode, List<Vertex> adjacentsList)
        {            
            int distanceBetweenNodes = int.MaxValue;

            int vertices = adjacentsList.Count;
            int permanentVertices = 0;

            foreach (Vertex vertex in adjacentsList)
            {
                if (vertex.Permanent)
                {
                    permanentVertices++;
                }
            }

            if (permanentVertices == vertices)
            {
                rootNode.Permanent = true;
            }
            else
            {
                foreach (Vertex vertex in adjacentsList)
                {
                    distanceBetweenNodes = adjMatrix[vertNameToIndex[rootNode.Name], vertNameToIndex[vertex.Name]] + rootNode.Distance;

                    if (((vertex.PreviousVertex == null) || (vertex.Distance > distanceBetweenNodes)) && (!vertex.Permanent))
                    {
                        vertex.PreviousVertex = rootNode;
                        vertex.Distance = distanceBetweenNodes;
                    }
                }

                SetPermanent(adjacentsList);
            }
        }

        private void SetPermanent(List<Vertex> adjacentsList)
        {
            Vertex closestVertex = null;
            bool foundNonPermanent = false;
            int incrementor = 0;

            do
            {
                if (!adjacentsList[incrementor].Permanent)
                {
                    closestVertex = adjacentsList[incrementor];
                    foundNonPermanent = true;
                }

                incrementor++;

            } while ((!foundNonPermanent) && (incrementor < adjacentsList.Count));

            if (closestVertex != null)
            {
                if (adjacentsList.Count > 1)
                {
                    for (int i = 1; i < adjacentsList.Count; i++)
                    {
                        if ((adjacentsList[i].Distance < closestVertex.Distance) && (!adjacentsList[i].Permanent))
                        {
                            closestVertex = adjacentsList[i];
                        }
                    }
                }

                vertices[vertNameToIndex[closestVertex.Name]].Permanent = true;
            }            
        }

        // Feel free to add any helper methods you may need!
        // <summary>
        /// Return the name (key) of the next unvisited vertex
        /// </summary>
        /// <param name="name">Name/Key of the Dictionary item to look for</param>
        /// <returns>Vertex of the next, previously unvisited vertex</returns>
        public List<Vertex> GetAdjacents(Vertex keyVertex, PriorityQueue priorityQueue)
        {
            List<Vertex> returnValue = new List<Vertex>();

            int indexOfVertexInList = 0;
            int incrementor = 0;
            
            if (vertNameToIndex.ContainsKey(keyVertex.Name))                                // Test if the key exists in the dictionary
            {
                indexOfVertexInList = vertices.IndexOf(keyVertex);                          // Get the index of the vertex

                do                                                                          // Loop through vertices until one is found that
                {                                                                           // wasn't found before
                    if (adjMatrix[indexOfVertexInList, incrementor] != 0)
                    {
                        returnValue.Add(vertices[incrementor]);                               // Return the next vertex

                        if ((!priorityQueue.Contains(vertices[incrementor])) && (!vertices[incrementor].Permanent))
                        {
                            priorityQueue.Enqueue(vertices[incrementor]);
                        }                        
                    }

                    incrementor++;                                                          // do/while incrementor

                } while (incrementor < adjMatrix.GetLength(1));        // Once we find a vertex, or get to the end of the matrix, break
            }

            UpdateDistances(priorityQueue.Peek(), returnValue);

            return returnValue;
        }


        /// <summary>
        /// Set Visited properties to false
        /// </summary>
        public void ResetVisited()
        {
            foreach (Vertex vertex in vertices)
            {
                vertex.Reset();
            }
        }

        #endregion
    }
}
