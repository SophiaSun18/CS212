using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.IO;

namespace Bingo
{
    class Program
    {
        private static RelationshipGraph rg;

        // Read RelationshipGraph whose filename is passed in as a parameter.
        // Build a RelationshipGraph in RelationshipGraph rg
        private static void ReadRelationshipGraph(string filename)
        {
            rg = new RelationshipGraph();                           // create a new RelationshipGraph object

            string name = "";                                       // name of person currently being read
            int numPeople = 0;
            string[] values;
            Console.Write("Reading file " + filename + "\n");
            try
            {
                string input = System.IO.File.ReadAllText(filename);// read file
                input = input.Replace("\r", ";");                   // get rid of nasty carriage returns 
                input = input.Replace("\n", ";");                   // get rid of nasty new lines
                string[] inputItems = Regex.Split(input, @";\s*");  // parse out the relationships (separated by ;)
                foreach (string item in inputItems) 
		{
                    if (item.Length > 2)                            // don't bother with empty relationships
                    {
                        values = Regex.Split(item, @"\s*:\s*");     // parse out relationship:name
                        if (values[0] == "name")                    // name:[personname] indicates start of new person
                        {
                            name = values[1];                       // remember name for future relationships
                            rg.AddNode(name);                       // create the node
                            numPeople++;
                        }
                        else
                        {               
                            rg.AddEdge(name, values[1], values[0]); // add relationship (name1, name2, relationship)

                            // handle symmetric relationships -- add the other way
                            if (values[0] == "hasSpouse" || values[0] == "hasFriend")
                                rg.AddEdge(values[1], name, values[0]);

                            // for parent relationships add child as well
                            else if (values[0] == "hasParent")
                                rg.AddEdge(values[1], name, "hasChild");
                            else if (values[0] == "hasChild")
                                rg.AddEdge(values[1], name, "hasParent");
                        }
                    }
                }
            }
            catch (Exception e)
            {
                Console.Write("Unable to read file {0}: {1}\n", filename, e.ToString());
            }
            Console.WriteLine(numPeople + " people read");
        }

        // Show the relationships a person is involved in
        private static void ShowPerson(string name)
        {
            GraphNode n = rg.GetNode(name);
            if (n != null)
                Console.Write(n.ToString());
            else
                Console.WriteLine("{0} not found", name);
        }

        // Show a person's friends
        private static void ShowFriends(string name)
        {
            GraphNode n = rg.GetNode(name);
            if (n != null)
            {
                Console.Write("{0}'s friends: ",name);
                List<GraphEdge> friendEdges = n.GetEdges("hasFriend");
                foreach (GraphEdge e in friendEdges) {
                    Console.Write("{0} ",e.To());
                }
                Console.WriteLine();
            }
            else
                Console.WriteLine("{0} not found", name);     
        }

        // Show a person's siblings
        private static void ShowSiblings(string name)
        {
            GraphNode n = rg.GetNode(name);
            if (n != null)
            {
                Console.Write("{0}'s siblings: ", name);
                List<GraphEdge> parentEdges = n.GetEdges("hasParent");              // get the parent of the person
                List<GraphNode> siblingNodes = new List<GraphNode>();
                foreach (GraphEdge e in parentEdges)
                {
                    GraphNode parent = rg.GetNode(e.To());
                    foreach (GraphEdge s in parent.GetEdges("hasChild"))            // search for the parent's children
                    {
                        GraphNode sibling = rg.GetNode(s.To());
                        if (sibling != n && (!siblingNodes.Contains(sibling)))      // if the child is not the person himself, put this child in the sibling list
                        {
                            siblingNodes.Add(sibling);
                        }
                    }
                }
                foreach (GraphNode s in siblingNodes)                               // write all names in the sibling list to the console
                {
                    Console.Write("{0} ", s.Name);
                }
                Console.WriteLine();
            }
            else                                                                    // if there's no sibling, write a message
                Console.WriteLine("{0} not found", name);
        }

        // Show a person's descendants
        private static void ShowDescendants(string name)
        {
            GraphNode n = rg.GetNode(name);
            if (n != null)
            {
                Console.Write("{0}'s descendants: ", name);
                getDesc(n);                                                         // call the getDesc() function to get all descendants
                Console.WriteLine();
            }
            else
                Console.WriteLine("{0} not found", name);
        }

        // Get a person's descendants (without the function of labeling the descendant)
        private static List<GraphNode> getDesc(GraphNode n)
        {
            foreach (GraphNode c in getChildren(n))                                 // call the getChildren() function to get all direct children
            {
                Console.Write("{0} ", c.Name);
                getDesc(c);                                                         // call the getDesc() for all children recursively to get all descendants
            }
            return getChildren(n);
        }

        // Get a person's direct children
        private static List<GraphNode> getChildren(GraphNode n)
        {
            List<GraphEdge> childEdges = n.GetEdges("hasChild");                    // get a list of all direct children
            List<GraphNode> childNodes = new List<GraphNode>();
            foreach (GraphEdge e in childEdges)
            {
                GraphNode child = rg.GetNode(e.To());
                if (!childNodes.Contains(child))
                {
                    childNodes.Add(child);
                }
            }
            return childNodes;
        }

        // Show all the orphans
        private static void ShowOrphans()
        {
            Console.Write("Orphans: ");
            foreach (GraphNode n in rg.nodes)
            {
                List<GraphEdge> parentEdges = n.GetEdges("hasParent");              // if the person has no parent, then he is considered an orphan
                int len = parentEdges.Count();                                      // count the number of parents a person has
                if (len == 0)
                {
                    Console.Write(n.Name + " ");
                }
            }
            Console.WriteLine();
        }

        // Show the shortest chain of relationship between two people
        private static void Bingo(string name1, string name2)
        {
            GraphNode n1 = rg.GetNode(name1);
            GraphNode n2 = rg.GetNode(name2);
            if(BFS(n1, n2) == false)                                                // do a breadth first search to the relationship graph to find the shortest path
            {
                Console.WriteLine("There is no relationship found!");
            }
        }

        // Do a breadth first search
        private static bool BFS(GraphNode n1, GraphNode n2)
        {
            foreach (GraphNode n in rg.nodes)                                       // set all people in the graph to be unvisited, so the command can be used multiple times
            {
                n.Label = "Unvisited";
            }

            Queue<GraphNode> q = new Queue<GraphNode>();                            // create a queue to store all the unvisited nodes
            List<GraphNode> v_l = new List<GraphNode>();                            // create a list to store all the visited nodes
            q.Enqueue(n1);                                                          // push the start node onto the queue

            while (q.Count() != 0)                                                  // keep searching until all nodes are searched and the queue is empty
            {
                GraphNode currentNode = q.Dequeue();                                // dequeue the first node in the queue
                v_l.Add(currentNode);                                               // add the current node to the visited list
                currentNode.Label = "Visited";                                      // label the node to be visited
                List<GraphEdge> currentEdges = currentNode.GetEdges();              // get a list of edges of the current node
                foreach (GraphEdge e in currentEdges)
                {
                    GraphNode toN = rg.GetNode(e.To());
                    if (toN.GetParent() == null)                                    // keep track of the path by keep track of the node before each node
                    {
                        toN.SetParent(currentNode);
                    }
                    if (toN.Label == "Unvisited")                                   // if the node has not been visited, enqueue the node to be visited later, and decide if it's the goal node
                    {
                        q.Enqueue(toN);
                        if (toN == n2)                                              // if the node is the goal node, call the getBingoResult() to get the info about the shortest path
                        {
                            getBingoResult(n1, n2);
                            return true;                                            // if the goal node is found, return true
                        }
                    }
                }
            }
            return false;                                                           // if the goal node is not found, return false
        }

        // Get all the relationships in the shortest relationship path
        private static void getBingoResult(GraphNode n1, GraphNode n2)
        {
            Stack<GraphNode> result = new Stack<GraphNode>();                       //create a stack to store all the nodes in the path
            result.Push(n2);                                                        // push n2 to be the deepest node on the stack
            string p = n2.GetParent();      
            GraphNode pNode = rg.GetNode(p);                                        // get the parent node of n2
            result.Push(pNode);                                                     // push the parent node of n2 onto the stack

            while (!result.Contains(n1))                                            // keep pushing until n1 is on the stack as the last one
            {
                p = pNode.GetParent();
                pNode = rg.GetNode(p);
                result.Push(pNode);
            }

            if (result.Count() > 1) {                                               // if the path contains more than 1 node, write each node into the console with "-->"
                while (result.Count() > 1)                                          // keep writing "-->" until only the last node is on the stack
                {
                    GraphNode n = result.Pop();
                    Console.Write(n.Name + " --> ");
                }
            }
            
            Console.Write(result.Pop().Name);                                       // write the last node to the console without "-->"
            Console.WriteLine();
        }
           
        // accept, parse, and execute user commands
        private static void CommandLoop()
        {
            string command = "";
            string[] commandWords;
            Console.Write("Welcome to Harry's Dutch Bingo Parlor!\n");

            while (command != "exit")
            {
                Console.Write("\nEnter a command: ");
                command = Console.ReadLine();
                commandWords = Regex.Split(command, @"\s+");        // split input into array of words
                command = commandWords[0];

                if (command == "exit")
                    ;                                               // do nothing

                // read a relationship graph from a file
                else if (command == "read" && commandWords.Length > 1)
                    ReadRelationshipGraph(commandWords[1]);

                // show information for one person
                else if (command == "show" && commandWords.Length > 1)
                    ShowPerson(commandWords[1]);

                else if (command == "friends" && commandWords.Length > 1)
                    ShowFriends(commandWords[1]);

                else if (command == "siblings" && commandWords.Length > 1)
                    ShowSiblings(commandWords[1]);

                else if (command == "descs" && commandWords.Length > 1)
                    ShowDescendants(commandWords[1]);

                // show all orphans
                else if (command == "orphans")
                    ShowOrphans();

                // show the shortest chain of relationship
                else if (command == "bingo" && commandWords.Length > 1)
                    Bingo(commandWords[1], commandWords[2]);

                // dump command prints out the graph
                else if (command == "dump")
                    rg.Dump();

                // illegal command
                else
                    Console.Write("\nLegal commands: read [filename], dump, show [personname], \nfriends [personname], siblings [personname], descs [personname], \norphans, bingo [personname1] [personname2] exit\n");
            }
        }

        static void Main(string[] args)
        {
            CommandLoop();
        }
    }
}
