using System;
using System.Collections.Generic;
using System.Text;

namespace Label
{
    class Program
    {
        public static void Main(string[] args)
        {
            // PUT YOUR CYCLE SIZES HERE
            int[] cycles_sizes = { 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13 };
            // 0 : Rho, 1: Sigma, 2: Graceful
            // Note, atumatically switches to Nearly Sigma/Graceful if # edges is 1 or 2 mod 4
            int label_type = 1;

            Graph g1 = new Graph(Build_Cycles(cycles_sizes), label_type);
            g1.Initialize_Random_Labeling();
            g1.Solve();
            g1.Print();
            Console.WriteLine(g1.Validate());
            //Console.ReadKey();
        }

        public static int[][] Build_Cycle(int size)
        {
            int i;
            int[][] cycle = new int[size][];
            for (i = 0; i < size; i++)
                cycle[i] = new int[2];
            cycle[0][0] = 1;
            cycle[0][1] = size - 1;
            cycle[size - 1][0] = 0;
            cycle[size - 1][1] = size - 2;
            for (i = 1; i < size - 1; i++)
            {
                cycle[i][0] = i - 1;
                cycle[i][1] = i + 1;
            }
            return cycle;
        }

        public static int[][] Build_Cycles(int[] cycle_sizes)
        {
            int i, j, index, head;
            int num_nodes = 0;
            foreach (int size in cycle_sizes)
                num_nodes += size;
            int[][] cycles = new int[num_nodes][];
            for (i = 0; i < num_nodes; i++)
                cycles[i] = new int[2];
            index = 0;
            head = 0;
            for (i = 0; i < cycle_sizes.Length; i++)
            {
                cycles[index][0] = index + 1;
                cycles[index][1] = head + cycle_sizes[i] - 1;
                index++;
                for (j = 1; j < cycle_sizes[i] - 1; j++)
                {
                    cycles[index][0] = index - 1;
                    cycles[index][1] = index + 1;
                    index++;
                }
                cycles[index][0] = head;
                cycles[index][1] = index - 1;
                index++;
                head += cycle_sizes[i];
            }
            return cycles;
        }
    }
    class Node
    {
        public int label;
        public List<Edge> left_list;
        public List<Edge> right_list;
        public int index;
    }

    class Edge
    {
        public int length;
        public Node left;
        public Node right;
    }

    class Graph
    {
        public Node[] node_heap;
        public Edge[] edge_heap;
        public List<Node> bad_node_list;
        public Dictionary<int, List<Node>> label_to_node_list;
        public Dictionary<int, List<Edge>> length_to_edge_list;
        public Random randy;
        public double update_size;
        public double p_of_accept_bad_update;
        public int K;
        int label_type; //0 for rho, 1 for sigma, 2 for graceful
        bool zero_or_three_mod_four;

        public Graph(int[][] node_neighbor_lists, int label_type = 0)
        {
            int i, j;
            int number_edges = Count_Edges(node_neighbor_lists);
            this.label_type = label_type;
            zero_or_three_mod_four = (number_edges % 4 == 0 || number_edges % 4 == 3);
            //Create Nodes
            node_heap = new Node[node_neighbor_lists.Length];
            for (i = 0; i < node_heap.Length; i++)
            {
                node_heap[i] = new Node();
                node_heap[i].index = i;
                node_heap[i].left_list = new List<Edge>();
                node_heap[i].right_list = new List<Edge>();
            }

            //Create Edges
            edge_heap = new Edge[number_edges];
            for (i = 0; i < edge_heap.Length; i++)
                edge_heap[i] = new Edge();

            //Connect Nodes and Edges
            int edge_index = 0;
            for (i = 0; i < node_neighbor_lists.Length; i++)
            {
                for (j = 0; j < node_neighbor_lists[i].Length; j++)
                {
                    if (i > node_neighbor_lists[i][j])
                    {
                        node_heap[i].left_list.Add(edge_heap[edge_index]);
                        node_heap[node_neighbor_lists[i][j]].right_list.Add(edge_heap[edge_index]);
                        edge_heap[edge_index].left = node_heap[node_neighbor_lists[i][j]];
                        edge_heap[edge_index].right = node_heap[i];
                        edge_index++;
                    }
                }
            }

            // Initialize everything else
            K = 2 * number_edges + 1;
            bad_node_list = new List<Node>();
            label_to_node_list = new Dictionary<int, List<Node>>();
            for (i = 0; i < K; i++)
                label_to_node_list.Add(i, new List<Node>());
            length_to_edge_list = new Dictionary<int, List<Edge>>();
            for (i = 0; i <= number_edges; i++)
                length_to_edge_list.Add(i, new List<Edge>());

            randy = new Random();
            p_of_accept_bad_update = 0.0;
            update_size = 1 / (Math.Pow(number_edges, 2.5));
        }

        public int Random_Node_Label()
        {
            if (label_type <= 1) //Rho or Sigma Labeling
                return randy.Next(0, K);
            if (zero_or_three_mod_four) //Graceful
                return randy.Next(0, edge_heap.Length + 1);
            return randy.Next(0, edge_heap.Length + 2); //Nearly Graceful
        }

        public void Initialize_Random_Labeling()
        {
            foreach (Node node in node_heap)
            {
                node.label = Random_Node_Label();
                Add_Node_To_List(node);
            }

            foreach (Edge edge in edge_heap)
            {
                //Console.WriteLine("edge.left.label = " + edge.left.label);
                edge.length = EdgeLength(edge.left.label, edge.right.label);
                Add_Edge_To_List(edge);
            }
        }

        public void Add_Node_To_List(Node node)
        {
            label_to_node_list[node.label].Add(node);

            if (label_to_node_list[node.label].Count > 1)
            {
                bad_node_list.Add(node);

                // Add the first element as well the first time this occurs
                if (label_to_node_list[node.label].Count == 2)
                    bad_node_list.Add(label_to_node_list[node.label][0]);
            }
        }

        public void Add_Edge_To_List(Edge edge)
        {
            length_to_edge_list[edge.length].Add(edge);

            if (edge.length == 0)
            {
                bad_node_list.Add(edge.left);
                bad_node_list.Add(edge.right);
            }
            else if (length_to_edge_list[edge.length].Count > 1)
            {
                bad_node_list.Add(edge.left);
                bad_node_list.Add(edge.right);

                if (length_to_edge_list[edge.length].Count == 2)
                {
                    bad_node_list.Add(length_to_edge_list[edge.length][0].left);
                    bad_node_list.Add(length_to_edge_list[edge.length][0].right);
                }
            }
        }

        public void Remove_Node_From_List(Node node)
        {
            label_to_node_list[node.label].Remove(node);

            if (label_to_node_list[node.label].Count > 0)
            {
                bad_node_list.Remove(node);
                if (label_to_node_list[node.label].Count == 1)
                    bad_node_list.Remove(label_to_node_list[node.label][0]);
            }
        }

        public void Remove_Edge_From_List(Edge edge)
        {
            length_to_edge_list[edge.length].Remove(edge);

            if (edge.length == 0)
            {
                bad_node_list.Remove(edge.left);
                bad_node_list.Remove(edge.right);
            }
            else if (length_to_edge_list[edge.length].Count > 0)
            {
                bad_node_list.Remove(edge.left);
                bad_node_list.Remove(edge.right);

                if (length_to_edge_list[edge.length].Count == 1)
                {
                    bad_node_list.Remove(length_to_edge_list[edge.length][0].left);
                    bad_node_list.Remove(length_to_edge_list[edge.length][0].right);
                }
            }
        }

        public void Update()
        {
            Node node;
            int old_score, old_label, new_label;
            double p = p_of_accept_bad_update;

            bool done = false;
            while (!done)
            {
                node = bad_node_list[randy.Next(0, bad_node_list.Count)];
                old_label = node.label;
                old_score = bad_node_list.Count;

                new_label = Random_Node_Label();
                Set_Node(node, new_label);

                if (bad_node_list.Count < old_score || randy.NextDouble() < p)
                    done = true;
                else
                {
                    Set_Node(node, old_label);
                    p += (1 - p) * update_size;
                }
            }
        }

        public void Set_Node(Node node, int label)
        {
            foreach (Edge left_edge in node.left_list)
                Remove_Edge_From_List(left_edge);
            foreach (Edge right_edge in node.right_list)
                Remove_Edge_From_List(right_edge);
            Remove_Node_From_List(node);

            node.label = label;
            foreach (Edge left_edge in node.left_list)
                left_edge.length = EdgeLength(left_edge.left.label, node.label);
            foreach (Edge right_edge in node.right_list)
                right_edge.length = EdgeLength(right_edge.right.label, node.label);

            Add_Node_To_List(node);
            foreach (Edge left_edge in node.left_list)
                Add_Edge_To_List(left_edge);
            foreach (Edge right_edge in node.right_list)
                Add_Edge_To_List(right_edge);
        }

        public int Count_Edges(int[][] node_neighbor_lists)
        {
            int total = 0;
            for (int i = 0; i < node_neighbor_lists.Length; i++)
                for (int j = 0; j < node_neighbor_lists[i].Length; j++)
                    total++;
            return total / 2;
        }

        public int EdgeLength(int node_label_1, int node_label_2)
        {
            int label = Math.Abs(node_label_1 - node_label_2);
            if (label_type == 0) //Rho Labeling
                return Math.Min(label, K - label);

            // Check "Nearly" Sigma/Graceful Condition
            if (!zero_or_three_mod_four && label == edge_heap.Length + 1)
                label--;
            if (label_type == 1) //Sigma
            {
                if (label <= edge_heap.Length)
                    return label;
                return 0;
            }
            //Graceful
            return label;
        }

        public string Validate()
        {
            int i;
            bool[] node_label_flag = new bool[K];
            bool[] edge_length_flag = new bool[edge_heap.Length];

            for (i = 0; i < K; i++)
                node_label_flag[i] = false;
            for (i = 0; i < edge_heap.Length; i++)
                edge_length_flag[i] = false;

            foreach (Node node in node_heap)
            {
                if (node_label_flag[node.label])
                    return "Not Good!";
                node_label_flag[node.label] = true;
            }

            foreach (Edge edge in edge_heap)
            {
                if (edge_length_flag[edge.length - 1])
                    return "Not Good!";
                edge_length_flag[edge.length - 1] = true;
            }
            return "Validated!";
        }

        public void Solve()
        {
            while (bad_node_list.Count > 0)
            {
                //Console.WriteLine(bad_node_list.Count);
                Update();
            }
        }

        public void Print()
        {
            StringBuilder sb = new StringBuilder();
            for (int i = 0; i < node_heap.Length; i++)
                sb.Append("Node " + i + ":\t" + node_heap[i].label + "\n");
            if (bad_node_list.Count > 0)
            {
                sb.Append("\nBad Nodes:\n");
                foreach (Node bad_node in bad_node_list)
                    sb.Append(bad_node.label + " ");
            }
            Console.WriteLine(sb.ToString());
        }
    }
}
