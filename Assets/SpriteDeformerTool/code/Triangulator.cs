using System;
using UnityEngine;
using System.Collections.Generic;
using Medvedya.GeometryMath;

namespace Medvedya.SpriteDeformerTools
{

    public class Triangulator
    {
        public class Node
        {
            public int index = 0;
            public List<Node> nodes = new List<Node>();
            public SpritePoint point;
            public bool isBigLoop = false;
            public List<Node> needCheck = new List<Node>();
            public bool useDeltaPosition = false;
            public Vector2 position {
                get {

                    Vector2 p = useDeltaPosition ? point.spritePosition + (Vector2)point.offset : point.spritePosition;
                    
                    //funny fix :)
                  //  p += new Vector2(UnityEngine.Random.Range(0.001f, 0.01f), UnityEngine.Random.Range(0.0001f, 0.01f));

                    return p; 
                }
            }

            public Node getMinimalAgleNode(Node fromNode)
            {
                Vector2 fromDir = (position - fromNode.position).normalized;
                float minAngle = float.MaxValue;
                Node minNode = null;
                foreach (Node n in nodes)
                {
                    if (n == fromNode) continue;
                    float ca = Vector2Utillites.AngleFromTo(fromDir, (position - n.position).normalized);
                    if (ca < minAngle)
                    {
                        minAngle = ca;
                        minNode = n;
                    }
                }
                return minNode;
            }
            public Node getMinimalAgleNode(Vector2 direction)
            {
                Vector2 fromDir = direction;
                float minAngle = float.MaxValue;
                Node minNode = null;
                foreach (Node n in nodes)
                {
                    float ca = Vector2Utillites.AngleFromTo(fromDir, (position - n.position).normalized);
                    if (ca < minAngle)
                    {
                        minAngle = ca;
                        minNode = n;
                    }
                }
                return minNode;
            }

        }
        public class Connection
        {
            public Node node1;
            public Node node2;
            public Connection(Node n1, Node n2)
            {
                node1 = n1;
                node2 = n2;
            }
            public bool constains(Node n)
            {
                return n == node1 || n == node2;
            }
        }
        public class BigLoop
        {
            public List<Node> edgeNodes = new List<Node>();
            public List<Node> insideNodes = new List<Node>();
            public List<Node> allNodes = new List<Node>();
            public Triangulator triangulator;
            public bool isBelong(Node node)
            {
                int npol = edgeNodes.Count;
                int j = npol - 1;
                bool c = false;
                for (int i = 0; i < npol; i++)
                {
                    if ((((edgeNodes[i].position.y <= node.position.y) && (node.position.y < edgeNodes[j].position.y)) || ((edgeNodes[j].position.y <= node.position.y) && (node.position.y < edgeNodes[i].position.y))) &&
                    (node.position.x > (edgeNodes[j].position.x - edgeNodes[i].position.x) * (node.position.y - edgeNodes[i].position.y) / (edgeNodes[j].position.y - edgeNodes[i].position.y) + edgeNodes[i].position.x))
                    {
                        c = !c;
                    }
                    j = i;
                }
                return c;
            }
            public void debug()
            {
                string s = "";
                foreach (Node n in edgeNodes)
                {
                    s += triangulator.nodes.IndexOf(n).ToString() + " ";
                }
                Debug.Log(s);
            }
            public void triangulate()
            {
                int countC = 0;
                foreach (Node n in allNodes)
                {
                    foreach (Node n2 in allNodes)
                    {
                        if (n2.isBigLoop && n.isBigLoop)
                        { 
                            int ind = this.edgeNodes.IndexOf(n);
                            Node back = edgeNodes[ind > 0 ? ind - 1 : this.edgeNodes.Count - 1];
                            Node next = edgeNodes[ind < this.edgeNodes.Count - 1 ? ind + 1 : 0];
                            float a = Vector2Utillites.AngleFromTo(n.position - back.position, n.position - next.position);
                            float ca = Vector2Utillites.AngleFromTo(n.position - back.position, n.position - n2.position);
                            //if (a == 0 && ca == 180f) continue;
                            //if (a == 180 && ca == 0f) continue;
                            if (Mathf.DeltaAngle(a, ca) == 180) continue;
                            if (ca <= a) continue;
                        }
                        if ( n2 != n && !n.nodes.Contains(n2))
                        {
                            n.needCheck.Add(n2);
                            countC++;
                        }
                    }
                    sortn = n;
                    n.needCheck.Sort(sortDistance);
                }
               // Debug.Log(countC);
                int errorC = 0;
                while (countC > 0)
                {
                    if (errorC > 100000)
                    {
                        Debug.Log("Error count check connection");
                        break;
                    }
                    errorC ++;
                    foreach (Node n in allNodes)
                    {
                        countC--;
                        if (n.needCheck.Count == 0) continue;
                        Node nc = n.needCheck[0];
                        
                        if (!triangulator.intractConnections(n, nc))
                        {
                            n.nodes.Add(nc);
                            nc.nodes.Add(n);
                            triangulator.connections.Add(new Connection(n, nc));
                            countC--;
                            nc.needCheck.Remove(n);
                        }
                        n.needCheck.Remove(nc);
                        
                        
                       // Debug.Log(countC);

                    }
                    
                }
            }
            Node sortn;
            int sortDistance(Node n1, Node n2)
            {
                float d1 = Vector2.SqrMagnitude(sortn.position - n1.position);
                float d2 = Vector2.SqrMagnitude(sortn.position - n2.position);
                return d1 < d2 ? -1 : 1;
            }




        }
        public struct Triangle
        {
            public int index1;
            public int index3;
            public int index2;
            public Triangle(Node n1, Node n2, Node n3)
            {

                Node left;
                Node o1;
                Node o2;
                if (n1.position.x <= n2.position.x && n1.position.x < n3.position.x)
                {
                    left = n1;
                    o1 = n2;
                    o2 = n3;
                }else
                    if (n2.position.x <= n1.position.x && n2.position.x < n3.position.x)
                    {
                        left = n2;
                        o1 = n1;
                        o2 = n3;
                    }
                    else
                    {
                        left = n3;
                        o1 = n1;
                        o2 = n2;
                    }


                if (Vector2Utillites.AngleFromTo(left.position - o1.position, left.position - o2.position) >= 180f)
                {
                    index1 = o1.index;
                    index2 = left.index;
                    index3 = o2.index;
                }
                else
                {
                    index1 = o2.index;
                    index2 = left.index;
                    index3 = o1.index;
                }
            }
        }
        public List<Triangle> trises = new List<Triangle>();
        public List<Node> nodes = new List<Node>();
        public List<Connection> connections = new List<Connection>();
        public List<BigLoop> bigLoops = new List<BigLoop>();
        public SpriteDeformer spriteDeformer;
        public int[] intTriangles;
        public bool useDeltaPosition = false;
        public Triangulator(SpriteDeformer spriteDeformer)
        {
            this.spriteDeformer = spriteDeformer;
        }
        public void trinagulate()
        {
            trises.Clear();
            nodes.Clear();
            connections.Clear();
            bigLoops.Clear();

            createNodes();
            getBigLoops();
            foreach (BigLoop b in bigLoops)
            {
                b.triangulate();
            }
            getTris();
            intTriangles = new int[trises.Count * 3];
            for (int i = 0; i < trises.Count; i++)
            {
                //Debug.Log(i);
                Triangle cTris = trises[i];
                intTriangles[i * 3] = cTris.index1;
                intTriangles[i * 3 + 1] = cTris.index2;
                intTriangles[i * 3 + 2] = cTris.index3;
                
            }
           // Debug.Log(trises.Count);

        }
        void createNodes()
        {
            for (int i = 0; i < spriteDeformer.points.Count; i++)
            {
                SpritePoint p = spriteDeformer.points[i];
                Node newNode = new Node();
                newNode.point = p;
                newNode.index = i;
                newNode.useDeltaPosition = useDeltaPosition;
                nodes.Add(newNode);
            }
            foreach (Edge e in spriteDeformer.edges)
            {
                int ind1 = spriteDeformer.points.IndexOf(e.point1);
                int ind2 = spriteDeformer.points.IndexOf(e.point2);
                nodes[ind1].nodes.Add(nodes[ind2]);
                nodes[ind2].nodes.Add(nodes[ind1]);
                connections.Add(new Connection(nodes[ind1], nodes[ind2]));

            }
        }
        private void getBigLoops()
        {
            List<Node> _nodes = nodes.GetRange(0, nodes.Count);
            _nodes.Sort(sortY);

            while (_nodes.Count > 0)
            {
                Node currentNode = _nodes[0];
                _nodes.RemoveAt(0);
                if (currentNode.nodes.Count < 2) continue;

                BigLoop bigLoop = new BigLoop();
                bigLoop.triangulator = this;

                Node nextNode = currentNode.getMinimalAgleNode(Vector2.up);
                _nodes.Remove(nextNode);
                Node before = currentNode;
                bigLoop.edgeNodes.Add(nextNode);
                bigLoop.allNodes.Add(nextNode);
                nextNode.isBigLoop = true;
                bool right = false; 
                int k = 0;
                while (k < 100000)
                {
                    k++;
                    Node newNext = nextNode.getMinimalAgleNode(before);
                    before = nextNode;
                    nextNode = newNext;
                    if (newNext != null)
                    {
                        _nodes.Remove(newNext);
                        bigLoop.edgeNodes.Add(nextNode);
                        bigLoop.allNodes.Add(nextNode);
                        nextNode.isBigLoop = true;
                    }
                    else
                    {
                        break;
                    }
                    if (nextNode == currentNode)
                    {
                        //Debug.Log("loop");
                        //bigLoop.debug();
                        right = true;
                        break;
                    }

                }
                if (right)
                {
                    List<Node> removeList = new List<Node>();
                    foreach (Node n in _nodes)
                    {
                        if (bigLoop.isBelong(n))
                        {
                            removeList.Add(n);
                            bigLoop.insideNodes.Add(n);
                            bigLoop.allNodes.Add(n);
                        }
                    }
                    foreach (Node n in removeList)
                    {
                        _nodes.Remove(n);
                    }
                   // Debug.Log("Count edgs " + bigLoop.edgeNodes.Count);
                   // Debug.Log("Count ins " + bigLoop.insideNodes.Count);
                   // Debug.Log("Count all " + bigLoop.allNodes.Count);
                    bigLoops.Add(bigLoop);
                    
                }
            }
        }
       
        private void getTris()
        {
            foreach (Connection c in connections)
            {
                Node left = c.node1.position.x < c.node2.position.x ? c.node1 : c.node2;
                Node right = c.node1 == left ? c.node2 : c.node1;
                
                

                Node up = right.getMinimalAgleNode(left);
                if (up == null) continue;
                //if (down.position.y < left.position.y || down.position.y < right.position.y) continue;
                if (Vector2Utillites.AngleFromTo( right.position - left.position,right.position - up.position) > 180) continue;
                //if (Vector2Utillites.AngleFromTo(right.position - left.position, right.position - up.position) > 180) continue;
                //if (left.position.x > right.position.x) continue;
                //if(up.position.y < right.position.y) continue;
                //if (left.position.y < up.position.y && right.position.y < up.position.y) continue;
                //if (!correctTris(left.index, right.index, up.index)) continue;
                //if (up.position.y  > Math.Max(left.position.y, right.position.y)) continue;
                //if (left.position.y < right.position.y) continue;

                //Node vup = left.position.y > right.position.y ? left : right;
                //if (up.position.y > vup.position.y) vup = up;
                //Node vl = 
               
                
                if (up.nodes.Contains(left))
                {
                    //Debug.Log(left.index.ToString() + " " + right.index.ToString() + " " + up.index.ToString());
                    Triangle t = new Triangle(left, up, right);
                    if (!trises.Contains(t))
                    {
                        //Debug.Log(t.index1.ToString() + " " + t.index2.ToString() +" " +t.index3.ToString());
                        trises.Add(t);
                    }
                }
            }
           // Debug.Log(trises.Count);
        }
       

        private bool intractConnections(Node n1, Node n2)
        {
            foreach (Connection c in connections)
            {
                if (c.constains(n1) || c.constains(n2)) continue;

                if (Line.distanceFromPointToSegment(n1.position, n2.position, c.node1.position) < 0.00001f) return true;
                if (Line.distanceFromPointToSegment(n1.position, n2.position, c.node2.position) < 0.00001f) return true;
                if (Line.isIntersection(n1.position, n2.position, c.node1.position, c.node2.position))
                    return true;

            }
            return false;
        }
        int sortY(Node n1, Node n2)
        {
            return n1.point.spritePosition.y < n2.point.spritePosition.y ? -1 : 1;
        }

    }
}
