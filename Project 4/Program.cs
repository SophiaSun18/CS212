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
                List<GraphEdge> parentEdges = n.GetEdges("hasParent");
                List<GraphNode> siblingNodes = new List<GraphNode>();
                foreach (GraphEdge e in parentEdges)
                {
                    GraphNode parent = rg.GetNode(e.To());
                    foreach (GraphEdge s in parent.GetEdges("hasChild"))
                    {
                        GraphNode sibling = rg.GetNode(s.To());
                        if (sibling != n && (!siblingNodes.Contains(sibling)))
                        {
                            siblingNodes.Add(sibling);
                        }
                    }
                }
                foreach (GraphNode s in siblingNodes)
                {
                    Console.Write("{0} ", s.Name);
                }
                Console.WriteLine();
            }
            else
                Console.WriteLine("{0} not found", name);
        }

        // Show a person's descendants
        private static void ShowDescendants(string name)
        {
            GraphNode n = rg.GetNode(name);
            if (n != null)
            {
                Console.Write("{0}'s descendants: ", name);
                getDesc(n);
                Console.WriteLine();
            }
            else
                Console.WriteLine("{0} not found", name);
        }

        // Get a person's descendants (without the function of labeling the descendant)
        private static List<GraphNode> getDesc(GraphNode n)
        {
            foreach (GraphNode c in getChildren(n))
            {
                Console.Write("{0} ", c.Name);
                getDesc(c);
            }
            return getChildren(n);
        }

        // Get a person's children
        private static List<GraphNode> getChildren(GraphNode n)
        {
            List<GraphEdge> childEdges = n.GetEdges("hasChild");
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
                List<GraphEdge> parentEdges = n.GetEdges("hasParent");
                int len = parentEdges.Count();
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
            string result = search(n1, n2);
            Console.WriteLine(result);
            Console.WriteLine("result written!");
        }

        // bingo Bezaleel Eneas
        // Searching...
        private static string search(GraphNode n1, GraphNode n2)
        {
            GraphNode startN = n1;
            GraphNode endN = n2;
            List<GraphEdge> start_edges = startN.GetEdges();

            startN.Label = "Visited";
            string result = "no relationship found!";

            foreach (GraphEdge e in start_edges)
            {
                GraphNode toN = rg.GetNode(e.To());
                Console.WriteLine("found " + toN.Name);

                if (toN.Label == "Unvisited")
                {
                    toN.Label = "Visited";

                    if (toN.Name == endN.Name)
                    {
                        Console.WriteLine("find " + endN.Name + " as " + e.Label);
                        result = ("result found!!!!!!!");
                    }

                    else if (toN.GetEdges().Count() != 0)
                    {
                        Console.WriteLine("searching " + toN.Name + "'s relationship");
                        search(toN, endN);
                    }

                }
                else
                {
                    Console.WriteLine("not searching, already visited.");
                }

            }

            return (result);
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
